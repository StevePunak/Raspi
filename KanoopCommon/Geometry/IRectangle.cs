using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public interface IRectangle : IPolygon
	{
		Double Width { get; }

		Double Height { get; }

		ILine ShortestLeg { get; }

		ILine LongestLeg { get; }
	}
}
