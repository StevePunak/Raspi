using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using RaspiCommon;
using RaspiCommon.PiGpio;

namespace Testing
{
	public class MicroStepDriver : ThreadBase
	{
		#region Constants

		Double MAGIC_NUMBER = 80000000;                             // got to figure out why this comes out to this

		public const Double RevDegrees = 1.8;
		static readonly UInt32 PWMDutyCycle = 500000;               // 50% Duty Cycle... Doesn't really matter. The frequency is important
		static readonly Double MaxFrequency = PWMDutyCycle * .6;    // Max Frequency (speed)

		#endregion

		#region Delegates and Events

		public delegate void FreeDriveCompleteHandler(SpinDirection direction, Double degrees);
		public event FreeDriveCompleteHandler FreeDriveComplete;

		#endregion

		#region Enumerations

		public enum PulseSetting
		{
			P400 = 400,
			P800 = 800,
			P1000 = 1000,
			P1600 = 1600,
			P2000 = 2000,
			P3200 = 3200,
			P5000 = 5000,
			P6400 = 6400,
			P10000 = 10000,
			P12800 = 12800,
			P25000 = 25000,
			P25600 = 25600,
			P50000 = 50000,
			P51200 = 51200,
		}

		private enum DriverStates
		{
			Idle,
			DriveHardDistance,
			StartFreeDrive,
			FreeDriving,
		}

		#endregion

		#region Public Properties

		public GpioPin PulsePin { get; set; }
		public GpioPin DirectionPin { get; set; }
		public GpioPin EnablePin { get; set; }

		public bool DebugLogging { get; set;}

		public Double Rotation { get; protected set; }

		Double _speed;
		public Double Speed
		{
			get { return _speed * 100; }
			set
			{
				if(value < 0 || value > 100)
				{
					throw new ArgumentException("Invalid motor speed. Must be between 0 and 100%");
				}
				_speed = value / 100;
				_frequency = FrequencyFromSpeed(_speed);
			}
		}

		SpinDirection _direction;
		public SpinDirection SpinDirection
		{
			get { return _direction; }
			set
			{
				_direction = value;
				Pigs.SetOutputPin(DirectionPin, _direction == SpinDirection.CounterClockwise ? PinState.Low : PinState.High);
			}
		}

