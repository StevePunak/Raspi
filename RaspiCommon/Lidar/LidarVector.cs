using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Lidar
{
	public class LidarVector
	{
		public Double Bearing { get; set; }
		public Double Range { get; set; }
		public DateTime RefreshTime { get; set; }
	}
}
