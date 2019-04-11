using System;
using KanoopCommon.Logging;
using RaspiCommon.Devices.MotorControl;
using RaspiCommon.Spatial;

namespace RaspiCommon.Devices.Locomotion
{
	public class L298N_DC_TankTracks
	{
		#region Constants

		int LEFT_MOTOR = 1;
		int RIGHT_MOTOR = 0;

		#endregion

		#region Public Properties

		bool _hardwarePWM;
		public bool HardwarePWM
		{
			get { return _hardwarePWM; }
			set { _hardwarePWM = Motors[RIGHT_MOTOR].HardwarePWM = Motors[LEFT_MOTOR].HardwarePWM = value ; }
		}

		L298N_DC_MotorControl[] Motors { get; set; }

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
			}
			get { return _rightspeed; }
		}

		#endregion

		#region Constructor

		public L298N_DC_TankTracks(GpioPin leftIn1, GpioPin leftIn2, GpioPin leftSpeed, GpioPin rightIn1, GpioPin rightIn2, GpioPin rightSpeed)
		{
			Motors = new L298N_DC_MotorControl[2];
			Motors[LEFT_MOTOR] = new L298N_DC_MotorControl(leftIn1, leftIn2, leftSpeed);
			Motors[RIGHT_MOTOR] = new L298N_DC_MotorControl(rightIn1, rightIn2, rightSpeed);

			Motors[LEFT_MOTOR].Start();
			Motors[RIGHT_MOTOR].Start();
		}
	
	#endregion

		#region Private Methods

		void SetValue(L298N_DC_MotorControl motor, double value)
		{
			if(HardwarePWM)
			{
				Double percent = Math.Min(Math.Abs(value) / 100, 1);
				Double speed = (Double)100 * percent;
				motor.Direction = value >= 0 ? Direction.Forward : Direction.Backward;
				motor.Speed = (UInt32)speed;
			}
			else
			{
				Double percent = Math.Min(Math.Abs(value) / 100, 1);
				Double speed = (Double)255 * percent;
				motor.Direction = value >= 0 ? Direction.Forward : Direction.Backward;
				motor.Speed = (UInt32)speed;
			}
		}

		#endregion

		#region Public Access Methods

		public void Stop()
		{
			Motors[LEFT_MOTOR].Speed = 0;
			Motors[RIGHT_MOTOR].Speed = 0;
		}

		#endregion
	}
}
