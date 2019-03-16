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

namespace RaspiCommon
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

		public class RangeFinderThread : ThreadBase
		{
			public HCSR04_RangeFinder Parent { get; private set; }

			public RangeFinderThread(HCSR04_RangeFinder parent)
				: base(parent.ToString())
			{
				Parent = parent;
				Interval = TimeSpan.FromMilliseconds(200);
			}

			public override bool Start()
			{
				return base.Start();
			}

			protected override bool OnRun()
			{
				PulsePin(Parent._outputPin, 10);
				return true;
			}
		}

		RangeFinderThread _workThread;
		GpioPin _outputPin, _inputPin;

		static UInt64 _startSweep;

		public TimeSpan Interval { get { return _workThread.Interval; } set { _workThread.Interval = value; } }
		public bool Running
		{
			get { return _workThread.State != ThreadBase.ThreadState.Stopped;  }
		}

		public Double Range { get; private set; }

		public HCSR04_RangeFinder(GpioPin inputPin, GpioPin outputPin)
		{
			Log.SysLogText(LogLevel.DEBUG, "Instantiating range finder on Echo {0}  Trigger {1}", inputPin, outputPin);
			_inputPin = inputPin;
			_outputPin = outputPin;
			Range = 0;

			_workThread = new RangeFinderThread(this);

			Interval = TimeSpan.FromMilliseconds(1000);
		}

		public void OnPinEdge(GpioPin pin, EdgeType edgeType, UInt64 nanoseconds)
		{
			if(edgeType == EdgeType.Rising)
			{
				_startSweep = nanoseconds;
			}
			else
			{
				Double meters = (Double)(nanoseconds - _startSweep);
				meters = meters / 5800000; // 1000 / 5800;
				Range = meters;
			}
		}

		public void Start()
		{
			Log.SysLogText(LogLevel.DEBUG, "-------------    Starting range finder (State {0}", _workThread.State);

			IntPtr callback = Marshal.GetFunctionPointerForDelegate(new GPIOInputCallback(OnPinEdge));
			RegisterDeviceCallback(callback, _inputPin, EdgeType.Falling | EdgeType.Rising, InputSetting.OpenSource);

			if(!Running)
			{
				_workThread.Start();
			}

			Interval = TimeSpan.FromMilliseconds(100);
			Log.SysLogText(LogLevel.DEBUG, "Done starting range finder");
		}

		public void Stop()
		{
			UnregisterCallback(_inputPin);
			if(Running)
			{
				_workThread.Stop();
			}
		}

	}
}
