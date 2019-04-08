using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;

namespace RaspiCommon
{
	public class Pigs
	{
		public static bool Running
		{
			get
			{
				return _client != null && _client.Connected;
			}
		}

		static TcpClient _client;

		internal static void Start()
		{
			if(_client != null)
			{
				_client = null;
			}
			_client = new TcpClient("127.0.0.1", 8888);
		}

		public static void SendCommand(PigCommand command)
		{
			if(_client == null)
			{
				Start();
			}
			_client.Client.Send(command.Serialize());
		}

		internal static void Stop()
		{
			_client.Close();
			_client = null;
		}

		#region Commands

		public static void SetMode(GpioPin gpioPin, PinMode mode)
		{
//			Console.WriteLine("PIGS set mode {0} {1}", gpioPin, mode);

			PigCommand command = new PigCommand(PigCommand.CommandType.MODES, gpioPin, (char)mode);
			SendCommand(command);
		}

		public static void SetHardwarePWM(GpioPin gpioPin, UInt32 frequency, UInt32 dutyCyclePercent)
		{
			if(dutyCyclePercent > 100)
			{
				throw new RaspiException("Duty cycle must be 0-100");
			}

			PigCommand command = new PigCommand(PigCommand.CommandType.HP, gpioPin, frequency, dutyCyclePercent * 10000);
			SendCommand(command);
		}

		public static void SetPWM(GpioPin gpioPin, UInt32 dutyCycle)
		{
//			Console.WriteLine("PIGS set duty cycle {0} {1}", gpioPin, dutyCycle);

			PigCommand command = new PigCommand(PigCommand.CommandType.PWM, gpioPin, dutyCycle);
			SendCommand(command);
		}

		public static void SetServoPosition(GpioPin gpioPin, int position)
		{
			SetServoPosition(gpioPin, (UInt32)position);
		}

		public static void SetServoPosition(GpioPin gpioPin, UInt32 position)
		{
//			Console.WriteLine("Set servo {0} to {1}", gpioPin, position);

			PigCommand command = new PigCommand(PigCommand.CommandType.SERVO, gpioPin, position);
			SendCommand(command);
		}

		public static void SetOutputPin(GpioPin gpioPin, PinState value)
		{
//			Console.WriteLine("PIGS output pin {0} {1}", gpioPin, value);

			PigCommand command = new PigCommand(PigCommand.CommandType.WRITE, gpioPin, (UInt32)value);
			SendCommand(command);
		}

		public static void SetOutputPin(GpioPin gpioPin, bool value)
		{
			PigCommand command = new PigCommand(PigCommand.CommandType.WRITE, gpioPin, value ? (UInt32)1 : (UInt32)0);
			SendCommand(command);
		}

		#endregion
	}
}
