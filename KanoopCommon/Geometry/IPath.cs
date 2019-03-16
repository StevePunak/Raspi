using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public interface IPath
	{
		Double Length { get; }

		IEnumerable<ILine> Lines { get; }
	}
}
