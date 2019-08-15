using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.Devices.MotorControl;
using RaspiCommon.PiGpio;

namespace Testing
{
	class L298NStepperTest : TestBase
	{
		SpinDirection Direction = SpinDirection.CounterClockwise;
		bool ChangeDirection = false;

		protected override void Run()
		{
			Direction = SpinDirection.Clockwise;

			Pigs.InterfaceType = Pigs.PigsType.Direct;
			//Pigs.SetTcpPigsHost("raspi3", 8888);
			//Pigs.SetTcpPigsPollInterval(TimeSpan.FromMilliseconds(10));

			L298NStepperController stepper = new L298NStepperController(
				"Test Controller",
				GpioPin.Pin12,
				GpioPin.Pin16,
				GpioPin.Pin20,
				GpioPin.Pin21);

			stepper.MoveComplete += OnStepperMoveComplete;
			stepper.StepType = StepType.FullStep;
			stepper.Start();
			stepper.SetRPM(40);
			stepper.DebugLogging = false;

			ChangeDirection = true;
			while(!Quit)
			{
				if(ChangeDirection)
				{
					Log.SysLogText(LogLevel.DEBUG, "Change direction OK");
					Direction = Direction == SpinDirection.Clockwise ? SpinDirection.CounterClockwise : SpinDirection.Clockwise;
					stepper.StepType = stepper.StepType == StepType.FullStep ? StepType.HalfStep : StepType.FullStep;
					stepper.TurnDegrees(360 * 10, Direction);
					ChangeDirection = false;
				}
			}
			stepper.Stop();
		}

		private void OnStepperMoveComplete()
		{
			Log.SysLogText(LogLevel.DEBUG, "Change direction");
			ChangeDirection = true;
		}
	}
}
