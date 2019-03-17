using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanoopCommon.Geometry
{
	public class Degrees
	{
		public static Double ClockwiseDifference(Double bearing1, Double bearing2)
		{
			if(bearing1 > bearing2)
			{
				bearing1 -= 360;
			}

			Double diff = bearing2 - bearing1;

			return diff;
		}

		public static Double CounterClockwiseDifference(Double bearing1, Double bearing2)
		{
			if(bearing2 > bearing1)
			{
				bearing2 -= 360;
			}

			Double diff = bearing1 - bearing2;

			return diff;
		}

		public static Double AngularDifference(Double bearing1, Double bearing2)
		{
			Double diff1 = bearing1 - bearing2;
			if(diff1 < 0)
				diff1 += 360;

			Double diff2 = bearing2 - bearing1;
			if(diff2 < 0)
				diff2 += 360;

			return Math.Min(diff1, diff2);
		}
	}
}
