using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.Spatial.Imaging;

namespace TrackBot.Spatial
{
	class RoamAndSeek : Activity
	{
		const Double MAX_RANGE_FROM_DEST = .15;

		public FuzzyPath Destination { get; private set; }

		public RoamAndSeek()
			: base(ActivityType.RoamAndSeek)
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
			SwitchState(ActivityStates.TravelToDest);
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
			return true;
		}

		#endregion

		#region TravelToDest State

		bool InitTravelToDestState()
		{
			// find a place to travel to
			if((Destination = Widgets.Instance.ImageEnvironment.FindGoodDestination(Program.Config.ShortRangeClearance)) == null)
			{
				Console.WriteLine("Could not find a good destination");
				return false;
			}

			Widgets.Instance.ImageEnvironment.FuzzyPath = Destination;
			Widgets.Instance.Tracks.TurnToBearing(Destination.Vector.Bearing);
			
			Log.LogText(LogLevel.DEBUG, "Found a good destination at {0} bearing {1:0.0}°... setting into path", Destination, Destination.Vector.Bearing);

			// set into environment to generate events
			Widgets.Instance.ImageEnvironment.FuzzyPath = Destination;

			/*** DEBUG ONLY */
			//			Widgets.Instance.Environment.ToImage GenerateBitmap();

			Widgets.Instance.Tracks.SetStart();
			Widgets.Instance.Tracks.Speed = Widgets.Instance.Tracks.Slow;

			return true;
		}

		override protected bool RunTravelToDestState()
		{
			Widgets.Instance.Tracks.TurnToBearing(Destination.Vector.Bearing);
			Widgets.Instance.Tracks.ForwardMeters(Math.Min(Destination.Vector.Range, 1), Widgets.Instance.Tracks.StandardSpeed);

			SwitchState(ActivityStates.TravelToDest);


			return true;
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
