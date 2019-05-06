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
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Performance;
using RaspiCommon.Extensions;

namespace RaspiCommon.GraphicalHelp
{
	public class LEDCandidate
	{
		public Point Center { get; set; }
		public Rectangle BoundingRectangle { get; set; }
		public Double Concentration { get; set; }

		public LEDCandidate(Point center, Rectangle boundingRectangle, Double concentration)
		{
			Center = center;
			BoundingRectangle = boundingRectangle;
			Concentration = concentration;
		}

		public static bool TryGetCandidate(Mat image, Point startPoint, Size chunkSize, out LEDCandidate candidate)
		{
			PerformanceTimer p1 = new PerformanceTimer("p1");
			PerformanceTimer p2 = new PerformanceTimer("p2");

			int total = 0;
			int processed = 0;

			ImageChunkList chunks = new ImageChunkList(image.Size);
			Mat used = new Mat(image.Size, DepthType.Cv8U, 1);
			used.SetTo(new MCvScalar(0));

			Log.SysLogText(LogLevel.DEBUG, "TOP");
			int regionCenterX = image.ValidX(startPoint.X + chunkSize.Width / 2);
			int regionCenterY = image.ValidY(startPoint.Y + chunkSize.Height / 2);

			int rowsToGet = 4;
			int colsToGet = 4;

			candidate = null;
			int lastTotalNonZero = 0;
			int finalY = 0;
			while(true)
			{
				Log.SysLogText(LogLevel.DEBUG, "Processing Next batch");
				for(int row = 0;row < rowsToGet;row++)
				{
					int tmpY = Math.Max(regionCenterY - (rowsToGet / 2) * chunkSize.Height, 0);
					int y = image.ValidY(tmpY + row * chunkSize.Height);
					finalY = Math.Max(y, finalY);
					int regionWidth = colsToGet * chunkSize.Width;
					for(int x = image.ValidX(regionCenterX - (colsToGet / 2) * chunkSize.Width);x < image.ValidX(regionCenterX + (colsToGet / 2) * chunkSize.Width);x += chunkSize.Width)
					{
						p1.Start();
						++total;
						PointD here = new PointD(x, y);
						bool beenHere = used.GetPixel(here, 0) != 0;
						if(beenHere)
							continue;
						++processed;
						p1.Stop();
						used.SetPixel(here, 0, 1);
						//Log.SysLogText(LogLevel.DEBUG, "Processing x: {0}  y: {1}", x, y);
						Rectangle rect = image.ValidRectangle(new Point(x, y), chunkSize);
						Mat roi = new Mat(image, rect);
						p2.Start();
						if(chunks.Exists(rect) == false)
						{
							ImageChunk chunk = new ImageChunk(image, roi, rect);
							chunks.AddChunk(chunk);
						}
						p2.Stop();
						//Mat roi = image.GetROIWithinImage(startPoint, chunkSize, x, y);
					}
				}
				int newNonZero = chunks.TotalNonZero;
				if(newNonZero == lastTotalNonZero || finalY >= image.Height - chunkSize.Height)
				{
					p1.DumpToLog();
					p2.DumpToLog();
					chunks.TrimEmpty();
					if(chunks.Count > 0)
					{
						int minx = chunks.MinX;
						int minY = chunks.MinY;
						Size resultSize = new Size(chunks.MaxX - chunks.MinX + chunkSize.Width, chunks.MaxY - chunks.MinY + chunkSize.Height);
						Point upperLeft = new Point(minx, minY);
						resultSize = new Size(Math.Min(resultSize.Width, image.Width - minx), Math.Min(resultSize.Height, image.Height - minY));
						Rectangle resultRect = new Rectangle(upperLeft, resultSize);
						Rectangle trimmedRect = image.ShrinkROIToNonZero(resultRect);
						Mat roi = new Mat(image, trimmedRect);
						Double nonZero = CvInvoke.CountNonZero(roi);
						Double ratio = nonZero / (Double)(trimmedRect.Height * trimmedRect.Width);
						candidate = new LEDCandidate(resultRect.Centroid().ToPoint(), trimmedRect, ratio);
					}
					break;
				}
				lastTotalNonZero = newNonZero;
				rowsToGet += 2;
				colsToGet += 2;

			}
			return candidate != null;
		}

		public override string ToString()
		{
			return String.Format("ø: {0}  Rect: {1}  Conc: {2:0.000}", Center, BoundingRectangle, Concentration);
		}

		class ExpandingSearchInfo
		{
			public Mat Image { get; private set; }
			public Point Origin { get; private set; }
			public Size CellSize { get; private set; }
			public int LeftX { get; private set; }
			public int RightX { get; private set; }
			public int TopY { get; private set; }
			public int BottomY { get; private set; }

			public bool TopLocked { get; set; }
			public bool BottomLocked { get; set; }
			public bool LeftLocked { get; set; }
			public bool RightLocked { get; set; }

			public ExpandingSearchInfo(Mat image, Point origin, Size cellSize)
			{
				Image = image;
				Origin = origin;
				CellSize = cellSize;

				TopLocked = BottomLocked = LeftLocked = RightLocked = false;

				TopY = image.ValidY(Origin.Y - CellSize.Height / 2);
				BottomY = image.ValidY(Origin.Y + CellSize.Height / 2);
				LeftX = image.ValidX(Origin.X - CellSize.Width / 2);
				RightX = image.ValidX(Origin.X + CellSize.Width / 2);
			}

			public void Expand()
			{
				if(TopLocked == false)
					TopY = Image.ValidY(TopY - CellSize.Height);
			}
		}

		class ImageChunk
		{
			public Mat Image { get; set; }
			public Mat ROI { get; set; }
			public Rectangle Region { get; set; }
			public int NonZero { get; set; }

			public ImageChunk(Mat image, Mat roi, Rectangle rect)
			{
				Image = image;
				ROI = roi;
				Region = rect;

				NonZero = CvInvoke.CountNonZero(roi);
			}

			public override string ToString()
			{
				return String.Format("{0} - NZ: {1}", Region, NonZero);
			}
		}

		class ImageChunkList : List<ImageChunk>
		{
			public int TotalNonZero { get { return this.Sum(i => i.NonZero); } }
			public int MinX { get { return this.Min(c => c.Region.X); } }
			public int MinY { get { return this.Min(c => c.Region.Y); } }
			public int MaxX { get { return this.Max(c => c.Region.X); } }
			public int MaxY { get { return this.Max(c => c.Region.Y); } }

			public Size Size { get { return new Size(MaxX - MinX, MaxY - MinY); } }

			public Size ImageSize { get; private set; }

			Mat _used;

			public ImageChunkList(Size imageSize)
			{
				ImageSize = imageSize;
				_used = new Mat(imageSize, DepthType.Cv8U, 1);
				_used.SetTo(new MCvScalar(0));
			}

			public void AddChunk(ImageChunk chunk)
			{
				Add(chunk);
				_used.SetPixel(new PointD(chunk.Region.Location), 0, 1);
			}

			public bool Exists(Rectangle region)
			{
				bool beenHere = _used.GetPixel(new PointD(region.Location),  0) != 0;
				if(beenHere)
				{
					int i = 1;
				}
				return beenHere;
			}

			public void TrimEmpty()
			{
				RemoveAll(c => c.NonZero == 0);
			}
		}
	}

	public class LEDCandidateList : List<LEDCandidate>
	{
		public bool Contains(Point point)
		{
			bool result = false;
			foreach(LEDCandidate candidate in this)
			{
				if(candidate.BoundingRectangle.Contains(point))
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}


}
