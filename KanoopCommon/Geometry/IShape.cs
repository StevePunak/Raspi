using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public interface IShape
	{
		IRectangle GetMinimumBoundingRectangle();

		void Move(Double bearing, Double distance);
	}
}
