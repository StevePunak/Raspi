using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon.Extensions;
using RaspiCommon.Server;

namespace RaspiCommon.Lidar.Environs
{
	public class LidarEnvironment
	{
		#region Constants

		const Double MINIMUM_LANDMARK_SEGMENT = .10;		// line segments must be at least this many meters in order to be considered
		const Double MINIMUM_CONNECT_LENGTH = .025;			// right angle rays must be within this many meters to connect
		const Double MAXIMUM_CONNECT_LENGTH = 1;			// will not connect right angle this far apart
		const Double RIGHT_ANGLE_SLACK_DEGREES = 1;			// slack from 90° which may still constitute right angle
		const Double MINIMUM_LANDMARK_SEPARATION = .1;		// landmarks must be this many meters apart
		const Double BEARING_SLACK = 2;                     // degrees slack when computing similar angles
		const Double LINE_PATH_WIDTH = .1;                  // lines must be within this many meters to be consolidated into a single line

		public Double RangeFuzz { get; set; }

		#endregion

		#region Events

		public event FuzzyPathChangedHandler FuzzyPathChanged;
		public event LandmarksChangedHandler LandmarksChanged;
		public event BarriersChangedHandler BarriersChanged;

		#endregion

		public Double MetersSquare { get; private set; }
		public Double PixelsPerMeter { get; set; }

		public PointD Location { get; set; }
		public Double Bearing { get { return Lidar.Bearing; }  set { Lidar.Bearing = value; } }

		public LandmarkList Landmarks { get; private set; }
		public BarrierList Barriers { get; private set; }
		public List<RectangleD> Paths { get; private set; }

		public Mat Image { get; private set; }

		public PointD BitmapCenter { get { return new PointD(Image.Width / 2, Image.Height / 2); } }
		public PointD GeoCenter { get { return BitmapCenter; } }

		public RPLidar Lidar { get; set; }

		public PointD RelativeLocation { get; set; }

		FuzzyPath _fuzzyPath;
		public FuzzyPath FuzzyPath
		{
			get{ return _fuzzyPath; }
			set
			{
				_fuzzyPath = value;
				FuzzyPathChanged(value);
			}
		}

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

		TelemetryServer _server;

		List<String> _debugTags = new List<string>() { "006", "009", "014" };

		public LidarEnvironment(Double metersSquare, Double pixelsPerMeter)
		{
			MetersSquare = metersSquare;
			PixelsPerMeter = pixelsPerMeter;

			//			Image = new Image<Bgr, byte>((int)(MetersSquare * PixelsPerMeter), (int)(MetersSquare * PixelsPerMeter));
			//			Image.Draw(new Rectangle(0, 0, Image.Size.Width, Image.Size.Height), new Bgr(Color.Black), 0);

			//Location = new PointD(Image.Width / 2, Image.Height / 2);

			Landmarks = new LandmarkList();
			Barriers = new BarrierList();
			Paths = new List<RectangleD>();

			FuzzyPathChanged += delegate {};
			LandmarksChanged += delegate {};
			BarriersChanged += delegate { };

			Log.SysLogText(LogLevel.DEBUG, "Init 3");
			_colorIndex = 0;
		}

		public void ProcessImage(Mat image, Double imageOrientation, Double imagePixelsPerMeter)
		{
			Log.SysLogText(LogLevel.DEBUG, "Lidar Processing image of {0}", image.Size);

			Image = image;

			Double cannyThreshold = 120;				// 180
			Double cannyThresholdLinking = 120;         // 120
			PointD center = image.Center();

			Double rhoRes = 2;
			Double thetaRes = Math.PI / 45;
			int threshold = 20;							// decrease to get more lines

			Mat outputImage = Image.Clone();

			CvInvoke.Canny(Image, outputImage, cannyThreshold, cannyThresholdLinking);
			LineSegment2D[] segments = CvInvoke.HoughLinesP(outputImage, rhoRes, thetaRes, (int)threshold);
			LineList lines = segments.ToLineList();
			lines.RemoveInvalid();

			lines.DumpToLog();

			Log.SysLogText(LogLevel.DEBUG, "---------------Sort ----------------");

			lines.SortUpperLeft();

			Log.SysLogText(LogLevel.DEBUG, "Have {0} segments", lines.Count);

			Mat temp = Image.Clone();
			foreach(Line line in lines)
			{
				CvInvoke.Line(temp, line.P1.ToPoint(), line.P2.ToPoint(), new Bgr(Color.GreenYellow).MCvScalar, 3);
			}
			temp.Save(@"\\raspi\pi\tmp\junk.png");

			LandmarkList landmarks;
			BarrierList barriers;
			FindLandmarksAndBarriers(lines, center, out landmarks, out barriers);

			Barriers = barriers;
			Landmarks = landmarks;

			Log.SysLogText(LogLevel.DEBUG, "******** {0} landmarks found", landmarks.Count);

			LandmarksChanged(landmarks);
			BarriersChanged(barriers);
		}

