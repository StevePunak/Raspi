using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using RaspiCommon.Devices.Servos;
using RaspiCommon.PiGpio;

namespace RaspiCommon.Devices.RobotArms
{
	public class RobotArm
	{
		public const int DEFAULT_MIN = 600;
		public const int DEFAULT_MAX = 2400;
		const int MINIMUM_STEP = 2;

		int Minimum { get { return DEFAULT_MIN; } }
		int Maximum { get { return DEFAULT_MAX; } }

		int _rotation;
		public int Rotation
		{
			get { return _rotation; }
			set
			{
				if(_rotation.AbsoluteDifference(value) >= MINIMUM_STEP)
				{
					_rotation = value;
					SetPin(RotationPin, _rotation);
				}
			}
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				if(_left.AbsoluteDifference(value) >= MINIMUM_STEP)
				{
					_left = value;
					SetPin(LeftPin, _left);
				}
			}
		}

		int _right;
		public int Right
		{
			get { return _right; }
			set
			{
				if(_right.AbsoluteDifference(value) >= MINIMUM_STEP)
				{
					_right = value;
					SetPin(RightPin, _right);
				}
			}
		}

		int _claw;
		public int Claw
		{
			get { return _claw; }
			set
			{
				if(_claw.AbsoluteDifference(value) >= MINIMUM_STEP)
				{
					_claw = value;
					SetPin(ClawPin, _claw);
				}
			}
		}

		public GpioPin RotationPin { get; private set; }
		public GpioPin RightPin { get; private set; }
		public GpioPin LeftPin { get; private set; }
		public GpioPin ClawPin { get; private set; }

		public RobotArm(GpioPin rotationPin, GpioPin leftPin, GpioPin rightPin, GpioPin clawPin)
		{
			RotationPin = rotationPin;
			RightPin = rightPin;
			LeftPin = leftPin;
			ClawPin = clawPin;

			Pigs.SetMode(RotationPin, PinMode.Output);
			Pigs.SetMode(RightPin, PinMode.Output);
			Pigs.SetMode(LeftPin, PinMode.Output);
			Pigs.SetMode(ClawPin, PinMode.Output);
		}

		public void Home()
		{
			Rotation = Left = Right = Claw = 50;
		}

		public void Stop()
		{
			Pigs.SetServoPosition(RotationPin, 0);
			Pigs.SetServoPosition(RightPin, 0);
			Pigs.SetServoPosition(LeftPin, 0);
			Pigs.SetServoPosition(ClawPin, 0);
		}

		private void SetPin(GpioPin pin, int value)
		{
			int servoValue = MakeServoSettingFromPercentage(value);
			Console.WriteLine(
				$"(Rotation: {Rotation} {MakeServoSettingFromPercentage(Rotation)}) " +
				$"(Left: {Left} {MakeServoSettingFromPercentage(Left)}) " +
				$"(Right: {Right} {MakeServoSettingFromPercentage(Right)})" +
				$"(Claw: {Claw} {MakeServoSettingFromPercentage(Claw)}) ");
			Pigs.SetServoPosition(pin, MakeServoSettingFromPercentage(value));
		}

		private int MakeServoSettingFromPercentage(int percent)
		{
			return (int)(((Double)Maximum - (Double)Minimum) * ((Double)percent / 100) + Minimum);
		}

		public override string ToString()
		{
			return String.Format("MeArm Rot: {0}  Elev: {1}  Thr: {2}  Claw: {3}", RotationPin, RightPin, LeftPin, ClawPin);
		}
	}
}
