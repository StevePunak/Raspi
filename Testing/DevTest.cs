using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;

namespace Testing
{
	class DevTest : TestBase
	{
		FileStream _fs;
		protected override void Run()
		{
			String device = "/dev/pigpio2";
			_fs = new FileStream(device, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
			Console.WriteLine($"opened {device} {_fs}");
			while(!Quit)
			{
				StartRead();
				//				int bytesRead = _fs.Read(readBuffer, 0, 12);
				//				Console.WriteLine($"read {bytesRead}");
				Sleep(1000000);
			}
		}

		void StartRead()
		{
			byte[] readBuffer = new byte[12];
			_fs.BeginRead(readBuffer, 0, 12, AsyncRead, this);
		}

		void AsyncRead(IAsyncResult result)
		{
			try
			{
				int bytesRead = _fs.EndRead(result);
				Log.SysLogText(LogLevel.DEBUG, $"read {bytesRead}");
				StartRead();
			}
			catch{ }
		}

		protected override bool OnStop()
		{
			_fs.Close();
			return base.OnStop();
		}
	}
}
