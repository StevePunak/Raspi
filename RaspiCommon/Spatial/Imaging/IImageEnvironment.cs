using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Geometry;

namespace RaspiCommon.Spatial.Imaging
{
	public interface IImageEnvironment
	{
		event FuzzyPathChangedHandler FuzzyPathChanged;
		event LandmarksChangedHandler LandmarksChanged;
		event BarriersChangedHandler BarriersChanged;

		void Start();
		void Stop();

		IVector[] Vectors { get; }

		Double Range { get; }
		Double CompassOffset { get; set; }
		Double Bearing { get; set; }
		Double DebugAngle { get; set; }

		Size PixelSize { get; }
		Double MetersSquare { get; }
		Double PixelsPerMeter { get; set; }
		Double RenderPixelsPerMeter { get; set; }
		Double VectorSize { get; }
		PointD Location { get; set; }
		PointD RelativeLocation { get; set; }
		Double RangeFuzz { get; set; }
		FuzzyPath FuzzyPath { get; set; }

		void ProcessImage(Mat image, Double imageOrientation, Double imagePixelsPerMeter);
		Mat GetEnvironmentImage(bool drawDebugLines);

		Double GetRangeAtBearing(Double bearing);
		Double FuzzyRangeAtBearing(Double bearing, Double fuzz);
		Double ShortestRangeAtBearing(Double bearing, Double fuzz);
		DateTime GetLastSampleTimeAtBearing(Double bearing);
		Mat PointsToBitmap();
		FuzzyPath FindGoodDestination();
		ImageVectorList Landmarks { get; }

		void Reset();
	}
}
