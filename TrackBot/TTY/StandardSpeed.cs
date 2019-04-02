using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("ss")]
	class StandardSpeed : CommandBase
	{
		public StandardSpeed()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			int parm;
			if(commandParts.Count < 2 || int.TryParse(commandParts[1], out parm) == false || parm < 10 || parm > 100)
			{
				throw new CommandException("Invalid parameter value");
			}

			Widgets.Tracks.StandardSpeed = parm;
			Program.Config.StandardSpeed = parm;
			Program.Config.Save();

			Console.WriteLine("Standard speed set to {0}", Widgets.Tracks.StandardSpeed);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "ss [speed 1 - 100]";
			description = "set standard speed for maneuvering";
		}

	}
}
