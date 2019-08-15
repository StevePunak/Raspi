using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using KanoopCommon.Logging;
using RaspiCommon.Devices.Compass;

namespace RaspiCommon.Devices.Spatial
{
	public class RPFileLidar : RPLidarBase
	{
		public String Filename { get; private set; }

		Timer _timer;
		FileStream _traceFile;

		public RPFileLidar(String filename, double vectorSize, ICompass compass)
			: base(filename.ToString(), vectorSize, compass)
		{
			Filename = filename;

			_traceFile = File.OpenRead(filename);
		}

		public override void Start()
		{
			base.Start();

			_timer = new Timer(1000);
			_timer.Elapsed += OnTimerElapsed;
			_timer.AutoReset = false;
			_timer.Start();
			ForceMultiScanMeasurement = true;
		}

		private void OnTimerElapsed(object sender, ElapsedEventArgs e)
		{
			byte[] buffer = new byte[4096];
			int bytesRead = _traceFile.Read(buffer, 0, buffer.Length);
			HandleDataReceived(buffer, bytesRead);
			if(bytesRead < buffer.Length)
			{
				_timer.Stop();
				Log.SysLogText(LogLevel.INFO, "Done");
			}
			else
			{
				_timer.Start();
			}
		}

		public override void Stop()
		{
		}

	}
}