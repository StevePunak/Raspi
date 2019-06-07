using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using RaspberrySharp.IO.GeneralPurpose;

namespace RaspiCommon.Devices.Spatial
{
	public class HCSR04_RangeFinder
	{
		[DllImport("libgpiosharp.so")]
		public static extern int RegisterDeviceCallback(IntPtr cb, GpioPin gpioPin, EdgeType edgeType, InputSetting input);

		[DllImport("libgpiosharp.so")]
		public static extern void UnregisterCallback(GpioPin pin);

		[DllImport("libgpiosharp.so")]
		public static extern void SetOutputPin(GpioPin pin, PinState state);

		[DllImport("libgpiosharp.so")]
		public static extern void PulsePin(GpioPin pin, UInt32 microseconds);

		static MutexLock _lock;
		static Dictionary<GpioPin, TriggerThread> _triggerThreads;

		public class TriggerThread : ThreadBase
		{
			public GpioPin TriggerPin { get; private set; }
			public int Count { get; set; }

			public TriggerThread(GpioPin triggerPin)
				: base(String.Format("Trigger {0}", triggerPin))
			{
				TriggerPin = triggerPin;
				Interval = TimeSpan.FromMilliseconds(200);
			}

			public override bool Start()
			{
				return base.Start();
			}

			protected override bool OnRun()
			{
				PulsePin(TriggerPin, 10);
				return true;
			}
		}

		GpioPin _triggerPin, _echoPin;

		UInt64 _startSweep;

		TriggerThread _triggerThread;

		public TimeSpan Interval { get { return _triggerThread.Interval; } set { _triggerThread.Interval = value; } }
		public bool Running
		{
			get { return _triggerThread.State != ThreadBase.ThreadState.Stopped;  }
		}

		public Double Range { get; private set; }

		public RFDir Direction { get; private set; }

		static Dictionary<GpioPin, HCSR04_RangeFinder> _rangeFinders;

		static HCSR04_RangeFinder()
		{
			_lock = new MutexLock();
			_triggerThreads = new Dictionary<GpioPin, TriggerThread>();
			_rangeFinders = new Dictionary<GpioPin, HCSR04_RangeFinder>();
		}

		public HCSR04_RangeFinder(GpioPin echoPin, GpioPin triggerPin, RFDir direction)
		{
			Log.SysLogText(LogLevel.DEBUG, "Instantiating range finder on Echo {0}  Trigger {1}", echoPin, triggerPin);
			_lock.Lock();

			_echoPin = echoPin;
			_triggerPin = triggerPin;
			Range = 99;
			Direction = direction;

			if(_triggerThreads.TryGetValue(_triggerPin, out _triggerThread) == false)
			{
				_triggerThread = new TriggerThread(_triggerPin);
				_triggerThreads.Add(_triggerPin, _triggerThread);
			}
			_triggerThread.Count++;

			if(_rangeFinders.ContainsKey(_echoPin) == false)
			{
				_rangeFinders.Add(echoPin, this);
			}
			else
			{
				_rangeFinders[_echoPin] = this;
			}
			_lock.Unlock();
		}

		public static void OnPinEdge(GpioPin pin, EdgeType edgeType, UInt64 nanoseconds)
		{

			try
			{
				HCSR04_RangeFinder rangeFinder;
				_lock.Lock();
				if(_rangeFinders.TryGetValue(pin, out rangeFinder))
				{
					if(edgeType == EdgeType.Rising)
					{
						rangeFinder._startSweep = nanoseconds;
					}
					else
					{
						Double meters = (Double)(nanoseconds - rangeFinder._startSweep);
						meters = meters / 5800000;
						if(meters < 1)
						{
							rangeFinder.Range = meters;
						}
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("EXCEPTION PinEdge {0}", e.Message);
				Console.WriteLine("EXCEPTION PinEdge pin {0} {1} {2}", pin, edgeType, nanoseconds);
			}
			finally
			{
				_lock.Unlock();
			}
		}

		public void Start()
		{
			Log.SysLogText(LogLevel.DEBUG, "-------------    Starting range finder (State {0}", _triggerThread.State);

			IntPtr callback = Marshal.GetFunctionPointerForDelegate(new GPIOInputCallback(OnPinEdge));
			RegisterDeviceCallback(callback, _echoPin, EdgeType.Falling | EdgeType.Rising, InputSetting.OpenSource);

			_lock.Lock();
			if(!Running)
			{
				_triggerThread.Start();
			}
			_lock.Unlock();

			Interval = TimeSpan.FromMilliseconds(100);
			Log.SysLogText(LogLevel.DEBUG, "Done starting range finder");
		}

		public void Stop()
		{
			_lock.Lock();
			if(_triggerThread.Count == 1)       // we are the last one
			{
				_triggerThread.Stop();
				_triggerThreads.Remove(_echoPin);
			}
			_triggerThread.Count--;
			UnregisterCallback(_echoPin);
			_lock.Unlock();
		}

	}
}
