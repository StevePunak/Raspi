using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;

namespace RaspiCommon.PiGpio
{
	internal class PigsDirect : IPigsGpio
	{
		[DllImport("libpigpiod_if2.so")]
		public static extern int pigpio_start(IntPtr addr, IntPtr port);

		[DllImport("libpigpiod_if2.so")]
		public static extern void pigpio_stop(int pi);

		[DllImport("libpigpiod_if2.so")]
		public static extern int set_mode(int pi, uint gpio, uint mode);

		[DllImport("libpigpiod_if2.so")]
		public static extern int get_mode(int pi, uint gpio);

		[DllImport("libpigpiod_if2.so")]
		public static extern int notify_open(int pi);

		[DllImport("libpigpiod_if2.so")]
		public static extern int notify_begin(int pi, uint handle, uint bits);

		[DllImport("libpigpiod_if2.so")]
		public static extern int notify_close(int pi, uint handle);

		[DllImport("libpigpiod_if2.so")]
		public static extern int notify_pause(int pi, uint handle);

		[DllImport("libpigpiod_if2.so")]
		public static extern int hardware_PWM(int pi, uint gpio, uint frequency, uint dutyCycle);

		[DllImport("libpigpiod_if2.so")]
		public static extern int set_PWM_dutycycle(int pi, uint gpio, uint dutyCycle);

		[DllImport("libpigpiod_if2.so")]
		public static extern int set_servo_pulsewidth(int pi, uint gpio, uint pulsewidth);

		[DllImport("libpigpiod_if2.so")]
		public static extern int gpio_write(int pi, uint gpio, uint level);

		[DllImport("libpigpiod_if2.so")]
		public static extern int set_pull_up_down(int pi, uint gpio, uint pud);

		[DllImport("libpigpiod_if2.so")]
		public static extern GpioPin time_sleep(Double seconds);

		[DllImport("libpigpiod_if2.so")]
		public static extern int gpio_read(int pi, uint gpio);

		public bool Running
		{
			get
			{
				return false;
			}
		}

		static PigsDirect Self { get { return Pigs.Instance as PigsDirect; } }
		int PiHandle { get; set; }
		bool PiHandleInitialized { get; set; }

		public PigsDirect()
		{
			PiHandle = 0;
			PiHandleInitialized = false;
			EnsurePigsReady();
		}

		class GpioReadItem
		{
			public GpioPin Pin { get; private set; }
			public FileStream Stream { get; private set; }
			public String Device { get; private set; }
			public GpioInputCallback Callback { get; private set; }
			public UInt32 BitMask { get; private set; }

			public int NotifyHandle { get; set; }

			byte[] _readBuffer;

			static UInt64 StartTicks;
			static UInt32 LastEventTick;

			static GpioReadItem()
			{
				StartTicks = LastEventTick = 0;
			}

			public GpioReadItem(GpioPin pin, GpioInputCallback callback)
			{
				NotifyHandle = notify_open(PigsDirect.Self.PiHandle);
				Log.SysLogText(LogLevel.INFO, $"Opened PiGpio notify handle {NotifyHandle}");

				Pin = pin;
				BitMask = MakeBits(Pin);
				int result = notify_begin(Self.PiHandle, (uint)NotifyHandle, BitMask);
				Device = $"/dev/pigpio{(int)NotifyHandle}";
				Log.SysLogText(LogLevel.DEBUG, "Open Pin Item at {0} with bits = 0x{1:X8} gave result {2}", Device, BitMask, result);
				Callback = callback;
				Stream = new FileStream(Device, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true);
				StartRead();
			}

			void StartRead()
			{
				_readBuffer = new byte[12];
				Stream.BeginRead(_readBuffer, 0, _readBuffer.Length, ReadCallback, this);
			}

			void ReadCallback(IAsyncResult asyncResult)
			{
				int bytesRead = 0;
				try
				{
					bytesRead = Stream.EndRead(asyncResult);
					if(bytesRead > 0)
					{
						using(MemoryStream ms = new MemoryStream(_readBuffer))
						using(BinaryReader br = new BinaryReader(ms))
						{
							UInt16 sequence = br.ReadUInt16();
							UInt16 flags = br.ReadUInt16();
							UInt32 tick = br.ReadUInt32();
							UInt32 level = br.ReadUInt32();

							// account for rollover
							if(tick < LastEventTick)
							{
								StartTicks += 0xffffffff;
							}
							LastEventTick = tick;

							if(flags == 0)
							{
								EdgeType edge = (level & BitMask) != 0 ? EdgeType.Rising : EdgeType.Falling;
								Pigs.MaybeLog(LogLevel.DEBUG, $"Making callback {Pin} {edge} {Device} seq: {sequence}  flags 0x{flags:X4}  tick: {tick}  level: 0x{level:X8}",
									sequence, flags, tick, level);
								Callback(Pin, edge, StartTicks + tick, sequence);
							}

						}
						StartRead();
					}
				}
				catch(Exception e)
				{
					Log.SysLogText(LogLevel.WARNING, $"PIGS read callback exception: {e.Message}. {bytesRead} bytes read");
				}
			}

