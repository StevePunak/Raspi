using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;

namespace TrackBot.TTY
{
	[CommandText("dump")]
	class DumpCommand : CommandBase
	{
		public DumpCommand()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Log.SysLogText(LogLevel.DEBUG, "----------   LED Positions   ----------");
			Program.Config.LEDTravelHistory.DumpToLog();
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "dump";
			description = "dump debug info";
		}


	}
}
