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
		public Double Vector1 { get; set; }
		public Double Vector2 { get; set; }
		public Double CenterBearing { get { return Vector1.AddDegrees(Vector1.AngularDifference(Vector2) / 2); } }
		public PointCloud2DSlice FrontLeft { get; set; }
		public PointCloud2DSlice FrontRight { get; set; }

		public Double ShortestRange { get { return Math.Min(FrontLeft.Min(e => e.Range), FrontRight.Min(e => e.Range)); } }

		/// <summary>
		/// Center Bearing (a2 - a1) / 2
		/// </summary>
		public Double Bearing { get { return Vector1.AddDegrees(Vector1.AngularDifference(Vector2) / 2); } }

		public FuzzyPath()
			: this(0, 0, null, null) {}

		public FuzzyPath(Double v1, Double v2, PointCloud2DSlice frontLeft, PointCloud2DSlice frontRight)
		{
			Vector1 = v1;
			Vector2 = v2;
			FrontLeft = frontLeft;
			FrontRight = frontRight;
		}

		public FuzzyPath(byte[] input)
		{
			FrontLeft = new PointCloud2DSlice();
			FrontRight = new PointCloud2DSlice();
			using(BinaryReader br = new BinaryReader(new MemoryStream(input)))
			{
				// get the vectors
				Vector1 = br.ReadDouble();
				Vector2 = br.ReadDouble();

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
			byte[] output = new byte[BearingAndRange.ByteArraySize * (FrontLeft.Count + 1)];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(output)))
			{
				bw.Write(Vector1);
				bw.Write(Vector2);
				bw.Write(FrontLeft.Serialize());
				bw.Write(FrontRight.Serialize());
			}
			return output;
		}

		public FuzzyPath Clone()
		{
			return new FuzzyPath(Vector1, Vector2, FrontLeft.Clone(), FrontRight.Clone());
		}

		public override string ToString()
		{
			return String.Format("Fuzzy: {0} {1}", Vector1, Vector2);
		}
	}
}
