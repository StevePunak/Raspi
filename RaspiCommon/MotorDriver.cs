using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

namespace RaspiCommon
{
	public class MotorDriver : ThreadBase
	{
		int _index;

		int _intervalMs;
		bool _rotatingToDestination;
		Double _stepsLeft; 

		public bool Running { get; set; }
		public Direction Direction { get; set; }

		static readonly List<List<bool>> _steps = new List<List<bool>>()
		{
			new List<bool>() { true,  false, true,  false },
			new List<bool>() { false, true,  true,  false },
			new List<bool>() { false, true,  false, true },
			new List<bool>() { true,  false, false, true },
		};
		static readonly Dictionary<MotorSpeed, int> _speeds = new Dictionary<MotorSpeed, int>()
		{
			{ MotorSpeed.Stopped,      1000 },
			{ MotorSpeed.VerySlow,     50 },
			{ MotorSpeed.Slow,         30 },
			{ MotorSpeed.MediumSlow,   25  },
			{ MotorSpeed.Medium,       20  },
			{ MotorSpeed.MediumFast,   15  },
			{ MotorSpeed.Fast,         10  },
			{ MotorSpeed.VeryFast,     5  },
			{ MotorSpeed.FullSpeed,    1   },
		};

		GpioPin A1, A2, B1, B2;

		MotorSpeed _speed;
		public MotorSpeed Speed
		{
			get { return _speed; }
			set
			{
				SetSpeed(value);
			}
		}

		public MotorDriver(GpioPin a1, GpioPin a2, GpioPin b1, GpioPin b2)
			: base(typeof(MotorDriver).Name)
		{
			Log.SysLogText(LogLevel.DEBUG, "Instantiating motor driver");

			A1 = a1;
			A2 = a2;
			B1 = b1;
			B2 = b2;

			Running = false;
			Direction = Direction.Forward;
			SetSpeed(MotorSpeed.Stopped);
			_intervalMs = 1000;

			_index = 0;
		}

		protected override bool OnStart()
		{
			Log.SysLogText(LogLevel.DEBUG, "Started motor driver");

			return base.OnStart();
		}

		protected override bool OnStop()
		{
			return base.OnStop();
		}

		public void Rotate(Direction direction, Double steps)
		{
			Direction = direction;
			_stepsLeft = steps;
			_rotatingToDestination = true;
		}

		protected override bool OnRun()
		{
			try
			{
				if(_rotatingToDestination)
				{
					if(_stepsLeft == 0)
					{
						_rotatingToDestination = false;
						Running = false;
					}
					else
					{
						_stepsLeft--;
					}
				}

				if(Running)
				{
					List<bool> step = _steps[_index];

					// Log.SysLogText(LogLevel.DEBUG, "pin {0} {1} {2} {3}", step[0], step[1], step[2], step[3], _stepsLeft);
					Pigs.SetOutputPin(A1, step[0]);
					Pigs.SetOutputPin(A2, step[1]);
					Pigs.SetOutputPin(B1, step[2]);
					Pigs.SetOutputPin(B2, step[3]);
					if(Direction == Direction.Forward)
					{
						if(++_index == 4)
							_index = 0;
					}
					else
					{
						if(--_index < 0)
							_index = 3;
					}
				}

				Interval = TimeSpan.FromMilliseconds(_intervalMs);
			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.DEBUG, "Exception: {0}", e.Message);
			}
			return true;
		}

		private void SetSpeed(MotorSpeed speed)
		{
			_speed = speed;
			if(_speed == MotorSpeed.Stopped)
			{
				Running = false;
			}
			else
			{
				_intervalMs = _speeds[_speed];
				Running = true;
			}
		}
	}
}
