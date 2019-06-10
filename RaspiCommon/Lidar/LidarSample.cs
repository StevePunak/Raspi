using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Lidar
{
	public class LidarSample
	{
		public Double Bearing { get; private set; }
		public Double Distance { get; private set; }
		public DateTime Timestamp { get; private set; }

		public LidarSample(Double bearing, Double distance, DateTime timestamp) 
		{
			Bearing = bearing;
			Distance = distance;
			Timestamp = timestamp;
		}

		public override string ToString()
		{
			return String.Format("{0:0.0}°  {1:0.000}m", Bearing, Distance);
		}
	}
}
