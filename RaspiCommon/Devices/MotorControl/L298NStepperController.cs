using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using RaspiCommon.PiGpio;

namespace RaspiCommon.Devices.MotorControl
{
	public class L298NStepperController : ThreadBase
	{
		#region Delegate / Events

		public delegate void StepperMoveCompleteHandler();
		public event StepperMoveCompleteHandler MoveComplete;
		
		#endregion

		#region Constants

		static readonly double DegreesPerFullStep = 1.8;
		static readonly double DegreesPerHalfStep = DegreesPerFullStep / 2;

		#endregion

		#region Public Properties

		public GpioPin Pin1 { get; set; }
		public GpioPin Pin2 { get; set; }
		public GpioPin Pin3 { get; set; }
		public GpioPin Pin4 { get; set; }
		public SpinDirection Direction { get; set; }

		public bool Running { get; set; }
		public bool DebugLogging { get; set; }

		Double _rpm;
		public Double RPM
		{
			get { return _rpm; }
			set { SetRPM(value); }
		}

		StepType _stepType;
		public StepType StepType { get { return _stepType; } set { SetStepType(value); } }

		#endregion

		#region Sequence Tables

		static readonly TwoPhaseSequence FullSteps = new TwoPhaseSequence()
		{
			new StepList() { true,		false,	true,   false },
			new StepList() { false,		true,	true,   false },
			new StepList() { false,		true,	false,  true },
			new StepList() { true,		false,  false,  true },
		};

		// this table is for 17HS13-0404S1 half-steps
		static readonly TwoPhaseSequence HalfSteps = new TwoPhaseSequence()
		{
			new StepList() { true,      false,      true,       false },
			new StepList() { false,     false,      true,       false },
			new StepList() { false,     true,       true,       false },
			new StepList() { false,     true,       false,      false },
			new StepList() { false,     true,       false,      true },
			new StepList() { false,     false,      false,      true },
			new StepList() { true,      false,      false,      true },
			new StepList() { true,      false,      false,      false },
		};


		static readonly Dictionary<StepType, TwoPhaseSequence> SequenceTables = new Dictionary<StepType, TwoPhaseSequence>()
		{
			{ StepType.FullStep,    FullSteps },
			{ StepType.HalfStep,    HalfSteps },
		};

		#endregion

		#region Private Member Variables

		int StepIndex { get; set; }
		int StepCount { get; set; }
		int StepsToRun { get; set; }

		#endregion

		#region Constructors


		public L298NStepperController(String name, GpioPin pin1, GpioPin pin2, GpioPin pin3, GpioPin pin4)
			: base(name)
		{
			Pin1 = pin1;
			Pin2 = pin2;
			Pin3 = pin3;
			Pin4 = pin4;

			StepType = StepType.FullStep;
			StepIndex = 0;

			DebugLogging = false;

			MoveComplete += delegate { };

			StopMotor();
		}

		#endregion

		#region Public Access Methods

		public void TurnDegrees(Double degrees, SpinDirection direction)
		{
			lock(this)
			{
				StepsToRun = (int)(degrees / (StepType == StepType.FullStep ? DegreesPerFullStep : DegreesPerHalfStep));
				Log.SysLogText(LogLevel.DEBUG, $"Says we have to run {StepsToRun} to reach {degrees.ToAngleString()}");
				Direction = direction;
				StepCount = 0;
				Running = true;
				SetRPM(_rpm);
			}
		}

		public void SetRPM(Double rpm)
		{
			lock(this)
			{
				_rpm = rpm;
				Double msPerMinute = TimeSpan.FromMinutes(1).TotalMilliseconds;
				Double stepsPerRevoultion = 360.0 / (StepType == StepType.FullStep ? DegreesPerFullStep : DegreesPerHalfStep);
				Double interval = msPerMinute / stepsPerRevoultion / _rpm;
				Interval = TimeSpan.FromMilliseconds(interval);
				Log.SysLogText(LogLevel.DEBUG, $"Set Interval to {interval} ms at {stepsPerRevoultion} steps/per/rev");
			}
			Unblock();
		}

		public void SetStepType(StepType stepType)
		{
			lock(this)
			{
				_stepType = stepType;
				StepIndex = 0;
			}
		}

		public void StopMotor()
		{
			Log.SysLogText(LogLevel.DEBUG, $"Stopping Motor");
			lock(this)
			{
				Running = false;
				StepsToRun = 0;
				Interval = TimeSpan.FromSeconds(1);
			}
		}

		#endregion

		#region Run

		protected override bool OnRun()
		{
			if(Running)
			{
				lock(this)
				{
					TwoPhaseSequence sequenceTable = SequenceTables[StepType];
					StepList steps = sequenceTable[StepIndex];

					if(DebugLogging)
					{
						Log.SysLogText(LogLevel.DEBUG, $"{this} {Direction} {StepType} Step {StepIndex} Count {StepCount} {steps[0]} {steps[1]} {steps[2]} {steps[3]}");
					}

					Pigs.SetOutputPin(Pin1, steps[0]);
					Pigs.SetOutputPin(Pin2, steps[1]);
					Pigs.SetOutputPin(Pin3, steps[2]);
					Pigs.SetOutputPin(Pin4, steps[3]);

					IncrementIndex();
					StepCount++;

					if(StepCount > StepsToRun)
					{
						StopMotor();
						MoveComplete();
					}
				}
			}
			return true;
		}

		private void IncrementIndex()
		{
			if(Direction == SpinDirection.Clockwise)
			{
				if(++StepIndex >= SequenceTables[StepType].Count)
					StepIndex = 0;
			}
			else
			{
				if(--StepIndex < 0)
					StepIndex = SequenceTables[StepType].Count - 1;
			}
		}

		#endregion
	}
}
