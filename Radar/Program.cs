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
using KanoopCommon.Extensions;
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
using RaspiCommon.GraphicalHelp;
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
			SolidColorAnalysis.Compass = Compass = new NullCompass();

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
			int x = 30;
			String s = x.ToString().PadLeft(4, '0');
			Double diff = (Double)(67.024).AngularDifference(77.674, SpinDirection.CounterClockwise);
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
			Program.Config.BlueThresholds = new ColorThreshold(Color.Blue, 150, 70);
			Program.Config.GreenThresholds = new ColorThreshold(Color.Green, 150, 100);
			Program.Config.RedThresholds = new ColorThreshold(Color.Red, 150, 70);
			Config.Save();

			SolidColorAnalysis.Width = 800;
			SolidColorAnalysis.Height = 600;
			SolidColorAnalysis.ColorThresholds[Color.Blue] = Program.Config.BlueThresholds;
			SolidColorAnalysis.ColorThresholds[Color.Green] = Program.Config.GreenThresholds;
			SolidColorAnalysis.ColorThresholds[Color.Red] = Program.Config.RedThresholds;

			String remoteFile = @"\\raspi\pi\images\snap-0003.bmp";
			String workDir = @"c:\pub\tmp\junk";
			String inputFile = Path.Combine(workDir, Path.GetFileName(remoteFile));

			File.Copy(remoteFile, inputFile, true);
			Camera.ImageDirectory = workDir;
			SolidColorAnalysis.ImageAnalysisDirectory = workDir;

			Mat image = new Mat(inputFile);
//		Camera.ConvertImage(outputFile, new Size(LEDImageAnalysis.Width, LEDImageAnalysis.Height), ImageType.RawRGB, ImageType.Bitmap, 0, out outputFile);

			SolidColorAnalysis analysis = new SolidColorAnalysis(new NullCamera());
			analysis.AnalyzeImage(image, new Size(6, 6)); //, workDir, out files, out leds, out candidates);
			//ImageAnalysis analysis = new ImageAnalysis(files, leds, candidates);
			//analysis.Serialize();

			//ledAnalysis.LEDs = leds;

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
