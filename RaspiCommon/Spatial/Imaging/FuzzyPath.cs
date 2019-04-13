using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;

namespace RaspiCommon.Spatial.Imaging
{
	public class FuzzyPath
	{
		public PointCloud2DSlice FrontLeft { get; set; }
		public PointCloud2DSlice FrontRight { get; set; }

		public Double ShortestRange { get { return Math.Min(FrontLeft.MinimumRange, FrontRight.MinimumRange); } }

		/// <summary>
		/// Center Bearing (a2 - a1) / 2
		/// </summary>
		public Double Bearing { get { return FrontLeft.Bearing.AddDegrees(FrontLeft.Bearing.AngularDifference(FrontRight.Bearing) / 2); } }

		public FuzzyPath()
			: this(null, null) {}

		public FuzzyPath(PointCloud2DSlice frontLeft, PointCloud2DSlice frontRight)
		{
			FrontLeft = frontLeft;
			FrontRight = frontRight;
		}

		public FuzzyPath(byte[] input)
		{
			FrontLeft = new PointCloud2DSlice();
			FrontRight = new PointCloud2DSlice();
			using(BinaryReader br = new BinaryReader(new MemoryStream(input)))
			{
				FrontLeft = new PointCloud2DSlice(br);
				FrontRight = new PointCloud2DSlice(br);
			}
		}

		public Line GetLineFrom(PointD point, Double range)
		{
			return new Line(point, point.GetPointAt(Bearing, range));
		}

		public byte[] Serizalize()
		{
			byte[] output = new byte[FrontLeft.ByteArraySize + FrontRight.ByteArraySize];

			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(output)))
			{
				bw.Write(FrontLeft.Serialize());
				bw.Write(FrontRight.Serialize());
			}
			return output;
		}

		public FuzzyPath Clone()
		{
			return new FuzzyPath(FrontLeft.Clone(), FrontRight.Clone());
		}

		public override string ToString()
		{
			return String.Format("Fuzzy: {0} {1} {2}  Bearing {3}",
				ShortestRange.ToMetersString(),
				FrontLeft.Bearing.ToAngleString(0),
				FrontRight.Bearing.ToAngleString(0),
				Bearing.ToAngleString(0));
		}
	}

	public class FuzzyPathList : List<FuzzyPath>
	{
		public FuzzyPath Longest { get { return this.Find(p => p.ShortestRange == this.Max(f => f.ShortestRange)); } }

		public FuzzyPath FurthestFrom(Double bearing)
		{
			FuzzyPath furthestPath = null;
			Double furthestAngularDifference = 0;
			foreach(FuzzyPath path in this)
			{
				Double diff = path.Bearing.AngularDifference(bearing);
				if(furthestPath == null || diff > furthestAngularDifference)
				{
					furthestPath = path;
					furthestAngularDifference = diff;
				}
			}

			return furthestPath;
		}

		public FuzzyPath ClosestTo(Double bearing)
		{
			FuzzyPath closestPath = null;
			Double closestAngularDifference = 360;
			foreach(FuzzyPath path in this)
			{
				Double diff = path.Bearing.AngularDifference(bearing);
				if(closestPath == null || diff < closestAngularDifference)
				{
					closestPath = path;
					closestAngularDifference = diff;
				}
			}

			return closestPath;
		}

		public void DumpToLog(String tag = null)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Dumping Fuzzy Paths: {0}\n", tag == null ? String.Empty : tag);
			foreach(FuzzyPath path in this)
			{
				sb.AppendFormat("  {0}\n", path);
			}

			Log.SysLogText(LogLevel.DEBUG, "\n{0}", sb);
		}

		public bool ContainsPathNearBearing(Double bearing, Double howNear)
		{
			bool result = false;
			foreach(FuzzyPath path in this)
			{
				if(path.Bearing.AngularDifference(bearing) <= howNear)
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
