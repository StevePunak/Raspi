using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public interface IEllipse
	{
		Double Area { get; }

		IPoint Center { get; }

		Double Eccentricity { get; }

		IPoint[] Foci { get; }

		ILine MajorAxis { get; }

		Double MajorRadius { get; }

		ILine MinorAxis { get; }

		Double MinorRadius { get; }

		void Move(IPoint where);
	}
}
