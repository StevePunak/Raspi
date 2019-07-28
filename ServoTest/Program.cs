using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using RaspiCommon;
using RaspiCommon.Devices.Analog;
using RaspiCommon.PiGpio;

namespace ServoTest
{
	class Program
	{
		const Double MAX_VOLTAGE = 2.5;
		const Double MIN_SERVO = 600;
		const Double MAX_SERVO = 2400;
		const int MINIMUM_CHANGE = 50;

		static List<GpioPin> Pins;
		static List<int> LastValues;

		static void Main(string[] args)
		{
			try
			{
				Pins = new List<GpioPin>();
				LastValues = new List<int>();
				if(args.Length > 0)
				{
					foreach(String arg in args)
					{
						int pin = int.Parse(arg);
						Pins.Add((GpioPin)pin);
						LastValues.Add(0);
					}
				}
				else
				{
					Pins.Add(GpioPin.Pin18);        // default rotation
					Pins.Add(GpioPin.Pin22);     // default left-side
					Pins.Add(GpioPin.Pin27);     // default right-side
					Pins.Add(GpioPin.Pin17);        // default claw
					LastValues.AddRange(new int[] { 0, 0, 0, 0 });
				}
				if(!WiringPi.Setup())
				{
					throw new Exception($"WiringPI setup failed");
				}

				ADS1115 reader = new ADS1115(0x48)
				{
					Interval = TimeSpan.FromMilliseconds(100),
				};
				reader.Gain = ADS1115.GainType.GAIN_6_144;
				reader.VoltageReceived += OnReader_VoltageReceived;
				reader.Start();

				Thread.Sleep(10000000);
			}
			catch(Exception e)
			{
				Console.WriteLine($"ERROR: {e.Message}");
			}

		}

		private static void OnReader_VoltageReceived(ADS1115.InputPin pin, double voltage)
		{
			switch(pin)
			{
				case ADS1115.InputPin.A0:
					SetServoPosition(0, voltage);
					break;
				case ADS1115.InputPin.A1:
					SetServoPosition(1, voltage);
					break;
				case ADS1115.InputPin.A2:
					SetServoPosition(2, voltage);
					break;
				case ADS1115.InputPin.A3:
					SetServoPosition(3, voltage);
					break;
			}
		}

		private static void SetServoPosition(int index, double voltage)
		{
			if(index >= Pins.Count)
				return;

			int servoSetting = GetServoValue(voltage);
			Console.WriteLine($"Make {servoSetting} from {voltage} for {Pins[index]}");
			if(Math.Abs(servoSetting - LastValues[index]) > MINIMUM_CHANGE)
			{
				Pigs.SetServoPosition(Pins[index], servoSetting);
				LastValues[index] = servoSetting;
			}
		}

		private static int GetServoValue(Double voltage)
		{
			voltage = voltage.EnsureBetween(0, MAX_VOLTAGE);
			Double servoSetting = (((voltage / MAX_VOLTAGE) * (MAX_SERVO - MIN_SERVO)) + MIN_SERVO).EnsureBetween(MIN_SERVO, MAX_SERVO);
			return (int)servoSetting;
		}
	}
}
