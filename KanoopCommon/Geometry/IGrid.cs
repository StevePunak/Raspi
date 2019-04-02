using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public interface IGrid : IRectangle
	{
		Int32 NumberOfRows { get; }

		Int32 NumberOfColumns { get; }
	}
}
