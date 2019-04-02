using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KanoopCommon.Logging;
using KanoopCommon.PersistentConfiguration;
using RaspiCommon;

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
			String file = @"c:\pub\tmp\blob.bin";
			byte[] buffer = File.ReadAllBytes(file);
			using(BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
			{
				for(int offset = 0;offset < 360*4;offset++)
				{
					Double range = br.ReadDouble();
					Log.LogText(LogLevel.DEBUG, "{0}", range);
				}
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
