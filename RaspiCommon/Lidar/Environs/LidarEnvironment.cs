using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.Spatial;
using RaspiCommon.Extensions;
using RaspiCommon.Network;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.LidarImaging;
using RaspiCommon.System;

namespace RaspiCommon.Lidar.Environs
{
	public class LidarEnvironment
	{
		#region Constants

		public Double RangeFuzz { get; set; }

		#endregion

		#region Events

		public event FuzzyPathChangedHandler FuzzyPathChanged;
		public event LandmarksChangedHandler LandmarksChanged;
		public event BarriersChangedHandler BarriersChanged;

		#endregion

		#region Public Properties

		public Double MetersSquare { get; private set; }
		public Double PixelsPerMeter { get; set; }

		public PointD Location { get; set; }
		public Double Bearing { get { return Lidar.Bearing; }  set { Lidar.Bearing = value; } }

		public ImageVectorList Landmarks { get; private set; }
		public BarrierList Barriers { get; private set; }
		public List<RectangleD> Paths { get; private set; }

		public Mat Image { get; private set; }

		public PointD BitmapCenter { get { return new PointD(Image.Width / 2, Image.Height / 2); } }
		public PointD GeoCenter { get { return BitmapCenter; } }

		public LidarBase Lidar { get; set; }

		public PointD RelativeLocation { get; set; }

		public ProcessingMetrics ProcessingMetrics { get; set; }

