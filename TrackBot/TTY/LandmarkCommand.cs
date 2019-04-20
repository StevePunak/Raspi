using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Spatial.LidarImaging;

namespace TrackBot.TTY
{
	[CommandText("lc")]
	class LandmarkCommand : CommandBase
	{
		public LandmarkCommand()
			: base(true) { }

		public override bool Execute(List<string> commandParts)
		{
			if(commandParts.Count < 2)
			{
				throw new CommandException("Invalid parameter value");
			}

			String subCommand = commandParts[1].ToLower();
			if(subCommand == "ls")			// list saved
			{
				foreach(ImageVector landmark in Program.Config.Landmarks)
				{
					Console.WriteLine("{0}", landmark);
				}
			}
			else if(subCommand == "lc")		// list found
			{
				foreach(ImageVector landmark in Widgets.Instance.ImageEnvironment.Landmarks)
				{
					Console.WriteLine("{0}", landmark);
				}
			}
			else if(subCommand == "addall")    // add all
			{
				Program.Config.Landmarks.AddRange(Widgets.Instance.ImageEnvironment.Landmarks);
				Program.Config.Save();
			}
			else if(subCommand == "clear")    // clear
			{
				Program.Config.Landmarks.Clear();
				Program.Config.Save();
			}
			else
			{
				Console.WriteLine("Unknown parameter");
			}

			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "lc [subcommand]";
			description = "manipulate landmarks";
		}
	}
}
