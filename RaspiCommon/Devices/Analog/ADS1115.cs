using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Threading;

namespace RaspiCommon.Devices.Analog
{
	public class ADS1115 : ThreadBase
	{
		[DllImport("libwiringPi.so")]
		public static extern int ads1115Setup(int pinbase, int i2caddress);

		#region Enumerations

		public enum InputPin
		{
			A0 = 0, A1 = 1, A2 = 2, A3 = 3
		}
		public enum GainType { GAIN_6_144, GAIN_4_096, GAIN_2_048, GAIN_1_024, GAIN_0_512, GAIN_0_256 };

		#endregion

		#region Constants

		const int GPIOSHARP_BASE_ADDR = 120;
		static readonly Double[] _gainDivisors =
		{
			6.144, 4.096, 2.048, 1.024, 0.512, 0.256
		};

		#endregion

		#region Properties

		GainType _gain;
		public GainType Gain
		{
			get { return _gain; }
			set
			{
				_gain = value;
				SetGain(value);
			}
		}

		public Byte I2CAddress { get; set; }

		#endregion

		#region Delegates and Events

		public delegate void VoltageReceivedHandler(InputPin pin, Double voltage);

		public event VoltageReceivedHandler VoltageReceived;

		#endregion

		#region Construct / Start

		public ADS1115(byte i2cAddress)
			: base(typeof(ADS1115).Name)
		{
			Interval = TimeSpan.FromSeconds(1);
			VoltageReceived += delegate {};
			I2CAddress = i2cAddress;
			Gain = GainType.GAIN_6_144;
		}

		protected override bool OnStart()
		{
			bool result;
			if((result = base.OnStart()))
			{
				if((result = WiringPi.Setup()))
				{
					result = ads1115Setup(GPIOSHARP_BASE_ADDR, I2CAddress) == 1;
					SetGain(Gain);
				}
			}
			return result;
		}

		#endregion

		#region Run

		protected override bool OnRun()
		{
			for(InputPin pin = InputPin.A0;pin <= InputPin.A3;pin++)
			{
				int result = WiringPi.analogRead(GPIOSHARP_BASE_ADDR + (int)pin);
				//Console.WriteLine($"Will multiply {result} by {_gainDivisors[(int)Gain]}");
				Double voltage = (Double)result * _gainDivisors[(int)Gain] / 32768.0;
				VoltageReceived(pin, voltage);
			}

			return true;
		}

		#endregion

		#region Utility

		private void SetGain(GainType gain)
		{
			WiringPi.digitalWrite(GPIOSHARP_BASE_ADDR, (int)gain);
		}

		#endregion
	}
}