		public Mat GetImageWithLandmarks()
		{
			Mat output = Image.Clone();

			PointD imageCenter = output.Center();
			foreach(Landmark landmark in Landmarks)
			{
				CvInvoke.Circle(output, landmark.GetPoint().ToPoint(), 4, new Bgr(Color.Blue).MCvScalar);
			}

			return output;
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

		private void FindLandmarksAndBarriers(	LineList segments, 
												PointD origin,
												out LandmarkList landmarks, 
												out BarrierList barriers)
		{
			landmarks = new LandmarkList();
			barriers = new BarrierList();

			Paths = new List<RectangleD>();

			// get rid of short lines
			LineList lines = new LineList(segments);
			Double minimumSizePixels = MINIMUM_LANDMARK_SEGMENT * PixelsPerMeter;
			lines.RemoveShorterThan(minimumSizePixels);
			Log.SysLogText(LogLevel.DEBUG, "Keeping {0} out of {1} after applying size filter", lines.Count, segments.Count);

			// tag remaining objects for debugging
			int lineNumber = 0;
			foreach(Line line in lines)
			{
				line.Tag = String.Format("Line_{0:000}", lineNumber++);
			}

			// group similar lines into groups
			List<LineList> groups = GroupSimilarLines(lines);

			// now consolidate each group into a single line
			lines = ConsolidateLines(groups);

			// set these as barriers
			barriers = new BarrierList(origin, lines);

			// create the landmarks by finding 90° intersections
			landmarks = ComputeLandmarks(origin, lines);

			// consolidate the landmarks
			landmarks = ConsolidateLandmarks(origin, landmarks);
		}

		private LandmarkList ConsolidateLandmarks(PointD origin, LandmarkList from)
		{
			Double minimumPixelDistance = MINIMUM_LANDMARK_SEPARATION * PixelsPerMeter;

			// group them into bunches by range
			LandmarkGroupList groups = new LandmarkGroupList();
			while(from.Count > 0)
			{
				Landmark landmark = from[0];
				LandmarkList group = groups.FindGroupWithinRangeOfPoint(landmark.GetPoint(), minimumPixelDistance);
				if(group == null)
				{
					group = new LandmarkList();
					groups.Add(group);
				}
				group.Add(landmark);
				from.RemoveAt(0);
			}

			LandmarkList landmarks = new LandmarkList();
			foreach(LandmarkList group in groups)
			{
				Log.SysLogText(LogLevel.DEBUG, "Dumping Group");
				group.DumpToLog();

				Landmark centroid = group.GetCentroid(origin);
				landmarks.Add(centroid);
				Log.SysLogText(LogLevel.DEBUG, "Centroid is {0}", centroid);
			}

			return landmarks;
		}

		private LandmarkList ComputeLandmarks(PointD origin, LineList lines)
		{
			LandmarkList landmarks = new LandmarkList();
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
								Landmark landmark = new Landmark(origin, new BearingAndRange(origin, intersection));
								landmarks.Add(landmark);

								Log.SysLogText(LogLevel.DEBUG, "Added landmark {0}", landmark);
							}
						}
					}
				}
			}
			return landmarks;
		}

		private LineList ConsolidateLines(List<LineList> groups)
		{
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
			return consolidatedLines;
		}

		private List<LineList> GroupSimilarLines(LineList lines)
		{
			// find the segments that are alike
			List<LineList> groups = new List<LineList>();
			Double pathWidthPixels = LINE_PATH_WIDTH * PixelsPerMeter;
			while(lines.Count > 0)
			{
				Double extendPath = .15 * PixelsPerMeter;

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
				if(similarLines.Count > 0)
				{
					bool foundGroup = false;
					for(int x = 0;x < groups.Count;x++)
					{
						LineList group = groups[x];
						if(group.AverageBearing.AngularDifference(similarLines.AverageBearing) <= BEARING_SLACK &&
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
			return groups;
		}

		public Double ConvertToEnviroment(Double length)
		{
			return length / PixelsPerMeter;
		}

		public Double ConvertToBitmap(Double length)
		{
			return length * PixelsPerMeter;
		}

		public Mat GetEnvironmentImage(bool drawDebugLines)
		{
			Mat image = Image.Clone();

			Cross2DF cross = new Cross2DF(BitmapCenter.ToPoint(), 4, 4);
			CvInvoke.Line(image, cross.Vertical.P1.ToPoint(), cross.Vertical.P2.ToPoint(), new Bgr(Color.GreenYellow).MCvScalar);
			CvInvoke.Line(image, cross.Horizontal.P1.ToPoint(), cross.Horizontal.P2.ToPoint(), new Bgr(Color.GreenYellow).MCvScalar);

			foreach(Barrier barrier in Barriers)
			{
				Color color = NextColor;
				Line line = barrier.GetLine();
				CvInvoke.Line(image, line.P1.ToPoint(), line.P2.ToPoint(), new Bgr(color).MCvScalar, 2);
			}

			foreach(Landmark landmark in Landmarks)
			{
				CircleF circle1 = new CircleF(landmark.Origin.ToPoint(), 3);
				CvInvoke.Circle(image, circle1.Center.ToPoint(), (int)circle1.Radius, new Bgr(Color.Red).MCvScalar, 1);
			}

			return image;
		}

		public Double ShortestRangeAtBearing(Double trueBearing, Double angularWidth)
		{
			Double start = trueBearing.SubtractDegrees(angularWidth / 2);
			Double end = trueBearing.AddDegrees((angularWidth / 2) + 1);

			List<Double> allDistances = new List<double>();
			Double angle = start;
			while(angle.IsWithinDegressOf(end, Lidar.VectorSize) == false)
			{
				Double distance = Lidar.GetRangeAtBearing(angle);
				//Log.SysLogText(LogLevel.DEBUG, "At {0:0.000}° range is {1:0.000}", angle, distance);
				if(distance != 0)
				{
					allDistances.Add(distance);
				}

				angle = angle.AddDegrees(Lidar.VectorSize);
			}

			return allDistances.Count > 0 ? allDistances.Min() : 0;
		}

		public Double FuzzyRangeAtBearing(Double trueBearing, Double angularWidth)
		{
			Double start = trueBearing.SubtractDegrees(angularWidth / 2);
			Double end = trueBearing.AddDegrees((angularWidth / 2) + 1);

			List<Double> allDistances = new List<double>();
			Double angle = start;
			while(angle.IsWithinDegressOf(end, Lidar.VectorSize) == false)
			{
				Double distance = Lidar.GetRangeAtBearing(angle);
				 //Log.SysLogText(LogLevel.DEBUG, "At {0:0.000}° range is {1:0.000}", angle, distance);
				if(distance != 0)
				{
					allDistances.Add(distance);
				}

				angle = angle.AddDegrees(Lidar.VectorSize);
			}

			return allDistances.Count > 0 ? allDistances.Average() : 0;
		}

		public void Reset()
		{
			Lidar.ClearDistanceVectors();
		}

		public FuzzyPath MakeFuzzyPath(Double bearing, Double rangeFuzz)
		{
			Log.SysLogText(LogLevel.DEBUG, "Finding fuzzy path at bearing {0:0.00}°  fuzz {1:0.00}°", bearing, rangeFuzz);

			Double start = bearing.SubtractDegrees(rangeFuzz / 2);
			Double end = bearing.AddDegrees((rangeFuzz / 2) + 1);

			BearingAndRangeList vectors = new BearingAndRangeList();
			Double angle = start;
			while(angle.IsWithinDegressOf(end, Lidar.VectorSize) == false)
			{
				Double range = Lidar.GetRangeAtBearing(angle);
				if(range != 0)
				{
					vectors.Add(new BearingAndRange(angle, range));
				}

				angle = angle.AddDegrees(Lidar.VectorSize);
			}
			FuzzyPath path = new FuzzyPath(bearing, vectors);
			return path;
		}

	}
}
