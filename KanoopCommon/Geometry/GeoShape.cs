using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public abstract class GeoShape :  ICloneable
	{
		public abstract IRectangle GetMinimumBoundingRectangle();

		public abstract object Clone();

		public abstract void Move(Double bearing, Double distance);
	}
}
