using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public interface IPolygon
	{
		List<ILine> Lines { get; }

		List<IPoint> Points { get; }

		Double Area { get; }

		void Move(Double bearing, Double distance);

		IRectangle GetBoundingRectangle();
	}
}
