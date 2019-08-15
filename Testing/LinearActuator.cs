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
	public class LinearActuator : MicroStepDriver
	{
		#region Delegates and Events

		public delegate void LimitSwitchTriggeredHandler(LimitType limit, bool state);
		public event LimitSwitchTriggeredHandler LimitSwitchTriggered;

		#endregion

		#region Enumerations

		public enum LimitType { Upper, Lower }

		#endregion

		#region Public Properties

		public Double TravelLength { get; set; }
		public Double Pitch { get; set; }

		public GpioPin UpperLimitSwitchPin { get; set; }
		public GpioPin LowerLimitSwitchPin { get; set; }
		public MomentarySwitchMode LimitSwitchMode { get; set; }
		public bool AutoLimit { get; set; }
		public bool LimitedUp { get; private set; }
		public bool LimitedDown { get; private set; }

		public bool FindingLowerLimit { get; private set; }
		public bool FindingUpperLimit { get; private set; }
		public bool HasHome { get; private set; }
		public bool HasTravel { get; private set; }

		public Direction Direction { get { return SpinDirection == SpinDirection.CounterClockwise ? Direction.Down : Direction.Up;  } }

		public Double Position
		{
			get
			{
				return (Rotation / 360) * Pitch;
			}
		}

		#endregion

		#region Private Member Variables

		Double _lowerLimitRotation;
		Double _upperLimitRotation;
		MutexEvent _lowerLimitEvent;
		MutexEvent _upperLimitEvent;

		#endregion

		#region Constructor

		public LinearActuator(
			String name,
			GpioPin pulsePin,
			GpioPin directionPin,
			GpioPin enableOuputPin,
			GpioPin upperLimitSwitchPin,
			GpioPin lowerLimitSwitchPin,
			MomentarySwitchMode limitSwitchMode,
			PulseSetting pulseSetting,
			Double pitch)
			: base(name, pulsePin, directionPin, enableOuputPin, pulseSetting)
		{
			UpperLimitSwitchPin = upperLimitSwitchPin;
			LowerLimitSwitchPin = lowerLimitSwitchPin;
			LimitSwitchMode = limitSwitchMode;
			Pitch = pitch;

			AutoLimit = true;
			LimitedUp = LimitedDown = false;
			FindingLowerLimit = false;
			HasHome = false;
			HasTravel = false;

			_upperLimitEvent = new MutexEvent();
			_lowerLimitEvent = new MutexEvent();

			LimitSwitchTriggered += delegate {};
			FreeDriveComplete += OnLinearActuatorFreeDriveComplete;
		}

		#endregion

		#region Public Access Methods

		public void MoveToLowerLimit()
		{
			FindingLowerLimit = true;
			StartMotor(MotorDirection(Direction.Down));
		}

		public void MoveToUpperLimit()
		{
			FindingUpperLimit = true;
			StartMotor(MotorDirection(Direction.Up));
		}

		public void Move(Double distance, Direction direction)
		{
			if( direction == Direction.Up && LimitedUp ||
				direction == Direction.Down && LimitedDown)
			{
				Log.LogText(LogLevel.INFO, $"Tried to move {direction} while limited");
			}
			else
			{
				Double degrees = 360.0 / (Pitch / distance);
				TurnDegrees(degrees, MotorDirection(direction));
			}
		}

		public bool WaitForUpperLimit(TimeSpan timeout)
		{
			return _upperLimitEvent.Wait(timeout);
		}

		public bool WaitForLowerLimit(TimeSpan timeout)
		{
			return _lowerLimitEvent.Wait(timeout);
		}

		#endregion

		#region Overrides

		protected override void Home()
		{
			HasHome = true;
			base.Home();
		}

		protected override bool OnStart()
		{
			Pigs.StartInputPin(UpperLimitSwitchPin, EdgeType.Both, UpperLimitSwitchCallback);
			Pigs.StartInputPin(LowerLimitSwitchPin, EdgeType.Both, LowerLimitSwitchCallback);
			LimitedDown = PinStateToBool(Pigs.ReadInputPin(LowerLimitSwitchPin));
			LimitedUp = PinStateToBool(Pigs.ReadInputPin(UpperLimitSwitchPin));
			return base.OnStart();
		}

		#endregion

		#region Event Handlers

		private void OnLinearActuatorFreeDriveComplete(SpinDirection direction, double degrees)
		{
			Log.SysLogText(LogLevel.DEBUG, $"Actuator free drive complete {direction} {degrees.ToAngleString(1)}");
		}

		#endregion

		#region Limit Pin Input

		bool EdgeTypeToBool(EdgeType state)
		{
			bool result = LimitSwitchMode == MomentarySwitchMode.NormalClosed
				? state == EdgeType.Falling ? true : false
				: state == EdgeType.Falling ? false : true;
			return result;
		}

		bool PinStateToBool(PinState state)
		{
			bool result = LimitSwitchMode == MomentarySwitchMode.NormalClosed
				? state == PinState.Low ? true : false
				: state == PinState.Low ? false : true;
			return result;
		}

		SpinDirection MotorDirection(Direction direction)
		{
			return direction == Direction.Up ? SpinDirection.Clockwise : SpinDirection.CounterClockwise;
		}

		void LowerLimitSwitchCallback(GpioPin pin, EdgeType edgeType, UInt64 microseconds, UInt16 sequence)
		{
			bool state = EdgeTypeToBool(edgeType);

			if(AutoLimit)
			{
				if(state == true)
				{
					if(Direction == Direction.Down)
					{
						LimitedDown = true;
						StopMotor();
						if(FindingLowerLimit)
						{
							Home();
							_lowerLimitRotation = Rotation;
							_lowerLimitEvent.Set();
						}
					}
				}
				else
				{
					LimitedDown = false;
				}
			}
			LimitSwitchTriggered(LimitType.Lower, state);
		}

		void UpperLimitSwitchCallback(GpioPin pin, EdgeType edgeType, UInt64 microseconds, UInt16 sequence)
		{
			bool state = LimitSwitchMode == MomentarySwitchMode.NormalClosed
				? edgeType == EdgeType.Falling ? true : false
				: edgeType == EdgeType.Falling ? false : true;

			if(AutoLimit)
			{
				if(state == true)
				{
					if(Direction == Direction.Up)
					{
						LimitedUp = true;
						StopMotor();
						if(FindingUpperLimit)
						{
							_upperLimitRotation = Rotation;
							TravelLength = Position;
						}
						_upperLimitEvent.Set();
					}
				}
				else
				{
					LimitedUp = false;
				}
			}
			LimitSwitchTriggered(LimitType.Upper, state);
		}

		#endregion
	}
}
