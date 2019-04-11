using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon.Lidar;

namespace TrackBotCommon.Extensions
{
	public static class LidarVectorExtensions
	{
		public static BearingAndRangeList ToBearingAndRangeList(this LidarVector[] vectorSet)
		{
			BearingAndRangeList vectors = new BearingAndRangeList();
			for(int x = 0;x < vectorSet.Length;x++)
			{
				vectors.Add(new BearingAndRange(vectorSet[x].Bearing, vectorSet[x].Range));
			}
			return vectors;
		}
	}
}
