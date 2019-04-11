using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanoopCommon.Geometry
{
	public class PointCloud2DSlice : BearingAndRangeList
	{
		public PointD Origin { get; set; }

		public Double Bearing { get; set; }

		public PointCloud2DSlice()
			: base() {}

		public PointCloud2DSlice(PointD origin, Double bearing)
			: base()
		{
			Origin = origin;
			Bearing = bearing;
		}

		public PointCloud2DSlice(BinaryReader br)
			: base()
		{
			Origin = new PointD(br.ReadBytes(PointD.ByteArraySize));
			Bearing = br.ReadDouble();
			LoadFromReader(br);
		}

		public override byte[] Serialize()
		{
			byte[] serialized = new byte[sizeof(Double) + PointD.ByteArraySize + (Count * BearingAndRange.ByteArraySize)];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Origin.Serialize());
				bw.Write(Bearing);
				bw.Write(base.Serialize());
			}
			return serialized;
		}

		public new PointCloud2DSlice Clone()
		{
			PointCloud2DSlice list = new PointCloud2DSlice()
			{
				Origin = Origin,
				Bearing = Bearing,
			};
			foreach(BearingAndRange rab in this)
			{
				list.Add(rab.Clone());
			}
			return list;
		}

	}
}