			public void Close()
			{
				try
				{
					Stream.Close();
					notify_close(Self.PiHandle, (uint)NotifyHandle);
				}
				catch {}
			}

			static uint MakeBits(GpioPin pin)
			{
				int x = (int)pin;
				return (uint)(1 << x);
			}
		}

		Dictionary<GpioPin, GpioReadItem> _gpioReads;

		private void EnsurePigsReady()
		{
			if(PiHandleInitialized == false)
			{
				PiHandle = pigpio_start(IntPtr.Zero, IntPtr.Zero);
				if(PiHandle >= 0)
				{
					Log.SysLogText(LogLevel.INFO, $"Initialized PiGpio (handle {PiHandle})");
					PiHandleInitialized = true;
				}
			}
		}

		public void Start()
		{
			Log.SysLogText(LogLevel.DEBUG, $"Starting PigsDirect");
			_gpioReads = new Dictionary<GpioPin, GpioReadItem>();
		}

		public void Stop()
		{
			Log.SysLogText(LogLevel.DEBUG, $"Stopping PigsDirect");
			foreach(KeyValuePair<GpioPin, GpioReadItem> kvp in _gpioReads)
			{
				kvp.Value.Close();
			}
		}

		public void StartInputPin(GpioPin pin, EdgeType edgeType, GpioInputCallback callback)
		{
			GpioReadItem readItem;
			if(_gpioReads.TryGetValue(pin, out readItem))
			{
				readItem.Close();
				_gpioReads.Remove(pin);
			}
			readItem = new GpioReadItem(pin, callback);
			_gpioReads.Add(pin, readItem);
		}

		public void StopInputPin(GpioPin pin)
		{
			GpioReadItem readItem;
			if(_gpioReads.TryGetValue(pin, out readItem))
			{
				readItem.Close();
				_gpioReads.Remove(pin);
			}
			else
			{
				Log.SysLogText(LogLevel.ERROR, $"Can't find input pin handler for {pin}");
			}
		}

		#region Commands

		public void SetMode(GpioPin gpioPin, PinMode mode)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "PIGS set mode {0} {1}", gpioPin, mode);
			EnsurePigsReady();
			set_mode(PiHandle, (uint)gpioPin, (uint)mode);
		}

		public void SetHardwarePWM(GpioPin gpioPin, UInt32 frequency, UInt32 dutyCycle)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, "PIGS set Hardware PWM {0} freq {1}  duty cycle {2}", gpioPin, frequency, dutyCycle);
			EnsurePigsReady();
			hardware_PWM(PiHandle, (uint)gpioPin, frequency, dutyCycle);
		}

		public void SetPWM(GpioPin gpioPin, UInt32 dutyCycle)
		{
			EnsurePigsReady();
			Pigs.MaybeLog(LogLevel.DEBUG, "PIGS set PWM duty cycle {0} {1}", gpioPin, dutyCycle);
			set_PWM_dutycycle(PiHandle, (uint)gpioPin, dutyCycle);
		}

		public void SetServoPosition(GpioPin gpioPin, int position)
		{
			EnsurePigsReady();
			SetServoPosition(gpioPin, (UInt32)position);
		}

		public void SetServoPosition(GpioPin gpioPin, UInt32 pulsewidth)
		{
			EnsurePigsReady();
			Pigs.MaybeLog(LogLevel.DEBUG, "Set servo {0} to {1}", gpioPin, pulsewidth);
			set_servo_pulsewidth(PiHandle, (uint)gpioPin, pulsewidth);
		}

		public void SetOutputPin(GpioPin gpioPin, PinState value)
		{
			EnsurePigsReady();
			Pigs.MaybeLog(LogLevel.DEBUG, "*********** PIGS output pin {0} {1}", gpioPin, value);
			gpio_write(PiHandle, (uint)gpioPin, (uint)value);
		}

		public void SetOutputPin(GpioPin gpioPin, bool value)
		{
			EnsurePigsReady();
			Pigs.MaybeLog(LogLevel.DEBUG, ">>>>>>>>>>>>>>>> PIGS output pin {0} {1}", gpioPin, value);
			SetOutputPin(gpioPin, value ? PinState.High : PinState.Low);
		}

		public void SetPullUp(GpioPin gpioPin, PullUp state)
		{
			set_pull_up_down(PiHandle, (uint)gpioPin, (uint)state);
		}

		public PinState ReadInputPin(GpioPin gpioPin)
		{
			int result = gpio_read(PiHandle, (uint)gpioPin);
			return (PinState)result;
		}

		public void Delay(TimeSpan time)
		{
			Pigs.MaybeLog(LogLevel.DEBUG, $"Sleep {time.TotalMilliseconds}ms");
			time_sleep(time.TotalSeconds);
		}
	}

	#endregion
}

