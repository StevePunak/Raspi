using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Geometry;
using RaspiCommon.Lidar.Environs;

namespace TrackBot.Spatial
{
	class VirtualEnvironment : IEnvironment
	{
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

		public LandmarkList Landmarks { get { return new LandmarkList(); } }

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

		public Mat GetEnvironmentImage(bool drawDebugLines)
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
	}
}
