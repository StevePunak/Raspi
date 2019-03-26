using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;

namespace RaspiCommon.Lidar.Environs
{
	public class LidarEnvironment
	{
		#region Constants

		const Double MINIMUM_LANDMARK_SEGMENT = .5;
		const Double MINIMUM_CONNECT_LENGTH = .025;
		const Double RIGHT_ANGLE_SLACK_DEGREES = 1;

		#endregion

		public Double MetersSquare { get; private set; }
		public Double PixelsPerMeter { get; private set; }
		public Double Orientation { get; private set; }

		public PointD Location { get; private set; }

		public LandmarkList Landmarks { get; private set; }

		public Image<Bgr, Byte> Image { get; private set; }

		public LidarEnvironment(Double metersSquare, Double pixelsPerMeter, Double orientation = 0)
		{
			MetersSquare = metersSquare;
			PixelsPerMeter = pixelsPerMeter;
			Orientation = orientation;

			Image = new Image<Bgr, byte>((int)(MetersSquare * PixelsPerMeter), (int)(MetersSquare * PixelsPerMeter));
			Image.Draw(new Rectangle(0, 0, Image.Size.Width, Image.Size.Height), new Bgr(Color.Black), 0); 
		}

		public void ProcessImage(Image<Bgr, Byte> image, Double orientation)
		{
			Double cannyThreshold = 180;
			Double cannyThresholdLinking = 100;     // 120

			LineSegment2D[][] segments = image.HoughLines(cannyThreshold, cannyThresholdLinking, 1, Math.PI / 45, 20, 1, 10);

			LandmarkList landmarks;
			BarrierList barriers;
			FindLandmarks(segments[0], new PointD(image.Width / 2, image.Height / 2), orientation, out landmarks, out barriers);
		}

		public PointD ToEnvironment(PointD point, PointD sourceOrigin, Double sourcePixelsPerMeter, Double sourceOrientation)
		{
			Line l = new Line(sourceOrigin, point);
			Double scale = PixelsPerMeter / sourcePixelsPerMeter;
			Double finalLength = l.Length * scale;


			Double 
			Double destinationBearing = destination

		}

)

		public Bitmap ToImage()
		{
			//CircleF circle1 = new CircleF(segment.P1, 3);
			//image.Draw(circle1, new Bgr(Color.Red), 1);

			//CircleF circle2 = new CircleF(segment.P2, 4);
			//image.Draw(circle2, new Bgr(Color.Green), 1);
			return Image.ToBitmap();
		}

		private LandmarkList FindLandmarks(IEnumerable<LineSegment2D> segments, PointD center, Double currentHeading, out LandmarkList landmarks, out BarrierList barriers)
		{
			landmarks = new LandmarkList();
			barriers = new BarrierList();

			Double minimumSegmentPixels = PixelsPerMeter * MINIMUM_LANDMARK_SEGMENT;

			LineList lines = new LineList();
			foreach(LineSegment2D segment in segments)
			{
				Line line = new Line(segment.P1, segment.P2);
				Double actualLength = line.Length / PixelsPerMeter;

				Log.SysLogText(LogLevel.DEBUG, "Segment {0} is {1:0.00}m long", line, actualLength);

				if(actualLength < MINIMUM_LANDMARK_SEGMENT)
				{
					Log.SysLogText(LogLevel.DEBUG, "Abandoning segment");
					continue;
				}
				lines.Add(line);
			}

			foreach(Line l1 in lines)
			{
				foreach(Line l2 in lines)
				{
					if(l1 == l2)
						continue;

					LinePair pair = new LinePair(l1, l2);
					Line join = pair.ClosestPoints;

					Double actualDistance = ConvertToEnviroment(join.Length);
					if(actualDistance > MINIMUM_CONNECT_LENGTH)
					{
						Double angularDifference = Degrees.AngularDifference(l1.Bearing, l2.Bearing);
						if(angularDifference.IsWithinDegressOf(90, RIGHT_ANGLE_SLACK_DEGREES))
						{
							PointD intersection;
							FlatGeo.GetIntersection(l1, l2, out intersection);

							Line hereToThere = new Line(center, intersection);
							Double geodistance = ConvertToEnviroment(hereToThere.Length);
							Double bearing = currentHeading.SubtractDegrees(hereToThere.Bearing);
							PointD geolocation = FlatGeo.GetPoint(center, bearing, geodistance);
							Landmark landmark = new Landmark(geolocation);
							landmarks.Add(landmark);
						}
					}
				}
			}

			Line shortest = lines.Shortest;



			return landmarks;
		}

		public Double ConvertToEnviroment(Double length)
		{
			return length / PixelsPerMeter;
		}

		public Double ConvertToBitmap(Double length)
		{
			return length * PixelsPerMeter;
		}
	}
}
