using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RaspiCommon;
using RaspiCommon.Devices.Analog;

namespace Testing
{
	class WiringPiTest
	{
		public WiringPiTest()
		{
			bool result = WiringPi.Setup();
			ADS1115 reader = new ADS1115(0x48);
			reader.Gain = ADS1115.GainType.GAIN_6_144;
			reader.VoltageReceived += OnReader_VoltageReceived;
			Console.WriteLine($"wiring pi said {result}");
			reader.Start();

			Thread.Sleep(10000000);
		}

		private void OnReader_VoltageReceived(ADS1115.InputPin pin, double voltage)
		{
			Console.WriteLine($"{pin}: {voltage}");
		}
	}
}
