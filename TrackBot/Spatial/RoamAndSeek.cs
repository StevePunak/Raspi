using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon;

namespace TrackBot.Spatial
{
	class RoamAndSeek : Activity
	{
		const Double MAX_RANGE_FROM_DEST = .15;

		public Line Destination { get; private set; }

		public RoamAndSeek()
			: base(ActivityType.RoamAndSeek)
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
			SwitchState(ActivityStates.TravelToDest);
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
			return true;
		}

		#endregion

		#region TravelToDest State

		bool InitTravelToDestState()
		{
			// find a place to travel to
			Destination = Widgets.Environment.FindGoodDestination().GetLineFrom(Widgets.Environment.Location);
			if(Destination == null)
			{
				Console.WriteLine("Could not find a good destination");
				return false;
			}
			Widgets.Tracks.TurnToBearing(Destination.Bearing);

			Console.WriteLine("Found a good destination at {0} bearing {1:0.0}°", Destination, Destination.Bearing);


			/*** DEBUG ONLY */
//			Widgets.Environment.ToImage GenerateBitmap();

			Widgets.Tracks.SetStart();
			Widgets.Tracks.Speed = Widgets.Tracks.Slow;

			return true;
		}

		override protected bool RunTravelToDestState()
		{
			bool result = false;
			while(DateTime.UtcNow < EnterStateTime + TimeSpan.FromSeconds(10))
			{
				GpioSharp.Sleep(20);

				PointD location;
				if(Widgets.Tracks.TryCalculateCurrentLocation(out location))
				{
					Widgets.Environment.RelativeLocation = location;

					if(location.DistanceTo(Destination.P2 as PointD) < .15)
					{
						Console.WriteLine("Arrived at location");
						result = true;
						break;
					}

					if(Widgets.Environment.FuzzyRangeAtBearing(Widgets.GyMag.Bearing, Widgets.Environment.RangeFuzz) < .2)
					{
						Console.WriteLine("Hit obstacle");
						break;
					}
				}
			}

			if(!result)
			{
				Console.WriteLine("Abandoning attempt");
				SwitchState(ActivityStates.Idle);
			}
			else
			{
				SwitchState(ActivityStates.TravelToDest);
			}


			return result;
		}

		#endregion

		#region Stuck State

		protected override bool RunStuckState()
		{
			return true;
		}

		#endregion
	}
}
