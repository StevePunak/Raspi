using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Geometry;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.LidarImaging;

namespace TrackBot.Spatial
{
	class VirtualEnvironment : IImageEnvironment
	{
		public event FuzzyPathChangedHandler FuzzyPathChanged;
		public event LandmarksChangedHandler LandmarksChanged;
		public event BarriersChangedHandler BarriersChanged;

		public double Range { get; set; }

		public double CompassOffset { get; set; }
		public double Bearing { get; set; }
		public double DebugAngle { get; set; }

		public Size PixelSize { get; set; }

		public double RenderPixelsPerMeter { get; set; }
		public PointD Location { get; set; }
		public PointD RelativeLocation { get; set; }
		public double PixelsPerMeter { get; set; }
		public double VectorSize { get; set; }

		public double RangeFuzz { get; set; }

		public FuzzyPath FuzzyPath { get; set; }

		public ImageVectorList Landmarks { get { return new ImageVectorList(); } }

		public IVector[] Vectors => throw new NotImplementedException();

		public double MetersSquare { get; set; }

		public VirtualEnvironment()
		{
			Range = .2;
			CompassOffset = 0;
			Bearing = 0;
			DebugAngle = 0;
			PixelSize = new Size(500, 500);
			RenderPixelsPerMeter = 50;
			Location = new PointD(250, 250);
			RelativeLocation = new PointD(250, 250);

			FuzzyPathChanged += delegate {};
			LandmarksChanged += delegate {};
			BarriersChanged += delegate {};

			FuzzyPathChanged(new FuzzyPath());
			LandmarksChanged(new ImageVectorList());
			BarriersChanged(new BarrierList());
		}

		public double FuzzyRangeAtBearing(double bearing, double fuzz = 2)
		{
			return Range;
		}

		public Mat PointsToBitmap()
		{
			return new Mat();
		}

		public double GetRangeAtBearing(double bearing)
		{
			return Range;
		}

		public void ProcessImage(Mat image, double imageOrientation, double imagePixelsPerMeter)
		{
			
		}

		public void Reset()
		{
			
		}

		public void Start()
		{
			
		}

		public void Stop()
		{
			
		}

		public Mat CreateImage(SpatialObjects objects)
		{
			return new Mat();
		}

		public DateTime GetLastSampleTimeAtBearing(Double bearing)
		{
			return DateTime.UtcNow;
		}

		public double ShortestRangeAtBearing(double bearing, double fuzz = 2)
		{
			return 0;
		}

		public FuzzyPath FindGoodDestination()
		{
			return new FuzzyPath();
		}

		public byte[] MakeRangeBlob()
		{
			return new byte[0];
		}

		public double FuzzyRangeAtBearing(Chassis chassis, double bearingStraightAhead, double angularWidth)
		{
			throw new NotImplementedException();
		}

		public double FuzzyRangeAtBearing(BearingAndRange offset1, BearingAndRange offset2, double bearing, double fuzz, out PointCloud2D fromFrontLeft, out PointCloud2D fromFrontRight)
		{
			throw new NotImplementedException();
		}

		public double FuzzyRangeAtBearing(Chassis chassis, double bearingStraightAhead, double angularWidth, out PointCloud2D fromFrontLeft, out PointCloud2D fromFrontRight)
		{
			throw new NotImplementedException();
		}

		public double FuzzyRangeAtBearing(double bearing, BearingAndRange frontLeftWheelOffset, BearingAndRange frontRightWheelOffset, double angularWidth, out PointCloud2D frontLeftCloud, out PointCloud2D frontRightCloud)
		{
			throw new NotImplementedException();
		}

		public FuzzyPathList FindGoodDestinations(double requireClearUpTo)
		{
			throw new NotImplementedException();
		}
	}
}
