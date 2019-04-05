using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;

namespace RaspiCommon.Lidar
{
	public class LidarVector : IVector
	{
		public Double Bearing { get; set; }
		public Double Range { get; set; }
		public DateTime RefreshTime { get; set; }

		public LidarVector()
		{
			Bearing = Range = 0;
			RefreshTime = DateTime.MinValue;
		}

		public override string ToString()
		{
			return String.Format("{0:0.000}m at {1:0.00}°", Range, Bearing);
		}

	}
}
