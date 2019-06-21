using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using KanoopCommon.Reflection;
using KanoopCommon.Threading;
using RaspiCommon.Devices.GamePads;
using SharpDX.DirectInput;
using SharpDX.XInput;
using XState = SharpDX.XInput.State;

namespace TrackBotCommon.InputDevices.GamePads
{
	public abstract class GamePadBase : ThreadBase
	{
		public event StickEventHandler LeftStickChanged;
		public event StickEventHandler RightStickChanged;
		public event HatEventHandler HatChanged;
		public event AnalogInputChangedHandler LeftTriggerChanged;
		public event AnalogInputChangedHandler RightTriggerChanged;
		public event ButtonPressedHandler LeftStickPressedChanged;
		public event ButtonPressedHandler RightStickPressedChanged;
		public event ButtonPressedHandler LeftIndexButtonChanged;
		public event ButtonPressedHandler RightIndexButtonChanged;
		public event ButtonPressedHandler XButtonChanged;
		public event ButtonPressedHandler YButtonChanged;
		public event ButtonPressedHandler AButtonChanged;
		public event ButtonPressedHandler BButtonChanged;
		public event ButtonPressedHandler LeftArrowButtonChanged;
		public event ButtonPressedHandler RightArrowButtonChanged;
		public event ButtonPressedHandler OButtonChanged;
		public event GamepadEventHandler GamepadEvent;

		public Stick LeftStick { get { return ((StickMapping)States.ControlStates[GamePadControl.LeftStick]).Stick; } }
		public Stick RightStick { get { return ((StickMapping)States.ControlStates[GamePadControl.RightStick]).Stick; } }
		public Hat Hat { get { return ((HatMapping)States.ControlStates[GamePadControl.Hat]).Hat; } }

		public Button LeftStickPressed { get { return ((ButtonMapping)States.ControlStates[GamePadControl.LeftStickPressed]).Button; } }
		public Button RightStickPressed { get { return ((ButtonMapping)States.ControlStates[GamePadControl.RightStickPressed]).Button; } }
		public AnalogControl LeftTrigger { get { return ((AnalogMapping)States.ControlStates[GamePadControl.LeftTrigger]).AnalogControl; } }
		public AnalogControl RightTrigger { get { return ((AnalogMapping)States.ControlStates[GamePadControl.RightTrigger]).AnalogControl; } }
		public Button LeftIndex { get { return ((ButtonMapping)States.ControlStates[GamePadControl.LeftIndex]).Button; } }
		public Button RightIndex { get { return ((ButtonMapping)States.ControlStates[GamePadControl.RightIndex]).Button; } }
		public Button X { get { return ((ButtonMapping)States.ControlStates[GamePadControl.X]).Button; } }
		public Button Y { get { return ((ButtonMapping)States.ControlStates[GamePadControl.Y]).Button; } }
		public Button A { get { return ((ButtonMapping)States.ControlStates[GamePadControl.A]).Button; } }
		public Button B { get { return ((ButtonMapping)States.ControlStates[GamePadControl.B]).Button; } }
		public Button LeftArrow { get { return ((ButtonMapping)States.ControlStates[GamePadControl.LeftArrow]).Button; } }
		public Button RightArrow { get { return ((ButtonMapping)States.ControlStates[GamePadControl.RightArrow]).Button; } }
		public Button O { get { return ((ButtonMapping)States.ControlStates[GamePadControl.O]).Button; } }

		public GamepadConnection ConnectionType { get; private set; }
		public bool DumpButtonChanges { get; set; }

		public String MatchString { get; private set; }
		public bool Connected { get; protected set; }


		protected bool LastGamepadNeutral { get; set; }
		protected GamePadType GamePadType { get; private set; }
		protected GamePadStates States { get; private set; }

		protected GamePadBase(String name, GamePadType type, GamepadConnection connectionType)
			: base(name)
		{
			ConnectionType = connectionType;
			GamePadType = type;
			States = new GamePadStates();

			LeftStickChanged += delegate { };
			RightStickChanged += delegate { };
			LeftStickPressedChanged += delegate { };
			RightStickPressedChanged += delegate { };
			HatChanged += delegate { };
			LeftStickChanged += delegate { };
			RightStickChanged += delegate { };
			LeftTriggerChanged += delegate { };
			RightTriggerChanged += delegate { };
			LeftIndexButtonChanged += delegate { };
			RightIndexButtonChanged += delegate { };
			XButtonChanged += delegate { };
			YButtonChanged += delegate { };
			AButtonChanged += delegate { };
			BButtonChanged += delegate { };
			LeftArrowButtonChanged += delegate { };
			RightArrowButtonChanged += delegate { };
			OButtonChanged += delegate { };
			GamepadEvent += delegate { };

			DumpButtonChanges = false;

			InfoKeyAttribute attr = ReflectionUtilities.GetAttribute(typeof(GamePadType), typeof(InfoKeyAttribute), GamePadType) as InfoKeyAttribute;
			MatchString = attr.Value;
		}

