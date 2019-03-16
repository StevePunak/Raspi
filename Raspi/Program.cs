using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspberrySharp;
using RaspberrySharp.IO.GeneralPurpose;
using RaspberrySharp.IO.PulseWideModulation;
using RaspberrySharp.System.Timers;
using RaspiCommon;
using System.IO;
using System.Runtime.InteropServices;

namespace Raspi
{
	class Program
	{
		[DllImport("libgpiosharp.so")] //, CallingConvention = CallingConvention.StdCall)]
		public static extern int RegisterDeviceCallback(IntPtr cb, GpioPin gpioPin, EdgeType edgeType, InputSetting input);

		[DllImport("libgpiosharp.so")]
		public static extern void SetOutputPin(GpioPin pin, PinState state);

		[DllImport("libgpiosharp.so")]
		public static extern void PulsePin(GpioPin pin, UInt32 microseconds);

		static Log _log;

		public static void OnPin6Edge(GpioPin pin, EdgeType edgeType, UInt64 nanoseconds)
		{
			Console.WriteLine("in the pin 6 callback {0}, {1} {2}", pin, edgeType, nanoseconds);
		}

		static void Main(string[] args)
		{
			try
			{
				_log = new Log();
				_log.Open(LogLevel.ALWAYS, OpenFlags.COMBO_VERBOSE | OpenFlags.OUTPUT_TO_CONSOLE);
				Log.SystemLog = _log;
#if false
				L298N_DC_TankTracks wheels = new L298N_DC_TankTracks(GpioPin.Pin04, GpioPin.Pin17, GpioPin.Pin16, GpioPin.Pin27, GpioPin.Pin22, GpioPin.Pin20);
				wheels.Ratio = .96;
				wheels.Speed = 200;

				Timer.Sleep(1000);
				wheels.Speed = 0;

				return;
#endif
				TestSensor();
				return;

				Pigs.SetMode(GpioPin.Pin04, PinMode.Output);
				Pigs.SetMode(GpioPin.Pin17, PinMode.Output);
				Pigs.SetMode(GpioPin.Pin16, PinMode.Output);

				Pigs.SetPWM(GpioPin.Pin16, 200);
				Timer.Sleep(2000);
				Pigs.SetPWM(GpioPin.Pin16, 0);



				return;

				Servo servo = new Servo(GpioPin.Pin12);
				for(UInt32 x = 600;x < 2400;x++)
				{
					servo.Value = x;
				}
				return;
#if zero
				L298_DC_Control l298 = new L298_DC_Control(GpioPin.Pin04, GpioPin.Pin17, GpioPin.Pin16);
				l298.Start();
				l298.Direction = Direction.Forward;

				while(true)
				{
					String cmd = Console.ReadLine().Trim();
					if(cmd[0] == 'g')
					{

					}
					else if(cmd[0] == 's')
					{

					}
					else if(Char.IsDigit(cmd[0]))
					{
						Console.WriteLine("Proc: {0}", cmd[0]);
						if(cmd[0] == 'l')
						{
							l298.SpeedPWM.DutyCycle = 25;
						}
						else if(cmd[0] == 'm')
						{
							l298.SpeedPWM.DutyCycle = 50;
						}
						else if(cmd[0] == 'h')
						{
							l298.SpeedPWM.DutyCycle = 75;
						}
					}
				}

				l298.Speed = 800;
				Console.WriteLine("Speed {0}", l298.Speed);
				Timer.Sleep(TimeSpan.FromSeconds(3));

				l298.Speed = 1500;
				Console.WriteLine("Speed {0}", l298.Speed);
				Timer.Sleep(TimeSpan.FromSeconds(3));

				l298.Speed = 2000;
				Console.WriteLine("Speed {0}", l298.Speed);
				Timer.Sleep(TimeSpan.FromSeconds(3));

				Console.WriteLine("Speed {0}", l298.Speed);
				l298.Stop();

				//IntPtr monitor6PinFp = Marshal.GetFunctionPointerForDelegate(new GPIOInputCallback (Program.OnPin6Edge));
				//RegisterDeviceCallback(monitor6PinFp, GpioPin.Pin26, EdgeType.Falling | EdgeType.Rising, InputSetting.OpenDrain);

				//TestSensor();
				//while(true)
				//{
				//	PulsePin(GpioPin.Pin06, 10);
				//	Timer.Sleep(TimeSpan.FromMilliseconds(250));
				//}

				//addViaCallback(3, 6);

				// TestSysFS();
#endif
			}
			catch(Exception e)
			{
				Console.WriteLine("ERROR! {0}: {1}", e, e.Message);
			}
			finally
			{
				GpioSharp.DeInit();
			}

			// TestLedBlinking();
			//TestPWM2();
		}


