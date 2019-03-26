using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using RaspiCommon;
using TrackBot.ForkLift;
using TrackBot.Spatial;
using TrackBot.Tracks;

namespace TrackBot
{
	class Widgets
	{
		public static BotTracks Tracks { get; private set; }
		public static Dictionary<RFDir, HCSR04_RangeFinder> RangeFinders { get; private set; }
		public static Lift Lift { get; private set; }
		public static RPLidar Lidar { get; private set; }

		public static LSM9D51CompassAccelerometer GyMag { get; private set; }
		public static LidarEnvironment Environment { get; private set; }

		public static void StartWidgets()
		{
			StartRangeFinders();
			StartTracks();
			StartSpatial();
			StartActivities();
			StartLift();
			StartLidar();
		}

		public static void StopWidgets()
		{
			StopLift();
			StopActivities();
			Tracks.Stop();
			StopRangeFinders();
			StopSpatial();
			GpioSharp.DeInit();
			StopLidar();

			foreach(ThreadBase thread in ThreadBase.GetRunningThreads())
			{
				Log.SysLogText(LogLevel.DEBUG, "Remaining: {0}", thread);
			}
		}

		private static void StartLidar()
		{
			Lidar = new RPLidar(Program.Config.LidarComPort);
			Lidar.Offset = Program.Config.LidarOffsetDegrees;
			Lidar.Start();
			if(Lidar.GetDeviceInfo())
			{
				Log.SysLogText(LogLevel.DEBUG, "Retrieved LIDAR info");
				Lidar.StartScan();
				Log.SysLogText(LogLevel.DEBUG, "LIDAR scan started");
			}
		}

		private static void StopLidar()
		{
			Lidar.StopScan();
			GpioSharp.Sleep(250);
			Lidar.Reset();
			GpioSharp.Sleep(250);
			Lidar.Stop();

			Log.SysLogText(LogLevel.DEBUG, "LIDAR stopped");
		}

		private static void StartLift()
		{
			Lift = new Lift();
			Lift.Start();
		}

		private static void StopLift()
		{
			Lift.Stop();
		}

		private static void StartSpatial()
		{
			GyMag = new LSM9D51CompassAccelerometer();
			GyMag.MagneticDeviation = Program.Config.MagneticDeviation;
			GyMag.XAdjust = Program.Config.CompassXAdjust;
			GyMag.YAdjust = Program.Config.CompassYAdjust;

			Environment = new LidarEnvironment(); // new Area(5, 5, .1, new PointD(2.5, 2.5));

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
