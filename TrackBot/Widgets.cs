using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;
using TrackBot.Spatial;
using TrackBot.Tracks;

namespace TrackBot
{
	class Widgets
	{
		public static BotTracks Tracks { get; private set; }
		public static Dictionary<RFDir, HCSR04_RangeFinder> RangeFinders { get; private set; }

		public static LSM9D51CompassAccelerometer GyMag { get; private set; }
		public static Area Environment { get; private set; }

		public static void StartWidgets()
		{
			StartRangeFinders();
			StartTracks();
			StartSpatial();
			StartActivities();
		}

		public static void StopWidgets()
		{
			StopActivities();
			Tracks.Stop();
			StopRangeFinders();
			StopSpatial();
			GpioSharp.DeInit();
		}

		private static void StartSpatial()
		{
			GyMag = new LSM9D51CompassAccelerometer();
			GyMag.MagneticDeviation = Program.Config.MagneticDeviation;
			GyMag.XAdjust = Program.Config.CompassXAdjust;
			GyMag.YAdjust = Program.Config.CompassYAdjust;

			Environment = new Area(5, 5, .1, new PointD(2.5, 2.5));

		}

		private static void StopSpatial()
		{
		}

		private static void StartActivities()
		{
		}

		private static void StopActivities()
		{
			Activity.StopActivity();
		}

		private static void StartRangeFinders()
		{
			RangeFinders = new Dictionary<RFDir, HCSR04_RangeFinder>();
			Log.SysLogText(LogLevel.DEBUG, "There are {0} rangefinders", Program.Config.RangeFinderEchoPins.Count);
			foreach(KeyValuePair<RFDir, GpioPin> kvp in Program.Config.RangeFinderEchoPins)
			{
				Log.SysLogText(LogLevel.DEBUG, "Starting rangefinder {0} on echo pin {1}", kvp.Key, kvp.Value);
				HCSR04_RangeFinder rangeFinder = new HCSR04_RangeFinder(kvp.Value, Program.Config.RangeFinderInputPin, kvp.Key);
				rangeFinder.Start();

				RangeFinders.Add(kvp.Key, rangeFinder);
			}
		}

		private static void StopRangeFinders()
		{
			foreach(HCSR04_RangeFinder rangeFinder in RangeFinders.Values)
			{
				rangeFinder.Stop();
			}
			RangeFinders.Clear();
		}

		private static void StartTracks()
		{
			Tracks = new BotTracks();
			Tracks.LeftSpeed = 0;
			Tracks.RightSpeed = 0;
		}

		private static void StopTracks()
		{
			Tracks.LeftSpeed = 0;
			Tracks.RightSpeed = 0;
		}

	}
}