		static void TestESC()
		{
			ESC esc = new ESC(GpioPin.Pin25);
			Pigs.SetServoPosition(GpioPin.Pin25, 600);
			Timer.Sleep(2000);
			Pigs.SetServoPosition(GpioPin.Pin25, 2200);
			Timer.Sleep(2000);
			Pigs.SetServoPosition(GpioPin.Pin25, 800);

			Timer.Sleep(5000);
		}

		static void TestSysFS()
		{
			int pin = 12;
			try
			{
				String exportFile = "/sys/class/gpio/export";
				File.WriteAllText(exportFile, pin.ToString());

				String directionFile = String.Format("/sys/class/gpio/gpio{0}/direction", pin);
				File.WriteAllText(directionFile, "in");

				Console.WriteLine("waiting...");
				Timer.Sleep(TimeSpan.FromMinutes(5));
				//String readFile = String.Format("/sys/class/gpio/gpio{0}/value", pin);

				//FileStream infil = File.OpenRead(readFile);
				//int bytesRead = 0;
				//byte[] inBuffer = new byte[1024];
				//while((bytesRead = infil.Read(inBuffer, 0, inBuffer.Length)) >= 0)
				//{
				//	String readString = ASCIIEncoding.UTF8.GetString(inBuffer, 0, bytesRead);
				//	Console.WriteLine("Read {0} '{1}'", bytesRead, readString);
				//}
			}
			catch(IOException e)
			{
				Console.WriteLine("IERROR {0}: {1}", e, e.Message);
			}
			finally
			{
				String unexportFile = "/sys/class/gpio/unexport";
				File.WriteAllText(unexportFile, pin.ToString());

			}


		}

		static void ISRCallback()
		{
			Console.WriteLine("Pin Activated...");
		}

		private static void TestSensor()
		{
			HCSR04_RangeFinder rangeFinder = new HCSR04_RangeFinder(GpioPin.Pin21, GpioPin.Pin20);
			rangeFinder.Start();

			while(true)
			{
				Console.WriteLine("Range: {0:0.000}", rangeFinder.Range);
				RaspberrySharp.System.Timers.Timer.Sleep(TimeSpan.FromSeconds(1));
			}

			

#if zero
			InputPinConfiguration pinA = new InputPinConfiguration(ProcessorPin.Gpio20).PullUp();
			GpioConnection pinAConnection = new GpioConnection(pinA);
			pinAConnection.PinStatusChanged += OnPinAConnection_PinStatusChanged;

			GpioConnection outputConnection = new GpioConnection(ProcessorPin.Gpio21.Output());
			for(int i = 0;i < 100;i++)
			{
				outputConnection.Pins.First().Enabled = true;
				Timer.Sleep(TimeSpan.FromMilliseconds(1000));

				outputConnection.Pins.First().Enabled = false;
				Timer.Sleep(TimeSpan.FromMilliseconds(1000));
			}
#endif
		}

		private static void OnPinAConnection_PinStatusChanged(object sender, PinStatusEventArgs e)
		{
			Log.SysLogText(LogLevel.DEBUG, "Status changed: {0}", e.Enabled);
		}

		static void TestServo2()
		{
			Console.WriteLine("Socket open");

			PigCommand command = new PigCommand(PigCommand.CommandType.SERVO, 25, 600);
			UInt32 value;
			for(int x = 0;x < 4;x++)
			{
				for(value = 600;value < 2500;value++)
				{
					command.Parameter2 = value;
					Pigs.SendCommand(command);
					Timer.Sleep(1);
				}

				for(;value > 600;value--)
				{
					command.Parameter2 = value;
					Pigs.SendCommand(command);
					Timer.Sleep(1);
				}
			}
			Console.WriteLine("Socket closed");
		}

		static void TestUno()
		{
		}

		public static void TestLedBlinking()
		{
			// Here we create a variable to address a specific pin for output
			// There are two different ways of numbering pins--the physical numbering, and the CPU number
			// "P1Pinxx" refers to the physical numbering, and ranges from P1Pin01-P1Pin40
//			Console.WriteLine("Print name:' {0}'  '{1}'", led1.Name, led1);

			// Here we create a connection to the pin we instantiated above
			GpioConnection connection = new GpioConnection(ProcessorPin.Gpio16.Output());

			for(int i = 0;i < 100;i++)
			{
				connection.Pins.First().Enabled = true;
				Timer.Sleep(TimeSpan.FromMilliseconds(20));
				connection.Pins.First().Enabled = false;
				Timer.Sleep(TimeSpan.FromMilliseconds(100));
			}

			connection.Close();
		}

		public static void TestPWM1()
		{
			// Here we create a connection to the pin we instantiated above
			GpioConnection connection = new GpioConnection(ProcessorPin.Gpio17.Output());
			for(int i = 0;i < 100;i++)
			{
				connection.Pins.First().Enabled = true;
				System.Threading.Thread.Sleep(250);
				connection.Pins.First().Enabled = false;
				System.Threading.Thread.Sleep(250);
			}

			connection.Close();
		}

	}
}
