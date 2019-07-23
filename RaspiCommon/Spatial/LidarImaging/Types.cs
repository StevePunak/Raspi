using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Spatial.LidarImaging
{
	public delegate void FuzzyPathChangedHandler(FuzzyPath path);
	public delegate void LandmarksChangedHandler(ImageVectorList landmarks);
	public delegate void BarriersChangedHandler(BarrierList barriers);
	public delegate void CompassOffsetChangedHandler(Double offset);
}
