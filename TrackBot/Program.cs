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
				Program.Config.RemoteImageDirectory = "/home/pi/images";

				//Program.Config.BlueThresholds = new ColorThreshold(Color.Blue, 150, 70);
				//Program.Config.GreenThresholds = new ColorThreshold(Color.Green, 150, 100);
				//Program.Config.RedThresholds = new ColorThreshold(Color.Red, 150, 70); 
				//Program.Config.CameraBrightness = 0;
				//Program.Config.CameraContrast = 0;
				//Program.Config.CameraSaturation = 0;
				//Program.Config.CameraImageEffect = String.Empty;
				//Program.Config.CameraColorEffect = String.Empty;
				//Program.Config.CameraExposureType = String.Empty;
				//Program.Config.CameraBearingOffset = 4;
				//Program.Config.CameraImageDelay = TimeSpan.FromMilliseconds(1500);

				Program.Config.Save();
			}
			catch(Exception e)
			{
				Console.WriteLine("TEST CODE EXCEPTION: {0}", e.Message);
			}

#if zero
			Grid grid = new Grid("TrackGrid", 15, 15, .1);

			Cell cells = grid.GetCellAtLocation(new PointD(4, 7.5));
#endif

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
