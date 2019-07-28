using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using RaspiCommon;
using RaspiCommon.Devices.Spatial;

namespace TrackBot.TTY
{
	[CommandText("s")]
	public class Status : CommandBase
	{
		public Status()
			: base(true) {}

		public override bool Execute(List<string> commandParts)
		{
			Console.WriteLine("Heading: {0:0.0}", Widgets.Instance.Compass.Bearing);
			Console.WriteLine("Magnetic Deviation: {0:0.0}°", Widgets.Instance.Compass.MagneticDeviation);
			if(Widgets.Instance.HasImageEnvironment)
			{
				Console.WriteLine("Lidar Offset {0:0.0}°", Widgets.Instance.ImageEnvironment.CompassOffset);
			}
			if(RaspiConfig.Instance.ChassisEnabled)
			{
				Console.WriteLine("Track1: {0}  Track2: {1}", Widgets.Instance.Tracks.LeftSpeed, Widgets.Instance.Tracks.RightSpeed);
			}

#if ULTRASONIC
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Obstacle Distances:  ");
			foreach(HCSR04_RangeFinder rangeFinder in Widgets.Instance.RangeFinders.Values)
			{
				sb.AppendFormat("{0} => {1:0.00}m    ", rangeFinder.Direction, rangeFinder.Range);
			}
			Console.WriteLine(sb.ToString());
#endif
			if(RaspiConfig.Instance.SendRangeImagingTelemetry)
			{
				Double forwardBearing = Widgets.Instance.Compass.Bearing;
				Double backwardBearing = Widgets.Instance.Compass.Bearing.AddDegrees(180);
				Double forwardRange = Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.Chassis, forwardBearing, Program.Config.BearingFuzz);
				Double backwardRange = Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.Chassis, backwardBearing, Program.Config.BearingFuzz);
				Console.WriteLine("Range at [Forward {0:0.00}° {1:0.00}m]   [Backward {2:0.00}° {3:0.00}m]", forwardBearing, forwardRange, backwardBearing, backwardRange);
				if(Widgets.Instance.DeadReckoningEnvironment != null)
				{
					Console.WriteLine("DR Environs: {0}", Widgets.Instance.DeadReckoningEnvironment);
				}
				HCSR04_RangeFinder forwardUS, backwardUS;
				if(Widgets.Instance.RangeFinders.TryGetValue(RFDir.Front, out forwardUS) && Widgets.Instance.RangeFinders.TryGetValue(RFDir.Rear, out backwardUS))
				{
					Console.WriteLine("Ultrasonic [Forward {0}]   [Backward {1}]", forwardUS.Range.ToMetersString(), backwardUS.Range.ToMetersString());
				}
			}
			return true;
		}

		public override void Usage(out String commandSyntax, out String description)
		{
			commandSyntax = "s";
			description = "show TrackBot status";
		}
	}
}
