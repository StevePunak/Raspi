using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackBot.TTY
{
	[CommandText("a")]
	class ArmCommand : CommandBase
	{
		static int LeftPos = 50;
		static int RightPos = 50;
		static int ClawPos = 50;
		static int RotationPos = 50;

		public ArmCommand()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			int value;
			if( commandParts.Count < 3 || 
				int.TryParse(commandParts[2], out value) == false ||
				value < 0 || value > 100 ||
				(Char.ToLower(commandParts[1][0]) != 'c' && Char.ToLower(commandParts[1][0]) != 'l' && Char.ToLower(commandParts[1][0]) != 'r' && Char.ToLower(commandParts[1][0]) != 'z'))
			{
				throw new CommandException("Invalid parameter value");
			}

			char j = Char.ToLower(commandParts[1][0]);
			switch(j)
			{
				case 'c':
					Widgets.Instance.ServoController["claw"].SetDestinationPercentage(value);
					break;
				case 'l':
					Widgets.Instance.ServoController["left"].SetDestinationPercentage(value);
					break;
				case 'r':
					Widgets.Instance.ServoController["right"].SetDestinationPercentage(value);
					break;
				case 'z':
					Widgets.Instance.ServoController["rotation"].SetDestinationPercentage(value);
					break;
			}
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "b [ms / meters]";
			description = "move backward for the time or distance specified";
		}
	}
}
