using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanoopCommon.CommonObjects
{
	public class FormatException : CommonException
	{
		public FormatException(String format, params object[] parms)
			: base(format, parms) {}
	}
}
