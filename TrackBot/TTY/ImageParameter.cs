using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspiCommon.Devices.Optics;

namespace TrackBot.TTY
{
	[CommandText("ip")]
	class ImageParameter : CommandBase
	{
		public ImageParameter()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			int parm;
			if(commandParts.Count < 3)
			{
				throw new CommandException("Invalid parameter value");
			}

			Color color;
			if(commandParts[1] == "br" && int.TryParse(commandParts[2], out parm) == true)
			{
				Program.Config.CameraParameters.Brightness = Widgets.Instance.Camera.Parameters.Brightness = parm;
			}
			else if(commandParts[1] == "co" && int.TryParse(commandParts[2], out parm) == true)
			{
				Program.Config.CameraParameters.Contrast = Widgets.Instance.Camera.Parameters.Contrast = parm;
			}
			else if(commandParts[1] == "sa" && int.TryParse(commandParts[2], out parm) == true)
			{
				Program.Config.CameraParameters.Saturation = Widgets.Instance.Camera.Parameters.Saturation = parm;
			}
			else if(commandParts[1] == "delay" && int.TryParse(commandParts[2], out parm) == true)
			{
				Program.Config.CameraParameters.SnapshotDelay = Widgets.Instance.Camera.Parameters.SnapshotDelay = TimeSpan.FromMilliseconds(parm);
			}
			else if(commandParts[1] == "ex")
			{
				Program.Config.CameraParameters.Exposure = Widgets.Instance.Camera.Parameters.Exposure = commandParts[2];
			}
			else if(commandParts[1] == "mm")
			{
				Program.Config.CameraParameters.MeteringMode = Widgets.Instance.Camera.Parameters.MeteringMode = commandParts[2];
			}
			else if(commandParts[1] == "awb")
			{
				Program.Config.CameraParameters.AutoWhiteBalance = Widgets.Instance.Camera.Parameters.AutoWhiteBalance = commandParts[2];
			}
			else if(commandParts[1] == "ifx")
			{
				Program.Config.CameraParameters.ImageEffect = Widgets.Instance.Camera.Parameters.ImageEffect = commandParts[2];
			}
			else if(commandParts[1] == "cfx")
			{
				Program.Config.CameraParameters.ColorEffect = Widgets.Instance.Camera.Parameters.ColorEffect = commandParts[2];
			}
			else if(commandParts[1] == "low" && int.TryParse(commandParts[2], out parm) && (color = Color.FromName(commandParts[3])).ToArgb() !=  0)
			{
				if(color == Color.Blue)	Program.Config.BlueThresholds.MinimumValue = parm;
				else if(color == Color.Green) Program.Config.GreenThresholds.MinimumValue = parm;
				else if(color == Color.Red) Program.Config.RedThresholds.MinimumValue = parm;
			}
			else if(commandParts[1] == "high" && int.TryParse(commandParts[2], out parm) && (color = Color.FromName(commandParts[3])).ToArgb() != 0)
			{
				if(color == Color.Blue)	Program.Config.BlueThresholds.MaximumOtherValue = parm;
				else if(color == Color.Green) Program.Config.GreenThresholds.MaximumOtherValue = parm;
				else if(color == Color.Red) Program.Config.RedThresholds.MaximumOtherValue = parm;
			}
			else
			{
				throw new CommandException("Invalid paramters");
			}
			Program.Config.Save();

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "ip [br/ex/ifx] [value]";
			description = "set camera image brightness";
		}
	}
}
