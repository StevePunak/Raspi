using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public interface ITriangle
	{
		IPoint A	{ get; }
		IPoint B	{ get; }
		IPoint C	{ get; }

		ILine AtoB { get; }
		ILine AtoC { get; }
		ILine BtoC { get; }
		ILine BtoA { get; }
		ILine CtoA { get; }
		ILine CtoB { get; }
	}
}
