using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.Extensions;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial.Imaging;

namespace TrackBot.Spatial
{
	class TrackLidar : LidarEnvironment, IImageEnvironment
	{
		public Double Range { get { return FuzzyRangeAtBearing(Widgets.GyMag.Bearing, RangeFuzz); } }
		public Double CompassOffset { get { return Lidar.Offset; } set { Lidar.Offset = value; } }
		public Size PixelSize { get { return new Size((int)(PixelsPerMeter * MetersSquare), (int)(PixelsPerMeter * MetersSquare)); } }
		public PointD PixelCenter { get { return new PointD((int)(PixelsPerMeter * MetersSquare) / 2, (int)(PixelsPerMeter * MetersSquare) / 2); } }
		public Double RenderPixelsPerMeter { get { return Lidar.RenderPixelsPerMeter; } set { Lidar.RenderPixelsPerMeter = value; } }

		public double DebugAngle { get { return Lidar.DebugAngle; } set { Lidar.DebugAngle = value; } }

		public Double VectorSize { get { return Lidar.VectorSize; } }
		public IVector[] Vectors { get { return Lidar.Vectors; } }

		public TrackLidar(Double metersSquare, Double pixelsPerMeter)
			: base(metersSquare, pixelsPerMeter)
		{
			Lidar = new RPLidar(Program.Config.LidarComPort, .25);
			Lidar.Offset = Program.Config.LidarOffsetDegrees;
			RangeFuzz = Program.Config.RangeFuzz;
			Log.SysLogText(LogLevel.DEBUG, "Range fuzz is {0:0}°", RangeFuzz);
		}

		public void Start()
		{
			Lidar.Start();
			if(Lidar.GetDeviceInfo())
			{
				Log.SysLogText(LogLevel.DEBUG, "Retrieved LIDAR info");
				Lidar.StartScan();
				Log.SysLogText(LogLevel.DEBUG, "LIDAR scan started");
			}
		}

		public void Stop()
		{
			Lidar.StopScan();
			GpioSharp.Sleep(250);
			Lidar.Reset();
			GpioSharp.Sleep(250);
			Lidar.Stop();

			Log.SysLogText(LogLevel.DEBUG, "LIDAR stopped");
		}

		public FuzzyPath FindGoodDestination()
		{
			Console.WriteLine("Finding a destination");

			Line longestLine = null;

			Double startBearing = Widgets.GyMag.Bearing;
			PointD currentLocation = new PointD(0, 0);
			for(double x = 1;x < 360;x ++)
			{
				if(FuzzyRangeAtBearing(x, RangeFuzz) != 0)
				{
					Double bearing = startBearing.AddDegrees(x);
					//				Console.WriteLine("From {0:0}° to {1:0}°", startBearing, bearing);
					Line line = new Line(currentLocation, FlatGeo.GetPoint(currentLocation, bearing, Widgets.ImageEnvironment.GetRangeAtBearing(x)));
					if(line != null)
					{
						if(longestLine == null || line.Length > longestLine.Length)
						{
//							Console.WriteLine("Got longest line {0:0.0} meters [{1} to {2}] at {3:0.00}°", line.Length, line.P1, line.P2, line.Bearing);
							longestLine = line;
						}
					}
				}
			}

			FuzzyPath path = null;
			if(longestLine != null)
			{
				path = MakeFuzzyPath(longestLine.Bearing, RangeFuzz);
			}

			return path;

		}

		public Mat PointsToBitmap()
		{
			Mat mat = new Mat(PixelSize, DepthType.Cv8U, 3);
			MCvScalar dotColor = new Bgr(Color.YellowGreen).MCvScalar;
			PointD center = mat.Center();

			Rectangle rect;
			for(Double bearing = 0;bearing < 360;bearing += Lidar.VectorSize)
			{
				Double rangeMeters = Lidar.GetRangeAtBearing(bearing);
				Double range = rangeMeters * PixelsPerMeter;
				PointD point = PixelCenter.GetPointAt(bearing, range) as PointD;
				rect = new Rectangle(point.ToPoint(), new Size(1, 1));
				CvInvoke.Rectangle(mat, rect, dotColor);
			}

			rect = new Rectangle(PixelCenter.ToPoint(), new Size(1, 1));
			CvInvoke.Rectangle(mat, rect, new Bgr(Color.Yellow).MCvScalar);

			return mat;
		}

		public double GetRangeAtBearing(double bearing)
		{
			return Lidar.GetRangeAtBearing(bearing);
		}

		public DateTime GetLastSampleTimeAtBearing(Double bearing)
		{
			return Lidar.GetLastSampleTimeAtBearing(bearing);
		}
	}
}
