using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;

namespace RaspiCommon.Lidar.Environs
{
	public class BarrierList : List<Barrier>
	{
		public bool Contains(Barrier barrier, Double withinMeters, Double scale, Double bearingSlack)
		{
			Double withinPixels = withinMeters * scale;
			return this.Find(b => b.Line.SharesEndpointAndBearing(barrier.Line, withinPixels, bearingSlack)) != null;
		}
	}

	public class Barrier
	{
		public Line Line { get; private set; }

		public Barrier(PointD p1, PointD p2)
			: this(new Line(p1, p2)) {}

		public Barrier(Line line)
		{
			Line = line;
		}

		public override string ToString()
		{
			return String.Format("BARRIER: {0}", Line);
		}
	}
}
