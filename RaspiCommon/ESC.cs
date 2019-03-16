using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RaspiCommon
{
	public class ESC
	{
		public const UInt32 Minimum = 1000;
		public const UInt32 Maximum = 2000;

		public bool Calibrated { get; private set; }
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

		public ESC(GpioPin pin)
		{
			Calibrated = false;
			Pin = pin;
		}

		public void Calibrate()
		{
			Console.WriteLine("Calibrating");

			Value = Minimum;
			Thread.Sleep(2000);
			Value = Maximum;
			Thread.Sleep(2000);
			Value = Minimum;
		}
	}
}
