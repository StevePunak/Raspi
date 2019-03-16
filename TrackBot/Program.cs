using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using KanoopCommon.PersistentConfiguration;
using RaspiCommon;
using TrackBot.Tracks;
using TrackBot.TTY;

namespace TrackBot
{
	class Program
	{
		public static Log Log { get; private set; }
		public static RaspiConfig Config { get; private set; }

		static void Main(string[] args)
		{
			Test();

			OpenConfig();
			OpenLog();

			Widgets.StartWidgets();

			Terminal.Run();

			Widgets.StopWidgets();

		}

		private static void Test()
		{
		}

		static void Drive()
		{
			int speed = 75;

			GpioSharp.Sleep(TimeSpan.FromMilliseconds(1000));

			Widgets.Tracks.Speed = 60;

			while(true)
			{
				Console.WriteLine("range: {0:0.00}", Widgets.RangeFinder.Range);

				if(Widgets.RangeFinder.Range < .2)
				{
					Console.WriteLine("Hit range limit");
					Widgets.Tracks.Speed = -speed;

					GpioSharp.Sleep(TimeSpan.FromMilliseconds(500));

					Widgets.Tracks.Stop();

					break;

				}
				if(Console.KeyAvailable)
				{
					Console.WriteLine("got a key");
					break;
				}

				Console.WriteLine("range: {0:0.00}", Widgets.RangeFinder.Range);
				Thread.Sleep(100);
			}
		}

		private static void OpenConfig()
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

		private static void OpenLog()
		{
			Log = new Log();
			Log.Open(LogLevel.ALWAYS, Config.LogFileName, OpenFlags.COMBO_VERBOSE | OpenFlags.OUTPUT_TO_CONSOLE | OpenFlags.OUTPUT_TO_FILE);
			Log.SystemLog = Log;
		}
	}
}
