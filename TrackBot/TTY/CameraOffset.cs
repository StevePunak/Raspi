using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Devices.Optics;

namespace TrackBot.TTY
{
	[CommandText("co")]
	class CameraOffset : CommandBase
	{
		public CameraOffset()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			LEDImageAnalysis.Width = 800;
			LEDImageAnalysis.Height = 600;

			Widgets.Instance.LEDImageAnalysis.AnalyzeImage();

			List<String> outputFiles;
			LEDPositionList leds;
			LEDImageAnalysis.AnalyzeImage(
				Widgets.Instance.LEDImageAnalysis.LastImageAnalysisFile, 
				Widgets.Instance.Server.ImageDirectory, 
				Program.Config.LedLowThreshold, Program.Config.LedHighThreshold,
				out outputFiles, out leds);
			Widgets.Instance.Server.PublishImage(outputFiles);
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "sc [ms]";
			description = "Spin Clockwise from the given number of milliseconds";
		}
	}
}
