using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using RaspiCommon;
using RaspiCommon.Devices.MotorControl;
using RaspiCommon.PiGpio;

namespace Testing
{
	class L298NDCMotorTest : TestBase
	{
		protected override void Run()
		{
			Pigs.InterfaceType = Pigs.PigsType.IP;
			Pigs.DebugLogging = true;
			L298NDCMotorController motor = new L298NDCMotorController(GpioPin.Pin11, GpioPin.Pin13, GpioPin.Pin05);
			motor.Start();
			motor.Direction = Direction.Forward;
			uint speed = 20;
			uint step = 1;
			uint max = 255;
			while(!Quit)
			{
				Console.WriteLine($"{motor.Direction} {speed}");
				motor.Speed = speed;
				
				ConsoleKeyInfo key = Console.ReadKey();
				switch(key.Key)
				{
					case ConsoleKey.DownArrow:
						speed = speed >= step ? speed - step : 0;
						break;
					case ConsoleKey.UpArrow:
						speed = speed + step < max ? speed + step : max;
						break;
				}
			}

			motor.Stop();
		}

		private void OnConsole_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
