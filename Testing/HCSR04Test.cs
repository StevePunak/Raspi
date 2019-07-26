using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.Devices.Spatial;
using RaspiCommon.PiGpio;

namespace Testing
{
	class HCSR04Test : TestBase
	{
		protected override void Run()
		{
			Pigs.InterfaceType = Pigs.PigsType.Direct;
			HCSR04_RangeFinder rf = new HCSR04_RangeFinder(GpioPin.Pin20, GpioPin.Pin16, RFDir.Front);
			rf.Start();
			while(!Quit)
			{
				Log.SysLogText(LogLevel.DEBUG, $"{rf} {rf.Range}");
				Thread.Sleep(200);
			}
			rf.Stop();
		}
	}
}
