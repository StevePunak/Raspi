using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Geometry;
using RaspiCommon.Devices.Chassis;

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
		Mat CreateImage(SpatialObjects objects);
		byte[] MakeRangeBlob();

		Double GetRangeAtBearing(Double bearing);
		Double FuzzyRangeAtBearing(Chassis chassis, Double bearingStraightAhead, Double angularWidth);
		Double FuzzyRangeAtBearing(Chassis chassis, Double bearingStraightAhead, Double angularWidth, out PointCloud2D fromFrontLeft, out PointCloud2D fromFrontRight);
		Double FuzzyRangeAtBearing(BearingAndRange offset1, BearingAndRange offset2, Double bearing, Double fuzz, out PointCloud2D fromFrontLeft, out PointCloud2D fromFrontRight);
		Double ShortestRangeAtBearing(Double bearing, Double fuzz);
		DateTime GetLastSampleTimeAtBearing(Double bearing);
		Mat PointsToBitmap();
		FuzzyPath FindGoodDestination(Double requireClearUpTo);
		ImageVectorList Landmarks { get; }

		void Reset();
	}
}
