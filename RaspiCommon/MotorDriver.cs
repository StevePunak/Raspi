using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspberrySharp.IO.GeneralPurpose;
using RaspberrySharp.System.Timers;

namespace RaspiCommon
{
	public static class MotorDriver
	{

		static Thread _workThread;
		static int _index;
		static GpioConnection _connection;
		static bool _dieThread;

		static public bool Running { get; set; }
		static public Direction Direction { get; set; }
		public static TimeSpan SleepInterval { get; set; }

		static readonly List<List<bool>> _steps = new List<List<bool>>()
		{
			new List<bool>() { true,  false, true,  false },
			new List<bool>() { false, true,  true,  false },
			new List<bool>() { false, true,  false, true },
			new List<bool>() { true,  false, false, true },
		};

		static MotorDriver()
		{
			Log.SysLogText(LogLevel.DEBUG, "Instantiating motor driver");

			Running = false;
			Direction = Direction.Forward;
			SleepInterval = TimeSpan.FromMilliseconds(100);

			_index = 0;
		}

		public static void Start()
		{
			Log.SysLogText(LogLevel.DEBUG, "Starting motor driver");
			_dieThread = false;
			_workThread = new Thread(new ThreadStart(WorkThread));
			_workThread.Start();

			_connection = new GpioConnection(
				ProcessorPin.Gpio04.Output(), 
				ProcessorPin.Gpio17.Output(), 
				ProcessorPin.Gpio27.Output(), 
				ProcessorPin.Gpio22.Output());

			SleepInterval = TimeSpan.FromMilliseconds(5);
			Log.SysLogText(LogLevel.DEBUG, "Done starting motor driver");
		}

		public static void Stop()
		{
			_dieThread = true;
			Running = false;
			SleepInterval = TimeSpan.FromMilliseconds(100);
			_connection.Close();
		}

		static void WorkThread()
		{
			Log.SysLogText(LogLevel.DEBUG, "Thread 1");

			try
			{
				while(!_dieThread)
				{
					if(Running)
					{
						List<bool> step = _steps[_index];

//						Log.SysLogText(LogLevel.DEBUG, "pin {0} {1} {2} {3}", step[0], step[1], step[2], step[3]);
						int pinIndex = 0;
						foreach(ConnectedPin pin in _connection.Pins)
						{
							pin.Enabled = step[pinIndex++];
						}

						if(Direction == Direction.Forward)
						{
							if(++_index == 4)
								_index = 0;
						}
						else
						{
							if(--_index < 0)
								_index = 3;
						}
					}

					RaspberrySharp.System.Timers.Timer.Sleep(SleepInterval);
				}
			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.DEBUG, "Exception: {0}", e.Message);
			}
			Console.WriteLine("Motor Thread exiting");
		}
	}
}