		FuzzyPath _fuzzyPath;
		public FuzzyPath FuzzyPath
		{
			get{ return _fuzzyPath; }
			set
			{
				_fuzzyPath = value;
				FuzzyPathChanged(value);
				Log.SysLogText(LogLevel.DEBUG, "New fuzzy path {0}", _fuzzyPath);
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

		#endregion

		#region Private Members

		List<Color> _rotateColors = new List<Color>()
		{
			Color.AliceBlue, Color.Aquamarine, Color.Azure, Color.Beige, Color.BlanchedAlmond, Color.BlueViolet, Color.Brown, Color.Chartreuse, Color.CornflowerBlue,
			Color.DarkMagenta, Color.LightBlue, Color.LightGoldenrodYellow, Color.LightPink, Color.LightSteelBlue
		};
		int _colorIndex;

		List<String> _debugTags = new List<string>() { "006", "009", "014" };

		#endregion

		#region Constructor

		public LidarEnvironment(Double metersSquare, Double pixelsPerMeter)
		{
			MetersSquare = metersSquare;
			PixelsPerMeter = pixelsPerMeter;

			//			Image = new Image<Bgr, byte>((int)(MetersSquare * PixelsPerMeter), (int)(MetersSquare * PixelsPerMeter));
			//			Image.Draw(new Rectangle(0, 0, Image.Size.Width, Image.Size.Height), new Bgr(Color.Black), 0);

			//Location = new PointD(Image.Width / 2, Image.Height / 2);

			Landmarks = new ImageVectorList();
			Barriers = new BarrierList();
			Paths = new List<RectangleD>();

			FuzzyPathChanged += delegate { };
			LandmarksChanged += delegate { };
			BarriersChanged += delegate { };

			Log.SysLogText(LogLevel.DEBUG, "Init 3");
			_colorIndex = 0;

			ProcessingMetrics = new ProcessingMetrics()
			{
				CannyThreshold = 120,                // 180
				CannyThresholdLinking = 120,         // 120
				RhoRes = 2,
				ThetaResPiDivisor = 45,
				HoughThreshold = 10,                 // decrease to get more lines
				MinimumLandmarkSegment = .05,        // line segments must be at least this many meters in order to be considered
				MinimumConnectLength = .025,         // right angle rays must be within this many meters to connect
				MaximumConnectLength = .2,           // will not connect right angle this far apart
				RightAngleSlackDegrees = 1,          // slack from 90° which may still constitute right angle
				MinimumLandmarkSeparation = .1,      // landmarks must be this many meters apart
				BearingSlack = 2,                    // degrees slack when computing similar angles
				LinePathWidth = .1,                  // lines must be within this many meters to be consolidated into a single line
			};
		}

		#endregion

		public void ProcessImage(Mat image, Double imageOrientation, Double imagePixelsPerMeter)
		{
			Log.SysLogText(LogLevel.DEBUG, "Lidar Processing image of {0}", image.Size);

			Image = image;

			PointD center = image.Center();

			Mat outputImage = Image.Clone();

			CvInvoke.Canny(Image, outputImage, ProcessingMetrics.CannyThreshold, ProcessingMetrics.CannyThresholdLinking);
			LineSegment2D[] segments = CvInvoke.HoughLinesP(outputImage, ProcessingMetrics.RhoRes, Math.PI / ProcessingMetrics.ThetaResPiDivisor, ProcessingMetrics.HoughThreshold);
			LineList lines = segments.ToLineList();
			lines.RemoveInvalid();

			lines.SortUpperLeft();

			Log.SysLogText(LogLevel.DEBUG, "Have {0} segments", lines.Count);

			Mat temp = Image.Clone();
			foreach(Line line in lines)
			{
				CvInvoke.Line(temp, line.P1.ToPoint(), line.P2.ToPoint(), new Bgr(Color.GreenYellow).MCvScalar, 3);
			}
			temp.Save(@"\\raspi\pi\tmp\junk.png");

			ImageVectorList landmarks;
			BarrierList barriers;
			FindLandmarksAndBarriers(lines, center, out landmarks, out barriers);

			Barriers = barriers;
			Landmarks = landmarks;

			Log.SysLogText(LogLevel.DEBUG, "******** {0} landmarks found", landmarks.Count);

			LandmarksChanged(landmarks);
			BarriersChanged(barriers);
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
												out ImageVectorList landmarks, 
												out BarrierList barriers)
		{
			landmarks = new ImageVectorList();
			barriers = new BarrierList();

			Paths = new List<RectangleD>();

			// get rid of short lines
			LineList lines = new LineList(segments);
			Double minimumSizePixels = ProcessingMetrics.MinimumLandmarkSegment * PixelsPerMeter;
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

		private ImageVectorList ConsolidateLandmarks(PointD origin, ImageVectorList from)
		{
			Double minimumPixelDistance = ProcessingMetrics.MinimumLandmarkSeparation * PixelsPerMeter;

			// group them into bunches by range
			ImageVectorGroupList groups = new ImageVectorGroupList();
			while(from.Count > 0)
			{
				ImageVector landmark = from[0];
				ImageVectorList group = groups.FindGroupWithinRangeOfPoint(landmark.GetPoint(), minimumPixelDistance);
				if(group == null)
				{
					group = new ImageVectorList();
					groups.Add(group);
				}
				group.Add(landmark);
				from.RemoveAt(0);
			}

			ImageVectorList landmarks = new ImageVectorList();
			foreach(ImageVectorList group in groups)
			{
				Log.SysLogText(LogLevel.DEBUG, "Dumping Group");
				group.DumpToLog();

				ImageVector centroid = group.GetCentroid(origin);
				landmarks.Add(centroid);
				Log.SysLogText(LogLevel.DEBUG, "Centroid is {0}", centroid);
			}

			return landmarks;
		}

		private ImageVectorList ComputeLandmarks(PointD origin, LineList lines)
		{
			ImageVectorList landmarks = new ImageVectorList();
			foreach(Line l1 in lines)
			{
				foreach(Line l2 in lines)
				{
					if(l1 == l2)
						continue;

					LinePair pair = new LinePair(l1, l2);
					Line join = pair.ClosestPoints;

					Double actualDistance = ConvertToEnviromentLength(join.Length);
					if(actualDistance >= ProcessingMetrics.MinimumConnectLength && actualDistance <= ProcessingMetrics.MaximumConnectLength)
					{
						Double angularDifference = Degrees.AngularDifference(l1.Bearing, l2.Bearing);
						if(angularDifference.IsWithinDegressOf(90, ProcessingMetrics.RightAngleSlackDegrees))
						{
							PointD intersection;
							FlatGeo.GetIntersection(l1, l2, out intersection);

							if(landmarks.Contains(intersection, ProcessingMetrics.MinimumLandmarkSeparation, PixelsPerMeter) == false)
							{
								ImageVector landmark = new ImageVector(origin, new BearingAndRange(origin, intersection));
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
			Double pathWidthPixels = ProcessingMetrics.LinePathWidth * PixelsPerMeter;
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
					if(l1.Bearing.IsWithinDegressOf(l2.Bearing, ProcessingMetrics.BearingSlack) || l1.Bearing.AddDegrees(180).IsWithinDegressOf(l2.Bearing, ProcessingMetrics.BearingSlack))
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
						if(group.AverageBearing.AngularDifference(similarLines.AverageBearing) <= ProcessingMetrics.BearingSlack &&
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

		public Double ConvertToEnviromentLength(Double length)
		{
			return length / PixelsPerMeter;
		}

		public Double ConvertToBitmapLength(Double length)
		{
			return length * PixelsPerMeter;
		}

		public Mat CreateImage(SpatialObjects objects)
		{
			Mat image = Image.Clone();

			if((objects & SpatialObjects.CenterPoint) != 0)
			{
				Cross2DF cross = new Cross2DF(BitmapCenter.ToPoint(), 4, 4);
				CvInvoke.Line(image, cross.Vertical.P1.ToPoint(), cross.Vertical.P2.ToPoint(), new Bgr(Color.GreenYellow).MCvScalar);
				CvInvoke.Line(image, cross.Horizontal.P1.ToPoint(), cross.Horizontal.P2.ToPoint(), new Bgr(Color.GreenYellow).MCvScalar);
			}

			if((objects & SpatialObjects.Barriers) != 0)
			{
				foreach(ImageBarrier barrier in Barriers)
				{
					Color color = NextColor;
					Line line = barrier.GetLine();
					CvInvoke.Line(image, line.P1.ToPoint(), line.P2.ToPoint(), new Bgr(color).MCvScalar, 2);
				}
			}

			if((objects & SpatialObjects.Landmarks) != 0)
			{
				foreach(ImageVector landmark in Landmarks)
				{
					CircleF circle1 = new CircleF(landmark.GetPoint().ToPoint(), 3);
					CvInvoke.Circle(image, circle1.Center.ToPoint(), (int)circle1.Radius, new Bgr(Color.Blue).MCvScalar, 2);
				}
			}

			return image;
		}

		/// <summary>
		/// Get the shortest range centered at the given bearing in the given angular width
		/// </summary>
		/// <param name="trueBearing"></param>
		/// <param name="angularWidth"></param>
		/// <returns></returns>
		public Double ShortestRangeAtBearing(Double trueBearing, Double angularWidth)
		{
			Double start = trueBearing.SubtractDegrees(angularWidth / 2);
			Double end = trueBearing.AddDegrees((angularWidth / 2) + 1);

			List<Double> allDistances = new List<double>();
			Double angle = start;
			while(angle.IsWithinDegressOf(end, Lidar.VectorSize) == false)
			{
				Double distance = Lidar.GetRangeAtBearing(angle);
//				Log.SysLogText(LogLevel.DEBUG, "At {0:0.000}° range is {1:0.000}", angle, distance);
				if(distance != 0)
				{
					allDistances.Add(distance);
				}

				angle = angle.AddDegrees(Lidar.VectorSize);
			}

			return allDistances.Count > 0 ? allDistances.Min() : 0;
		}

		/// <summary>
		/// Find the best set of destinations on the map
		/// </summary>
		/// <param name="pointCloud"></param>
		/// <param name="chassis"></param>
		/// <param name="rangeFuzz"></param>
		/// <param name="requireClearUpTo"></param>
		/// <returns></returns>
		public static FuzzyPathList FindGoodDestinations(PointCloud2D pointCloud, Chassis chassis, Double rangeFuzz, int maxDestinations, Double requireClearUpTo)
		{
			Performance.Timers[TimerTypes.FindDestination].Start();

			Double window = 45;
			Log.SysLogText(LogLevel.DEBUG, "Finding a destination clear up to {0} with a range fuzz of {1} and {2} vectors in the tank", 
				requireClearUpTo.ToMetersString(), rangeFuzz.ToAngleString(), pointCloud.Count);

			Performance.Timers[TimerTypes.FindShortestRange].Start();

			Double bearing;
			//pointCloud.DumpToLog("PC");
			BearingAndRangeList vectors = new BearingAndRangeList();
			BearingAndRange[] pointCloudArray = pointCloud.ToArray();
			for(bearing = 0;bearing < 360;bearing++)
			{
				Double range = pointCloudArray.ShortestRangeBetween(bearing.SubtractDegrees(window / 2), bearing.AddDegrees(window / 2));
				vectors.Add(new BearingAndRange(bearing, range));
				//Log.SysLogText(LogLevel.DEBUG, "Shortest range between {0} and {1} is {2}m",
				//	bearing.SubtractDegrees(window / 2).ToAngleString(),
				//	bearing.AddDegrees(window / 2).ToAngleString(), range);
			}

			// break the ranges up into chunks
			// round all ranges to millimeter
			vectors.RoundRanges(2);

			Double lastRange = vectors[359].Range;
			//  2. find start point
			for(bearing = 0;bearing < 360 && vectors[(int)bearing].Range == lastRange;lastRange = vectors[(int)bearing].Range, bearing++) ;

			// group into chunks
			List<BearingAndRangeList> chunks = new List<BearingAndRangeList>();
			Double startBearing = bearing;
			BearingAndRangeList workingChunk = null;
			lastRange = -1;
			do
			{
				if(vectors[(int)bearing].Range != lastRange)
				{
					workingChunk = new BearingAndRangeList();
					chunks.Add(workingChunk);
					lastRange = vectors[(int)bearing].Range;
				}
				workingChunk.Add(vectors[(int)bearing]);

			} while((bearing = bearing.AddDegrees(1)) != startBearing);

			chunks.Sort(new BearingAndRangeList.BearingAndRangeListRangeComprarer());
			chunks.Reverse();

			Performance.Timers[TimerTypes.FindShortestRange].StopAndLog();

			const Double MIN_SEPARATION = 45;

			FuzzyPathList paths = new FuzzyPathList();
			for(int x = 0;x < chunks.Count && paths.Count < maxDestinations;x++)
			{
				Double chunkBearing = chunks[x].AverageBearing;
				if(paths.ContainsPathNearBearing(chunkBearing, MIN_SEPARATION) == false)
				{
					PointCloud2D leftCloud, rightCloud;
					FuzzyPath path = MakeFuzzyPath(pointCloud, chunkBearing, chassis.FrontLeftLidarVector, chassis.FrontRightLidarVector, rangeFuzz, out leftCloud, out rightCloud);
					paths.Add(path);
				}
			}

			Performance.Timers[TimerTypes.FindDestination].StopAndLog();

			return paths;

		}

		/// <summary>
		/// Get the fuzzy range at the given vector
		/// </summary>
		/// <param name="chassis"></param>
		/// <param name="bearing"></param>
		/// <param name="angularWidth"></param>
		/// <returns></returns>
		public Double FuzzyRangeAtBearing(Chassis chassis, Double bearing, Double angularWidth)
		{
			PointCloud2D left, right;
			return FuzzyRangeAtBearing(chassis, bearing, angularWidth, out left, out right);
		}

		/// <summary>
		/// Get the fuzzy range at the given vector
		/// </summary>
		/// <param name="chassis"></param>
		/// <param name="bearing"></param>
		/// <param name="angularWidth"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public Double FuzzyRangeAtBearing(Chassis chassis, Double bearing, Double angularWidth, out PointCloud2D left, out PointCloud2D right)
		{
			return FuzzyRangeAtBearing(bearing, chassis.FrontLeftLidarVector, chassis.FrontRightLidarVector, angularWidth, out left, out right);
		}

		/// <summary>
		/// Will return an average range from both the front right and front left chassis points
		/// </summary>
		/// <param name="frontLeftWheelOffset"></param>
		/// <param name="frontRightWheelOffset"></param>
		/// <param name="bearing"></param>
		/// <param name="angularWidth"></param>
		/// <returns></returns>
		public Double FuzzyRangeAtBearing(
			Double bearing,
			BearingAndRange frontLeftWheelOffset, BearingAndRange frontRightWheelOffset,
			Double angularWidth,
			out PointCloud2D frontLeftCloud, out PointCloud2D frontRightCloud)
		{
			FuzzyPath fuzzyPath = MakeFuzzyPath(bearing, frontLeftWheelOffset, frontRightWheelOffset, angularWidth, out frontLeftCloud, out frontRightCloud);
			Double range = fuzzyPath != null ? Math.Min(fuzzyPath.FrontLeft.MinimumRange, fuzzyPath.FrontRight.MinimumRange)
				: 0;
			return range;
		}

		/// <summary>
		/// Create a fuzzy path along the given vector
		/// </summary>
		/// <param name="bearing"></param>
		/// <param name="chasis"></param>
		/// <param name="angularWidth"></param>
		/// <returns></returns>
		public FuzzyPath MakeFuzzyPath(
			Double bearing,
			Chassis chasis,
			Double angularWidth)
		{
			PointCloud2D frontLeftCloud, frontRightCloud;
			return MakeFuzzyPath(bearing, chasis.FrontLeftLidarVector, chasis.FrontRightLidarVector, angularWidth, out frontLeftCloud, out frontRightCloud);
		}

		/// <summary>
		/// Create a fuzzy path along the given vector
		/// </summary>
		/// <param name="bearing"></param>
		/// <param name="frontLeftWheelOffset"></param>
		/// <param name="frontRightWheelOffset"></param>
		/// <param name="angularWidth"></param>
		/// <returns></returns>
		public FuzzyPath MakeFuzzyPath(
			Double bearing,
			BearingAndRange frontLeftWheelOffset, BearingAndRange frontRightWheelOffset,
			Double angularWidth)
		{
			PointCloud2D frontLeftCloud, frontRightCloud;
			return MakeFuzzyPath(bearing, frontLeftWheelOffset, frontRightWheelOffset, angularWidth, out frontLeftCloud, out frontRightCloud);
		}

		/// <summary>
		/// Create a fuzzy path along the given vector
		/// </summary>
		/// <param name="bearing"></param>
		/// <param name="frontLeftWheelOffset"></param>
		/// <param name="frontRightWheelOffset"></param>
		/// <param name="angularWidth"></param>
		/// <param name="frontLeftCloud"></param>
		/// <param name="frontRightCloud"></param>
		/// <returns></returns>
		public FuzzyPath MakeFuzzyPath(
			Double bearing, 
			BearingAndRange frontLeftWheelOffset, BearingAndRange frontRightWheelOffset, 
			Double angularWidth,
			out PointCloud2D frontLeftCloud, out PointCloud2D frontRightCloud)
		{
			FuzzyPath path = null;
			frontLeftCloud = frontRightCloud = null;
			try
			{
				if(angularWidth == 0)
				{
					angularWidth = RangeFuzz;
				}

				PointCloud2D vectorsFromLidar = Lidar.Vectors.ToPointCloud2D(360);

				path = MakeFuzzyPath(vectorsFromLidar, bearing, frontLeftWheelOffset, frontRightWheelOffset, angularWidth, out frontLeftCloud, out frontRightCloud);

			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.DEBUG, "Make fuzzy path EXCEPTION: {0}", e.Message);
			}
			
			return path;
		}

		/// <summary>
		/// Create a fuzzy path along the given vector
		/// </summary>
		/// <param name="bearing"></param>
		/// <param name="frontLeftWheelOffset"></param>
		/// <param name="frontRightWheelOffset"></param>
		/// <param name="angularWidth"></param>
		/// <param name="frontLeftCloud"></param>
		/// <param name="frontRightCloud"></param>
		/// <returns></returns>
		public static FuzzyPath MakeFuzzyPath(
			PointCloud2D vectorsFromLidar,
			Double bearing,
			BearingAndRange frontLeftWheelOffset, BearingAndRange frontRightWheelOffset,
			Double angularWidth,
			out PointCloud2D frontLeftCloud, out PointCloud2D frontRightCloud)
		{
			FuzzyPath path = null;

			frontLeftWheelOffset = frontLeftWheelOffset.Rotate(bearing);
			frontRightWheelOffset = frontRightWheelOffset.Rotate(bearing);

			frontLeftCloud = vectorsFromLidar.Move(frontLeftWheelOffset);
			frontRightCloud = vectorsFromLidar.Move(frontRightWheelOffset);

			PointCloud2DSlice frontLeftSlice = new PointCloud2DSlice(new PointD(0,0).GetPointAt(frontLeftWheelOffset), bearing, frontLeftCloud, angularWidth);
			PointCloud2DSlice frontRightSlice = new PointCloud2DSlice(new PointD(0,0).GetPointAt(frontRightWheelOffset), bearing, frontRightCloud, angularWidth);

			if(frontRightSlice.Count > 0 && frontRightSlice.Count > 0)
			{
				path = new FuzzyPath(frontLeftSlice, frontRightSlice);
			}

			return path;
		}

		/// <summary>
		/// Clear all range vectors
		/// </summary>
		public void Reset()
		{
			Lidar.ClearDistanceVectors();
		}
	}
}
