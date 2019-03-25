using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KanoopCommon.Logging;
using KanoopCommon.PersistentConfiguration;
using RaspiCommon;

namespace RaspiForm
{
	static class Program
	{
		public static Log Log { get; private set; }
		public static RaspiConfig Config { get; private set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Log = new Log();
			Log.Open(LogLevel.ALWAYS, "/tmp/raspi.log", OpenFlags.COMBO_VERBOSE | OpenFlags.OUTPUT_TO_DEBUG | OpenFlags.OUTPUT_TO_FILE);
			Log.SystemLog = Log;

			Log.LogText(LogLevel.DEBUG, "Opened Log");

			LoadConfig();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		static void LoadConfig()
		{
			String      configFileName = RaspiConfig.GetDefaultConfigFileName();

			if(Directory.Exists(Path.GetDirectoryName(configFileName)))
				Directory.SetCurrentDirectory(Path.GetDirectoryName(configFileName));
			else
				Directory.CreateDirectory(Path.GetDirectoryName(configFileName));

			ConfigFile  configFile = new ConfigFile(configFileName);
			Config = (RaspiConfig)configFile.GetConfiguration(typeof(RaspiConfig));

			Config.Save();
		}

	}
}
