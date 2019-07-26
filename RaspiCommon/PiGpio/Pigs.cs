using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;

namespace RaspiCommon.PiGpio
{
	public class Pigs
	{
		public enum PigsType
		{
			None,
			IP,
			Direct
		}

		public static bool DebugLogging { get; set; }

		public static void SetMode(GpioPin gpioPin, PinMode mode)
		{
			Instance.SetMode(gpioPin, mode);
		}

		public static void SetHardwarePWM(GpioPin gpioPin, UInt32 frequency, UInt32 dutyCyclePercent)
		{
			Instance.SetHardwarePWM(gpioPin, frequency, dutyCyclePercent);
		}

		public static void SetPWM(GpioPin gpioPin, UInt32 dutyCycle)
		{
			Instance.SetPWM(gpioPin, dutyCycle);
		}

		public static void SetServoPosition(GpioPin gpioPin, int position)
		{
			Instance.SetServoPosition(gpioPin, position);
		}

		public static void SetServoPosition(GpioPin gpioPin, UInt32 position)
		{
			Instance.SetServoPosition(gpioPin, position);
		}

		public static void SetOutputPin(GpioPin gpioPin, PinState value)
		{
			Instance.SetOutputPin(gpioPin, value);
		}

		public static void SetOutputPin(GpioPin gpioPin, bool value)
		{
			Instance.SetOutputPin(gpioPin, value);
		}

		public static void StartInputPin(GpioPin pin, EdgeType edgeType, GpioInputCallback callback)
		{
			Instance.StartInputPin(pin, edgeType, callback);
		}

		public static void StopInputPin(GpioPin pin)
		{
			Instance.StopInputPin(pin);
		}

		public static void SetPullUp(GpioPin gpioPin, PullUp state)
		{
			Instance.SetPullUp(gpioPin, state);
		}

		internal static void MaybeLog(LogLevel level, String format, params object[] parms)
		{
			if(DebugLogging)
			{
				Log.SysLogText(level, format, parms);
			}
		}

		public static void Stop()
		{
			Instance.Stop();
		}

		static IPigsGpio _instance;
		static internal IPigsGpio Instance
		{
			get
			{
				if(_instance == null)
				{
					throw new RaspiException("PIGS Interface type must be set prior to use");
				}
				return _instance;
			}
		}

		static PigsType _interfaceType;
		public static PigsType InterfaceType
		{
			get { return _interfaceType; }
			set
			{
				if(_interfaceType != value)
				{
					if(_instance != null)
					{
						_instance.Stop();
					}
					_interfaceType = value;
					_instance = Create();
					_instance.Start();
				}
			}
		}

		static IPigsGpio Create()
		{
			IPigsGpio pigs = null;
			switch(_interfaceType)
			{
				case PigsType.IP:
					pigs = new PigsIP();
					pigs.Start();
					break;

				case PigsType.Direct:
					pigs = new PigsDirect();
					pigs.Start();
					break;

				default:
					throw new RaspiException("Illegal Pigs type");
			}
			return pigs;
		}

		public class Constants
		{
			/* gpio: 0-53 */

			public const int PI_MIN_GPIO       = 0;
			public const int PI_MAX_GPIO      = 53;

			/* user_gpio: 0-31 */

			public const int PI_MAX_USER_GPIO = 31;

			/* level: 0-1 */

			public const int PI_OFF   = 0;
			public const int PI_ON    = 1;

			public const int PI_CLEAR = 0;
			public const int PI_SET   = 1;

			public const int PI_LOW   = 0;
			public const int PI_HIGH  = 1;

			/* level: only reported for GPIO time-out, see gpioSetWatchdog */

			public const int PI_TIMEOUT = 2;

			/* mode: 0-7 */

			public const int PI_INPUT  = 0;
			public const int PI_OUTPUT = 1;
			public const int PI_ALT0   = 4;
			public const int PI_ALT1   = 5;
			public const int PI_ALT2   = 6;
			public const int PI_ALT3   = 7;
			public const int PI_ALT4   = 3;
			public const int PI_ALT5   = 2;

			/* pud: 0-2 */

