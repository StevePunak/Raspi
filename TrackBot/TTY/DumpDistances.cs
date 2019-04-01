using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;

namespace TrackBot.TTY
{
	[CommandText("dd")]
	class DumpDistances : CommandBase
	{
		public DumpDistances()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Double a1, a2;
			if( commandParts.Count < 3 || 
				Double.TryParse(commandParts[1], out a1) == false || 
				Double.TryParse(commandParts[2], out a2) == false ||
				a1 < 0 || a1 >= 360 || a2 < 0 || a2 >= 360)
			{
				throw new CommandException("Invalid parameter value");
			}

			for(Double angle = a1;angle.AngularDifference(a2) > Widgets.Environment.VectorSize;angle = angle.AddDegrees(Widgets.Environment.VectorSize))
			{
				DateTime time = Widgets.Environment.GetLastSampleTimeAtBearing(angle);
				String timeString = time == DateTime.MinValue ? "MIN" : (DateTime.UtcNow - time).ToAbbreviatedFormat(true);
				Log.SysLogText(LogLevel.DEBUG, "At {0:0.00}°  range is {1:0.00}m  Time: {2}", angle, Widgets.Environment.GetRangeAtBearing(angle), timeString);
			}
			
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "dd [angle1 angle2]";
			description = "dump all the distance vectors";
		}


	}
}
