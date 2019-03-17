using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon
{
	public class L298N_DC_TankTracks
	{
		int LEFT_MOTOR = 1;
		int RIGHT_MOTOR = 0;

		L298_DC_MotorControl[] Motors { get; set; }

		int _leftspeed;
		/// <summary>
		/// Set left track speed as a value from -100 (full backward) to +100 (full forward)
		/// </summary>
		public int LeftSpeed
		{
			set
			{
				_leftspeed = value;
				SetValue(Motors[LEFT_MOTOR], value);
//				Console.WriteLine("Set left speed to {0} going {1}", _leftspeed, Motors[LEFT_MOTOR].Direction);
			}
			get { return _leftspeed; }
		}

		int _rightspeed;
		public int RightSpeed
		{
			set
			{
				_rightspeed = value;
				SetValue(Motors[RIGHT_MOTOR], value);
//				Console.WriteLine("Set right speed to {0} going {1}", _rightspeed, Motors[RIGHT_MOTOR].Direction);
			}
			get { return _rightspeed; }
		}

		void SetValue(L298_DC_MotorControl motor, double value)
		{
			Double percent = Math.Min(Math.Abs(value) / 100, 1);
			Double speed = (Double)255 * percent;
			motor.Direction = value >= 0 ? Direction.Forward : Direction.Backward;
			motor.Speed = (UInt32)speed;
		}

		public L298N_DC_TankTracks(GpioPin leftIn1, GpioPin leftIn2, GpioPin leftSpeed, GpioPin rightIn1, GpioPin rightIn2, GpioPin rightSpeed)
		{
			Motors = new L298_DC_MotorControl[2];
			Motors[LEFT_MOTOR] = new L298_DC_MotorControl(leftIn1, leftIn2, leftSpeed);
			Motors[RIGHT_MOTOR] = new L298_DC_MotorControl(rightIn1, rightIn2, rightSpeed);

			Motors[LEFT_MOTOR].Start();
			Motors[RIGHT_MOTOR].Start();
		}

		public void Stop()
		{
			Motors[LEFT_MOTOR].Speed = 0;
			Motors[RIGHT_MOTOR].Speed = 0;
		}

	}
}
