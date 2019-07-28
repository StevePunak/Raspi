using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;

namespace RaspiCommon.PiGpio
{
	internal class PigsIP : IPigsGpio
	{
		public bool Running
		{
			get
			{
				return _client != null && _client.Connected;
			}
		}

		TcpClient _client;
		public void Start()
		{
			if(_client != null)
			{
				_client = null;
			}
			String pigsHost = "localhost";
			int pigsPort = 8888;
			Log.SysLogText(LogLevel.DEBUG, $"Connect PIGS to {pigsHost}:{pigsPort}");
			_client = new TcpClient(pigsHost, pigsPort);
		}

		public void SendCommand(PigCommand command)
		{
			if(_client == null)
			{
				Start();
			}
			byte[] serialized = command.Serialize();
			_client.Client.Send(serialized);
		}

		public void Stop()
		{
			_client.Close();
			_client = null;
		}

		#region Commands

		public void SetMode(GpioPin gpioPin, PinMode mode)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "PIGS set mode {0} {1}", gpioPin, mode);

			PigCommand command = new PigCommand(PigCommand.CommandType.MODES, gpioPin, (char)mode);
			SendCommand(command);
		}

		public void SetHardwarePWM(GpioPin gpioPin, UInt32 frequency, UInt32 dutyCyclePercent)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "PIGS set Hardware PWM {0} freq {1}  duty cycle {2}", gpioPin, frequency, dutyCyclePercent);
			if(dutyCyclePercent > 100)
			{
				throw new RaspiException("Duty cycle must be 0-100");
			}

			PigCommand command = new PigCommand(PigCommand.CommandType.HP, gpioPin, frequency, dutyCyclePercent * 10000);
			SendCommand(command);
		}

		public void SetPWM(GpioPin gpioPin, UInt32 dutyCycle)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "PIGS set PWM duty cycle {0} {1}", gpioPin, dutyCycle);

			PigCommand command = new PigCommand(PigCommand.CommandType.PWM, gpioPin, dutyCycle);
			SendCommand(command);
		}

		public void SetServoPosition(GpioPin gpioPin, int position)
		{
			SetServoPosition(gpioPin, (UInt32)position);
		}

		public void SetServoPosition(GpioPin gpioPin, UInt32 position)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "Set servo {0} to {1}", gpioPin, position);

			PigCommand command = new PigCommand(PigCommand.CommandType.SERVO, gpioPin, position);
			SendCommand(command);
		}

		public void SetOutputPin(GpioPin gpioPin, PinState value)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "*********** PIGS output pin {0} {1}", gpioPin, value);

			PigCommand command = new PigCommand(PigCommand.CommandType.WRITE, gpioPin, (UInt32)value);
			SendCommand(command);
		}

		public void SetOutputPin(GpioPin gpioPin, bool value)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, ">>>>>>>>>>>>>>>> PIGS output pin {0} {1}", gpioPin, value);

			PigCommand command = new PigCommand(PigCommand.CommandType.WRITE, gpioPin, value ? (UInt32)1 : (UInt32)0);
			SendCommand(command);
		}

		public void SetPullUp(GpioPin gpioPin, PullUp state)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, ">>>>>>>>>>>>>>>> PIGS set pullup {0} {1}", gpioPin, state);

			PigCommand command = new PigCommand(PigCommand.CommandType.PUD, gpioPin, (UInt32)state);
			SendCommand(command);
		}

		public void StartInputPin(GpioPin pin, EdgeType edgeType, GpioInputCallback callback)
		{
			Log.SysLogText(LogLevel.ERROR, "StartInputPin not implemented for IP Pigs");
		}

		public void StopInputPin(GpioPin pin)
		{
			Log.SysLogText(LogLevel.ERROR, "StopInputPin not implemented for IP Pigs");
		}

		#endregion
	}
}
