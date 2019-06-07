using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using KanoopCommon.Performance;
using RaspiCommon.Extensions;

namespace RaspiCommon.GraphicalHelp
{
	public class LEDCandidate
	{
		public static int ByteArraySize { get { return ColorExtensions.ByteArraySize + PointExtensions.ByteArraySize + RectangleExtensions.ByteArraySize + sizeof(Double) + sizeof(int);  } }

		public Color Color { get; set; }
		public Point Center { get; set; }
		public Rectangle BoundingRectangle { get; set; }
		public Double Concentration { get; set; }
		public int CountNonZero { get; set; }

		public LEDCandidate(Color color, Point center, Rectangle boundingRectangle, Double concentration, int countNonZero)
		{
			Color = color;
			Center = center;
			BoundingRectangle = boundingRectangle;
			Concentration = concentration;
			CountNonZero = countNonZero;
		}

		public LEDCandidate(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				Color = ColorExtensions.Deserialize(br.ReadBytes(ColorExtensions.ByteArraySize));
				Center = PointExtensions.Deserialize(br);
				BoundingRectangle = RectangleExtensions.Deserialize(br);
				Concentration = br.ReadDouble();
				CountNonZero = br.ReadInt32();
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Color.Serialize());
				bw.Write(PointExtensions.Serialize(Center));
				bw.Write(RectangleExtensions.Serialize(BoundingRectangle));
				bw.Write(Concentration);
				bw.Write(CountNonZero);
			}
			return serialized;
		}

		public static bool TryGetCandidate(Mat image, Color color, Point startPoint, Size chunkSize, out LEDCandidate candidate)
		{
			image.Save(@"c:\pub\tmp\junk\junk.png");
			ImageChunkList chunks = new ImageChunkList(image.Size);

			ExpandingSearchInfo searchInfo = new ExpandingSearchInfo(image, startPoint, chunkSize);

			List<ImageDirection> directions = new List<ImageDirection>() { ImageDirection.Top, ImageDirection.Bottom, ImageDirection.Left, ImageDirection.Right };

			candidate = null;

			while(true)
			{
				searchInfo.Expand();

				foreach(ImageDirection direction in directions)
				{
					if(searchInfo.Parms[direction].Locked == false)
					{
						ImageChunk chunk = new ImageChunk(image, direction, searchInfo.Parms[direction].ROI, searchInfo.Parms[direction].ROIRectangle);
						if(chunk.NonZero > 0)
						{
							chunks.AddChunk(chunk);
						}
						else
						{
							searchInfo.Parms[direction].RevertAndLock();
						}
					}
				}

				if(searchInfo.AllLocked)
				{
					chunks.TrimEmpty();
					if(chunks.Count > 0)
					{
						Rectangle rect = searchInfo.Subsumed;
						Mat roi = new Mat(image, rect);
						Double nonZero = CvInvoke.CountNonZero(roi);
						Double ratio = nonZero / (Double)(rect.Height * rect.Width);
						candidate = new LEDCandidate(color, rect.Centroid().ToPoint(), rect, ratio, (int)nonZero);
					}
					break;
				}
			}
			return candidate != null;
		}

		public override string ToString()
		{
			return String.Format("{0} ø: {1}  Rect: {2}  Conc: {3:0.000}", Color.Name, Center, BoundingRectangle, Concentration);
		}

		class DirectionalParameters
		{
			public ImageDirection Direction { get; set; }
			public int Limit { get; set; }
			public int PreviousLimit { get; set; }
			public bool Locked { get; set; }
			public Mat ROI { get; private set; }
			public Rectangle ROIRectangle { get; set; }
			public bool Expanded { get; private set; }

			int _lastLimit;

			public DirectionalParameters(ImageDirection direction, int limit)
			{
				Direction = direction;
				Limit = PreviousLimit = _lastLimit = limit;
				Locked = false;
				Expanded = false;
			}

			public void ExpandTo(int limit)
			{
				PreviousLimit = Limit;
				Limit = limit;
//				Log.SysLogText(LogLevel.DEBUG, "Expanded {0} from {1} to {2}", Direction, PreviousLimit, Limit);
				if(Limit == _lastLimit)
				{
					Locked = true;
//					Log.SysLogText(LogLevel.DEBUG, "Locking {0}", Direction);
				}
				_lastLimit = limit;
			}

			public void RevertAndLock()
			{
//				Log.SysLogText(LogLevel.DEBUG, "Revert and Lock {0}", Direction, PreviousLimit, Limit);
				Limit = PreviousLimit;
				Locked = true;
			}

			public void SetROI(Mat image, Rectangle roi)
			{
				ROIRectangle = roi;
				ROI = new Mat(image, roi);
			}

			public override string ToString()
			{
				return String.Format("{0}  limit: {1}  Locked: {2}", Direction, Limit, Locked);
			}
		}

		class ExpandingSearchInfo
		{
			public Mat Image { get; private set; }
			public Point Origin { get; private set; }
			public Size CellSize { get; private set; }

			public Point UpperLeft { get { return new Point(Parms[ImageDirection.Left].Limit, Parms[ImageDirection.Top].Limit); } }
			public Size Size { get { return new Size(Parms[ImageDirection.Right].Limit - Parms[ImageDirection.Left].Limit, Parms[ImageDirection.Bottom].Limit - Parms[ImageDirection.Top].Limit); } }
			public Rectangle TopSlice { get { return new Rectangle(UpperLeft, new Size(Parms[ImageDirection.Right].Limit - Parms[ImageDirection.Left].Limit, Parms[ImageDirection.Top].PreviousLimit - Parms[ImageDirection.Top].Limit)); } }
			public Rectangle BottomSlice { get { return new Rectangle(new Point(Parms[ImageDirection.Left].Limit, Parms[ImageDirection.Bottom].PreviousLimit), new Size(Parms[ImageDirection.Right].Limit - Parms[ImageDirection.Left].Limit, Parms[ImageDirection.Bottom].Limit - Parms[ImageDirection.Bottom].PreviousLimit)); } }
			public Rectangle LeftSlice { get { return new Rectangle(UpperLeft, new Size(Parms[ImageDirection.Left].PreviousLimit - Parms[ImageDirection.Left].Limit, Parms[ImageDirection.Bottom].Limit - Parms[ImageDirection.Top].Limit)); } }
			public Rectangle RightSlice { get { return new Rectangle(new Point(Parms[ImageDirection.Right].PreviousLimit, Parms[ImageDirection.Top].Limit), new Size(Parms[ImageDirection.Right].Limit - Parms[ImageDirection.Right].PreviousLimit, Parms[ImageDirection.Bottom].Limit - Parms[ImageDirection.Top].Limit)); } }

			public Rectangle Subsumed { get { return new Rectangle(TopSlice.Location, new Size(RightSlice.Right - LeftSlice.Left, BottomSlice.Bottom - TopSlice.Top)); } }

			public Dictionary<ImageDirection, DirectionalParameters> Parms { get; set; }

			public bool AllLocked { get { return Parms.Count(p => p.Value.Locked == true) == Parms.Count;  } }

			public ExpandingSearchInfo(Mat image, Point origin, Size cellSize)
			{
				Image = image;
				Origin = origin;
				CellSize = cellSize;

				Parms = new Dictionary<ImageDirection, DirectionalParameters>()
				{
					{ ImageDirection.Top,		new DirectionalParameters(ImageDirection.Top, image.ValidY(Origin.Y)) },
					{ ImageDirection.Left,      new DirectionalParameters(ImageDirection.Left, image.ValidX(Origin.X)) },
					{ ImageDirection.Bottom,	new DirectionalParameters(ImageDirection.Bottom, image.ValidY(Origin.Y)) },
					{ ImageDirection.Right,		new DirectionalParameters(ImageDirection.Right, image.ValidX(Origin.X)) },
				};

			}

			public void Expand()
			{
				if(Parms[ImageDirection.Top].Locked == false)
				{
					Parms[ImageDirection.Top].ExpandTo(Image.ValidY(Parms[ImageDirection.Top].Limit - CellSize.Height));
				}
				if(Parms[ImageDirection.Left].Locked == false)
				{
					Parms[ImageDirection.Left].ExpandTo(Image.ValidX(Parms[ImageDirection.Left].Limit - CellSize.Width));
				}
				if(Parms[ImageDirection.Bottom].Locked == false)
				{
					Parms[ImageDirection.Bottom].ExpandTo(Image.ValidY(Parms[ImageDirection.Bottom].Limit + CellSize.Height));
				}
				if(Parms[ImageDirection.Right].Locked == false)
				{
					Parms[ImageDirection.Right].ExpandTo(Image.ValidX(Parms[ImageDirection.Right].Limit + CellSize.Width));
				}

				// create ROIs for unlocked regions
				if(Parms[ImageDirection.Top].Locked == false)
				{
					Parms[ImageDirection.Top].SetROI(Image, TopSlice);
				}
				if(Parms[ImageDirection.Bottom].Locked == false)
				{
					Parms[ImageDirection.Bottom].SetROI(Image, BottomSlice);
				}
				if(Parms[ImageDirection.Left].Locked == false)
				{
					Parms[ImageDirection.Left].SetROI(Image, LeftSlice);
				}
				if(Parms[ImageDirection.Right].Locked == false)
				{
					Parms[ImageDirection.Right].SetROI(Image, RightSlice);
				}
			}

			public override string ToString()
			{
				return String.Format("{0}", Subsumed);
			}

		}

		class ImageChunk
		{
			public Mat Image { get; set; }
			public Mat ROI { get; set; }
			public Rectangle Region { get; set; }
			public ImageDirection Direction { get; set; }
			public int NonZero { get; set; }

			public ImageChunk(Mat image, ImageDirection direction, Mat roi, Rectangle rect)
			{
				Image = image;
				ROI = roi;
				Region = rect;
				Direction = direction;

				NonZero = CvInvoke.CountNonZero(roi);
			}

			public override string ToString()
			{
				return String.Format("{0} ({1}) - NZ: {2}", Region, Direction, NonZero);
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
		public int ByteArraySize { get { return LEDCandidate.ByteArraySize * Count; } }

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				foreach(LEDCandidate candidate in this)
				{
					bw.Write(candidate.Serialize());
				}
			}
			return serialized;
		}

		public bool ContainsAny(IEnumerable<Point> points)
		{
			bool result = false;
			foreach(LEDCandidate candidate in this)
			{
				if(candidate.BoundingRectangle.ContainsAny(points))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool ContainsAny(IEnumerable<PointD> points)
		{
			bool result = false;
			foreach(LEDCandidate candidate in this)
			{
				if(candidate.BoundingRectangle.ContainsAny(points))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool TryGetCandidateAtPoint(PointD point, out LEDCandidate candidate)
		{
			return TryGetCandidateAtPoint(new List<PointD>() { point }, out candidate);
		}

		public bool TryGetCandidateAtPoint(IEnumerable<PointD> points, out LEDCandidate candidate)
		{
			candidate = null;
			foreach(LEDCandidate c in this)
			{
				if(c.BoundingRectangle.ContainsAny(points))
				{
					candidate = c;
					break;
				}
			}
			return candidate != null;
		}

		public bool Contains(PointD point)
		{
			return Contains(point.ToPoint());
		}

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

		public void DumpToLog()
		{
			foreach(LEDCandidate candidate in this)
			{
				Log.SysLogText(LogLevel.DEBUG, "{0}", candidate);
			}
		}
	}
}
