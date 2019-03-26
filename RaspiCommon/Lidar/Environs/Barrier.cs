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
	}

	public class Barrier
	{
		public Line Line { get; private set; }

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
