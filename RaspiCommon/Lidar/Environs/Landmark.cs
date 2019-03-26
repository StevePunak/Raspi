using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;

namespace RaspiCommon.Lidar.Environs
{
	public class LandmarkList : List<Landmark>
	{
	}

	public class Landmark
	{
		public PointD Location { get; private set; }

		public Landmark(PointD location)
		{
			Location = location;
		}

		public static Landmark FromSource(PointD point, PointD sourceOrigin, Double sourceOrientation, LidarEnvironment destination)
		{

		}

		public override string ToString()
		{
			return String.Format("Landmark @ {0}", Location);
		}
	}
}
