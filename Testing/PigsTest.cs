using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.PiGpio;

namespace Testing
{
	class PigsTest : TestBase
	{
		void PinCallback1(GpioPin pin, EdgeType edgeType, UInt64 microseconds, UInt16 sequence)
		{
			Log.SysLogText(LogLevel.DEBUG, $"Callback1 {pin} {edgeType} {microseconds}");
		}

		void PinCallback2(GpioPin pin, EdgeType edgeType, UInt64 microseconds)
		{
			Log.SysLogText(LogLevel.DEBUG, $"Callback2 {pin} {edgeType} {microseconds}");
		}

		protected override void Run()
		{
			Pigs.InterfaceType = Pigs.PigsType.Direct;
			Pigs.StartInputPin(GpioPin.Pin05, EdgeType.Both, PinCallback1);
			Pigs.StartInputPin(GpioPin.Pin06, EdgeType.Both, PinCallback1);
			Pigs.SetPullUp(GpioPin.Pin05, PullUp.Down);
			while(!Quit)
			{
				Log.SysLogText(LogLevel.DEBUG, "Setting high");
				Pigs.SetOutputPin(GpioPin.Pin26, true);
				Sleep(1000);
				Log.SysLogText(LogLevel.DEBUG, "Setting low");
				Pigs.SetOutputPin(GpioPin.Pin26, false);
				Sleep(1000);
			}
			Pigs.Stop();
		}
	}
}
