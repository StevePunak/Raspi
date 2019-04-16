using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.Extensions;

namespace TrackBot.TTY
{
	[CommandText("t")]
	class Test : CommandBase
	{
		public Test()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Log.SysLogText(LogLevel.DEBUG, "DR environment location is at {0}", Widgets.Instance.DeadReckoningEnvironment.CurrentLocation);
			Widgets.Instance.DeadReckoningEnvironment.ProcessEnvironment(Widgets.Instance.ImageEnvironment.Vectors.ToPointCloud2D(360));

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "stop";
			description = "stop all activities";
		}
	}
}
