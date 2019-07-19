using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RaspiCommon;
using RaspiCommon.Devices.Analog;
using RaspiCommon.Devices.RobotArms;

namespace Testing
{
	class ClawControlTest
	{
		Double Minimum = 0;
		Double Maximum = 2.5;

		RobotArm Arm { get; set; }

		public ClawControlTest()
		{
			Arm = new RobotArm(Program.Config.ClawRotationPin, Program.Config.ClawLeftPin, Program.Config.ClawRightPin, Program.Config.ClawPin);
			bool result = WiringPi.Setup();
			ADS1115 reader = new ADS1115(0x48)
			{
				Interval = TimeSpan.FromMilliseconds(100),
			};
			reader.Gain = ADS1115.GainType.GAIN_6_144;
			reader.VoltageReceived += OnReader_VoltageReceived;
			reader.Start();

			Thread.Sleep(10000000);
		}

		private void OnReader_VoltageReceived(ADS1115.InputPin pin, double voltage)
		{
			int percent = MakePercentage(voltage);
			switch(pin)
			{
				case ADS1115.InputPin.A0:
					Arm.Rotation = percent;
					break;
				case ADS1115.InputPin.A1:
					Arm.Left = percent;
					break;
				case ADS1115.InputPin.A2:
					Arm.Right = percent;
					break;
				case ADS1115.InputPin.A3:
					Arm.Claw = percent;
					break;
			}
		}

		private int MakePercentage(Double voltage)
		{
			Double percent = voltage / Maximum * 100;
			//Console.WriteLine($"Make {percent} from {voltage}");
			return (int)percent;
		}
	}
}
