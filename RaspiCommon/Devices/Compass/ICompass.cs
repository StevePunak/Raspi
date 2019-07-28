using System;
using RaspiCommon.Spatial;

namespace RaspiCommon.Devices.Compass
{
	public interface ICompass
	{
		event NewBearingHandler NewBearing;
		Double Bearing { get; }
		Double MagneticDeviation { get; set; }

		void Start();
		void Stop();
	}
}
