using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using KanoopCommon.Serial;

namespace Testing
{
	class SerialTest
	{
		MonoSerialPort _port;

		public SerialTest() 
		{
			Log.SysLogText(LogLevel.DEBUG, "Opening port");

			_port = new MonoSerialPort("/dev/ttyS0");
			_port.Port.BaudRate = 115200;
			_port.DataReceived += OnDataReceived;
			Console.WriteLine("opening");
			_port.Open();
			Console.WriteLine("opened");

			//_lidarPort = new MonoSerialPort("/dev/ttyS0");
			//_lidarPort.DataReceived += OnDataReceived;
			//_lidarPort.Open();

			{
				byte[] output = new byte[] { 0xa5, 0x50 };
				_port.Write(output, 0, 2);
				Console.WriteLine("wrote data");

			}

			Thread.Sleep(5000);
			_port.Close();
			Log.SysLogText(LogLevel.DEBUG, "Port closed");

			//{
			//	byte[] output = new byte[] { 0xa5, 0x20 };
			//	_port.Write(output, 0, 2);
			//}

			//while(true)
			//{
			//	DoRead();
			//}
			//Thread.Sleep(20000);
		}

		void DoRead()
		{
			//byte[] input = new byte[1024];
			//int read = _port.Read(input, 0, input.Length);
			//Log.SysLogText(LogLevel.DEBUG, "Read {0}", read);
			//Log.SysLogHex(LogLevel.DEBUG, input, 0, read);
		}

		private void OnDataReceived(byte[] buffer, int length)
		{
			Log.SysLogText(LogLevel.DEBUG, "RECV {0}", buffer.Length);
			Log.SysLogHex(LogLevel.DEBUG, buffer);
		}
	}
}
