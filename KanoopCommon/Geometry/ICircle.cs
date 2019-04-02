using System;

namespace KanoopCommon.Geometry
{
	public interface ICircle
	{
		Double Area { get; }

		Double Diameter { get; }

		IPoint Center { get; }

		Double Radius { get; }

	}
}
