using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;

namespace KanoopCommon.Extensions
{
	public static class BearingAndRangeExtensions
	{
		public static byte[] MakeBlob(this BearingAndRangeList vectors)
		{
			byte[] output = new byte[sizeof(Double) * vectors.Count];

			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(output)))
			{
				for(int offset = 0;offset < vectors.Count;offset++)
				{
					IVector vector = vectors[offset];
					bw.Write(vector.Range);
				}
			}
			return output;
		}

		/// <summary>
		/// Create a new Point Cloud offseting in 2D space by the 'offset' vector
		/// </summary>
		/// <param name="inputVectors"></param>
		/// <param name="offsetFromOrigin"></param>
		/// <returns></returns>
		public static PointCloud2D Move(this PointCloud2D inputVectors, BearingAndRange offsetFromOrigin)
		{
			PointCloud2D output = new PointCloud2D(inputVectors.Count);

			Double vectorSize = (Double)360 / (Double)inputVectors.Count;

			PointD originalOrigin = new PointD(0, 0);
			PointD newOrigin = originalOrigin.GetPointAt(offsetFromOrigin);

			int offset = 0;
			for(Double originalBearing = 0;originalBearing < 360;originalBearing += vectorSize, offset++)
			{
				if(inputVectors[offset].Range != 0)
				{
					PointD pointFromOriginalOrigin = originalOrigin.GetPointAt(inputVectors[offset]);
					Line oline = new Line(originalOrigin, pointFromOriginalOrigin);
					Line line = new Line(newOrigin, pointFromOriginalOrigin);

					Double outputOffset = line.Bearing / vectorSize;
					output[(int)outputOffset].Bearing = line.Bearing;
					output[(int)outputOffset].Range = line.Length;
				}
			}

			return output;
		}

	}
}
