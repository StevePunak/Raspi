using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;

namespace Radar.Scan3D
{
	public class Exception3D : CommonException
	{
		public Exception3D(String format, params object[] parms)
			: base(format, parms) { }
	}
}
