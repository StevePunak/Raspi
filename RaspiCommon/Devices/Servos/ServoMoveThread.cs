using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

namespace RaspiCommon.Devices.Servos
{
	public class ServoMoveThread : ThreadBase
	{
		public ServoParameters this[GpioPin pin]
		{
			get
			{
				ServoParameters servo = null;
				lock(Servos)
				{
					servo = Servos[pin];
				}
				return servo;
			}
		}

		public ServoParameters this[String name]
		{
			get
			{
				ServoParameters servo = null;
				lock(Servos)
				{
					servo = new List<ServoParameters>(Servos.Values).Find(s => s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
				}
				return servo;
			}
		}

		Dictionary<GpioPin, ServoParameters> Servos { get; set; }

		public ServoMoveThread()
			: base(typeof(ServoMoveThread).Name)
		{
			Servos = new Dictionary<GpioPin, ServoParameters>();
			Interval = TimeSpan.FromMilliseconds(100);
		}

		public void MoveToPercent(GpioPin pin, int value)
		{
			ServoParameters servo = null;
			lock(Servos)
			{
				if(Servos.TryGetValue(pin, out servo) == false)
				{
					servo = null;
				}
			}

			if(servo != null)
			{
				servo.SetDestinationPercentage(value);
			}
		}

		public void AddServo(ServoParameters servo)
		{
			lock(Servos)
			{
				Servos.Add(servo.Pin, servo);
				servo.SetDestinationPercentage(50);
				Log.LogText(LogLevel.DEBUG, "ServoController Adding {0}", servo);
			}
		}

		public void Center(GpioPin pin)
		{
			lock(Servos)
			{
				Servos[pin].Center();
			}
		}

		protected override bool OnRun()
		{
			lock(Servos)
			{
				foreach(ServoParameters servo in Servos.Values)
				{
					if(servo.IsIdle == false)
					{
						MoveServo(servo);
					}
				}
			}

			return true;
		}

		protected override bool OnStop()
		{
			lock(Servos)
			{
				foreach(ServoParameters servo in Servos.Values)
				{
					SetIdle(servo);
				}
			}
			return base.OnStop();
		}

		void MoveServo(ServoParameters servo)
		{
			int newSetting = servo.CurrentSetting;
			if(servo.DestinationSetting > servo.CurrentSetting)
				newSetting = Math.Min(Math.Min(servo.CurrentSetting + servo.Quantum, servo.DestinationSetting), servo.Maximum);
			else if(servo.DestinationSetting < servo.CurrentSetting)
				newSetting = Math.Max(Math.Max(servo.CurrentSetting - servo.Quantum, servo.DestinationSetting), servo.Minimum);
			servo.CurrentSetting = newSetting;
			SetServoPosition(servo, newSetting);
		}

		protected virtual void SetServoPosition(ServoParameters servo, int value)
		{
			Log.LogText(LogLevel.DEBUG, "Moving {0}", servo);
		}

		protected virtual void SetIdle(ServoParameters servo)
		{
			Log.LogText(LogLevel.DEBUG, "Setting Idle {0}", servo);
		}

	}
}
