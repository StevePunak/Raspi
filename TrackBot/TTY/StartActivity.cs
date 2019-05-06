using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using TrackBot.Spatial;

namespace TrackBot.TTY
{
	[CommandText("start")]
	class StartActivity : CommandBase
	{
		public StartActivity()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			if(commandParts.Count < 2)
			{
				throw new CommandException("not enough parameters");
			}

			ActivityType activityType;
			if(Enum.TryParse<ActivityType>(commandParts[1], true, out activityType))
			{
				List<Object> parms = new List<object>();

				for(int index = 2;index < commandParts.Count;index++)
				{
					parms.Add(commandParts[index]);
				}
				Activity.StartActivity(activityType, parms.ToArray(), false);
			}
			else
			{
				Log.SysLogText(LogLevel.DEBUG, "Could not parse activity type");
			}
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "start";
			description = "start [activity]";
		}
	}
}
