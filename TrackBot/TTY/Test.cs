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
			Program.Config.LiftInputPin1 = GpioPin.Pin12;
			Program.Config.LiftInputPin2 = GpioPin.Pin26;
			Program.Config.LiftInputPin3 = GpioPin.Pin16;
			Program.Config.LiftInputPin4 = GpioPin.Pin20;

			Program.Config.Save();

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
