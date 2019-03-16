using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon
{
	public class PWMDevice
	{
		public const String PIGS = "/usr/bin/pigs";

		UInt32 _frequency;
		public UInt32 Frequency { get { return _frequency; } set { _frequency = value; Initialized = false; } }
		UInt32 _minimum;
		public UInt32 Minumum { get { return _minimum; } set { _minimum = value; Initialized = false; } }
		UInt32 _maximum;
		public UInt32 Maximum { get { return _maximum; } set { _maximum = value; Initialized = false; } }
		UInt32 _value;
		public UInt32 Value { get { return _value; } set { _value = value; SetValue(); } }
		public GpioPin Pin { get; private set; }
		public UInt32 Mid { get { return Minumum + ((Maximum - Minumum) / 2); } }

		bool Initialized { get; set; }

		public PWMDevice(GpioPin gpioPin)
		{
			Pin = gpioPin;
			Frequency = 50;
			Minumum = 600;
			Maximum = 2200;

			Initialized = false;

			Initialize();

		}

		void Initialize()
		{
			try
			{
				Console.WriteLine("Setting pin {0} freq to {1}", Pin, Frequency);
				Pigs.SetMode(Pin, PinMode.Output);
				Pigs.SetPWM(Pin, 0);
				Initialized = true;
			}
			catch(Exception e)
			{
				Console.WriteLine("EXCEPTION: {0}", e.Message);
			}
		}

		void SetValue()
		{
			try
			{
				if(!Initialized)
				{
					Initialize();
				}
				Console.WriteLine("seting duty cycle to {0}", _value);
				Pigs.SetPWM(Pin, Frequency);
			}
			catch(Exception e)
			{
				Console.WriteLine("EXCEPTION: {0}", e.Message);
			}
		}

	}
}