		public static GamePadBase Create(GamePadType type)
		{
			GamePadBase pad = null;
			switch(type)
			{
				case GamePadType.XBoxOneController:
					pad = new XBoxController();
					break;

				case GamePadType.ESM:
					pad = new EZSmxGamePad();
					break;

				case GamePadType.USB:
					pad = new USBGamePad();
					break;

				case GamePadType.SteelSeries:
					pad = new SteelSeriesXL(GamepadConnection.USB);
					break;

				case GamePadType.ZD:
					pad = new ZDVGamePad();
					break;
			}
			return pad;
		}

		protected virtual void InterpretStateUSB(JoystickState state)
		{
		}

		protected virtual void InterpretStateBluetooth(JoystickState state)
		{
		}

		protected virtual void InterpretStateXInput(SharpDX.XInput.State state)
		{
		}
		protected void CallEvents()
		{
			if(States.AnyChanged)
			{

			}

			if(States.ControlStates[GamePadControl.LeftStick].Control.Changed)
				LeftStickChanged(this, LeftStick);
			if(States.ControlStates[GamePadControl.RightStick].Control.Changed)
				RightStickChanged(this, RightStick);
			if(States.ControlStates[GamePadControl.LeftTrigger].Control.Changed)
				LeftTriggerChanged(this, LeftTrigger);
			if(States.ControlStates[GamePadControl.RightTrigger].Control.Changed)
				RightTriggerChanged(this, RightTrigger);
			if(States.ControlStates[GamePadControl.Hat].Control.Changed)
				HatChanged(this, Hat);
			if(States.ControlStates[GamePadControl.LeftIndex].Control.Changed)
				LeftIndexButtonChanged(this, LeftIndex);
			if(States.ControlStates[GamePadControl.RightIndex].Control.Changed)
				RightIndexButtonChanged(this, RightIndex);
			if(States.ControlStates[GamePadControl.X].Control.Changed)
				XButtonChanged(this, X);
			if(States.ControlStates[GamePadControl.Y].Control.Changed)
				YButtonChanged(this, Y);
			if(States.ControlStates[GamePadControl.A].Control.Changed)
				AButtonChanged(this, A);
			if(States.ControlStates[GamePadControl.B].Control.Changed)
				BButtonChanged(this, B);
			if(States.ControlStates[GamePadControl.LeftArrow].Control.Changed)
				LeftArrowButtonChanged(this, LeftArrow);
			if(States.ControlStates[GamePadControl.RightArrow].Control.Changed)
				RightArrowButtonChanged(this, RightArrow);
			if(States.ControlStates[GamePadControl.O].Control.Changed)
				OButtonChanged(this, O);
			if(States.ControlStates[GamePadControl.LeftStickPressed].Control.Changed)
				LeftStickPressedChanged(this, LeftStickPressed);
			if(States.ControlStates[GamePadControl.RightStickPressed].Control.Changed)
				RightStickPressedChanged(this, RightStickPressed);
		}

		protected bool IsNeutral()
		{
			bool neutral = LeftStick.Neutral && RightStick.Neutral && LeftIndex.Changed == false && RightIndex.Changed == false &&
				X.Changed == false && Y.Changed == false && A.Changed == false && B.Changed == false && 
				LeftArrow.Changed == false && RightArrow.Changed == false && O.Changed == false &&
				Hat.Neutral &&
				RightStickPressed.Changed == false && LeftStickPressed.Changed == false;
			return neutral;
		}

		protected bool SetButtonState(JoystickState joystickState, GamePadControl controlType)
		{
			ButtonMapping mapping = States.ControlStates[controlType] as ButtonMapping;
			bool currentButtonState = joystickState.Buttons[mapping.ButtonIndex];
			if(currentButtonState != mapping.Button.State || (currentButtonState == true && mapping.Button.Continuous))
			{
				mapping.Button.Changed = true;
				States.AnyChanged = true;
			}
			mapping.Button.State = currentButtonState;
			return currentButtonState;
		}

		protected bool SetButtonState(GamePadControl controlType, bool currentButtonState)
		{
			ButtonMapping mapping = States.ControlStates[controlType] as ButtonMapping;
			if(currentButtonState != mapping.Button.State || (currentButtonState == true && mapping.Button.Continuous))
			{
				mapping.Button.State = currentButtonState;
				mapping.Button.Changed = true;
				States.AnyChanged = true;
			}
			else if(currentButtonState == mapping.Button.State)
			{
				mapping.Button.Changed = false;
			}
			mapping.Button.State = currentButtonState;
			return currentButtonState;
		}

