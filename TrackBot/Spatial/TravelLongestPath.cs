using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Extensions;
using RaspiCommon;
using KanoopCommon.Logging;
using RaspiCommon.Spatial.Imaging;

namespace TrackBot.Spatial
{
	class TravelLongestPath : Activity
	{
		const Double MAX_RANGE_FROM_DEST = .15;
		const Double FORWARD_COLLISION_WARNING = .15;
		const Double MAX_BEARING_DIFFERENTIAL = 5;

		public FuzzyPath Destination { get; private set; }

		int _unstuckTries;

		public TravelLongestPath()
			: base(ActivityType.TravelLongestPath)
		{
			Interval = TimeSpan.FromMilliseconds(100);
			ActivityState = ActivityStates.Init;
		}

		protected override bool OnStop()
		{
			Widgets.Instance.Tracks.Stop();
			return base.OnStop();
		}

		#region Init State

		protected override bool RunInitState()
		{
			Widgets.Instance.Tracks.Stop();
			SwitchState(ActivityStates.FindDestination);
			return true;
		}

		#endregion

		#region Idle State

		bool InitIdleState()
		{
			Widgets.Instance.Tracks.Stop();
			Interval = TimeSpan.FromMilliseconds(500);
			return true;
		}

		protected override bool RunIdleState()
		{
			Interval = TimeSpan.FromMilliseconds(500);

			return true;
		}

		#endregion

		#region Find Destination State

		bool InitFindDestinationState()
		{
			// sit tight for one second
			Interval = TimeSpan.FromSeconds(1);

			return true;
		}

		protected override bool RunFindDestinationState()
		{
			bool result = false;

			// find a place to travel to
			Destination = Widgets.Instance.ImageEnvironment.FindGoodDestination(Program.Config.ShortRangeClearance);
			if(Destination != null)
			{
				Console.WriteLine("Found a good destination at {0}", Destination);
				Widgets.Instance.ImageEnvironment.FuzzyPath = Destination;

				SwitchState(ActivityStates.TravelToDest);
				result = true;
			}
			else
			{
				Console.WriteLine("Could not find a good destination");
			}

			return result;
		}


		#endregion

		#region TravelToDest State

		bool InitTravelToDestState()
		{
			bool result = false;

			if(Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.Chassis, Widgets.Instance.GyMag.Bearing, Widgets.Instance.ImageEnvironment.RangeFuzz) < FORWARD_COLLISION_WARNING)
			{
				Console.WriteLine("Backing it up");
				Widgets.Instance.Tracks.BackwardTime(TimeSpan.FromSeconds(1), Widgets.Instance.Tracks.Slow);
				GpioSharp.Sleep(TimeSpan.FromSeconds(1.5));
			}

			Log.LogText(LogLevel.DEBUG, "Turn to bearing {0:0.00}°", Destination.CenterBearing);

			Widgets.Instance.Tracks.TurnToBearing(Destination.CenterBearing);

			Double diff = Widgets.Instance.GyMag.Bearing.AngularDifference(Destination.CenterBearing);
			if(diff > MAX_BEARING_DIFFERENTIAL)
			{
				Console.WriteLine("We are stuck! Angular difference of {0:0.0}° too damn high", diff);
				SwitchState(ActivityStates.Stuck);
				result = true;
			}
			else
			{
				Console.WriteLine("Rotated to within {0:0.0}° of {1:0.0}°", diff, Destination.CenterBearing);

				GpioSharp.Sleep(TimeSpan.FromSeconds(5));

				Widgets.Instance.Tracks.SetStart();

				result = true;
			}

			return result;
		}

		override protected bool RunTravelToDestState()
		{
			bool result = true;

			// make sure we are not hitting anything
			if(Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.Chassis, Widgets.Instance.GyMag.Bearing, Widgets.Instance.ImageEnvironment.RangeFuzz) < FORWARD_COLLISION_WARNING)
			{
				Console.WriteLine("activating emergency stop");
				SwitchState(ActivityStates.Idle);
				Widgets.Instance.Tracks.Stop();
				result = false;
			}
			else
			{
				Double range = Destination.ShortestRange;
				if(range > 2)
				{
					Log.SysLogText(LogLevel.DEBUG, "Shortening original range of {0:0.000}m to {1:0.000}m for no real reason", Destination.ShortestRange, 1);
					range = 2;
				}
				Widgets.Instance.Tracks.ForwardMeters(range, Widgets.Instance.Tracks.StandardSpeed);
				SwitchState(ActivityStates.FindDestination);
			}

			return result;
		}

		#endregion

		#region Stuck State

		bool InitStuckState()
		{
			// settle down
			Log.SysLogText(LogLevel.DEBUG, "Settling down to get unstuck");
			Interval = TimeSpan.FromSeconds(1);
			_unstuckTries = 3;

			return true;
		}

		override protected bool RunStuckState()
		{
			const Double UNSTUCK_FIDGET_DISTANCE = .25;

			bool result = true;

			// which way is closest?
			Double forwardRange = Widgets.Instance.GetRangeAtDirection(Direction.Forward);
			Double backwardRange = Widgets.Instance.GetRangeAtDirection(Direction.Backward);

			Direction direction = forwardRange > backwardRange ? Direction.Forward : Direction.Backward;
			Log.SysLogText(LogLevel.DEBUG, "Going to move {0} {1} to try and get unstuck", UNSTUCK_FIDGET_DISTANCE, direction);

			Widgets.Instance.Tracks.MoveMeters(direction, .3, Widgets.Instance.Tracks.StandardSpeed);
			GpioSharp.Sleep(TimeSpan.FromSeconds(1));

			SpinDirection spinDirection = Utility.GetClosestSpinDirection(Widgets.Instance.Compass.Bearing, Destination.CenterBearing);

			Log.LogText(LogLevel.DEBUG, "Turn {0} to bearing {1:0.00}°", spinDirection, Destination.CenterBearing);
			Widgets.Instance.Tracks.TurnToBearing(Destination.CenterBearing, spinDirection);

			Double diff = Widgets.Instance.GyMag.Bearing.AngularDifference(Destination.CenterBearing);
			if(diff > MAX_BEARING_DIFFERENTIAL)
			{
				spinDirection = spinDirection == SpinDirection.Clockwise ? SpinDirection.CounterClockwise : SpinDirection.Clockwise;

				Log.LogText(LogLevel.DEBUG, "OK. Will try it {0}. Turn to bearing {1:0.00}°", spinDirection, Destination.CenterBearing);
				GpioSharp.Sleep(TimeSpan.FromSeconds(1));
				Widgets.Instance.Tracks.TurnToBearing(Destination.CenterBearing, spinDirection);

				diff = Widgets.Instance.GyMag.Bearing.AngularDifference(Destination.CenterBearing);
				if(diff > MAX_BEARING_DIFFERENTIAL)
				{
					if(--_unstuckTries > 0)
					{
						Log.LogText(LogLevel.DEBUG, "Gonna rest for a few... I've got {0} tries left", _unstuckTries);
						Utility.LogSleep("Resting", TimeSpan.FromSeconds(15));
					}
					else
					{
						Log.LogText(LogLevel.DEBUG, "Fuck it. I give up.");
						SwitchState(ActivityStates.Idle);
					}
				}
				else
				{
					SwitchState(ActivityStates.TravelToDest);
				}
			}
			else
			{
				SwitchState(ActivityStates.TravelToDest);
			}

			return result;
		}

		#endregion

	}
}
