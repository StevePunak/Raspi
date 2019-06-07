using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;

namespace RaspiCommon.Devices.MotorControl
{
	public class L298N_DC_MotorControl
	{
		public GpioPin IN1 { get; private set; }
		public GpioPin IN2 { get; private set; }
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
					Pigs.SetOutputPin(IN1, PinState.High);
					Pigs.SetOutputPin(IN2, PinState.Low);
				}
				else
				{
					Pigs.SetOutputPin(IN1, PinState.Low);
					Pigs.SetOutputPin(IN2, PinState.High);
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
					_speed = value;
					Pigs.SetHardwarePWM(SpeedPin, 800, _speed);
				}
			}
		}

		public L298N_DC_MotorControl(GpioPin in1, GpioPin in2, GpioPin enable)
		{
			IN1 = in1;
			IN2 = in2;
			SpeedPin = enable;

			Pigs.SetMode(IN1, PinMode.Output);
			Pigs.SetMode(IN2, PinMode.Output);
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
			Pigs.SetOutputPin(IN1, PinState.Low);
			Pigs.SetOutputPin(IN2, PinState.Low);
		}

	}
}
