using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KanoopCommon.Database;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.PersistentConfiguration;
using KanoopCommon.Serialization;
using RaspiCommon;
using RaspiCommon.Data.DataSource;
using RaspiCommon.Data.Entities;
using RaspiCommon.Server;
using RaspiCommon.Spatial;
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
			EnvironmentInfo info = new EnvironmentInfo()
			{
				BackwardPrimaryRange = 12.3,
				ForwardPrimaryRange = 1,
				ForwardSecondaryRange = 2.1123,
				BackwardSecondaryRange = 27,
				Bearing = 231.4,
				DestinationBearing = 7,
				DistanceLeft = 0,
				DistanceToTravel = 1
			};

			byte[] serialized = BinarySerializer.Serialize(info);

			EnvironmentInfo info2 = BinarySerializer.Deserialize<EnvironmentInfo>(serialized);
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
