using System;
using RaspiCommon.Spatial;

namespace RaspiCommon.Devices.Compass
{
	public interface ICompass
	{
		event NewBearingHandler NewBearing;
		Double Bearing { get; }
	}
}
