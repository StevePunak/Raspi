using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Addresses;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

namespace RaspiCommon.PiGpio
{
	internal class PigsIP : IPigsGpio
	{
		#region Public Properties

		IPv4AddressPort _hostAddress;
		public IPv4AddressPort Host
		{
			get { return _hostAddress; }
			set
			{
				_hostAddress = value;
				Stop();
				Start();
			}
		}

		public TimeSpan PollInterval
		{
			set { _pollThread.Interval = value; }
		}

		public bool Running
		{
			get
			{
				return _client != null && _client.Connected;
			}
		}

		#endregion

		#region Private

		GpioPollThread _pollThread;

		#endregion

		#region Constructor

		internal PigsIP()
		{
			_hostAddress = new IPv4AddressPort("localhost", 8888);
		}

		#endregion

		#region Public Access Methods

		TcpClient _client;
		public void Start()
		{
			if(_pollThread == null)
			{
				_pollThread = new GpioPollThread(this);
				_pollThread.Start();
			}
		}

		private void StartClient()
		{
			if(_client != null)
			{
				_client = null;
			}

			Log.SysLogText(LogLevel.DEBUG, $"Connect PIGS to {_hostAddress}");
			try
			{
				_client = new TcpClient(_hostAddress.HostName, _hostAddress.Port);
			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.WARNING, $"ERROR connecting pigs to {_hostAddress}: {e.Message}");
			}
		}

		public void SendCommand(PigsCommand command)
		{
			PigsCommand result;
			SendCommand(command, out result);
		}

		public void SendCommand(PigsCommand command, out PigsCommand result)
		{
			if(_client == null)
			{
				StartClient();
			}
			byte[] serialized = command.Serialize();
			_client.Client.Send(serialized);

			byte[] readBuffer = new byte[256];
			int bytesRead = _client.Client.Receive(readBuffer);
			result = new PigsCommand(readBuffer, bytesRead);
		}

		public void Stop()
		{
			if(_client != null)
			{
				_client.Close();
			}
			_client = null;

			if(_pollThread != null)
			{
				_pollThread.Stop();
				_pollThread = null;
			}
		}

		#endregion

		#region Commands

