using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;

namespace TrackBot.TTY
{
	class CommandException : CommonException
	{
		public CommandException(String format, params object[] parms)
			: base(format, parms) {}
	}
}
