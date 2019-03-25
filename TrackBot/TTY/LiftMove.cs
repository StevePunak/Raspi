using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("lm")]
	class LiftMove : CommandBase
	{
		public LiftMove()
			: base(true) {}

		public override bool Execute(List<string> commandParts)
		{

			Direction direction = Direction.Forward;
			if(commandParts[1][0] == 'u')
			{
				direction = Direction.Forward;
			}
			else if(commandParts[1][0] == 'd')
			{
				direction = Direction.Backward;
			}
			else
			{
				throw new CommandException("Need u or d for up or down");
			}

			Double steps;
			if(Double.TryParse(commandParts[2], out steps) == false)
			{
				throw new CommandException("Could not parse # of steps");
			}

			Widgets.Lift.Running = true;
			Widgets.Lift.Speed = MotorSpeed.VeryFast;
			Widgets.Lift.Rotate(direction, steps);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "lm [u/d] [steps]";
			description = "move lift the given number of steps";
		}

	}
}
