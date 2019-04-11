using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;

namespace KanoopCommon.Geometry
{
	public class PointCloud2D : BearingAndRangeList
	{
		public Double VectorSize { get { return (Double)360 / (Double)this.Count; } }

		public PointCloud2D()
			: base()
		{
		}

		public PointCloud2D(Double size)
			: base((int)size)
		{

		}

		public PointCloud2DSlice GetPointCloud2DSlice(PointD origin, Double bearing, Double from, Double to)
		{
			PointCloud2DSlice slice = new PointCloud2DSlice(origin, bearing);
			for(Double offset = from / VectorSize;offset != to / VectorSize;offset = offset.AddDegrees(VectorSize))
			{
				if(this[(int)offset].Range != 0)
				{
					slice.Add(this[(int)offset]);
				}
			}
			return slice;
		}

		public Double GetRangeAtBearing(Double bearing)
		{
			Double offset = bearing / VectorSize;
			return this[(int)offset].Range;
		}

		public new PointCloud2D Clone()
		{
			PointCloud2D list = new PointCloud2D();
			foreach(BearingAndRange rab in this)
			{
				list.Add(rab.Clone());
			}
			return list;
		}
	}
}
