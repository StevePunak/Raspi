using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using KanoopCommon.Database;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.PersistentConfiguration;
using KanoopCommon.Serialization;
using RaspiCommon;
using RaspiCommon.Data.DataSource;
using RaspiCommon.Data.Entities;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Devices.Locomotion;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Network;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.DeadReckoning;
using RaspiCommon.Spatial.LidarImaging;
using TrackBotCommon.Environs;

namespace Radar
{
	static class Program
	{
		public static RaspiConfig Config { get; private set; }
		public static Log Log { get; private set; }
		public static NullCompass Compass { get; private set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			LEDImageAnalysis.Compass = Compass = new NullCompass();

			OpenLog();

			OpenConfig();

			SetConfigDefaults();

			Test();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new RadarForm());
		}

		static void Test()
		{
			//TestImage();

			//Program.Config.RemoteImageDirectory = @"\\raspi\pi\images";
			//Program.Config.Save();

			//TrackDataSource tds = DataSourceFactory.Create<TrackDataSource>(Config.DBCredentials);
			//DeadReckoningEnvironment env = new DeadReckoningEnvironment("ManCave", 10, 10, .1, 0, new PointD(8, 8));
			//////tds.DeleteDREnvironment("ManCave");
			//////tds.CreateDREnvironment(env);
			//tds.GetDREnvironment("ManCave", out env);
			////byte[] serialized = env.Serialize();

			//PointD point = env.GetGridPoint(env.CurrentLocation, 180, 10);
			////env = new DeadReckoningEnvironment(serialized);
		}

		static void SetConfigDefaults()
		{
			Config.RadarHost = "thufir";
			Config.RemoteImageDirectory = @"\\raspi\pi\images";
		}

		static void TestImage()
		{
			LEDImageAnalysis.Width = 800;
			LEDImageAnalysis.Height = 600;

			String workDir = @"c:\pub\tmp\junk";
			Camera.ImageDirectory = workDir;

			RaspiCamCv camera = new RaspiCamCv();
			LEDImageAnalysis ledAnalysis = new LEDImageAnalysis(camera);

			String inputFile = Path.Combine(workDir, "fullimage.bmp");
			Mat image = new Mat(inputFile);
//		Camera.ConvertImage(outputFile, new Size(LEDImageAnalysis.Width, LEDImageAnalysis.Height), ImageType.RawRGB, ImageType.Bitmap, 0, out outputFile);

			List <String> files;
			LEDPositionList leds;
			LEDImageAnalysis.AnalyzeImage(image, workDir, out files, out leds);

			ledAnalysis.LEDs = leds;

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

		private static void OpenLog()
		{
			Console.WriteLine("Opening log");
			Log = new Log();
			UInt32 flags = OpenFlags.CONTENT_TIMESTAMP | OpenFlags.OUTPUT_TO_FILE | OpenFlags.OUTPUT_TO_DEBUG;
			Log.Open(LogLevel.ALWAYS, "robo.log", flags);
			Log.SystemLog = Log;
			Console.WriteLine("Log opened");
		}
	}
}
