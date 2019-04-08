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

		public FuzzyPath Destination { get; private set; }

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
			Widgets.Instance.Tracks.Stop();

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

			if(Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.GyMag.Bearing, Widgets.Instance.ImageEnvironment.RangeFuzz) < FORWARD_COLLISION_WARNING)
			{
				Console.WriteLine("Backing it up");
				Widgets.Instance.Tracks.BackwardTime(TimeSpan.FromSeconds(1), Widgets.Instance.Tracks.Slow);
				GpioSharp.Sleep(TimeSpan.FromSeconds(1.5));
			}

			Log.LogText(LogLevel.DEBUG, "Turn to bearing {0:0.00}°", Destination.Vector.Bearing);

			Widgets.Instance.Tracks.TurnToBearing(Destination.Vector.Bearing);

			Double diff = Widgets.Instance.GyMag.Bearing.AngularDifference(Destination.Vector.Bearing);
			if(diff > 5)
			{
				Console.WriteLine("Abandoning due to angular difference of {0:0.0}° too high", diff);
				result = false;
			}
			else
			{
				Console.WriteLine("Rotated to within {0:0.0}° of {1:0.0}°", diff, Destination.Vector.Bearing);

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
			if(Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.GyMag.Bearing, Widgets.Instance.ImageEnvironment.RangeFuzz) < FORWARD_COLLISION_WARNING)
			{
				Console.WriteLine("activating emergency stop");
				SwitchState(ActivityStates.Idle);
				Widgets.Instance.Tracks.Stop();
				result = false;
			}
			else
			{
				Double range = Destination.Vector.Range;
				if(Destination.ShortestRange > Destination.Vector.Range)
				{
					Log.SysLogText(LogLevel.DEBUG, "Shortening original range of {0:0.000}m to {1:0.000}m due to shortest", Destination.Vector.Range, Destination.ShortestRange);
					range = Destination.ShortestRange;
				}
				else if(Destination.Vector.Range > 2)
				{
					Log.SysLogText(LogLevel.DEBUG, "Shortening original range of {0:0.000}m to {1:0.000}m for no real reason", Destination.Vector.Range, 1);
					range = 2;
				}
				Widgets.Instance.Tracks.ForwardMeters(range, Widgets.Instance.Tracks.StandardSpeed);
				SwitchState(ActivityStates.FindDestination);
			}

			return result;
		}

		#endregion

	}
}
