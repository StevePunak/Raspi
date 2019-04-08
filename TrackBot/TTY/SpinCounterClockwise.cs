using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("s-")]
	class SpinCounterClockwise : CommandBase
	{
		public SpinCounterClockwise()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Double time;
			if(commandParts.Count < 2 || Double.TryParse(commandParts[1], out time) == false)
			{
				throw new CommandException("Invalid time");
			}

			SpinDirection direction = SpinDirection.CounterClockwise;
			Widgets.Instance.Tracks.Spin(direction, Widgets.Instance.Tracks.StandardSpeed);

			DateTime startTime = DateTime.UtcNow;

			while(DateTime.UtcNow < startTime + TimeSpan.FromMilliseconds(time))
			{
				GpioSharp.Sleep(TimeSpan.FromMilliseconds(25));
			}

			Widgets.Instance.Tracks.Stop();

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "sc [ms]";
			description ="Spin Counter-Clockwise from the given number of milliseconds";
		}
	}
}