			public const int PI_PUD_OFF  = 0;
			public const int PI_PUD_DOWN = 1;
			public const int PI_PUD_UP   = 2;

			/* dutycycle: 0-range */

			public const int PI_DEFAULT_DUTYCYCLE_RANGE   = 255;

			/* range: 25-40000 */

			public const int PI_MIN_DUTYCYCLE_RANGE        = 25;
			public const int PI_MAX_DUTYCYCLE_RANGE     = 40000;

			/* pulsewidth: 0, 500-2500 */

			public const int PI_SERVO_OFF = 0;
			public const int PI_MIN_SERVO_PULSEWIDTH = 500;
			public const int PI_MAX_SERVO_PULSEWIDTH = 2500;

			/* hardware PWM */

			public const int PI_HW_PWM_MIN_FREQ = 1;
			public const int PI_HW_PWM_MAX_FREQ = 125000000;
			public const int PI_HW_PWM_RANGE = 1000000;

			/* hardware clock */

			public const int PI_HW_CLK_MIN_FREQ = 4689;
			public const int PI_HW_CLK_MAX_FREQ = 250000000;

			public const int PI_NOTIFY_SLOTS  = 32;

			public const int PI_NTFY_FLAGS_EVENT    = (1 <<7);
			public const int PI_NTFY_FLAGS_ALIVE    = (1 <<6);
			public const int PI_NTFY_FLAGS_WDOG     = (1 <<5);
			// public const int PI_NTFY_FLAGS_BIT(= x) (((x)<<0)&31);

			public const int PI_WAVE_BLOCKS     = 4;
			public const int PI_WAVE_MAX_PULSES = (PI_WAVE_BLOCKS * 3000);
			public const int PI_WAVE_MAX_CHARS  = (PI_WAVE_BLOCKS *  300);

			public const int PI_BB_I2C_MIN_BAUD     = 50;
			public const int PI_BB_I2C_MAX_BAUD = 500000;

			public const int PI_BB_SPI_MIN_BAUD     = 50;
			public const int PI_BB_SPI_MAX_BAUD = 250000;

			public const int PI_BB_SER_MIN_BAUD     = 50;
			public const int PI_BB_SER_MAX_BAUD = 250000;

			public const int PI_BB_SER_NORMAL = 0;
			public const int PI_BB_SER_INVERT = 1;

			public const int PI_WAVE_MIN_BAUD      = 50;
			public const int PI_WAVE_MAX_BAUD = 1000000;

			public const int PI_SPI_MIN_BAUD     = 32000;
			public const int PI_SPI_MAX_BAUD = 125000000;

			public const int PI_MIN_WAVE_DATABITS = 1;
			public const int PI_MAX_WAVE_DATABITS = 32;

			public const int PI_MIN_WAVE_HALFSTOPBITS = 2;
			public const int PI_MAX_WAVE_HALFSTOPBITS = 8;

			public const int PI_WAVE_MAX_MICROS = (30 * 60 * 1000000) /* half an hour */;

			public const int PI_MAX_WAVES = 250;

			public const int PI_MAX_WAVE_CYCLES = 65535;
			public const int PI_MAX_WAVE_DELAY  = 65535;

			public const int PI_WAVE_COUNT_PAGES = 10;

			/* wave tx mode */

			public const int PI_WAVE_MODE_ONE_SHOT      = 0;
			public const int PI_WAVE_MODE_REPEAT        = 1;
			public const int PI_WAVE_MODE_ONE_SHOT_SYNC = 2;
			public const int PI_WAVE_MODE_REPEAT_SYNC   = 3;

			/* special wave at return values */

			public const int PI_WAVE_NOT_FOUND  = 9998 /* Transmitted wave not found. */;
			public const int PI_NO_TX_WAVE      = 9999 /* No wave being transmitted. */;

			/* Files, I2C, SPI, SER */

			public const int PI_FILE_SLOTS = 16;
			public const int PI_I2C_SLOTS  = 64;
			public const int PI_SPI_SLOTS  = 32;
			public const int PI_SER_SLOTS  = 16;

			public const int PI_MAX_I2C_ADDR = 0x7F;

