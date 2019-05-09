using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("ml")]
	class MotorLeftCommand : CommandBase
	{
		public MotorLeftCommand()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			int parm;
			if(commandParts.Count < 2 || int.TryParse(commandParts[1], out parm) == false || parm < -100 || parm > 100)
			{
				throw new CommandException("Invalid parameter value");
			}

			Widgets.Instance.Tracks.LeftSpeed = parm;

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "ml [speed -100 - 100]";
			description = "run left track motor";
		}

	}
}