		bool _enabled;
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				Pigs.SetOutputPin(EnablePin, _enabled);
				if(!_enabled)
				{
					Pigs.SetHardwarePWM(PulsePin, 0, PWMDutyCycle);
				}
			}
		}

		PulseSetting _pulsePerRev;
		public PulseSetting PulsePerRev
		{
			get { return _pulsePerRev; }
			set
			{
				if(Enum.IsDefined(typeof(PulseSetting), value) == false)
				{
					throw new ArgumentException("Invalid Pulse Setting");
				}
				_pulsePerRev = value;
			}
		}

		Double _frequency;
		public Double Frequency
		{
			get { return _frequency; }
			set
			{
				_frequency = value;
			}
		}

		#endregion

		#region Private Member Variables

		DriverStates DriverState { get; set; }
		UInt32 _driveMicroseconds;
		DateTime _startFreeMoveTime;
		Double _startFreeDriveRotation;

		#endregion

		#region Constructor

		public MicroStepDriver(String name, GpioPin pulsePin, GpioPin directionPin, GpioPin enablePin, PulseSetting pulseSetting)
			: base(name)
		{
			PulsePin = pulsePin; Pigs.SetMode(PulsePin, PinMode.Output);
			DirectionPin = directionPin; Pigs.SetMode(DirectionPin, PinMode.Output);
			EnablePin = enablePin; Pigs.SetMode(EnablePin, PinMode.Output);

			PulsePerRev = pulseSetting;

			Speed = 10;

			if(Pigs.IsHardwarePWM(PulsePin) == false)
			{
				throw new RaspiException($"Invalid hardware PWM pin {PulsePin}");
			}

			Enabled = false;
			SpinDirection = SpinDirection.Clockwise;

			DriverState = DriverStates.Idle;
			FreeDriveComplete += delegate {};

			Home();

			Interval = TimeSpan.FromSeconds(1);
		}

		#endregion

		#region Public Access Methods

		/// <summary>
		/// Blocking turn for amount of time
		/// </summary>
		/// <param name="degrees"></param>
		/// <param name="direction"></param>
		public void TurnHardDegrees(Double degrees, SpinDirection direction)
		{
			_driveMicroseconds = MicrosecondsForDegrees(degrees);
			DriverState = DriverStates.DriveHardDistance;
			SpinDirection = direction;
			Unblock();
		}

		public void TurnDegrees(Double degrees, SpinDirection direction)
		{
			_driveMicroseconds = MicrosecondsForDegrees(degrees);
			DriverState = DriverStates.StartFreeDrive;
			SpinDirection = direction;
			Unblock();
		}

		public void StartMotor(SpinDirection direction)
		{
			_driveMicroseconds = 0;		// no limit
			DriverState = DriverStates.StartFreeDrive;
			SpinDirection = direction;
			Unblock();
		}

		public void StopMotor()
		{
			Pigs.SetHardwarePWM((GpioPin)PulsePin, (UInt32)0, PWMDutyCycle);
			DriverState = DriverStates.Idle;
		}

		#endregion

		#region Protected Methods

		protected virtual void Home()
		{
			Rotation = 0;
		}

		#endregion

		#region Thread

		protected override bool OnRun()
		{
			switch(DriverState)
			{
				case DriverStates.Idle:
					break;

				// this will become obsolete since there is no way to track position
				case DriverStates.DriveHardDistance:
					SpinDirection startDirection = _direction;
					Pigs.SetHardwarePWM((GpioPin)PulsePin, (UInt32)_frequency, PWMDutyCycle);
					Enabled = true;
					Pigs.Delay(_driveMicroseconds);
					Pigs.SetHardwarePWM((GpioPin)PulsePin, (UInt32)0, PWMDutyCycle);
					DriverState = DriverStates.Idle;
					Rotation = GetAdjustedRotation(Rotation, DegreesForMicroseconds(_driveMicroseconds), _direction);
					if(startDirection == SpinDirection.Clockwise)
						Rotation += DegreesForMicroseconds(_driveMicroseconds);
					else
						Rotation -= DegreesForMicroseconds(_driveMicroseconds);
					break;

				case DriverStates.StartFreeDrive:
					_startFreeMoveTime = DateTime.UtcNow;
					_startFreeDriveRotation = Rotation;
					Pigs.SetHardwarePWM((GpioPin)PulsePin, (UInt32)_frequency, PWMDutyCycle);
					Enabled = true;
					DriverState = DriverStates.FreeDriving;
					Interval = TimeSpan.FromMilliseconds(1);
					break;

				case DriverStates.FreeDriving:
					Double microseconds = (DateTime.UtcNow - _startFreeMoveTime).TotalMicroseconds();
					Double degrees = DegreesForMicroseconds(microseconds);
					Rotation = GetAdjustedRotation(_startFreeDriveRotation, degrees, _direction);
					if(_driveMicroseconds != 0 && microseconds >= _driveMicroseconds)
					{
						Pigs.SetHardwarePWM((GpioPin)PulsePin, (UInt32)0, PWMDutyCycle);        // stop motor
						FreeDriveComplete(_direction, Rotation);
						DriverState = DriverStates.Idle;
					}
					break;

				default:
					break;
			}
			return true;
		}

		#endregion

		#region Utility

		Double GetAdjustedRotation(Double start, Double degrees, SpinDirection direction)
		{
			Double rotation = _direction == SpinDirection.CounterClockwise ? start - degrees : start + degrees;
			return rotation;
		}

		/// <summary>
		/// Calculate frequency as a function of Max Frequency as a percentatge (0.00 - 1.00)
		/// </summary>
		/// <param name="speedAsPercentage"></param>
		/// <returns></returns>
		private Double FrequencyFromSpeed(Double speedAsPercentage)
		{
			return MaxFrequency * speedAsPercentage;
		}

		/// <summary>
		/// Return the time needed to turn n degrees in microseconds
		/// </summary>
		/// <param name="degrees"></param>
		/// <returns></returns>
		protected UInt32 MicrosecondsForDegrees(Double degrees)
		{
			Double stepsPerFullRevolution = MAGIC_NUMBER / (Double)PulsePerRev;             // pulses to turn 360.0°
			Double revolutionsToTurn = degrees / 360.0;
			Double stepsToRun = stepsPerFullRevolution * revolutionsToTurn;

			Double microseconds = (stepsToRun / _frequency) * 1000000;

			Log.MaybeLog(DebugLogging, LogLevel.DEBUG, $"Will turn {revolutionsToTurn} full revolutions in {stepsToRun} steps taking {microseconds}us");

			return (UInt32)microseconds;
		}

		/// <summary>
		/// Return the number of degrees moved in n microseconds
		/// </summary>
		/// <param name="microseconds"></param>
		/// <returns></returns>
		protected Double DegreesForMicroseconds(Double microseconds)
		{
			Double stepsPerFullRevolution = MAGIC_NUMBER / (Double)PulsePerRev;					// pulses to turn 360.0°
			Double usecsPerFullRevolution = stepsPerFullRevolution / _frequency * 1000000;      // how long to turn 360° at this speed

			Double ratio = microseconds / usecsPerFullRevolution;
			Double degrees = 360 * ratio;
			Double revolutionsToTurn = degrees / 360.0;
			return degrees;
		}

		#endregion

		#region Pigs Delay Thread



		#endregion
	}
}