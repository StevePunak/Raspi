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
		public bool Contains(PointD landmark, Double withinMeters, Double scale)
		{
			Double withinPixels = withinMeters * scale;
			return this.Find(l => l.Location.DistanceTo(landmark) <= withinPixels) != null;
		}
	}

	public class Landmark
	{
		public PointD Location { get; private set; }

		public Landmark(PointD location)
		{
			Location = location;
		}

		public override string ToString()
		{
			return String.Format("Landmark @ {0}", Location);
		}
	}
}
