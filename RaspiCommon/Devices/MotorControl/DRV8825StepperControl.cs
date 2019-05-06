using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;

namespace RaspiCommon.Devices.MotorControl
{
	public class DRV8825StepperControl
	{
		const Int32 MIN_FREQUENCY = 0;
		const Int32 MAX_FREQUENCY = 500;

		GpioPin EnablePin { get; set; }
		GpioPin DirectionPin { get; set; }
		GpioPin StepPin { get; set; }

		bool _enabled;
		public bool Enabled
		{
			set
			{
				Log.SysLogText(LogLevel.DEBUG, "Setting enabled {0}", value);
				_enabled = value;
				Pigs.SetOutputPin(EnablePin, _enabled ? PinState.Low : PinState.High);
			}
			get { return _enabled; }
		}

		private int _speed;
		public int Speed
		{
			set
			{
				_speed = value;
				UInt32 frequency = (UInt32)(((MAX_FREQUENCY - MIN_FREQUENCY) / 100) * Math.Abs(value));
				if(value > 0)
				{
					Direction = Direction.Forward;
					Enabled = true;
					SetFrequency(frequency);
				}
				else if(value < 0)
				{
					Direction = Direction.Backward;
					Enabled = true;
					SetFrequency(frequency);
				}
				else
				{
					Enabled = false;
				}

			}
			get { return _speed; }
		}

		Direction _direction;
		public Direction Direction
		{
			set
			{
				Pigs.SetOutputPin(DirectionPin, value == Direction.Forward ? true : false);
				_direction = value;
			}
			get { return _direction; }
		}

		public DRV8825StepperControl(GpioPin enablePin, GpioPin directionPin, GpioPin stepPin)
		{
			EnablePin = enablePin;
			DirectionPin = directionPin;
			StepPin = stepPin;
		}

		void SetFrequency(UInt32 frequency)
		{
			Pigs.SetHardwarePWM(StepPin, frequency, 50);
		}

		public void Stop()
		{
			Pigs.SetHardwarePWM(StepPin, 0, 50);
			Enabled = false;
		}

	}
}
