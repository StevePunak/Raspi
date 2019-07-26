using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspiCommon.Devices.Servos;
using RaspiCommon.PiGpio;

namespace RaspiCommon.Devices.RobotArms
{
	public class MeArm
	{
		public const int DEFAULT_MIN = 600;
		public const int DEFAULT_MAX = 2400;
		public const int DEFAULT_QUANTUM = 50;

		public int Rotation
		{
			get { return ServoController[RotationPin].CurrentSetting; }
			set { ServoController[RotationPin].SetDestinationPercentage(value); }
		}

		public int Elevation
		{
			get { return ServoController[ElevationPin].CurrentSetting; }
			set { ServoController[ElevationPin].SetDestinationPercentage(value); }
		}

		public int Thrust
		{
			get { return ServoController[ThrustPin].CurrentSetting; }
			set { ServoController[ThrustPin].SetDestinationPercentage(value); }
		}

		public int Claw
		{
			get { return ServoController[ClawPin].CurrentSetting; } set { ServoController[ClawPin].SetDestinationPercentage(value); }
		}

		public ServoMoveThread ServoController { get; private set; }

		public GpioPin RotationPin { get; private set; }
		public GpioPin ElevationPin { get; private set; }
		public GpioPin ThrustPin { get; private set; }
		public GpioPin ClawPin { get; private set; }

		public int RotationPinMin { get { return ServoController[RotationPin].Minimum; } set { ServoController[RotationPin].Minimum = value; } }
		public int RotationPinMax { get { return ServoController[RotationPin].Maximum; } set { ServoController[RotationPin].Maximum = value; } }
		public int ElevationPinMin { get { return ServoController[ElevationPin].Minimum; } set { ServoController[ElevationPin].Minimum = value; } }
		public int ElevationPinMax { get { return ServoController[ElevationPin].Maximum; } set { ServoController[ElevationPin].Maximum = value; } }
		public int ThrustPinMin { get { return ServoController[ThrustPin].Minimum; } set { ServoController[ThrustPin].Minimum = value; } }
		public int ThrustPinMax { get { return ServoController[ThrustPin].Maximum; } set { ServoController[ThrustPin].Maximum = value; } }
		public int ClawPinMin { get { return ServoController[ClawPin].Minimum; } set { ServoController[ClawPin].Minimum = value; } }
		public int ClawPinMax { get { return ServoController[ClawPin].Maximum; } set { ServoController[ClawPin].Maximum = value; } }

		public MeArm(ServoMoveThread servoController, GpioPin rotationPin, GpioPin elevationPin, GpioPin thrustPin, GpioPin clawPin)
		{
			ServoController = servoController;

			ServoController.AddServo(new ServoParameters()
			{
				Name = "Rotation",
				Pin = rotationPin,
				Minimum = DEFAULT_MIN,
				Maximum = DEFAULT_MAX,
				Quantum = 50,
			});

			ServoController.AddServo(new ServoParameters()
			{
				Name = "Left",
				Pin = elevationPin,
				Minimum = DEFAULT_MIN,
				Maximum = DEFAULT_MAX,
				Quantum = 50,
			});

			ServoController.AddServo(new ServoParameters()
			{
				Name = "Right",
				Pin = thrustPin,
				Minimum = DEFAULT_MIN,
				Maximum = DEFAULT_MAX,
				Quantum = 50,
			});

			ServoController.AddServo(new ServoParameters()
			{
				Name = "Claw",
				Pin = clawPin,
				Minimum = DEFAULT_MIN,
				Maximum = DEFAULT_MAX,
				Quantum = 50,
			});

			RotationPin = rotationPin;
			ElevationPin = elevationPin;
			ThrustPin = thrustPin;
			ClawPin = clawPin;

			Pigs.SetMode(RotationPin, PinMode.Output);
			Pigs.SetMode(ElevationPin, PinMode.Output);
			Pigs.SetMode(ThrustPin, PinMode.Output);
			Pigs.SetMode(ClawPin, PinMode.Output);

			RotationPinMin = DEFAULT_MIN;
			RotationPinMax = DEFAULT_MAX;
			ElevationPinMin = DEFAULT_MIN;
			ElevationPinMax = DEFAULT_MAX;
			ThrustPinMin = DEFAULT_MIN;
			ThrustPinMax = DEFAULT_MAX;
			ClawPinMin = DEFAULT_MIN;
			ClawPinMax = DEFAULT_MAX;
		}

		public void Home()
		{
			Rotation = Elevation = Thrust = Claw = 50;
		}

		public void Stop()
		{
			Pigs.SetServoPosition(RotationPin, 0);
			Pigs.SetServoPosition(ElevationPin, 0);
			Pigs.SetServoPosition(ThrustPin, 0);
			Pigs.SetServoPosition(ClawPin, 0);
		}

		public override string ToString()
		{
			return String.Format("MeArm Rot: {0}  Elev: {1}  Thr: {2}  Claw: {3}", RotationPin, ElevationPin, ThrustPin, ClawPin);
		}
	}
}
