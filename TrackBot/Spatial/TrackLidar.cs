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
using RaspiCommon.Lidar.Environs;

namespace TrackBot.Spatial
{
	class TrackLidar : LidarEnvironment, IEnvironment
	{
		public Double Range { get { return FuzzyRangeAtBearing(Widgets.GyMag.Bearing); } }
		public Double CompassOffset { get { return Lidar.Offset; } set { Lidar.Offset = value; } }
		public Size PixelSize { get { return new Size((int)(PixelsPerMeter * MetersSquare), (int)(PixelsPerMeter * MetersSquare)); } }
		public PointD PixelCenter { get { return new PointD((int)(PixelsPerMeter * MetersSquare) / 2, (int)(PixelsPerMeter * MetersSquare) / 2); } }
		public Double RenderPixelsPerMeter { get { return Lidar.RenderPixelsPerMeter; } set { Lidar.RenderPixelsPerMeter = value; } }

		public double DebugAngle { get { return Lidar.DebugAngle; } set { Lidar.DebugAngle = value; } }

		public Double VectorSize { get { return Lidar.VectorSize; } }

		public TrackLidar(Double metersSquare, Double pixelsPerMeter)
			: base(metersSquare, pixelsPerMeter)
		{
			Lidar = new RPLidar(Program.Config.LidarComPort, .25);
			Lidar.Offset = Program.Config.LidarOffsetDegrees;
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

		public Line FindGoodDestination()
		{
			Console.WriteLine("Finding a destination");

			Line longestLine = null;

			Double startBearing = Widgets.GyMag.Bearing;
			PointD currentLocation = new PointD(0, 0);
			for(double x = 1;x < 360;x ++)
			{
				if(FuzzyRangeAtBearing(x) != 0)
				{
					Double bearing = startBearing.AddDegrees(x);
					//				Console.WriteLine("From {0:0}° to {1:0}°", startBearing, bearing);
					Line line = new Line(currentLocation, FlatGeo.GetPoint(currentLocation, bearing, Widgets.Environment.GetRangeAtBearing(x)));
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

			return longestLine;

		}

		public Mat GenerateBitmap(bool radarLines = false, bool drawTank = false)
		{
			Mat mat = new Mat(PixelSize, DepthType.Cv8U, 3);
			MCvScalar lineColor = new Bgr(Color.Green).MCvScalar;
			MCvScalar botColor = new Bgr(Color.AliceBlue).MCvScalar;
			MCvScalar dotColor = new Bgr(Color.YellowGreen).MCvScalar;

			if(radarLines)
			{
				// lines and circles
				List<Double> offsets = new List<Double>() { 1, 2, 3, 4, 5 };
				foreach(Double offset in offsets)
				{
					CvInvoke.Circle(mat, PixelCenter.ToPoint(), (int)(Widgets.Environment.RenderPixelsPerMeter * offset), lineColor);
				}

				CvInvoke.Line(mat, new Point((int)PixelCenter.X, 0), new Point((int)PixelCenter.X, mat.Height), lineColor);
				CvInvoke.Line(mat, new Point(0, (int)PixelCenter.Y), new Point(mat.Width, (int)PixelCenter.Y), lineColor);

				// show where we are
				CvInvoke.Circle(mat, PixelCenter.ToPoint(), 5, botColor, 2);
			}

			PointD center = mat.CenterPoint();

			if(drawTank)
			{
				if(File.Exists(Program.Config.BotImage))
				{
					Mat tank = new Mat(Program.Config.BotImage);
					CvInvoke.Resize(tank, tank, new Size(25, 25));

					PointD tankcenter = tank.CenterPoint();
					Mat rotationMatrix = new Mat();
					CvInvoke.GetRotationMatrix2D(tankcenter.ToPoint(), Widgets.GyMag.Bearing.SubtractDegrees(180), 1, rotationMatrix);
					Mat rotated = new Mat();
					CvInvoke.WarpAffine(tank, rotated, rotationMatrix, tank.Size);

					PointD drawLocation = new PointD(center.X - tank.Width / 2, center.Y - tank.Height / 2);

					Rectangle roi = new Rectangle((int)drawLocation.X, (int)drawLocation.Y, tank.Width, tank.Height);
					rotated.CopyTo(new Mat(mat, roi));
				}
				else
				{
					Log.SysLogText(LogLevel.DEBUG, "File {0} not found in {1}", Program.Config.BotImage, Directory.GetCurrentDirectory());
				}
			}

			//if(DestinationPoint != null)
			//{
			//	String centerString = "O";
			//	size = g.MeasureString(centerString, font);
			//	Line line = new Line(center, DestinationPoint);
			//	PointD dest = PointD.FindCenterUpperLeft(FlatGeo.GetPoint(center, line.Bearing, line.Length * Widgets.Lidar.RenderPixelsPerMeter), size);
			//	g.DrawString(centerString, font, new SolidBrush(Color.Green), dest.ToPoint());
			//}

			Rectangle rect;
			for(Double bearing = 0;bearing < 360;bearing += Lidar.VectorSize)
			{
				Double rangeMeters = Lidar.GetRangeAtBearing(bearing);
				Double range = rangeMeters * PixelsPerMeter;
				PointD point = PixelCenter.GetPointAt(bearing, range) as PointD;
				rect = new Rectangle(point.ToPoint(), new Size(1, 1));
				CvInvoke.Rectangle(mat, rect, dotColor);
				if(bearing.AngularDifference(0) < 2)
				{
					Log.SysLogText(LogLevel.DEBUG, "Range at {0:0.00}° is {1:0.000}m  drew dot at {2}", bearing, rangeMeters, point);
				}
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
