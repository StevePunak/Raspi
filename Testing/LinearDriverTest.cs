using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using RaspiCommon;
using RaspiCommon.PiGpio;

namespace Testing
{
	class LinearDriverTest : TestBase
	{
		LinearActuator Actuator;

		public LinearDriverTest()
		{
			Log.LogMicroseconds = true;

		}

		bool FreeDriveComplete { get; set; }
		protected override void Run()
		{
			Pigs.InterfaceType = Pigs.PigsType.IP;
			Pigs.SetTcpPigsHost("raspi3", 8888);
			Pigs.SetTcpPigsPollInterval(TimeSpan.FromMilliseconds(10));

			Actuator = new LinearActuator("Linear Actuator",
				GpioPin.Pin12,									// pulse (output)
				GpioPin.Pin26,									// direction (output)
				GpioPin.Pin13,									// enable (output)
				GpioPin.Pin24,									// upper limit switch (input)
				GpioPin.Pin23,                                  // lower limit switch (input)
				MomentarySwitchMode.NormalOpen,					// mode for limit switches
				MicroStepDriver.PulseSetting.P1600,				// DIP setting on controller
				.005);											// pitch = 5mm
			Actuator.DebugLogging = true;
			Actuator.LimitSwitchTriggered += OnLimitSwitchTriggered;
			Actuator.FreeDriveComplete += OnFreeDriveComplete;
			Actuator.Start();
			Actuator.Enabled = true;
			Actuator.Speed = 40;

			FreeDrive(.05, Direction.Down);

			FindBottom();

			FindTop();

			FreeDrive(.05, Direction.Down);

			Log.SysLogText(LogLevel.DEBUG, $"Total Travel Length {Actuator.TravelLength}");

			WaitForQuit();

			Actuator.Enabled = false;

			Actuator.Stop();
			Pigs.Stop();
		}

		void FindBottom()
		{
			Actuator.MoveToLowerLimit();
			while(!Quit && Actuator.HasHome == false)
			{
				Log.LogText(LogLevel.DEBUG, $"{Actuator.Rotation.ToAngleString()} {Actuator.Position}");
				Sleep(1000);
			}
		}

		void FindTop()
		{
			Actuator.MoveToUpperLimit();
			while(!Quit && Actuator.LimitedUp == false)
			{
				Log.LogText(LogLevel.DEBUG, $"{Actuator.Rotation.ToAngleString()} {Actuator.Position}");
				Sleep(1000);
			}
		}

		void FreeDrive(Double distance, Direction direction)
		{
			FreeDriveComplete = false;
			Actuator.Move(distance, direction);

			while(!Quit && FreeDriveComplete == false)
			{
				Log.LogText(LogLevel.DEBUG, $"{Actuator.Rotation.ToAngleString()} {Actuator.Position}");
				Sleep(1000);
			}
		}

		private void OnFreeDriveComplete(SpinDirection direction, double degrees)
		{
			Log.SysLogText(LogLevel.DEBUG, $"Free drive complete at {Actuator.Position}");
			FreeDriveComplete = true;
		}

		private void OnLimitSwitchTriggered(LinearActuator.LimitType limit, bool state)
		{
			Log.SysLogText(LogLevel.DEBUG, $"Limit switch triggered {limit} {state}");
		}

		ulong lastusecs = 0;
		void GpioInputCallback(GpioPin pin, EdgeType edgeType, UInt64 microseconds, UInt16 sequence)
		{
			Log.SysLogText(LogLevel.DEBUG, $"==============>>>>> {pin} {edgeType} {microseconds}  {sequence} {microseconds - lastusecs}");
			lastusecs = microseconds;
		}

	}

}
