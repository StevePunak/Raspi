using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using RaspiCommon;

namespace TrackBot.TTY
{
	[CommandText("t")]
	class Test : CommandBase
	{
		public Test()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			for(int speed = 100;speed >= 70;speed = speed - 1)
			{
				Console.WriteLine("Speed = {0}", speed);
				Widgets.Tracks.Speed = speed;
				GpioSharp.Sleep(250);
			}


			Console.WriteLine("Done...");

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "stop";
			description = "stop all activities";
		}
	}
}
