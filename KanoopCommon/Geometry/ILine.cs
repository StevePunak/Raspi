using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public interface ILine
	{
		#region Properties

		Double Bearing { get; }

		Double Length { get; }

		IPoint MidPoint { get; }

		IPoint P1 { get; }

		IPoint P2 { get; }

		#endregion

		#region Methods

		bool IsEndPoint(IPoint p1, int precision);

		bool SharesEndPointWith(ILine other, int precision);

		ILine Clone();

		String ToString(int precision);

		bool Equals(ILine other);

		#endregion
	}
}
