using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.PiGpio;

namespace RaspiCommon.Devices.Servos
{
	public class Servo
	{
		public static readonly UInt32 MINIMUM = 600;
		public static readonly UInt32 MAXIMUM = 2400;
		public static readonly UInt32 NEUTRAL = 2400;

		public UInt32 Minimum { get; set; }
		public UInt32 Maximum { get; set; }
		public UInt32 Mid { get { return Minimum + ((Maximum - Minimum) / 2); } }

		public GpioPin Pin { get; private set; }

		UInt32 _value;
		public UInt32 Value
		{
			get { return _value; }
			set
			{
				_value = value;
				Pigs.SetServoPosition(Pin, value);
			}
		}

		public Servo(GpioPin pin)
		{
			Minimum = MINIMUM;
			Maximum = MAXIMUM;
			Pin = pin;
		}

		public UInt32 MakePercentage(int percent)
		{
			return (UInt32)(((Double)Maximum - (Double)Minimum) * ((Double)percent / 100) + Minimum);
		}
	}
}
