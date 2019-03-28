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

		const Double MINIMUM_LANDMARK_SEGMENT = .15;		// line segments must be at least this many meters in order to be considered
		const Double MINIMUM_CONNECT_LENGTH = .025;			// right angle rays must be within this many meters to connect
		const Double MAXIMUM_CONNECT_LENGTH = 1;			// will not connect right angle this far apart
		const Double RIGHT_ANGLE_SLACK_DEGREES = 1;			// slack from 90° which may still constitute right angle
		const Double MINIMUM_LANDMARK_SEPARATION = .1;		// landmarks must be this many meters apart
		const Double BEARING_SLACK = 2;                     // degrees slack when computing similar angles
		const Double LINE_PATH_WIDTH = .1;					// lines must be within this many meters to be consolidated into a single line

		#endregion

		public Double MetersSquare { get; private set; }
		public Double PixelsPerMeter { get; private set; }
		public Double Orientation { get; private set; }

		public PointD Location { get; private set; }
		public Double Bearing { get; set; }

		public LandmarkList Landmarks { get; private set; }
		public BarrierList Barriers { get; private set; }
		public List<RectangleD> Paths { get; private set; }

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
			Color.AliceBlue, Color.Aquamarine, Color.Azure, Color.Beige, Color.BlanchedAlmond, Color.BlueViolet, Color.Brown, Color.Chartreuse, Color.CornflowerBlue,
			Color.DarkMagenta, Color.LightBlue, Color.LightGoldenrodYellow, Color.LightPink, Color.LightSteelBlue
		};
		int _colorIndex;

		List<String> _debugTags = new List<string>() { "_000", "_020", "_004", "_028" };

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
			Paths = new List<RectangleD>();

			Bearing = 0;

			_colorIndex = 0;
		}

		public void ProcessImage(Image<Bgr, Byte> image, Double imageOrientation, Double imagePixelsPerMeter)
		{
			PointD imageCenter = new PointD(image.Width / 2, image.Height / 2);

			Double cannyThreshold = 120;			// 180
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
				b.Line.Tag = barrier.Line.Tag;
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

		private LandmarkList FindLandmarks(	IEnumerable<LineSegment2D> segments, 
											PointD center, 
											out LandmarkList landmarks, 
											out BarrierList barriers)
		{
			landmarks = new LandmarkList();
			barriers = new BarrierList();

			Paths = new List<RectangleD>();

			LineList lines = new LineList();

			// get rid of any short segments, and aggregate into Line objects
			int lineNumber = 0;
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

				if(line.P1.Y > 400)
				{
					int i = 1;
				}
				line.Tag = String.Format("Line_{0:000}", lineNumber++);
//				if(_debugTags.Find(c => line.ToString().Contains(c)) != null)
				{
					lines.Add(line);

					barriers.Add(new Barrier(line));
				}
			}

			// find the segments that are alike
			List<LineList> groups = new List<LineList>();
			Double pathWidthPixels = LINE_PATH_WIDTH * PixelsPerMeter;
			while(lines.Count > 0)
			{
//				Double extendPath = 1 * PixelsPerMeter;

				Line l1 = lines[0];
				LineList similarLines = new LineList();
				similarLines.Add(l1);
				for(int x = 1;x < lines.Count - 1;x++)
				{
					Line l2 = lines[x];

					// is line1 like line2?
					//  1. Check to see if bearing is similar
					if(l1.Bearing.IsWithinDegressOf(l2.Bearing, BEARING_SLACK) || l1.Bearing.AddDegrees(180).IsWithinDegressOf(l2.Bearing, BEARING_SLACK))
					{
						// do the lines lie on leach others path?
						RectangleD rect1, rect2;
						if(l2.LiesAlongThePathOf(l1, pathWidthPixels, out rect1, out rect2))
						{
							// if so, they are simlar to each other
							similarLines.Add(l2);
						}
						Paths.Add(rect1);
						if(rect2 != null) Paths.Add(rect2);
					}
				}

				// if there are more than one line in the similar bucket, add them to the right slot
				// and remove them from the list
				if(similarLines.Count > 1)
				{
					bool foundGroup = false;
					for(int x = 0;x < groups.Count;x++)
					{
						LineList group = groups[x];
						if( group.AverageBearing.AngularDifference(similarLines.AverageBearing) <= BEARING_SLACK && 
							group.ContainsLinesAlongThePathOf(similarLines, pathWidthPixels))
						{
							group.AddRange(similarLines);
							foundGroup = true;
						}
					}

					if(foundGroup == false)
					{
						LineList group = new LineList();
						group.AddRange(similarLines);
						groups.Add(group);
					}

				}

				// get the similar lines out of the main hunt list, including the one we are working on (lines[0])
				foreach(Line similarLine in similarLines)
				{
					lines.Remove(similarLine);
				}
			}

			// now consolidate each group into a single line
			LineList consolidatedLines = new LineList();
			foreach(LineList group in groups)
			{
				Line consolidated = group[0];
				for(int x = 1;x < group.Count;x++)
				{
					Line line = group[x];
					consolidated = Line.ConsolidateLongest(consolidated, line);
				}
				consolidated.Tag = String.Format("CONS_{0:000}", consolidatedLines.Count);
				consolidatedLines.Add(consolidated);
			}

			lines = consolidatedLines;

			bool remakeBarriers = false;
			if(remakeBarriers)
			{
				barriers = new BarrierList();
				foreach(Line line in lines)
				{
					Barrier barrier = new Barrier(line);
					barrier.Line.Tag = line.Tag;
					barriers.Add(barrier);
				}
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

		public Bitmap ToImage()
		{
			Image<Bgr, Byte> image = Image.Clone();

			image.Draw(new Cross2DF(BitmapCenter.ToPoint(), 4, 4), new Bgr(Color.GreenYellow), 2);

			int last = 0;
			foreach(Barrier barrier in Barriers)
			{
				Color color = NextColor;
				image.Draw(new LineSegment2D(barrier.Line.P1.ToPoint(), barrier.Line.P2.ToPoint()), new Bgr(color), 2, LineType.EightConnected);
				PointD where = ((PointD)barrier.Line.P1).GetPointAt(180, 20 + last) as PointD;
//				image.Draw(barrier.ToString(), where.ToPoint(), FontFace.HersheyPlain, 1, new Bgr(color));

				//last += 5;
				Log.SysLogText(LogLevel.DEBUG, "---- {0}", barrier);
			}

			foreach(Landmark landmark in Landmarks)
			{
				CircleF circle1 = new CircleF(landmark.Location.ToPoint(), 3);
				image.Draw(circle1, new Bgr(Color.Red), 1);
			}

			return image.ToBitmap();
		}

	}
}
