using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Database;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using RaspiCommon;
using RaspiCommon.Data.DataSource;
using RaspiCommon.Data.Entities;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Server;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.Imaging;
using TrackBot.ForkLift;
using TrackBot.Spatial;
using TrackBot.Tracks;
using TrackBotCommon.Environs;

namespace TrackBot
{
	class Widgets
	{
		public static BotTracks Tracks { get; private set; }
		public static Dictionary<RFDir, HCSR04_RangeFinder> RangeFinders { get; private set; }
		public static Lift Lift { get; private set; }

		public static LSM9D51CompassAccelerometer GyMag { get; private set; }
		public static IImageEnvironment ImageEnvironment { get; private set; }
		public static ILandscape Landscape { get; private set; }
		public static SpatialPoll SpatialPollThread { get; private set; }

		public static SaveImageThread SaveImageThread { get; set; }
		public static TelemetryServer Server { get; private set; }

		public static TrackDataSource DataSource { get; private set; }

		public static void StartWidgets()
		{
			StartDatabase();
			StartRangeFinders();
			StartTracks();
			StartSpatial();
			StartActivities();
			StartLift();
			StartSaveImageThread();
			StartSpatialPolling();
		}

		public static void StopWidgets()
		{
			StopSpatialPolling();
			StopLift();
			StopActivities();
			Tracks.Stop();
			StopRangeFinders();
			StopSpatial();
			GpioSharp.DeInit();
			StopSaveImageThread();
			StopDatabase();

			foreach(ThreadBase thread in ThreadBase.GetRunningThreads())
			{
				Log.SysLogText(LogLevel.DEBUG, "Remaining: {0}", thread);
			}
		}

		private static void StartDatabase()
		{
			try
			{
				TrackBotLandscape landscape;
				DataSource = DataSourceFactory.Create<TrackDataSource>(Program.Config.DBCredentials);
				if(DataSource.LandscapeGet<TrackBotLandscape>("Man Cave", out landscape).ResultCode != DBResult.Result.Success)
				{
					throw new TrackBotException("Failed to get landscape");
				}
				Landscape = landscape;
			}
			catch(Exception e)
			{
				Console.WriteLine("Widgets Start Exception: {0}", e.Message);
			}
		}

		private static void StopDatabase()
		{
		}

		private static void StartTelemetryServer()
		{
			Server = new TelemetryServer((TrackLidar)ImageEnvironment, Landscape, GyMag, Program.Config.RadarHost, "trackbot-lidar");
			Server.Start();
		}

		private static void StopTelemetryServer()
		{
			Server.Stop();
			Server = null;
		}

		private static void StartSpatialPolling()
		{
			SpatialPollThread = new SpatialPoll();
			SpatialPollThread.Start();
		}

		private static void StopSpatialPolling()
		{
			SpatialPollThread.Stop();
		}

		private static void StartSaveImageThread()
		{
			Log.SysLogText(LogLevel.DEBUG, "Starting save image thread");
			SaveImageThread = new SaveImageThread();
			SaveImageThread.Start();
		}

		private static void StopSaveImageThread()
		{
			SaveImageThread.Stop();
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
			GyMag.NewBearing += OnNewBearing;

			List<String> ports = new List<String>(SerialPort.GetPortNames());
			ports.TerminateAll();
			if(ports.Contains(Program.Config.LidarComPort) == true)
			{
				if(Program.Config.RadarHost == null)
				{
					Program.Config.RadarHost = "192.168.0.50";
					Program.Config.Save();
				}
				ImageEnvironment = new TrackLidar(Program.Config.LidarMetersSquare, Program.Config.LidarPixelsPerMeter);
				StartTelemetryServer();
			}
			else
			{
				Console.WriteLine("Lidar com port {0} does not exist... initializing virtual environment", Program.Config.LidarComPort);
				ImageEnvironment = new VirtualEnvironment();
			}



			ImageEnvironment.Start();
		}

		private static void StopSpatial()
		{
			if(ImageEnvironment is TrackLidar)
			{
				StopTelemetryServer();
			}
			ImageEnvironment.Stop();
		}

		private static void OnNewBearing(double bearing)
		{
			if(ImageEnvironment != null)
			{
				ImageEnvironment.Bearing = bearing;
			}
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
