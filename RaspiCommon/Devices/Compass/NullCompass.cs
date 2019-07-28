using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Spatial;

namespace RaspiCommon.Devices.Compass
{
	public class NullCompass : ICompass
	{
		public double Bearing { get; set; }
		public double MagneticDeviation { get; set; }

		public event NewBearingHandler NewBearing;

		public NullCompass()
		{
			NewBearing += delegate {};
			NewBearing(0);
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}
	}
}
