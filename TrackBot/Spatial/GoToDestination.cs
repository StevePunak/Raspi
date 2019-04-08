using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;

namespace TrackBot.Spatial
{
	class GoToDestination : Activity
	{
		const Double MAX_RANGE_FROM_DEST = .15;

		public BearingAndRange Destination { get; private set; }

		public GoToDestination()
			: base(ActivityType.GoToDestination)
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
			if(Widgets.Instance.ImageEnvironment.FuzzyPath != null)
			{
				Log.LogText(LogLevel.INFO, "Getting path at {0}", Widgets.Instance.ImageEnvironment.FuzzyPath);
				Destination = Widgets.Instance.ImageEnvironment.FuzzyPath.Vector;
				SwitchState(ActivityStates.TravelToDest);
			}
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
			Widgets.Instance.Tracks.TurnToBearing(Destination.Bearing);

			/*** DEBUG ONLY */
			//			Widgets.Instance.Environment.ToImage GenerateBitmap();

			Widgets.Instance.Tracks.SetStart();
			Widgets.Instance.Tracks.Speed = Widgets.Instance.Tracks.Slow;

			return true;
		}

		override protected bool RunTravelToDestState()
		{
			bool result = true;

			Widgets.Instance.Tracks.TurnToBearing(Destination.Bearing);
			Widgets.Instance.Tracks.ForwardMeters(Math.Min(Destination.Range, 1), Widgets.Instance.Tracks.StandardSpeed);

			SwitchState(ActivityStates.Idle);

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
