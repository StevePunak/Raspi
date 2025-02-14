using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("s+")]
	public class SpinClockwise : CommandBase
	{
		public SpinClockwise()
			: base(true) {}

		public override bool Execute(List<string> commandParts)
		{
			Double time;
			if(commandParts.Count < 2 || Double.TryParse(commandParts[1], out time) == false)
			{
				throw new CommandException("Invalid time");
			}

			SpinDirection direction = SpinDirection.Clockwise;
			Widgets.Instance.Tracks.Spin(direction, Widgets.Instance.Tracks.StandardSpeed);

			DateTime startTime = DateTime.UtcNow;

			while(DateTime.UtcNow < startTime + TimeSpan.FromMilliseconds(time))
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(25));
			}

			Widgets.Instance.Tracks.Stop();

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "sc [ms]";
			description = "Spin Clockwise from the given number of milliseconds";
		}
	}
}
