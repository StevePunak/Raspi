using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

namespace TrackBot.Spatial
{
	class SpatialPoll : ThreadBase
	{
		public bool Paused { get; set; }

		public SpatialPoll()
			: base(typeof(SpatialPoll).Name)
		{
			Interval = TimeSpan.FromMilliseconds(500);
			Paused = true;
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
				Widgets.Environment.Bearing = Widgets.GyMag.Bearing;
				Log.LogText(LogLevel.DEBUG, "Set bearing to {0:0.00}°", Widgets.Environment.Bearing);
			}
			return true;
		}
	}
}
