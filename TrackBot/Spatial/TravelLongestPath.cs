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
using TrackBot.System;

namespace TrackBot.Spatial
{
	class TravelLongestPath : Activity
	{
		const Double MAX_RANGE_FROM_DEST = .15;
		const Double FORWARD_COLLISION_WARNING = .15;
		const Double MAX_BEARING_DIFFERENTIAL = 5;

		public FuzzyPath Destination { get; private set; }

		public FuzzyPath LastDestination { get { return _lastPaths.Last(); } }

		int _unstuckTries;

		FuzzyPathList _lastPaths;

		public TravelLongestPath()
			: base(ActivityType.TravelLongestPath)
		{
			Interval = TimeSpan.FromMilliseconds(100);
			ActivityState = ActivityStates.Init;

			_lastPaths = new FuzzyPathList();
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

		protected override bool RunFindDestinationState()
		{
			Log.SysLogText(LogLevel.DEBUG, "================ RunFindDestinationState");

			bool result = false;

			// find a place to travel to
			FuzzyPathList possibleDestinations = Widgets.Instance.ImageEnvironment.FindGoodDestinations(Program.Config.ShortRangeClearance);
			if(possibleDestinations.Count != 0)
			{
				possibleDestinations.DumpToLog(String.Format("Finding good destination with current bearing {0} and {1} previous destinations", 
					Destination == null ? "None" : Destination.Bearing.ToAngleString(), _lastPaths.Count));
				if(_lastPaths.Count == 0)
				{
					Destination = possibleDestinations.Longest;
				}
				else
				{
					// get the paths cloest to right angles from our previous
					Double diff1 = LastDestination.Bearing.AddDegrees(90);
					Double diff2 = LastDestination.Bearing.AddDegrees(270);
					FuzzyPath p1 = possibleDestinations.ClosestTo(diff1);
					FuzzyPath p2 = possibleDestinations.ClosestTo(diff2);

					Log.SysLogText(LogLevel.DEBUG, "Prev: {0} - Path closest to {1} is {2}", Destination.Bearing, diff1.ToAngleString(), p1);
					Log.SysLogText(LogLevel.DEBUG, "Prev: {0} - Path closest to {1} is {2}", Destination.Bearing, diff2.ToAngleString(), p2);

					// of the two, pick the longest one
					Destination = p1.ShortestRange > p2.ShortestRange ? p1 : p2;
					Log.SysLogText(LogLevel.DEBUG, "Found best choice at {0}  (Previous: {1})", Destination, LastDestination);
				}
				_lastPaths.Add(Destination);

				Log.LogText(LogLevel.DEBUG, "Found a good destination at {0} count {1}", Destination, _lastPaths.Count);
				Widgets.Instance.ImageEnvironment.FuzzyPath = Destination;

				SwitchState(ActivityStates.RotateToNewBearing);
				result = true;
			}
			else
			{
				Log.LogText(LogLevel.DEBUG, "Could not find a good destination");
			}

			return result;
		}


		#endregion

		#region Rotate To Bearing

		protected override bool RunRotateToNewBearingState()
		{
			bool result = false;

			Double currentForwardRange = Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.Chassis, Widgets.Instance.GyMag.Bearing, Widgets.Instance.ImageEnvironment.RangeFuzz);
			Log.LogText(LogLevel.DEBUG, "Sitting at {0:0.000}m from forward obstacle with a destination of {1}", currentForwardRange, Destination);
			if(currentForwardRange < FORWARD_COLLISION_WARNING)
			{
				Log.LogText(LogLevel.DEBUG, "Backing it up");
				Widgets.Instance.Tracks.BackwardTime(TimeSpan.FromMilliseconds(500), Widgets.Instance.Tracks.Slow);
				GpioSharp.Sleep(TimeSpan.FromSeconds(1.5));
			}

			Log.LogText(LogLevel.DEBUG, "Turn to bearing {0:0.00}°", Destination.Bearing);

			Widgets.Instance.Tracks.TurnToBearing(Destination.Bearing);

			Double diff = Widgets.Instance.GyMag.Bearing.AngularDifference(Destination.Bearing);
			if(diff > MAX_BEARING_DIFFERENTIAL)
			{
				Console.WriteLine("We are stuck! Angular difference of {0:0.0}° too damn high", diff);
				SwitchState(ActivityStates.Stuck);
				result = true;
			}
			else
			{
				Console.WriteLine("Rotated to within {0:0.0}° of {1:0.0}°", diff, Destination.Bearing);

				GpioSharp.Sleep(TimeSpan.FromSeconds(5));

				Widgets.Instance.Tracks.SetStart();
				SwitchState(ActivityStates.TravelToDest);

				result = true;
			}

			return result;
		}

		#endregion

		#region TravelToDest State

		bool InitTravelToDestState()
		{
			return true;
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
			Interval = TimeSpan.FromSeconds(3);
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

			SpinDirection spinDirection = Utility.GetClosestSpinDirection(Widgets.Instance.Compass.Bearing, Destination.Bearing);

			Log.LogText(LogLevel.DEBUG, "Turn {0} to bearing {1:0.00}°", spinDirection, Destination.Bearing);
			Widgets.Instance.Tracks.TurnToBearing(Destination.Bearing, spinDirection);

			Double diff = Widgets.Instance.GyMag.Bearing.AngularDifference(Destination.Bearing);
			if(diff > MAX_BEARING_DIFFERENTIAL)
			{
				spinDirection = spinDirection == SpinDirection.Clockwise ? SpinDirection.CounterClockwise : SpinDirection.Clockwise;

				Log.LogText(LogLevel.DEBUG, "OK. Will try it {0}. Turn to bearing {1:0.00}°", spinDirection, Destination.Bearing);
				GpioSharp.Sleep(TimeSpan.FromSeconds(1));
				Widgets.Instance.Tracks.TurnToBearing(Destination.Bearing, spinDirection);

				diff = Widgets.Instance.GyMag.Bearing.AngularDifference(Destination.Bearing);
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
					SwitchState(ActivityStates.RotateToNewBearing);
				}
			}
			else
			{
				SwitchState(ActivityStates.RotateToNewBearing);
			}

			return result;
		}

		#endregion

	}
}
