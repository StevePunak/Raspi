using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon.Lidar;

namespace RaspiCommon.Extensions
{
	public static class LidarVectorExtensions
	{
		/// <summary>
		/// Convert to a point cloud
		/// </summary>
		/// <param name="vectorSet">input vectors</param>
		/// <param name="size">the size in units of the return set (must be divisible by 360)</param>
		/// <param name="scale"></param>
		/// <returns>returns a scaled point cloud</returns>
		public static PointCloud2D ToPointCloud2D(this IVector[] vectorSet, int size = 0, Double scale = 1)
		{
			Double step = 1;
			if(size != 0)
			{
				step = vectorSet.Length / size;
			}
			PointCloud2D vectors = new PointCloud2D();
			for(Double x = 0;x < vectorSet.Length;x += step)
			{
				vectors.Add(new BearingAndRange(vectorSet[(int)x].Bearing, vectorSet[(int)x].Range * scale));
			}
			return vectors;
		}

	}
}
