using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("exit")]
	class ExitCommand : CommandBase
	{
		public ExitCommand()
			: base(true) {}

		public override bool Execute(List<string> commandParts)
		{
			return false;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "exit";
			description = "leave bot shell";
		}
	}
}
