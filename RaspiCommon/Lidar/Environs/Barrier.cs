using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;

namespace RaspiCommon.Lidar.Environs
{
	public class BarrierList : List<Barrier>
	{
		public BarrierList()
			: base() { }

		public BarrierList(PointD origin, LineList lines)
		{
			foreach(Line line in lines)
			{
				Barrier barrier = new Barrier(origin, line);
				barrier.Tag = line.Tag;
				Add(barrier);
			}
		}

		public BarrierList(byte[] barriers)
			: base()
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(barriers)))
			{
				for(int x = 0;x < barriers.Length / Barrier.ByteArraySize;x++)
				{
					Barrier barrier = new Barrier(br.ReadBytes(Barrier.ByteArraySize));
					Add(barrier);
				}
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[Barrier.ByteArraySize * Count];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				foreach(Barrier barrier in this)
				{
					bw.Write(barrier.Serialize());
				}
			}
			return serialized;
		}

		public bool Contains(Barrier barrier, Double withinMeters, Double scale, Double bearingSlack)
		{
			Double withinPixels = withinMeters * scale;
			return this.Find(b => b.GetLine().SharesEndpointAndBearing(barrier.GetLine(), withinPixels, bearingSlack)) != null;
		}

		public BarrierList Clone()
		{
			BarrierList list = new BarrierList();
			foreach(Barrier barrier in this)
			{
				list.Add(barrier.Clone());
			}
			return list;
		}
	}

	public class Barrier
	{
		public const int ByteArraySize = PointD.ByteArraySize + BearingAndRange.ByteArraySize + BearingAndRange.ByteArraySize;

		public PointD Origin { get; set; }
		public BearingAndRange V1 { get; set; }
		public BearingAndRange V2 { get; set; }

		public object Tag { get; set; }

		public Barrier(PointD origin, PointD p1, PointD p2)
			: this(origin, new BearingAndRange(origin, p1), new BearingAndRange(origin, p2)) {}

		public Barrier(PointD origin, Line line)
			: this(origin, line.P1, line.P2) { }

		public Barrier(PointD origin, BearingAndRange v1, BearingAndRange v2)
		{
			Origin = origin;
			V1 = v1;
			V2 = v2;
		}

		public Barrier(byte[] serialized)
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

		public Barrier Clone()
		{
			return new Barrier(Origin.Clone(), V1.Clone(), V2.Clone());
		}

		public override string ToString()
		{
			return String.Format("BARRIER: {0}{1}", GetLine(), Tag != null && Tag is String ? (String)Tag : String.Empty);
		}
	}
}
