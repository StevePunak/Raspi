using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Devices.Analog;

namespace Testing
{
	class BMP280Test
	{
		public BMP280Test()
		{
			BMP280 device = new BMP280();
			device.GetTemperature();
		}
	}
}
