using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

		public int Left
		{
			get { return ServoController[LeftPin].CurrentSetting; }
			set { ServoController[LeftPin].SetDestinationPercentage(value); }
		}

		public int Right
		{
			get { return ServoController[RightPin].CurrentSetting; }
			set { ServoController[RightPin].SetDestinationPercentage(value); }
		}

		public int Claw
		{
			get { return ServoController[ClawPin].CurrentSetting; } set { ServoController[ClawPin].SetDestinationPercentage(value); }
		}

		public ServoMoveThread ServoController { get; private set; }

		public GpioPin RotationPin { get; private set; }
		public GpioPin LeftPin { get; private set; }
		public GpioPin RightPin { get; private set; }
		public GpioPin ClawPin { get; private set; }

		public int RotationPinMin { get { return ServoController[RotationPin].Minimum; } set { ServoController[RotationPin].Minimum = value; } }
		public int RotationPinMax { get { return ServoController[RotationPin].Maximum; } set { ServoController[RotationPin].Maximum = value; } }
		public int LeftPinMin { get { return ServoController[LeftPin].Minimum; } set { ServoController[LeftPin].Minimum = value; } }
		public int LeftPinMax { get { return ServoController[LeftPin].Maximum; } set { ServoController[LeftPin].Maximum = value; } }
		public int RightPinMin { get { return ServoController[RightPin].Minimum; } set { ServoController[RightPin].Minimum = value; } }
		public int RightPinMax { get { return ServoController[RightPin].Maximum; } set { ServoController[RightPin].Maximum = value; } }
		public int ClawPinMin { get { return ServoController[ClawPin].Minimum; } set { ServoController[ClawPin].Minimum = value; } }
		public int ClawPinMax { get { return ServoController[ClawPin].Maximum; } set { ServoController[ClawPin].Maximum = value; } }

		public MeArm(ServoMoveThread servoController, GpioPin rotationPin, GpioPin leftPin, GpioPin rightPin, GpioPin clawPin)
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
				Pin = leftPin,
				Minimum = DEFAULT_MIN,
				Maximum = DEFAULT_MAX,
				Quantum = 50,
			});

			ServoController.AddServo(new ServoParameters()
			{
				Name = "Right",
				Pin = rightPin,
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
				Quantum = 150,
			});

			RotationPin = rotationPin;
			LeftPin = leftPin;
			RightPin = rightPin;
			ClawPin = clawPin;

			Pigs.SetMode(RotationPin, PinMode.Output);
			Pigs.SetMode(LeftPin, PinMode.Output);
			Pigs.SetMode(RightPin, PinMode.Output);
			Pigs.SetMode(ClawPin, PinMode.Output);

			RotationPinMin = DEFAULT_MIN;
			RotationPinMax = DEFAULT_MAX;
			LeftPinMin = DEFAULT_MIN;
			LeftPinMax = DEFAULT_MAX;
			RightPinMin = DEFAULT_MIN;
			RightPinMax = DEFAULT_MAX;
			ClawPinMin = DEFAULT_MIN;
			ClawPinMax = DEFAULT_MAX;
		}

		public void Home()
		{
			Rotation = RaspiConfig.Instance.ClawRotationHomePosition;
			Left = RaspiConfig.Instance.ClawLeftHomePosition;
			Right = RaspiConfig.Instance.ClawRightHomePosition;
			Claw = RaspiConfig.Instance.ClawClawHomePosition;
		}

		public void Stop()
		{
			Pigs.SetServoPosition(RotationPin, 0);
			Pigs.SetServoPosition(LeftPin, 0);
			Pigs.SetServoPosition(RightPin, 0);
			Pigs.SetServoPosition(ClawPin, 0);
		}

		public override string ToString()
		{
			return String.Format("MeArm Rot: {0}  Left: {1}  Right: {2}  Claw: {3}", RotationPin, LeftPin, RightPin, ClawPin);
		}
	}
}
