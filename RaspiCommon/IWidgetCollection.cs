using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.Imaging;

namespace RaspiCommon
{
	public interface IWidgetCollection
	{
		event NewBearingHandler BearingChanged;
		event ForwardPrimaryRangeHandler ForwardPrimaryRange;
		event BackwardPrimaryRangeHandler BackwardPrimaryRange;
		event ForwardSecondaryRangeHandler ForwardSecondaryRange;
		event BackwardSecondaryRangeHandler BackwardSecondaryRange;
		event NewDestinationBearingHandler NewDestinationBearing;
		event DistanceToTravelHandler DistanceToTravel;
		event DistanceLeftHandler DistanceLeft;
		event FuzzyPathChangedHandler FuzzyPathChanged;
		event LandmarksChangedHandler LandmarksChanged;
		event BarriersChangedHandler BarriersChanged;

		ICompass Compass { get; }
		IImageEnvironment ImageEnvironment { get; }
		ILandscape Landscape { get; }
		Chassis Chassis { get; }
	}
}
