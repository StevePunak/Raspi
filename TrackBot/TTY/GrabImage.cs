using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
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
			LEDImageAnalysis.DebugAnalysis = true;

			Mat image;
			if(Widgets.Instance.Camera.TryTakeSnapshot(out image) == false)
			{
				throw new CommandException("Could not retrieve camera image");
			}

			Color color;
			if(commandParts.Count > 1 && (color = Color.FromName(commandParts[1])).ToArgb() != 0)
			{
				Widgets.Instance.LEDImageAnalysis.AnalyzeImage(image, color);

				Double ledBearing;
				if(Widgets.Instance.LEDImageAnalysis.TryGetBearing(color, Widgets.Instance.Compass.Bearing, out ledBearing))
				{
					Log.SysLogText(LogLevel.DEBUG, "LED Bearing is {0}", ledBearing.ToAngleString());
				}
			}
			else
			{
				Widgets.Instance.LEDImageAnalysis.AnalyzeImage(image);
			}

			LEDImageAnalysis.DebugAnalysis = false;
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "grab";
			description = "grab an image and serve in mqtt";
		}
	}
}
