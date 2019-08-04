using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using RaspiCommon;
using RaspiCommon.Devices.Spatial;

namespace TrackBot.Spatial
{
	class SpatialPoll : ThreadBase
	{
		public bool Paused { get; set; }

		public SpatialPoll()
			: base(typeof(SpatialPoll).Name)
		{
			Interval = TimeSpan.FromMilliseconds(500);
			Paused = false;
		}

		protected override bool OnStart()
		{
			Log.LogText(LogLevel.DEBUG, "Starting spatial poll");
			return base.OnStart();
		}

		protected override bool OnStop()
		{
			Log.LogText(LogLevel.DEBUG, "Stopping spatial poll");

			return base.OnStop();
		}

		protected override bool OnRun()
		{
			if(Paused == false)
			{
				HCSR04_RangeFinder rangeFinder;
				if(Widgets.Instance.RangeFinders.TryGetValue(RFDir.Front, out rangeFinder))
				{
					Widgets.Instance.SetForwardSecondaryRange(rangeFinder.Range, rangeFinder.Valid);
				}
				if(Widgets.Instance.RangeFinders.TryGetValue(RFDir.Rear, out rangeFinder))
				{
					Widgets.Instance.SetBackwardSecondaryRange(rangeFinder.Range, rangeFinder.Valid);
				}
				//Log.LogText(LogLevel.DEBUG, "There are {0} landmarks", Widgets.Instance.Environment.Landmarks.Count);
			}
			return true;
		}
	}
}
