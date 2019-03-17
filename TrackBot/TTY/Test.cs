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
			Double _startBearing = Widgets.GyMag.Bearing;
			PointD _startLocation = Widgets.Environment.Location;

			Widgets.Tracks.Spin(SpinDirection.Clockwise, Widgets.Tracks.StandardSpeed);

			for(int x =- 0;x < 20;x++)
			{
				Console.WriteLine("Started SpinSeek at {0}  bearing {1:0.00}°", _startLocation, _startBearing);
				GpioSharp.Sleep(100);
			}

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "stop";
			description = "stop all activities";
		}
	}
}