		protected void SetHatState(JoystickState joystickState, GamePadControl controlType)
		{
			HatMapping mapping = States.ControlStates[controlType] as HatMapping;
			bool up = joystickState.PointOfViewControllers[mapping.PointOfViewIndex] == 0;
			bool right = joystickState.PointOfViewControllers[mapping.PointOfViewIndex] == 9000;
			bool down = joystickState.PointOfViewControllers[mapping.PointOfViewIndex] == 18000;
			bool left = joystickState.PointOfViewControllers[mapping.PointOfViewIndex] == 27000;
			if(mapping.Hat.Up.State != up || mapping.Hat.Down.State != down || mapping.Hat.Left.State != left || mapping.Hat.Right.State != right || up != down || left || right)
			{
				mapping.Hat.Changed = true;
				mapping.Hat.Up.State = up;
				mapping.Hat.Down.State = down;
				mapping.Hat.Left.State = left;
				mapping.Hat.Right.State = right;
				States.AnyChanged = true;
			}
		}

		protected void SetHatState(GamePadControl controlType, bool up, bool down, bool left, bool right)
		{
			HatMapping mapping = States.ControlStates[controlType] as HatMapping;
			if(mapping.Hat.Up.State != up || mapping.Hat.Down.State != down || mapping.Hat.Left.State != left || mapping.Hat.Right.State != right || up || down || left || right)
			{
				mapping.Control.Changed = true;
				mapping.Hat.Up.State = up;
				mapping.Hat.Down.State = down;
				mapping.Hat.Left.State = left;
				mapping.Hat.Right.State = right;
				States.AnyChanged = true;
			}
			else
			{
				mapping.Hat.Changed = false;
			}
		}

		protected void SetStickState(GamePadControl controlType, int x, int y)
		{
			StickMapping mapping = States.ControlStates[controlType] as StickMapping;
			bool xdead = mapping.Stick.DeadZoneX != 0 && x.IsBetween(GamePadTypes.NEUTRAL - mapping.Stick.DeadZoneX, GamePadTypes.NEUTRAL + mapping.Stick.DeadZoneX);
			bool ydead = mapping.Stick.DeadZoneY != 0 && y.IsBetween(GamePadTypes.NEUTRAL - mapping.Stick.DeadZoneY, GamePadTypes.NEUTRAL + mapping.Stick.DeadZoneY);

			if(xdead) x = GamePadTypes.NEUTRAL;
			if(ydead) y = GamePadTypes.NEUTRAL;

			if(mapping.Stick.X != x || mapping.Stick.Y != y)
			{
				if(mapping.Stick.Reverse)
				{
					x = Math.Abs(x - GamePadTypes.MAX_STICK);
					y = Math.Abs(y - GamePadTypes.MAX_STICK);
				}
				mapping.Stick.X = x;
				mapping.Stick.Y = y;
				mapping.Control.Changed = true;
				States.AnyChanged = true;
			}
		}

		protected void SetAnalogState(GamePadControl controlType, int value)
		{
			AnalogMapping mapping = States.ControlStates[controlType] as AnalogMapping;
			if(mapping.AnalogControl.Value != value)
			{
				if(mapping.AnalogControl.DeadZone != 0)
				{
					if(value > GamePadTypes.MAX_ANALOG - mapping.AnalogControl.DeadZone)
					{
						value = GamePadTypes.MAX_ANALOG;
					}
					else if(value < mapping.AnalogControl.DeadZone)
					{
						value = 0;
					}
				}
				mapping.AnalogControl.Value = value;
				mapping.AnalogControl.Changed = true;
				States.AnyChanged = true;
			}
			else
			{
				mapping.AnalogControl.Changed = false;
			}
		}

		protected void CallGamepadEvent()
		{
			GamepadEvent(this);
		}

		public virtual void Dump()
		{
		}

		public void DumpStates()
		{
			States.DumpStates();
		}

		protected class ControlTypeAttribute : Attribute
		{
			public Type ControlType { get; set; }
			public ControlTypeAttribute(Type type)
			{
				ControlType = type;
			}
		}

