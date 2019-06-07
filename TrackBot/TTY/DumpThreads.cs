using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Threading;

namespace TrackBot.TTY
{
	[CommandText("threads")]
	class DumpThreads : CommandBase
	{
		public DumpThreads()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			ThreadBase.DumpPerformanceData();
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "threads";
			description = "Dump performance data for threads";
		}
	}
}
