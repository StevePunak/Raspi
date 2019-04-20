using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;

namespace RaspiCommon.Spatial.LidarImaging
{
	public class BarrierList : List<ImageBarrier>
	{
		public BarrierList()
			: base() { }

		public BarrierList(PointD origin, LineList lines)
		{
			foreach(Line line in lines)
			{
				ImageBarrier barrier = new ImageBarrier(origin, line);
				barrier.Tag = line.Tag;
				Add(barrier);
			}
		}

		public BarrierList(byte[] barriers)
			: base()
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(barriers)))
			{
				for(int x = 0;x < barriers.Length / ImageBarrier.ByteArraySize;x++)
				{
					ImageBarrier barrier = new ImageBarrier(br.ReadBytes(ImageBarrier.ByteArraySize));
					Add(barrier);
				}
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ImageBarrier.ByteArraySize * Count];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				foreach(ImageBarrier barrier in this)
				{
					bw.Write(barrier.Serialize());
				}
			}
			return serialized;
		}

		public bool Contains(ImageBarrier barrier, Double withinMeters, Double scale, Double bearingSlack)
		{
			Double withinPixels = withinMeters * scale;
			return this.Find(b => b.GetLine().SharesEndpointAndBearing(barrier.GetLine(), withinPixels, bearingSlack)) != null;
		}

		public BarrierList Clone()
		{
			BarrierList list = new BarrierList();
			foreach(ImageBarrier barrier in this)
			{
				list.Add(barrier.Clone());
			}
			return list;
		}
	}

	public class ImageBarrier
	{
		public const int ByteArraySize = PointD.ByteArraySize + BearingAndRange.ByteArraySize + BearingAndRange.ByteArraySize;

		public PointD Origin { get; set; }
		public BearingAndRange V1 { get; set; }
		public BearingAndRange V2 { get; set; }

		public object Tag { get; set; }

		public ImageBarrier(PointD origin, PointD p1, PointD p2)
			: this(origin, new BearingAndRange(origin, p1), new BearingAndRange(origin, p2)) {}

		public ImageBarrier(PointD origin, Line line)
			: this(origin, line.P1, line.P2) { }

		public ImageBarrier(PointD origin, BearingAndRange v1, BearingAndRange v2)
		{
			Origin = origin;
			V1 = v1;
			V2 = v2;
		}

		public ImageBarrier(byte[] serialized)
		{
			using(BinaryReader br =new BinaryReader(new MemoryStream(serialized)))
			{
				Origin = new PointD(br.ReadBytes(PointD.ByteArraySize));
				V1 = new BearingAndRange(br.ReadBytes(BearingAndRange.ByteArraySize));
				V2 = new BearingAndRange(br.ReadBytes(BearingAndRange.ByteArraySize));
			}
		}

		public Line GetLine()
		{
			return new Line(Origin.GetPointAt(V1.Bearing, V1.Range), Origin.GetPointAt(V2.Bearing, V2.Range));
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Origin.Serialize());
				bw.Write(V1.Serialize());
				bw.Write(V2.Serialize());
			}
			return serialized;
		}

		public ImageBarrier Clone()
		{
			return new ImageBarrier(Origin.Clone(), V1.Clone(), V2.Clone());
		}

		public override string ToString()
		{
			return String.Format("BARRIER: {0}{1}", GetLine(), Tag != null && Tag is String ? (String)Tag : String.Empty);
		}
	}
}
