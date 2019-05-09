using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Database;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.PersistentConfiguration;
using RaspiCommon;
using RaspiCommon.Data.DataSource;
using RaspiCommon.Data.Entities;
using RaspiCommon.Devices.MotorControl;
using RaspiCommon.Devices.Optics;
using TrackBot.Spatial;
using TrackBot.Tracks;
using TrackBot.TTY;

namespace TrackBot
{
	class Program
	{
		[DllImport("libgpiosharp.so")]
		public static extern void PulsePin(GpioPin pin, UInt32 microseconds);

		public static Log Log { get; private set; }
		public static RaspiConfig Config { get; private set; }

		static void Main(string[] args)
		{
			Console.WriteLine("Initializing...");

			OpenLog(args.Contains("-c"));

			Console.WriteLine("Opening config...");
			OpenConfig();

			Console.WriteLine("Running pretests...");
			Test();

			Console.WriteLine("Starting Widgets...");
			Widgets.Instance.StartWidgets();

			Console.WriteLine("Starting TTY...");
			Terminal.Run();

			Widgets.Instance.StopWidgets();

		}

		private static void Test()
		{
			try
			{
				Program.Config.TracksLeftA1Pin = GpioPin.Pin11;
				Program.Config.TracksLeftA2Pin = GpioPin.Pin13;
				Program.Config.TracksRightA1Pin = GpioPin.Pin10;
				Program.Config.TracksRightA2Pin = GpioPin.Pin09;

				Log.SysLogText(LogLevel.DEBUG, "LEFT: {0} {1}", Program.Config.TracksLeftA1Pin, Program.Config.TracksLeftA2Pin);
				Log.SysLogText(LogLevel.DEBUG, "Right: {0} {1}", Program.Config.TracksRightA1Pin, Program.Config.TracksRightA2Pin);
				Program.Config.Save();
			}
			catch(Exception e)
			{
				Console.WriteLine("TEST CODE EXCEPTION: {0}", e.Message);
			}
		}

		void Defaults()
		{
			Program.Config.RemoteImageDirectory = "/home/pi/images";
			Program.Config.RadarHost = "192.168.0.50";
			Program.Config.BlueThresholds = new ColorThreshold(Color.Blue, 150, 70);
			Program.Config.GreenThresholds = new ColorThreshold(Color.Green, 150, 100);
			Program.Config.RedThresholds = new ColorThreshold(Color.Red, 150, 70);
			Program.Config.CameraBrightness = 0;
			Program.Config.CameraContrast = 0;
			Program.Config.CameraSaturation = 0;
			Program.Config.CameraImageEffect = String.Empty;
			Program.Config.CameraColorEffect = String.Empty;
			Program.Config.CameraExposureType = String.Empty;
			Program.Config.CameraBearingOffset = 4;
			Program.Config.CameraImageDelay = TimeSpan.FromMilliseconds(1500);
		}

		private static void OpenConfig()
		{
			String      configFileName = RaspiConfig.GetDefaultConfigFileName();

			if(Directory.Exists(Path.GetDirectoryName(configFileName)))
				Directory.SetCurrentDirectory(Path.GetDirectoryName(configFileName));
			else
				Directory.CreateDirectory(Path.GetDirectoryName(configFileName));

			Log.LogText(LogLevel.DEBUG, "Opening config...");

			ConfigFile  configFile = new ConfigFile(configFileName);
			Config = (RaspiConfig)configFile.GetConfiguration(typeof(RaspiConfig));

			Config.Save();
		}

		private static void OpenLog(bool console)
		{
			Console.WriteLine("Opening log");
			Log = new Log();
			UInt32 flags = OpenFlags.CONTENT_TIMESTAMP | OpenFlags.OUTPUT_TO_FILE;
			if(console)
			{
				flags |= OpenFlags.OUTPUT_TO_CONSOLE;
			}
			Log.Open(LogLevel.ALWAYS, "/var/log/robo/robo.log", flags);
			Log.SystemLog = Log;
			Console.WriteLine("Log opened");
		}
	}
}
