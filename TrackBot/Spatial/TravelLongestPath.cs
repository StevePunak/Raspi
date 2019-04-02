using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Extensions;
using RaspiCommon;
using KanoopCommon.Logging;

namespace TrackBot.Spatial
{
	class TravelLongestPath : Activity
	{
		const Double MAX_RANGE_FROM_DEST = .15;
		const Double FORWARD_COLLISION_WARNING = .3;

		public Line Destination { get; private set; }

		public TravelLongestPath()
			: base(ActivityType.TravelLongestPath)
		{
			Interval = TimeSpan.FromMilliseconds(100);
			ActivityState = ActivityStates.Init;
		}

		protected override bool OnStop()
		{
			Widgets.Tracks.Stop();
			return base.OnStop();
		}

		#region Init State

		protected override bool RunInitState()
		{
			Widgets.Tracks.Stop();
			SwitchState(ActivityStates.FindDestination);
			return true;
		}

		#endregion

		#region Idle State

		bool InitIdleState()
		{
			Widgets.Tracks.Stop();
			Interval = TimeSpan.FromMilliseconds(500);
			return true;
		}

		protected override bool RunIdleState()
		{
			Interval = TimeSpan.FromMilliseconds(500);
			Widgets.Tracks.Stop();

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
			Destination = Widgets.Environment.FindGoodDestination().GetLineFrom(Widgets.Environment.Location);
			if(Destination != null)
			{
				Console.WriteLine("Found a good destination at {0} bearing {1:0.0}°", Destination, Destination.Bearing);
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

			if(Widgets.Environment.FuzzyRangeAtBearing(Widgets.GyMag.Bearing, Widgets.Environment.RangeFuzz) < FORWARD_COLLISION_WARNING)
			{
				Console.WriteLine("Backing it up");
				Widgets.Tracks.BackwardTime(TimeSpan.FromSeconds(1), Widgets.Tracks.Slow);
			}

			Log.LogText(LogLevel.DEBUG, "Turn to bearing {0:0.00}°", Destination.Bearing);

			Widgets.Tracks.TurnToBearing(Destination.Bearing);

			Double diff = Widgets.GyMag.Bearing.AngularDifference(Destination.Bearing);
			if(diff > 5)
			{
				Console.WriteLine("Abandoning due to angular difference of {0:0.0}° too high", diff);
				result = false;
			}
			else
			{
				Console.WriteLine("Rotated to within {0:0.0}° of {1:0.0}°", diff, Destination.Bearing);
				Widgets.Tracks.SetStart();
				Widgets.Tracks.Speed = Widgets.Tracks.Slow;

				Interval = TimeSpan.FromMilliseconds(50);
				result = true;
			}

			return result;
		}

		override protected bool RunTravelToDestState()
		{
			bool result = true;

			// make sure we are not hitting anything
			if(Widgets.Environment.FuzzyRangeAtBearing(Widgets.GyMag.Bearing, Widgets.Environment.RangeFuzz) < FORWARD_COLLISION_WARNING)
			{
				Console.WriteLine("activating emergency stop");
				SwitchState(ActivityStates.Idle);
				Widgets.Tracks.Stop();
				result = false;
			}
			else
			{
				Widgets.Tracks.AdjustSpeed(Destination.Bearing, Widgets.GyMag.Bearing);
			}

			return result;
		}

		#endregion

	}
}
