using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;

namespace RaspiCommon.Lidar.Environs
{
	public class LinePair
	{
		public Line Line1 { get; private set; }
		public Line Line2 { get; private set; }

		public Line ClosestPoints
		{
			get
			{
				LineList lines = new LineList(new List<Line>()
					{
						new Line(Line1.P1, Line2.P1),
						new Line(Line1.P1, Line2.P2),
						new Line(Line1.P2, Line2.P1),
						new Line(Line1.P2, Line2.P2)
					});

				return lines.Shortest;
			}
		}

		public LinePair(Line line1, Line line2)
		{
			Line1 = line1;
			Line2 = line2;
		}

		public override string ToString()
		{
			return String.Format("{0} to {1}", Line1, Line2);
		}
	}
}