		public void SetMode(GpioPin gpioPin, PinMode mode)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "PIGS set mode {0} {1}", gpioPin, mode);

			PigsCommand command = new PigsCommand(PigsCommand.CommandType.MODES, gpioPin, (char)mode);
			SendCommand(command);
		}

		public void SetHardwarePWM(GpioPin gpioPin, UInt32 frequency, UInt32 dutyCycle)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "PIGS set Hardware PWM {0} freq {1}  duty cycle {2}", gpioPin, frequency, dutyCycle);
			PigsCommand command = new PigsCommand(PigsCommand.CommandType.HP, gpioPin, frequency, dutyCycle);
			SendCommand(command);
		}

		public void SetPWM(GpioPin gpioPin, UInt32 dutyCycle)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "PIGS set PWM duty cycle {0} {1}", gpioPin, dutyCycle);

			PigsCommand command = new PigsCommand(PigsCommand.CommandType.PWM, gpioPin, dutyCycle);
			SendCommand(command);
		}

		public void SetServoPosition(GpioPin gpioPin, int position)
		{
			SetServoPosition(gpioPin, (UInt32)position);
		}

		public void SetServoPosition(GpioPin gpioPin, UInt32 position)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "Set servo {0} to {1}", gpioPin, position);

			PigsCommand command = new PigsCommand(PigsCommand.CommandType.SERVO, gpioPin, position);
			SendCommand(command);
		}

		public void SetOutputPin(GpioPin gpioPin, PinState value)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "*********** PIGS output pin {0} {1}", gpioPin, value);

			PigsCommand command = new PigsCommand(PigsCommand.CommandType.WRITE, gpioPin, (UInt32)value);
			SendCommand(command);
		}

		public void SetOutputPin(GpioPin gpioPin, bool value)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, ">>>>>>>>>>>>>>>> PIGS output pin {0} {1}", gpioPin, value);

			PigsCommand command = new PigsCommand(PigsCommand.CommandType.WRITE, gpioPin, value ? (UInt32)1 : (UInt32)0);
			SendCommand(command);
		}

		public void SetPullUp(GpioPin gpioPin, PullUp state)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, ">>>>>>>>>>>>>>>> PIGS set pullup {0} {1}", gpioPin, state);

			PigsCommand command = new PigsCommand(PigsCommand.CommandType.PUD, gpioPin, (UInt32)state);
			SendCommand(command);
		}

		public PinState ReadInputPin(GpioPin gpioPin)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, ">>>>>>>>>>>>>>>> PIGS read pin {0}", gpioPin);

			PigsCommand command = new PigsCommand(PigsCommand.CommandType.READ, gpioPin, 0);
			PigsCommand result;
			SendCommand(command, out result);
			return (PinState)result.ExtendedData;
		}

		public void StartInputPin(GpioPin pin, EdgeType edgeType, GpioInputCallback callback)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, $">>>>>>>>>>>>>>>> Start GPIO pin {pin} {edgeType}");
			_pollThread.SetPinPoll(pin, edgeType, callback);
		}

		public void StopInputPin(GpioPin pin)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, $">>>>>>>>>>>>>>>> Stop GPIO pin {pin}");
			_pollThread.StopPinPoll(pin);
		}

		public void Delay(TimeSpan time)
		{
			Thread.Sleep(time);
		}

		#endregion

		#region Gpio Poll Thread

		class GpioPollThread : ThreadBase
		{
			PigsIP Pigs { get; set; }

			class GpioReadItem
			{
				public GpioPin Pin { get; set; }
				public GpioInputCallback Callback { get; set; }
				public PinState State { get; set; }
				public EdgeType EdgeType { get; set; }

				public int Sequence { get; set; }
				public int LastSequence { get; set; }

				public GpioReadItem(GpioPin pin)
					: this(pin, EdgeType.Both, null) { }

				public GpioReadItem(GpioPin pin, EdgeType edgeType, GpioInputCallback callback)
				{
					Sequence = LastSequence = 0;
					Log.SysLogText(LogLevel.INFO, $"Starting GpioPoll on {pin}");

					Pin = pin;
					Callback = callback;
					State = PinState.Low;
					EdgeType = edgeType;
				}

			}

			Dictionary<GpioPin, GpioReadItem> _gpioReads;

			public GpioPollThread(PigsIP pigs)
				: base(typeof(GpioPollThread).Name)
			{
				Pigs = pigs;
				_gpioReads = new Dictionary<GpioPin, GpioReadItem>();
				Interval = TimeSpan.FromMilliseconds(100);
			}

			public void SetPinPoll(GpioPin pin, EdgeType edgeType, GpioInputCallback callback)
			{
				lock(this)
				{
					GpioReadItem item;
					if(_gpioReads.TryGetValue(pin, out item) == false)
					{
						item = new GpioReadItem(pin);
						_gpioReads.Add(pin, item);
					}
					item.EdgeType = edgeType;
					item.Callback = callback;
					item.Sequence = item.LastSequence = 0;
				}
			}

			public void StopPinPoll(GpioPin pin)
			{
				lock(this)
				{
					if(_gpioReads.ContainsKey(pin))
					{
						_gpioReads.Remove(pin);
					}
				}
			}

			protected override bool OnRun()
			{
				Dictionary<GpioPin, GpioReadItem> reads = null;
				lock(this)
				{
					reads = new Dictionary<GpioPin, GpioReadItem>(_gpioReads);
				}

				foreach(KeyValuePair<GpioPin, GpioReadItem> kvp in reads)
				{
					GpioReadItem item = kvp.Value;
					PinState state = Pigs.ReadInputPin(kvp.Key);
					if(state != item.State)
					{
						item.State = state;
						item.Sequence++;
						if(item.EdgeType == EdgeType.Both || 
							item.EdgeType == EdgeType.Rising || state == PinState.High ||
							item.EdgeType == EdgeType.Falling && state == PinState.Low)
						{
							ulong usecs = (ulong)TimeSpanExtensions.CurrentMicroseconds;
							item.Callback(item.Pin, state == PinState.High ? EdgeType.Rising : EdgeType.Falling, usecs, (ushort)item.Sequence);
						}
					}
				}

				return true;
			}
		}

		#endregion
	}
}
