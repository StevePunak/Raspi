using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspiCommon.Devices.Analog;

namespace Testing
{
	class ADS1115Test
	{
		public ADS1115Test()
		{
			ADS1115 reader = new ADS1115(0x48)
			{
				Interval = TimeSpan.FromMilliseconds(250),
			};
			reader.Gain = ADS1115.GainType.GAIN_6_144;
			reader.VoltageReceived += OnReader_VoltageReceived;
			reader.Start();

			Thread.Sleep(10000000);
		}

		private void OnReader_VoltageReceived(ADS1115.InputPin pin, double voltage)
		{
			Console.WriteLine($"{pin}  {voltage}");
		}
	}
}
