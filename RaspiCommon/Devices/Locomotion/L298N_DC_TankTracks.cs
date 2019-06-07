using System;
using System.Collections.Generic;
using KanoopCommon.Geometry;
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

		public static SortedDictionary<int, Double> SpeedAtPower { get; private set; }
		static List<int> _sortedSpeeds;

		public int RightAdjust { get; set; }
		public int LeftAdjust { get; set; }

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
				_leftspeed = AdjustedSpeed(value, LeftAdjust);
				SetValue(Motors[LEFT_MOTOR], _leftspeed);
			}
			get { return _leftspeed; }
		}

		int _rightspeed;
		public int RightSpeed
		{
			set
			{
				_rightspeed = AdjustedSpeed(value, RightAdjust);
				SetValue(Motors[RIGHT_MOTOR], _rightspeed);
			}
			get { return _rightspeed; }
		}

		#endregion

		#region Constructor

		static L298N_DC_TankTracks()
		{
			SpeedAtPower = new SortedDictionary<int, double>()
			{
				{ 50,   .2125 },
				{ 60,   .325 },
				{ 70,   .425 },
				{ 75,   .475 },
				{ 80,   .5 },
				{ 85,   .53 },
				{ 90,   .575 },
				{ 95,   .625 },
				{100,   .74 },
			};
			_sortedSpeeds = new List<int>(SpeedAtPower.Keys);
		}

		public L298N_DC_TankTracks(GpioPin leftIn1, GpioPin leftIn2, GpioPin leftSpeed, GpioPin rightIn1, GpioPin rightIn2, GpioPin rightSpeed)
		{
			Motors = new L298N_DC_MotorControl[2];
			Motors[LEFT_MOTOR] = new L298N_DC_MotorControl(leftIn1, leftIn2, leftSpeed);
			Motors[RIGHT_MOTOR] = new L298N_DC_MotorControl(rightIn1, rightIn2, rightSpeed);

			Motors[LEFT_MOTOR].Start();
			Motors[RIGHT_MOTOR].Start();
		}
	
		public static TimeSpan TimeToTravel(Double distance, int speed)
		{
			speed = Math.Abs(speed);
			int index = _sortedSpeeds.BinarySearch(speed);
			if(index < 0)
			{
				index = ~index;
				index = Math.Min(index, _sortedSpeeds.Count - 1);
				if(index > 0)
				{
					int diff1 = Math.Abs(_sortedSpeeds[index] - speed);
					int diff2 = Math.Abs(_sortedSpeeds[index - 1] - speed);
					if(diff2 < diff1)
					{
						--index;
					}
				}
			}
			Double thisSpeed = SpeedAtPower[_sortedSpeeds[index]];
			Double travelTime = distance / thisSpeed;
			return TimeSpan.FromSeconds(travelTime);
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

		protected int AdjustedSpeed(int speed, int adjustment)
		{
			if(speed > 0)
			{
				speed += adjustment;
			}
			else if(speed < 0)
			{
				speed -= adjustment;
			}
			return speed;
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
