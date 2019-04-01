using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;

namespace TrackBot.Spatial
{
	public class Area
	{
		public Grid Grid { get; private set; }

		PointD _location;
		public PointD Location
		{
			get { return _location; }
			set
			{
				_location = value;
				Cell cell;
				if((cell = Grid.GetCellAtLocation(_location)) != null)
				{
					cell.RoboIsHere = true;
					cell.RoboWasHere = true;
				}
			}
		}

		public PointD StartLoction { get; private set; }

		public Area(Double width, Double height, Double cellSize, PointD startLocation)
		{
			Console.WriteLine("Creating grid");
			Grid = new Grid("Tracker", width, height, cellSize);
			Console.WriteLine("Grid complete");
			Location = startLocation;
			StartLoction = startLocation;
		}

		public void Reset()
		{
			Grid.Clear();
			Location = StartLoction;
		}

		public void ClearPathToObstacle(PointD obstacleLocation, bool markObstacle)
		{
//			Console.WriteLine("Clearing path from {0} to {1}", Location, obstacleLocation);

			Cell obstacleCell = Grid.GetCellAtLocation(obstacleLocation);
			Line line = new Line(Location, obstacleLocation);
			if(obstacleCell != null)
			{
				for(PointD point = Location;
					point != null && FlatGeo.Distance(Location, point) <= line.Length;
					point = FlatGeo.GetPoint(point, line.Bearing, Grid.CellSize))
				{
					Cell cell = Grid.GetCellAtLocation(point);
//					Console.WriteLine("   Clearing {0}", cell.Center);

					cell.Contents = CellContents.Empty;
				}

				if(markObstacle)
				{
					obstacleCell.Contents = CellContents.Barrier;
				}
			}

		}

		public PointD FindGoodDestination()
		{
			Console.WriteLine("Finding a destination");

			Line longestLine = null;

			Double startBearing = Widgets.GyMag.Bearing;
			for(double x = 0;x < 360;x += 10)
			{
				Double bearing = startBearing.AddDegrees(x);
//				Console.WriteLine("From {0:0}° to {1:0}°", startBearing, bearing);
				Line line = GetClearLine(bearing);
				if(line != null)
				{
					if(longestLine == null || line.Length > longestLine.Length)
					{
//						Console.WriteLine("Got longest line {0:0.0} meters [{1} to {2}] at {3:0.00}°", line.Length, line.P1, line.P2, line.Bearing);
						longestLine = line;
					}
				}
			}

			PointD ret = longestLine != null ? longestLine.P2 as PointD : null;
			return ret;
		}

		public void SaveBitmap()
		{
			Mat bitmap = Widgets.Environment.GenerateBitmap(false, false);  // Widgets.Environment.Grid.ConvertToBitmap(10);
			bitmap.Save("/var/www/html/grid.png");

			Console.WriteLine("Bitmap saved");
		}

		Line GetClearLine(Double bearing)
		{
			Line line = null;

			Cell location = Grid.GetCellAtLocation(Location);
			if(location != null)
			{
				for(Double length = Grid.CellSize;length < Grid.HeightMeters;length += Grid.CellSize)
				{
					Line thisLine = new Line(Location, bearing, length);
//					Console.WriteLine("getting from {0} bearing {1} length {2}", Location, bearing, length);
					Cell thisCell = Grid.GetCellAtLocation(thisLine.P2 as PointD);
					if(thisCell == null)
					{
						break;
					}

					if(thisCell.Contents == CellContents.Empty)
					{
						line = thisLine;
					}
					else if(thisCell.Contents == CellContents.Barrier)
					{
						break;
					}
				}
			}
			// Console.WriteLine("returning line {0}", line == null ? "null" : line.ToString());
			return line;
		}
	}
}
