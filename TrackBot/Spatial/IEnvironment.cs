using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Geometry;

namespace TrackBot.Spatial
{
	public interface IEnvironment
	{
		void Start();
		void Stop();

		Double Range { get; }
		Double CompassOffset { get; set; }
		Double Bearing { get; set; }
		Double DebugAngle { get; set; }

		Size PixelSize { get; }
		Double PixelsPerMeter { get; set; }
		Double RenderPixelsPerMeter { get; set; }
		Double VectorSize { get; }
		PointD Location { get; set; }
		PointD RelativeLocation { get; set; }

		void ProcessImage(Mat image, Double imageOrientation, Double imagePixelsPerMeter);
		Mat GetEnvironmentImage(bool drawDebugLines);

		Double GetRangeAtBearing(Double bearing);
		Double FuzzyRangeAtBearing(Double bearing, Double fuzz = 2);
		Double ShortestRangeAtBearing(Double bearing, Double fuzz = 2);
		DateTime GetLastSampleTimeAtBearing(Double bearing);
		Mat GenerateBitmap(bool radarLines, bool drawVehicle);

		Line FindGoodDestination();

		void Reset();
	}
}
