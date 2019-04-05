using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("da")]
	class DebugAngle : CommandBase
	{
		public DebugAngle()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Double to;
			if(commandParts.Count < 2 || Double.TryParse(commandParts[1], out to) == false)
			{
				throw new CommandException("Invalid bearing");
			}

			Console.WriteLine("Setting debug angle to {0}", to);
			Widgets.ImageEnvironment.DebugAngle = to;

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "da [bearing]";
			description = "Set debug angle to bearing";
		}
	}
}
