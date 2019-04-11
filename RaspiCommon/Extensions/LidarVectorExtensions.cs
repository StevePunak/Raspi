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
		public static PointCloud2D ToPointCloud2D(this LidarVector[] vectorSet)
		{
			PointCloud2D vectors = new PointCloud2D();
			for(int x = 0;x < vectorSet.Length;x++)
			{
				vectors.Add(new BearingAndRange(vectorSet[x].Bearing, vectorSet[x].Range));
			}
			return vectors;
		}

	}
}
