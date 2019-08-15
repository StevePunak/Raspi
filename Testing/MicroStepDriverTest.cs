using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.PiGpio;

namespace Testing
{
	class MicroStepDriverTest : TestBase
	{
		MicroStepDriver Driver;

		public MicroStepDriverTest()
		{
			Log.LogMicroseconds = true;

		}

		protected override void Run()
		{
			Pigs.InterfaceType = Pigs.PigsType.IP;
			Pigs.SetTcpPigsHost("raspi3", 8888);
			Pigs.SetTcpPigsPollInterval(TimeSpan.FromMilliseconds(10));

			Driver = new MicroStepDriver("Microstep Driver Test",
				GpioPin.Pin12,                                  // pulse (output)
				GpioPin.Pin26,                                  // direction (output)
				GpioPin.Pin13,                                  // enable (output)
				MicroStepDriver.PulseSetting.P1600);            // DIP setting on controller
			Driver.DebugLogging = true;
			Driver.Start();
			Driver.Enabled = true;
			Driver.Speed = 40;

			Driver.TurnHardDegrees(10, SpinDirection.Clockwise);
			//Driver.StartMotor(SpinDirection.Clockwise);

			//Log.SysLogText(LogLevel.DEBUG, $"Total Travel Length {Driver.TravelLength}");

			WaitForQuit();

			Driver.Enabled = false;

			Driver.Stop();
			Pigs.Stop();
		}
	}
}
