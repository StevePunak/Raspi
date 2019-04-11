using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Serialization;

namespace KanoopCommon.Geometry
{
	[IsSerializable]
	public class BearingAndRange : IVector
	{
		public const int ByteArraySize = sizeof(Double) + sizeof(Double);

		public Double Range  { get; set; }
		public Double Bearing { get; set; }

		public BearingAndRange()
			: this(0, 0) { }

		public BearingAndRange(PointD origin, PointD to)
		{
			Line line = new Line(origin, to);
			Bearing = line.Bearing;
			Range = line.Length;
		}

		public BearingAndRange(Double bearing, Double range)
		{
			Bearing = bearing;
			Range = range;
		}

		public BearingAndRange(byte[] serialized)
		{
			using(BinaryReader br = new BinaryReader(new MemoryStream(serialized)))
			{
				Bearing = br.ReadDouble();
				Range = br.ReadDouble();
			}
		}

		public static BearingAndRange Empty { get { return new BearingAndRange(); } }

		/// <summary>
		/// Roteate the bearing around the origin
		/// </summary>
		/// <param name="degrees"></param>
		public void Rotate(Double degrees)
		{
			Bearing = Bearing.AddDegrees(degrees);
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Bearing);
				bw.Write(Range);
			}
			return serialized;
		}

		public BearingAndRange Clone()
		{
			return new BearingAndRange(Bearing, Range);
		}

		public override string ToString()
		{
			return String.Format("{0:0.00} at {1:0.00}°", Range, Bearing);
		}
	}

	public class BearingAndRangeList : List<BearingAndRange>
	{
		public BearingAndRangeList()
			: base() { }

		public BearingAndRangeList(int size)
		{
			for(int x = 0;x < size;x++)
			{
				Add(new BearingAndRange());
			}
		}

		protected void LoadFromReader(BinaryReader br)
		{
			int count = br.ReadInt32();
			for(int x = 0;x < count;x++)
			{
				Add(new BearingAndRange(br.ReadBytes(BearingAndRange.ByteArraySize)));
			}
		}

		public virtual byte[] Serialize()
		{
			byte[] serialized = new byte[sizeof(Int32) + (Count * BearingAndRange.ByteArraySize)];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Count);
				foreach(BearingAndRange bar in this)
				{
					bw.Write(bar.Serialize());
				}
			}
			return serialized;
		}

		public BearingAndRangeList Clone()
		{
			BearingAndRangeList list = new BearingAndRangeList();
			foreach(BearingAndRange rab in this)
			{
				list.Add(rab.Clone());
			}
			return list;
		}
	}
}