			public const int PI_NUM_AUX_SPI_CHANNEL = 3;
			public const int PI_NUM_STD_SPI_CHANNEL = 2;

			public const int PI_MAX_I2C_DEVICE_COUNT = (1<<16);
			public const int PI_MAX_SPI_DEVICE_COUNT = (1<<16);

			/* max pi_i2c_msg_t per transaction */

			public const int PI_I2C_RDRW_IOCTL_MAX_MSGS = 42;

			/* flags for i2cTransaction, pi_i2c_msg_t */

			public const int PI_I2C_M_WR           = 0x0000 /* write data */;
			public const int PI_I2C_M_RD           = 0x0001 /* read data */;
			public const int PI_I2C_M_TEN          = 0x0010 /* ten bit chip address */;
			public const int PI_I2C_M_RECV_LEN     = 0x0400 /* length will be first received byte */;
			public const int PI_I2C_M_NO_RD_ACK    = 0x0800 /* if I2C_FUNC_PROTOCOL_MANGLING */;
			public const int PI_I2C_M_IGNORE_NAK   = 0x1000 /* if I2C_FUNC_PROTOCOL_MANGLING */;
			public const int PI_I2C_M_REV_DIR_ADDR = 0x2000 /* if I2C_FUNC_PROTOCOL_MANGLING */;
			public const int PI_I2C_M_NOSTART      = 0x4000 /* if I2C_FUNC_PROTOCOL_MANGLING */;

			/* bbI2CZip and i2cZip commands */

			public const int PI_I2C_END          = 0;
			public const int PI_I2C_ESC          = 1;
			public const int PI_I2C_START        = 2;
			public const int PI_I2C_COMBINED_ON  = 2;
			public const int PI_I2C_STOP         = 3;
			public const int PI_I2C_COMBINED_OFF = 3;
			public const int PI_I2C_ADDR         = 4;
			public const int PI_I2C_FLAGS        = 5;
			public const int PI_I2C_READ         = 6;
			public const int PI_I2C_WRITE        = 7;

			/* BSC registers */

			public const int BSC_DR         = 0;
			public const int BSC_RSR        = 1;
			public const int BSC_SLV        = 2;
			public const int BSC_CR         = 3;
			public const int BSC_FR         = 4;
			public const int BSC_IFLS       = 5;
			public const int BSC_IMSC       = 6;
			public const int BSC_RIS        = 7;
			public const int BSC_MIS        = 8;
			public const int BSC_ICR        = 9;
			public const int BSC_DMACR     = 10;
			public const int BSC_TDR       = 11;
			public const int BSC_GPUSTAT   = 12;
			public const int BSC_HCTRL     = 13;
			public const int BSC_DEBUG_I2C = 14;
			public const int BSC_DEBUG_SPI = 15;

			public const int BSC_CR_TESTFIFO = 2048;
			public const int BSC_CR_RXE  = 512;
			public const int BSC_CR_TXE  = 256;
			public const int BSC_CR_BRK  = 128;
			public const int BSC_CR_CPOL  = 16;
			public const int BSC_CR_CPHA   = 8;
			public const int BSC_CR_I2C    = 4;
			public const int BSC_CR_SPI    = 2;
			public const int BSC_CR_EN     = 1;

			public const int BSC_FR_RXBUSY = 32;
			public const int BSC_FR_TXFE   = 16;
			public const int BSC_FR_RXFF    = 8;
			public const int BSC_FR_TXFF    = 4;
			public const int BSC_FR_RXFE    = 2;
			public const int BSC_FR_TXBUSY  = 1;

			/* BSC GPIO */

			public const int BSC_SDA_MOSI = 18;
			public const int BSC_SCL_SCLK = 19;
			public const int BSC_MISO     = 20;
			public const int BSC_CE_N     = 21;

			/* Longest busy delay */

			public const int PI_MAX_BUSY_DELAY = 100;

			/* timeout: 0-60000 */

			public const int PI_MIN_WDOG_TIMEOUT = 0;
			public const int PI_MAX_WDOG_TIMEOUT = 60000;

			/* timer: 0-9 */

			public const int PI_MIN_TIMER = 0;
			public const int PI_MAX_TIMER = 9;

			/* millis: 10-60000 */

