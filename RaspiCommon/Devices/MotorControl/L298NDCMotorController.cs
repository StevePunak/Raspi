using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon.PiGpio;

namespace RaspiCommon.Devices.MotorControl
{
	public class L298NDCMotorController
	{
		public GpioPin ForwardDirectionPin { get; private set; }
		public GpioPin BackwardDirectionPin { get; private set; }
		public GpioPin SpeedPin { get; private set; }

		public bool HardwarePWM { get; set; }

		Direction _direction;
		public Direction Direction
		{
			get { return _direction; }
			set
			{
				_direction = value;
				if(value == Direction.Forward)
				{
					Pigs.SetOutputPin(ForwardDirectionPin, PinState.High);
					Pigs.SetOutputPin(BackwardDirectionPin, PinState.Low);
				}
				else
				{
					Pigs.SetOutputPin(ForwardDirectionPin, PinState.Low);
					Pigs.SetOutputPin(BackwardDirectionPin, PinState.High);
				}
			}
		}

		UInt32 _speed;
		public UInt32 Speed
		{
			get
			{
				return _speed;
			}
			set
			{
				if(HardwarePWM == false && value >= 0 && value <= 255)
				{
					_speed = value;
					Pigs.SetPWM(SpeedPin, _speed);
				}
				else if(HardwarePWM == true && value >= 0 && value <= 100)
				{
					throw new NotImplementedException("The signature of this call needs to change");
#if zero
					_speed = value;
					Pigs.SetHardwarePWM(SpeedPin, 800, _speed);
#endif
				}
			}
		}

		public L298NDCMotorController(GpioPin forwardDirectionPin, GpioPin backwardDirectionPin, GpioPin enable)
		{
			ForwardDirectionPin = forwardDirectionPin;
			BackwardDirectionPin = backwardDirectionPin;
			SpeedPin = enable;

			Pigs.SetMode(ForwardDirectionPin, PinMode.Output);
			Pigs.SetMode(BackwardDirectionPin, PinMode.Output);
			Pigs.SetMode(SpeedPin, PinMode.Output);
			Direction = Direction.Forward;
			Speed = 0;
		}



		public void Start()
		{
			Direction = _direction;
		}

		public void Stop()
		{
			Pigs.SetOutputPin(ForwardDirectionPin, PinState.Low);
			Pigs.SetOutputPin(BackwardDirectionPin, PinState.Low);
		}

	}
}
