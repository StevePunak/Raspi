using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;

namespace TrackBot
{
	class TrackBotException : CommonException
	{
		public TrackBotException(String format, params object[] parms)
			: base(format, parms) {}
	}
}