			public const int PI_MIN_MS = 10;
			public const int PI_MAX_MS = 60000;

			public const int PI_MAX_SCRIPTS       = 32;

			public const int PI_MAX_SCRIPT_TAGS   = 50;
			public const int PI_MAX_SCRIPT_VARS  = 150;
			public const int PI_MAX_SCRIPT_PARAMS = 10;

			/* script status */

			public const int PI_SCRIPT_INITING = 0;
			public const int PI_SCRIPT_HALTED  = 1;
			public const int PI_SCRIPT_RUNNING = 2;
			public const int PI_SCRIPT_WAITING = 3;
			public const int PI_SCRIPT_FAILED  = 4;

			/* signum: 0-63 */

			public const int PI_MIN_SIGNUM = 0;
			public const int PI_MAX_SIGNUM = 63;

			/* timetype: 0-1 */

			public const int PI_TIME_RELATIVE = 0;
			public const int PI_TIME_ABSOLUTE = 1;

			public const int PI_MAX_MICS_DELAY = 1000000 /* 1 second */;
			public const int PI_MAX_MILS_DELAY = 60000   /* 60 seconds */;

			/* cfgMillis */

			public const int PI_BUF_MILLIS_MIN = 100;
			public const int PI_BUF_MILLIS_MAX = 10000;

			/* cfgMicros: 1, 2, 4, 5, 8, or 10 */

			/* cfgPeripheral: 0-1 */

			public const int PI_CLOCK_PWM = 0;
			public const int PI_CLOCK_PCM = 1;

			/* DMA channel: 0-14 */

			public const int PI_MIN_DMA_CHANNEL = 0;
			public const int PI_MAX_DMA_CHANNEL = 14;

			/* port */

			public const int PI_MIN_SOCKET_PORT = 1024;
			public const int PI_MAX_SOCKET_PORT = 32000;


			/* ifFlags: */

			public const int PI_DISABLE_FIFO_IF   = 1;
			public const int PI_DISABLE_SOCK_IF   = 2;
			public const int PI_LOCALHOST_SOCK_IF = 4;

			/* memAllocMode */

			public const int PI_MEM_ALLOC_AUTO    = 0;
			public const int PI_MEM_ALLOC_PAGEMAP = 1;
			public const int PI_MEM_ALLOC_MAILBOX = 2;

			/* filters */

			public const int PI_MAX_STEADY  = 300000;
			public const int PI_MAX_ACTIVE = 1000000;

			/* gpioCfgInternals */

			public const int PI_CFG_DBG_LEVEL         = 0 /* bits 0-3 */;
			public const int PI_CFG_ALERT_FREQ        = 4 /* bits 4-7 */;
			public const int PI_CFG_RT_PRIORITY       = (1<<8);
			public const int PI_CFG_STATS             = (1<<9);

			public const int PI_CFG_ILLEGAL_VAL       = (1<<10);

			/* gpioISR */

			public const int RISING_EDGE  = 0;
			public const int FALLING_EDGE = 1;
			public const int EITHER_EDGE  = 2;


			/* pads */

			public const int PI_MAX_PAD = 2;

			public const int PI_MIN_PAD_STRENGTH = 1;
			public const int PI_MAX_PAD_STRENGTH = 16;

			/* files */

			public const int PI_FILE_NONE   = 0;
			public const int PI_FILE_MIN    = 1;
			public const int PI_FILE_READ   = 1;
			public const int PI_FILE_WRITE  = 2;
			public const int PI_FILE_RW     = 3;
			public const int PI_FILE_APPEND = 4;
			public const int PI_FILE_CREATE = 8;
			public const int PI_FILE_TRUNC  = 16;
			public const int PI_FILE_MAX    = 31;

			public const int PI_FROM_START   = 0;
			public const int PI_FROM_CURRENT = 1;
			public const int PI_FROM_END     = 2;

			/* Allowed socket connect addresses */

			public const int MAX_CONNECT_ADDRESSES = 256;

			/* events */

			public const int PI_MAX_EVENT = 31;

			/* Event auto generated on BSC slave activity */

			public const int PI_EVENT_BSC = 31;
		}
	}
}
