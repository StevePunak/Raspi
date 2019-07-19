using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon
{
	public class WiringPi
	{
		[DllImport("libwiringPi.so")]
		public static extern int wiringPiSetup();

		[DllImport("libwiringPi.so")]
		public static extern int analogRead(int pin);

		[DllImport("libwiringPi.so")]
		public static extern void analogWrite(int pin, int value);

		[DllImport("libwiringPi.so")]
		public static extern int digitalRead(int pin);

		[DllImport("libwiringPi.so")]
		public static extern void digitalWrite(int pin, int value);

		[DllImport("libwiringPi.so")]
		public static extern uint digitalRead8(int pin);

		[DllImport("libwiringPi.so")]
		public static extern void digitalWrite8(int pin, int value);

		public static bool Setup()
		{
			return wiringPiSetup() == 0;
		}

		public static int AnalogRead(int pin)
		{
			return analogRead(pin);
		}

		public static void AnalogWrite(int pin, int value)
		{
			analogWrite(pin, value);
		}

		public static int DigitalRead(int pin)
		{
			return digitalRead(pin);
		}

		public static void DigitalWrite(int pin, int value)
		{
			digitalWrite(pin, value);
		}

		public static uint DigitalRead8(int pin)
		{
			return digitalRead8(pin);
		}

		public static void DigitalWrite8(int pin, int value)
		{
			digitalWrite8(pin, value);
		}
	}
}
