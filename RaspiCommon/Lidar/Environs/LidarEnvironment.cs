using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;

namespace RaspiCommon.Lidar.Environs
{
	public class LidarEnvironment
	{
		#region Constants

		const Double MINIMUM_LANDMARK_SEGMENT = .15;
		const Double MINIMUM_CONNECT_LENGTH = .025;
		const Double MAXIMUM_CONNECT_LENGTH = 20;
		const Double RIGHT_ANGLE_SLACK_DEGREES = 1;
		const Double MINIMUM_LANDMARK_SEPARATION = .1;
		const Double BEARING_SLACK = 2;

		#endregion

		public Double MetersSquare { get; private set; }
		public Double PixelsPerMeter { get; private set; }
		public Double Orientation { get; private set; }

		public PointD Location { get; private set; }
		public Double Bearing { get; set; }

		public LandmarkList Landmarks { get; private set; }
		public BarrierList Barriers { get; private set; }

		public Image<Bgr, Byte> Image { get; private set; }

		public PointD BitmapCenter { get { return new PointD(Image.Width / 2, Image.Height / 2); } }
		public PointD GeoCenter { get { return BitmapCenter; } }

		Color NextColor
		{
			get
			{
				Color color = _rotateColors[_colorIndex];
				if(++_colorIndex >= _rotateColors.Count)
					_colorIndex = 0;
				return color;
			}
		}

		List<Color> _rotateColors = new List<Color>()
		{
			Color.AliceBlue, Color.Aquamarine, Color.Azure, Color.Beige, Color.BlanchedAlmond, Color.BlueViolet, Color.Brown, Color.Chartreuse, Color.CornflowerBlue
		};
		int _colorIndex;

		public LidarEnvironment(Double metersSquare, Double pixelsPerMeter, Double orientation = 0)
		{
			MetersSquare = metersSquare;
			PixelsPerMeter = pixelsPerMeter;
			Orientation = orientation;

			Image = new Image<Bgr, byte>((int)(MetersSquare * PixelsPerMeter), (int)(MetersSquare * PixelsPerMeter));
			Image.Draw(new Rectangle(0, 0, Image.Size.Width, Image.Size.Height), new Bgr(Color.Black), 0);

			Location = new PointD(Image.Width / 2, Image.Height / 2);

			Landmarks = new LandmarkList();
			Barriers = new BarrierList();

			Bearing = 0;

			_colorIndex = 0;
		}

		public void ProcessImage(Image<Bgr, Byte> image, Double imageOrientation, Double imagePixelsPerMeter)
		{
			PointD imageCenter = new PointD(image.Width / 2, image.Height / 2);

			Double cannyThreshold = 1200;			// 180
			Double cannyThresholdLinking = 120;     // 120

			LineSegment2D[][] segments = image.HoughLines(cannyThreshold, cannyThresholdLinking, 1, Math.PI / 45, 20, 1, 10);

			LandmarkList landmarks;
			BarrierList barriers;
			FindLandmarks(segments[0], new PointD(image.Width / 2, image.Height / 2), out landmarks, out barriers);

			Log.SysLogText(LogLevel.DEBUG, "Have {0} segments", segments[0].Length);


			foreach(Landmark landmark in landmarks)
			{
				PointD p = ImagePointToInternalPoint(landmark.Location, imageCenter, imageOrientation, imagePixelsPerMeter);
				if(Landmarks.Contains(p, MINIMUM_LANDMARK_SEPARATION, PixelsPerMeter) == false)
				{
					Landmarks.Add(new Landmark(p));
				}
			}

			Log.SysLogText(LogLevel.DEBUG, "******** {0} landmarks found", landmarks.Count);

			foreach(Barrier barrier in barriers)
			{
				PointD p1 = ImagePointToInternalPoint(barrier.Line.P1 as PointD, imageCenter, imageOrientation, imagePixelsPerMeter);
				PointD p2 = ImagePointToInternalPoint(barrier.Line.P2 as PointD, imageCenter, imageOrientation, imagePixelsPerMeter);
				Barrier b = new Barrier(p1, p2);
				if(Barriers.Contains(b, MINIMUM_LANDMARK_SEPARATION, PixelsPerMeter, BEARING_SLACK) == false)
				{
					Barriers.Add(b);
				}
			}
		}

		PointD ImagePointToInternalPoint(PointD imagePoint, PointD imageOrigin, Double imageOrientation, Double imagePixelsPerMeter)
		{
			// get landmark distance and bearing in input bitmap
			Line imageLine = new Line(imageOrigin, imagePoint);

			// adjust bearing
			Double internalBearing = imageLine.Bearing.AddDegrees(imageOrientation);
			Double internalDistanceToLandmark = (imageLine.Length / imagePixelsPerMeter) * PixelsPerMeter;
			Line internalLine = new Line(Location, Location.GetPointAt(internalBearing, internalDistanceToLandmark));
			return internalLine.P2 as PointD;
		}

		public Bitmap ToImage()
		{
			Image<Bgr, Byte> image = Image.Clone();

			image.Draw(new Cross2DF(BitmapCenter.ToPoint(), 4, 4), new Bgr(Color.GreenYellow), 2);

			foreach(Landmark landmark in Landmarks)
			{
				CircleF circle1 = new CircleF(landmark.Location.ToPoint(), 3);
				image.Draw(circle1, new Bgr(Color.Red), 1);
			}

			foreach(Barrier barrier in Barriers)
			{
				image.Draw(new LineSegment2D(barrier.Line.P1.ToPoint(), barrier.Line.P2.ToPoint()), new Bgr(NextColor), 2, LineType.EightConnected);
			}

			return image.ToBitmap();
		}

		private LandmarkList FindLandmarks(	IEnumerable<LineSegment2D> segments, 
											PointD center, 
											out LandmarkList landmarks, 
											out BarrierList barriers)
		{
			landmarks = new LandmarkList();
			barriers = new BarrierList();

			LineList lines = new LineList();
			foreach(LineSegment2D segment in segments)
			{
				Line line = new Line(segment.P1, segment.P2);
				Double actualLength = line.Length / PixelsPerMeter;

				Log.SysLogText(LogLevel.DEBUG, "Segment {0} is {1:0.00}m long", line, actualLength);

				if(actualLength < MINIMUM_LANDMARK_SEGMENT)
				{
					Log.SysLogText(LogLevel.DEBUG, "Abandoning segment");
					//continue;
				}
				lines.Add(line);

				barriers.Add(new Barrier(line));
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
					if(actualDistance >= MINIMUM_CONNECT_LENGTH && actualDistance <= MAXIMUM_CONNECT_LENGTH)
					{
						Double angularDifference = Degrees.AngularDifference(l1.Bearing, l2.Bearing);
						if(angularDifference.IsWithinDegressOf(90, RIGHT_ANGLE_SLACK_DEGREES))
						{
							PointD intersection;
							FlatGeo.GetIntersection(l1, l2, out intersection);

							if(landmarks.Contains(intersection, MINIMUM_LANDMARK_SEPARATION, PixelsPerMeter) == false)
							{
								Landmark landmark = new Landmark(intersection);
								landmarks.Add(landmark);

								Log.SysLogText(LogLevel.DEBUG, "Added landmark {0}", landmark);
							}
						}
					}
				}
			}

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
