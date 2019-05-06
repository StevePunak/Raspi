using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;

namespace TrackBot.Spatial
{
	internal class ActivityException : CommonException
	{
		public ActivityException(String format, params object[] parms)
			: base(format, parms) { }
	}
}
