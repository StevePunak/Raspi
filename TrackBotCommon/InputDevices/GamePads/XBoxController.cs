using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using RaspiCommon.Devices.GamePads;
using SharpDX.XInput;
using XState = SharpDX.XInput.State;

namespace TrackBotCommon.InputDevices.GamePads
{
	public class XBoxController : GamePadBase
	{
		static readonly TimeSpan GO_NEUTRAL_TIME = TimeSpan.FromSeconds(1);
		const int STICK_ADJUSTMENT = 32768;

		public Controller XBoxDevice { get; set; }

		XState _currentState;

		DateTime _lastPacketTime;
		DateTime _lastInterpretTime;

		public XBoxController() 
			: base(typeof(XBoxController).Name, GamePadType.XBoxOneController, GamepadConnection.Bluetooth)
		{
			((StickMapping)States[GamePadControl.LeftStick]).Stick.DeadZoneX = 3000;
			((StickMapping)States[GamePadControl.LeftStick]).Stick.DeadZoneY = 3000;
			((StickMapping)States[GamePadControl.LeftStick]).Stick.Reverse = true;

			((StickMapping)States[GamePadControl.RightStick]).Stick.DeadZoneX = 3000;
			((StickMapping)States[GamePadControl.RightStick]).Stick.DeadZoneY = 3000;
			((StickMapping)States[GamePadControl.RightStick]).Stick.Reverse = true;

			((AnalogMapping)States[GamePadControl.LeftTrigger]).AnalogControl.DeadZone = 3000;
			((AnalogMapping)States[GamePadControl.RightTrigger]).AnalogControl.DeadZone = 3000;

			Interval = TimeSpan.FromMilliseconds(15);
		}

		protected override bool OnStart()
		{
			return true;
		}

		private void ConnectController()
		{
			Controller[] controllers = new Controller[] { new Controller(UserIndex.One), new Controller(UserIndex.Two), new Controller(UserIndex.Three), new Controller(UserIndex.Four) };

			// Get 1st controller available
			XBoxDevice = null;
			try
			{
				foreach(Controller selectControler in controllers)
				{
					if(selectControler.IsConnected)
					{
						XBoxDevice = selectControler;
						Connected = true;
						Interval = TimeSpan.FromMilliseconds(50);
						break;
					}
				}
			}
			catch(SharpDX.SharpDXException)
			{
				Connected = false;
				Interval = TimeSpan.FromSeconds(5);
			}
			if(XBoxDevice == null)
			{
				Connected = false;
				Interval = TimeSpan.FromSeconds(5);
			}
		}

		protected override bool OnRun()
		{
			if(Connected)
			{
				// Poll events from joystick
				XState state = XBoxDevice.GetState();
				if(_currentState.PacketNumber != state.PacketNumber || DateTime.UtcNow > _lastInterpretTime)
				{
					if(_currentState.PacketNumber != state.PacketNumber)
					{
						_lastPacketTime = DateTime.UtcNow;
					}
					_currentState = state;
					InterpretState();
					_lastInterpretTime = DateTime.Now;
				}
			}
			else
			{
				ConnectController();
			}
			return true;
		}

		private void InterpretState()
		{
			SetHatState(GamePadControl.Hat,
				(_currentState.Gamepad.Buttons & GamepadButtonFlags.DPadUp) > 0,
				(_currentState.Gamepad.Buttons & GamepadButtonFlags.DPadDown) > 0,
				(_currentState.Gamepad.Buttons & GamepadButtonFlags.DPadLeft) > 0,
				(_currentState.Gamepad.Buttons & GamepadButtonFlags.DPadRight) > 0);

			SetButtonState(GamePadControl.Start, (_currentState.Gamepad.Buttons & GamepadButtonFlags.Start) > 0);
			SetButtonState(GamePadControl.Back, (_currentState.Gamepad.Buttons & GamepadButtonFlags.Back) > 0);
			SetButtonState(GamePadControl.LeftStickPressed, (_currentState.Gamepad.Buttons & GamepadButtonFlags.LeftThumb) > 0);
			SetButtonState(GamePadControl.RightStickPressed, (_currentState.Gamepad.Buttons & GamepadButtonFlags.RightThumb) > 0);
			SetButtonState(GamePadControl.LeftIndex, (_currentState.Gamepad.Buttons & GamepadButtonFlags.LeftShoulder) > 0);
			SetButtonState(GamePadControl.RightIndex, (_currentState.Gamepad.Buttons & GamepadButtonFlags.RightShoulder) > 0);
			SetButtonState(GamePadControl.A, (_currentState.Gamepad.Buttons & GamepadButtonFlags.A) > 0);
			SetButtonState(GamePadControl.B, (_currentState.Gamepad.Buttons & GamepadButtonFlags.B) > 0);
			SetButtonState(GamePadControl.X, (_currentState.Gamepad.Buttons & GamepadButtonFlags.X) > 0);
			SetButtonState(GamePadControl.Y, (_currentState.Gamepad.Buttons & GamepadButtonFlags.Y) > 0);
			SetStickState(GamePadControl.LeftStick, _currentState.Gamepad.LeftThumbX + STICK_ADJUSTMENT, _currentState.Gamepad.LeftThumbY + STICK_ADJUSTMENT);
			SetStickState(GamePadControl.RightStick, _currentState.Gamepad.RightThumbX + STICK_ADJUSTMENT, _currentState.Gamepad.RightThumbY + STICK_ADJUSTMENT);
			SetAnalogState(GamePadControl.LeftTrigger, AdjustTrigger(_currentState.Gamepad.LeftTrigger));
			SetAnalogState(GamePadControl.RightTrigger, AdjustTrigger(_currentState.Gamepad.RightTrigger));

			CallEvents();

			bool neutral = IsNeutral();

			if(LastGamepadNeutral == false || neutral == false)
			{
				CallGamepadEvent();
			}
			LastGamepadNeutral = neutral;
		}

		private int AdjustTrigger(Double value)
		{
			const Double MIN = 0;
			const Double MAX = 255;

			Double adjusted = value * (MAX - MIN);
			return (int)adjusted;
		}

		private void GoNeutral()
		{
			// set analogs neutral
			Log.LogText(LogLevel.DEBUG, "Go neutral");
			if(LeftTrigger.Value > (GamePadTypes.MAX_ANALOG / 2))
			{
				SetAnalogState(GamePadControl.LeftTrigger, GamePadTypes.MAX_ANALOG);
			}
			else if(LeftTrigger.Value < (GamePadTypes.MAX_ANALOG / 2))
			{
				SetAnalogState(GamePadControl.LeftTrigger, 0);
			}
			if(RightTrigger.Value > (GamePadTypes.MAX_ANALOG / 2))
			{
				SetAnalogState(GamePadControl.RightTrigger, GamePadTypes.MAX_ANALOG);
			}
			else if(RightTrigger.Value < (GamePadTypes.MAX_ANALOG / 2))
			{
				SetAnalogState(GamePadControl.RightTrigger, 0);
			}
		}

		public override void Dump()
		{
			Log.LogText(LogLevel.DEBUG, "{0}", _currentState.Gamepad);
		}
	}
}
