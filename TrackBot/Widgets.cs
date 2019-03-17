using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon;
using TrackBot.Spatial;
using TrackBot.Tracks;

namespace TrackBot
{
	class Widgets
	{
		public static BotTracks Tracks { get; private set; }
		public static HCSR04_RangeFinder RangeFinder { get; private set; }
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
			RangeFinder = new HCSR04_RangeFinder(Program.Config.RangeFinderInputPin, Program.Config.RangeFinderOuputPin);
			RangeFinder.Start();
		}

		private static void StopRangeFinders()
		{
			RangeFinder.Stop();
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
