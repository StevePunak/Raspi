using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.PiGpio;

namespace TrackBot.TTY
{
	[CommandText("plog")]
	class PigsLogCommand : CommandBase
	{
		public PigsLogCommand()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			int to;
			if(commandParts.Count < 2 || int.TryParse(commandParts[1], out to) == false)
			{
				throw new CommandException("Invalid. need 1 or 0");
			}

			Pigs.DebugLogging = to == 1 ? true : false;
			Console.WriteLine("Setting pigs log to {0}", Pigs.DebugLogging);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "plog [0/1]";
			description = "Change PIGS logging";
		}
	}
}
