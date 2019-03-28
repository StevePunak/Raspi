#undef ASYNC_READ
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

namespace KanoopCommon.Serial
{
	public delegate void DataReceivedHandler(byte[] buffer, int length);

	public class MonoSerialPort
	{
		public event DataReceivedHandler DataReceived;

		public String PortName { get; private set; }

		public SerialPort Port { get; private set; }
		byte[] _recvBuffer;

		ReadDataThread _thread;

		public bool IsOpen { get { return Port.IsOpen; } }

		public MonoSerialPort(String portName)
		{
			PortName = portName;

			Port = new SerialPort(portName, 115200);
			_recvBuffer = new byte[16364];
#if !ASYNC_READ
			_thread = new ReadDataThread(this);
#endif

			DataReceived += delegate { };
		}

		public void Open()
		{
			Port.Open();

#if ASYNC_READ
			BeginRead();
#else
			_thread.Start();
#endif
		}

		public void Close()
		{
#if !ASYNC_READ
			Log.SysLogText(LogLevel.DEBUG, "Aborting {0}", _thread);
			_thread.Abort();
			// _thread.Stop(500);
#endif
			Port.Close();
		}

		public void BeginRead()
		{
			Port.BaseStream.BeginRead(_recvBuffer, 0, _recvBuffer.Length, ReadDataCallback, this);
		}

		private void ReadDataCallback(IAsyncResult ar)
		{
			try
			{
				int bytes = Port.BaseStream.EndRead(ar);

				if(bytes > 0)
				{
					Log.SysLogText(LogLevel.DEBUG, "ASYNC RECV {0} bytes:", bytes);
					Log.SysLogHex(LogLevel.DEBUG, _recvBuffer, 0, bytes);
					DataReceived(_recvBuffer, bytes);
					BeginRead();
				}
				else
				{
					Console.WriteLine("read timed out 2");
				}
			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.WARNING, "Serial port exception: {0}", e.Message);
			}
		}

		public void Write(byte[] buffer)
		{
			Write(buffer, 0, buffer.Length);
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			Port.Write(buffer, offset, count);
		}

		class ReadDataThread : ThreadBase
		{
			MonoSerialPort _parent;

			public ReadDataThread(MonoSerialPort parent)
				: base(typeof(ReadDataThread).Name)
			{
				_parent = parent;
			}

			protected override bool OnRun()
			{
				try
				{
					int bytes = _parent.Port.Read(_parent._recvBuffer, 0, _parent._recvBuffer.Length);

					if(bytes > 0)
					{
//						Log.SysLogText(LogLevel.DEBUG, "STD RECV {0} bytes:", bytes);
						//Log.SysLogHex(LogLevel.DEBUG, _parent._recvBuffer, 0, bytes);
						_parent.DataReceived(_parent._recvBuffer, bytes);
					}
					else
					{
						Console.WriteLine("read timed out");
					}
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
				return true;
			}

			protected override bool OnException(Exception e)
			{
				return true;
			}
		}
	}
}
