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
using RaspiCommon.Network;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.DeadReckoning;
using RaspiCommon.Spatial.Imaging;
using TrackBotCommon.Environs;

namespace Radar
{
	static class Program
	{
		public static RaspiConfig Config { get; private set; }
		public static Log Log { get; private set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			OpenLog();

			OpenConfig();

			Test();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new RadarForm());
		}

		static void Test()
		{
			//TestImage();

			TrackDataSource tds = DataSourceFactory.Create<TrackDataSource>(Config.DBCredentials);
			DeadReckoningEnvironment env = new DeadReckoningEnvironment("ManCave", 10, 10, .1, 0, new PointD(8, 8));
			////tds.DeleteDREnvironment("ManCave");
			////tds.CreateDREnvironment(env);
			tds.GetDREnvironment("ManCave", out env);
			//byte[] serialized = env.Serialize();

			PointD point = env.GetGridPoint(env.CurrentLocation, 180, 10);
			//env = new DeadReckoningEnvironment(serialized);
		}


		static void TestImage()
		{
			//int resx = 3280;
			//int resy = 2464;
			int resx = 800;
			int resy = 600;

			int cols = ((resx + 31) / 32) * 32;
			int rows = ((resy + 15) / 16) * 16;

			String file = @"\\raspi\pi\tmp\image.bgr";
			byte[] data = File.ReadAllBytes(file);
			Mat image = new Mat(new Size(cols, rows), DepthType.Cv8U, 3);
			unsafe
			{
				IntPtr ptr = image.DataPointer;
				Marshal.Copy(data, 0, ptr, (rows * cols * 3));

				int row, col;
				for(col = 0;col < image.Cols;col++)
				{
					for(row = 0;row < image.Rows;row++)
					{
						// 282, 171
						if(col == 282 && row == 171)
						{
							byte* b = (byte*)(ptr.ToPointer()) + ((row * resx) * 3) + (col * 3);
							byte b1 = b[0];
							byte b2 = b[1];
							byte b3 = b[2];
						}
					}

				}
			}

			Mat[] colors = image.Split();
			CvInvoke.Merge(new VectorOfMat(colors[2], colors[1], colors[0]), image);
			image.Save(@"c:\pub\tmp\output.bmp");

			{
				MCvScalar lowRange = new MCvScalar(50, 0, 0);
				MCvScalar topRange = new MCvScalar(256, 0, 0);
				Mat outputImage = new Mat(new Size(cols, rows), DepthType.Cv8U, 3);
				CvInvoke.InRange(image, new ScalarArray(lowRange), new ScalarArray(topRange), outputImage);
				outputImage.Save(@"c:\pub\tmp\blue.bmp");
			}
			{
				MCvScalar lowRange = new MCvScalar(0, 0, 50);
				MCvScalar topRange = new MCvScalar(0, 0, 256);
				Mat outputImage = new Mat(new Size(cols, rows), DepthType.Cv8U, 3);
				CvInvoke.InRange(image, new ScalarArray(lowRange), new ScalarArray(topRange), outputImage);
				outputImage.Save(@"c:\pub\tmp\red.bmp");
			}
			{
				MCvScalar lowRange = new MCvScalar(0, 50, 0);
				MCvScalar topRange = new MCvScalar(0, 256, 0);
				Mat outputImage = new Mat(new Size(cols, rows), DepthType.Cv8U, 3);
				CvInvoke.InRange(image, new ScalarArray(lowRange), new ScalarArray(topRange), outputImage);
				outputImage.Save(@"c:\pub\tmp\green.bmp");
			}
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