		protected enum GamePadControl
		{
			[ControlType(typeof(StickMapping))]
			LeftStick,
			[ControlType(typeof(StickMapping))]
			RightStick,
			[ControlType(typeof(ButtonMapping))]
			LeftStickPressed,
			[ControlType(typeof(ButtonMapping))]
			RightStickPressed,
			[ControlType(typeof(AnalogMapping))]
			LeftTrigger,
			[ControlType(typeof(AnalogMapping))]
			RightTrigger,
			[ControlType(typeof(HatMapping))]
			Hat,
			[ControlType(typeof(ButtonMapping))]
			LeftIndex,
			[ControlType(typeof(ButtonMapping))]
			RightIndex,
			[ControlType(typeof(ButtonMapping))]
			X,
			[ControlType(typeof(ButtonMapping))]
			Y,
			[ControlType(typeof(ButtonMapping))]
			A,
			[ControlType(typeof(ButtonMapping))]
			B,
			[ControlType(typeof(ButtonMapping))]
			LeftArrow,
			[ControlType(typeof(ButtonMapping))]
			RightArrow,
			[ControlType(typeof(ButtonMapping))]
			O,
			[ControlType(typeof(ButtonMapping))]
			Start,
			[ControlType(typeof(ButtonMapping))]
			Back,
		}

		protected abstract class GameControlMapping
		{
			public GamePadControl ControlType { get; set; }
			public abstract ControlBase Control { get; }

			public GameControlMapping(GamePadControl type)
			{
				ControlType = type;
			}
		}

		protected class ButtonMapping : GameControlMapping
		{
			public int ButtonIndex { get; set; }
			public Button Button { get; set; }
			public override ControlBase Control { get { return Button; } }

			public ButtonMapping(GamePadControl type)
				: base(type)
			{
				Button = new Button() { Name = type.ToString() };
			}

			public override string ToString()
			{
				return $"{ControlType} {Button}";
			}
		}

		protected class AnalogMapping : GameControlMapping
		{
			public int ButtonIndex { get; set; }
			public AnalogControl AnalogControl { get; set; }
			public override ControlBase Control { get { return AnalogControl; } }

			public AnalogMapping(GamePadControl type)
				: base(type)
			{
				AnalogControl = new AnalogControl() { Name = type.ToString() };
			}

			public override string ToString()
			{
				return $"{ControlType} {AnalogControl}";
			}
		}

		protected class HatMapping : GameControlMapping
		{
			public int PointOfViewIndex { get; set; }
			public Hat Hat { get; set; }
			public override ControlBase Control { get { return Hat; } }

			public HatMapping(GamePadControl type)
				: base(type)
			{
				Hat = new Hat();
			}

			public override string ToString()
			{
				return $"{ControlType} {Hat}";
			}
		}

		protected class StickMapping : GameControlMapping
		{
			public int ButtonIndex { get; set; }
			public Stick Stick { get; set; }
			public override ControlBase Control { get { return Stick; } }

			public StickMapping(GamePadControl type)
				: base(type)
			{
				Stick = new Stick();
			}

			public override string ToString()
			{
				return $"{ControlType} {Stick}";
			}
		}

		protected class GamePadStates
		{
			public Dictionary<GamePadControl, GameControlMapping> ControlStates { get; private set; }
			public bool AnyChanged { get; set; }

			public GameControlMapping this[GamePadControl controlType]
			{
				get { return ControlStates[controlType];  }
			}
			
			public GamePadStates()
			{
				ControlStates = new Dictionary<GamePadControl, GameControlMapping>();

				foreach(GamePadControl item in Enum.GetValues(typeof(GamePadControl)))
				{
					MemberInfo member = typeof(GamePadControl).GetMember(item.ToString())[0];
					ControlTypeAttribute attr = member.GetCustomAttribute<ControlTypeAttribute>();
					if(attr != null)
					{
						if(attr.ControlType == typeof(ButtonMapping))
							ControlStates.Add(item, new ButtonMapping(item));
						if(attr.ControlType == typeof(AnalogMapping))
							ControlStates.Add(item, new AnalogMapping(item));
						if(attr.ControlType == typeof(HatMapping))
							ControlStates.Add(item, new HatMapping(item));
						if(attr.ControlType == typeof(StickMapping))
							ControlStates.Add(item, new StickMapping(item));

					}
				}
			}

			public void DumpStates()
			{
				StringBuilder sb = new StringBuilder();
				foreach(GameControlMapping mapping in ControlStates.Values)
				{
					sb.AppendFormat("{0} ", mapping);
				}
				Log.SysLogText(LogLevel.DEBUG, "{0}", sb);
			}

			public void ClearChanged()
			{
				foreach(GameControlMapping mapping in ControlStates.Values)
				{
					mapping.Control.Changed = false;
				}
				AnyChanged = false;
			}

			public void MapButton(GamePadControl control, int index)
			{
				((ButtonMapping)ControlStates[control]).ButtonIndex = index;
			}

			public void MapAnalog(GamePadControl control, int index)
			{
			}
		}
	}
}
