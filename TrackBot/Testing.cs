using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RaspiCommon;
using RaspiCommon.Devices.Spatial;

namespace TrackBot
{
	static class Testing
	{
		public static void TestRangeFinders()
		{
			HCSR04_RangeFinder rangeFinder = new HCSR04_RangeFinder(GpioPin.Pin26, GpioPin.Pin25, RFDir.Front);
			rangeFinder.Start();

			while(true)
			{
				Thread.Sleep(1000);
				Console.WriteLine("Range: {0:0.000}m", rangeFinder.Range);
			}
		}
	}
}
