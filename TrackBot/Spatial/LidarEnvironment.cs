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

namespace TrackBot.Spatial
{
	public class LidarEnvironment2
	{
		public const Double RANGE_FUZZ = 2;

		public PointD Location { get { return new PointD(0, 0); } }
		public PointD RelativeLocation { get; set; }
		public PointD DestinationPoint { get; set; }
		public Double Range { get { return FuzzyRange(); } }

		public Line FindGoodDestination()
		{
			Console.WriteLine("Finding a destination");

			Line longestLine = null;

			Double startBearing = Widgets.GyMag.Bearing;
			PointD currentLocation = new PointD(0, 0);
			for(double x = 0;x < 360;x += 5)
			{
				if(Widgets.Lidar.GetDistance(x) != 0)
				{
					Double bearing = startBearing.AddDegrees(x);
					//				Console.WriteLine("From {0:0}° to {1:0}°", startBearing, bearing);
					Line line = new Line(currentLocation, FlatGeo.GetPoint(currentLocation, bearing, Widgets.Lidar.GetDistance(x)));
					if(line != null)
					{
						if(longestLine == null || line.Length > longestLine.Length)
						{
							Console.WriteLine("Got longest line {0:0.0} meters [{1} to {2}] at {3:0.00}°", line.Length, line.P1, line.P2, line.Bearing);
							longestLine = line;
						}
					}
				}
			}

			return longestLine;

		}

		public Bitmap GenerateBitmap(bool radarLines = false)
		{
			Bitmap bitmap = Widgets.Lidar.GenerateBitmap();

			PointD center = new PointD(bitmap.Width / 2, bitmap.Height / 2);
			Pen linePen = new Pen(Color.Green);
			using(Graphics g = Graphics.FromImage(bitmap))
			{

				if(radarLines)
				{
					// lines and circles
					List<Double> offsets = new List<Double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
					foreach(Double offset in offsets)
					{
						RectangleD rect = RectangleD.SquareFromCenter(center, Widgets.Lidar.RenderPixelsPerMeter * offset);
						g.DrawEllipse(linePen, rect.ToRectangle());
					}

					g.DrawLine(linePen, (float)center.X, 0, (float)center.X, bitmap.Height);
					g.DrawLine(linePen, 0, (float)center.Y, bitmap.Width, (float)center.Y);
				}

				Font font = new Font("Times New Roman", 12.0f);

				SizeF size;
				if(DestinationPoint != null)
				{
					String centerString = "O";
					size = g.MeasureString(centerString, font);
					Line line = new Line(center, DestinationPoint);
					PointD dest = PointD.FindCenterUpperLeft(FlatGeo.GetPoint(center, line.Bearing, line.Length * Widgets.Lidar.RenderPixelsPerMeter), size);
					g.DrawString(centerString, font, new SolidBrush(Color.Green), dest.ToPoint());
				}

				// show where we are
				String botString = "O";
				size = g.MeasureString(botString, font);
				PointD drawBot = PointD.FindCenterUpperLeft(center, size);
				g.DrawString(botString, font, new SolidBrush(Color.CadetBlue), drawBot.ToPoint());
			}

			return bitmap;
		}

		public Double FuzzyRange(Double angularWidth = RANGE_FUZZ)
		{
			Double start = Widgets.GyMag.Bearing.SubtractDegrees(angularWidth / 2);
			Double end = Widgets.GyMag.Bearing.AddDegrees(angularWidth / 2);

			List<Double> allDistances = new List<double>();
			for(Double d = (int)start;d <= (int)end;d = d.AddDegrees(1))
			{
				Double distance = Widgets.Lidar.GetDistance(d);
				Log.SysLogText(LogLevel.DEBUG, "At {0}° range is {1:0.000}", d, distance);
				if(distance != 0)
				{
					allDistances.Add(distance);
				}
			}

			return allDistances.Count > 0 ? allDistances.Average() : 0;
		}

		public void Reset()
		{

		}
	}
}
