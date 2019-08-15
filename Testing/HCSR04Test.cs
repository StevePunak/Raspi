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
			// 0x2501CD8F
			//       *
			// 0010 0101 0000 0001 1100 1101 1000 1111
			// 0x2100CD8F
			// 0010 0001 0000 0000 1100 1101 1000 1111
			// 0x25004D8F
			// 0010 0101 0000 0000 0100 1101 1000 1111

			Pigs.InterfaceType = Pigs.PigsType.Direct;
			Pigs.DebugLogging = true;
			HCSR04_RangeFinder rf = new HCSR04_RangeFinder(GpioPin.Pin16, GpioPin.Pin25, RFDir.Front);
			rf.Start();
			while(!Quit)
			{
				//Log.SysLogText(LogLevel.DEBUG, $"{rf} {rf.Range}");
				Thread.Sleep(200);
			}
			rf.Stop();
		}

		public void PinCallback(GpioPin pin, EdgeType edgeType, UInt64 microseconds, UInt16 sequence)
		{
			Console.WriteLine($"{pin}  {edgeType}  {microseconds} {sequence}");
		}

	}
}
