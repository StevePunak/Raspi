using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using RaspiCommon.Devices.Optics;

namespace TrackBot.TTY
{
	[CommandText("grab")]
	class GrabImage : CommandBase
	{
		public GrabImage()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			Widgets.Instance.LEDImageAnalysis.AnalyzeImage();

			Double ledBearing;
			if(Widgets.Instance.LEDImageAnalysis.TryGetBearing(Color.Green, Widgets.Instance.Compass.Bearing, out ledBearing))
			{
				Log.SysLogText(LogLevel.DEBUG, "LED Bearing is {0}", ledBearing.ToAngleString());
//				Widgets.Instance.Tracks.TurnToBearing(ledBearing);
			}

//			Widgets.Instance.Server.PublishImage(outputFiles);

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "grab";
			description = "grab an image and serve in mqtt";
		}
	}
}
