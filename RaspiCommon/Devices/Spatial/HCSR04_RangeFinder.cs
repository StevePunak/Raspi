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
using RaspiCommon.PiGpio;

namespace RaspiCommon.Devices.Spatial
{
	public class HCSR04_RangeFinder
	{
		#region Constants

		static readonly TimeSpan ValidTime = TimeSpan.FromSeconds(1);

		#endregion

		#region Trigger Thread

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
				Pigs.SetOutputPin(TriggerPin, PinState.High);
				Sleep(1);
				Pigs.SetOutputPin(TriggerPin, PinState.Low);
				return true;
			}
		}

		#endregion

		#region Public Properties

		GpioPin TriggerPin { get; set; }
		GpioPin EchoPin { get; set; }

		public TimeSpan Interval { get { return _triggerThread.Interval; } set { _triggerThread.Interval = value; } }
		public bool Running
		{
			get { return _triggerThread.State != ThreadBase.ThreadState.Stopped; }
		}

		public Double Range { get; private set; }
		public RFDir Direction { get; private set; }
		public bool Valid { get { return DateTime.UtcNow < _lastValidReading + ValidTime; } }

		#endregion

		#region Private Member Variables

		UInt64 _startSweep;
		TriggerThread _triggerThread;
		DateTime _lastValidReading;

		#endregion

		#region Static Members

		static Dictionary<GpioPin, HCSR04_RangeFinder> _rangeFinders;
		static MutexLock _lock;
		static Dictionary<GpioPin, TriggerThread> _triggerThreads;


		static HCSR04_RangeFinder()
		{
			_lock = new MutexLock();
			_triggerThreads = new Dictionary<GpioPin, TriggerThread>();
			_rangeFinders = new Dictionary<GpioPin, HCSR04_RangeFinder>();
		}

		#endregion

		#region Construct

		public HCSR04_RangeFinder(GpioPin echoPin, GpioPin triggerPin, RFDir direction)
		{
			Log.SysLogText(LogLevel.DEBUG, "Instantiating range finder on Echo {0}  Trigger {1}", echoPin, triggerPin);

			try
			{
				_lock.Lock();

				EchoPin = echoPin;
				TriggerPin = triggerPin;
				Range = 99;
				Direction = direction;

				Pigs.Instance.SetMode(triggerPin, PinMode.Input);

				if(_triggerThreads.TryGetValue(TriggerPin, out _triggerThread) == false)
				{
					_triggerThread = new TriggerThread(TriggerPin);
					_triggerThreads.Add(TriggerPin, _triggerThread);
				}
				_triggerThread.Count++;

				if(_rangeFinders.ContainsKey(EchoPin) == false)
				{
					_rangeFinders.Add(echoPin, this);
				}
				else
				{
					_rangeFinders[EchoPin] = this;
				}
			}
			finally
			{
				_lock.Unlock();
			}
		}

		#endregion

		#region Range Detection

		public static void OnPinEdge(GpioPin pin, EdgeType edgeType, UInt64 microseconds, UInt16 sequence)
		{
			//Log.SysLogText(LogLevel.DEBUG, $"{pin} {edgeType} {microseconds} {sequence}");
			try
			{
				HCSR04_RangeFinder rangeFinder;
				_lock.Lock();
				if(_rangeFinders.TryGetValue(pin, out rangeFinder))
				{
					if(edgeType == EdgeType.Rising)
					{
						rangeFinder._startSweep = microseconds;
					}
					else
					{
						Double interval = (Double)(microseconds - rangeFinder._startSweep);
						Double meters = interval / 5800;
						//if(rangeFinder.Direction == RFDir.Front)
						//{
						//	Log.SysLogText(LogLevel.DEBUG, $"{rangeFinder.Direction} {pin} {edgeType} {meters} {microseconds} {sequence} because {microseconds} - {rangeFinder._startSweep} == {interval}");
						//}
						if(meters < 1 && meters > .02)
						{
							rangeFinder.Range = meters;
							rangeFinder._lastValidReading = DateTime.UtcNow;
						}
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("EXCEPTION PinEdge {0}", e.Message);
				Console.WriteLine("EXCEPTION PinEdge pin {0} {1} {2}", pin, edgeType, microseconds);
			}
			finally
			{
				_lock.Unlock();
			}
		}

		#endregion

		#region Start / Stop Handlers

		public void Start()
		{
			Log.SysLogText(LogLevel.DEBUG, "-------------    Starting range finder (State {0}", _triggerThread.State);

			Pigs.StartInputPin(EchoPin, EdgeType.Both, OnPinEdge);

			_lock.Lock();
			if(!Running)
			{
				_triggerThread.Start();
			}
			_lock.Unlock();

			Log.SysLogText(LogLevel.DEBUG, "Done starting range finder");
		}

		public void Stop()
		{
			_lock.Lock();
			if(_triggerThread.Count == 1)       // we are the last one
			{
				_triggerThread.Stop();
				_triggerThreads.Remove(EchoPin);
			}
			_triggerThread.Count--;
			Pigs.StopInputPin(EchoPin);
			_lock.Unlock();
		}

		#endregion
	}
}
