using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Threading;
using SharpDX.DirectInput;
using RaspiCommon.Devices.GamePads;
using KanoopCommon.Logging;

namespace TrackBotCommon.InputDevices.GamePads
{
	public class EZSmxGamePad : StandardGamePad
	{
		internal EZSmxGamePad()
			: base(typeof(EZSmxGamePad).Name, GamePadType.ESM, GamepadConnection.Bluetooth)
		{
			States.MapButton(GamePadControl.A, 0);
			States.MapButton(GamePadControl.B, 1);
			States.MapButton(GamePadControl.X, 2);
			States.MapButton(GamePadControl.Y, 3);
			States.MapButton(GamePadControl.LeftIndex, 4);
			States.MapButton(GamePadControl.RightIndex, 5);
			States.MapButton(GamePadControl.LeftStickPressed, 10);
			States.MapButton(GamePadControl.RightStickPressed, 11);

			LeftStick.DeadZoneX = LeftStick.DeadZoneY = RightStick.DeadZoneX = RightStick.DeadZoneY = 500;
		}

		protected override void InterpretStateBluetooth(JoystickState state)
		{
			Log.SysLogText(LogLevel.DEBUG, "{0}", state);
			States.ClearChanged();

			SetStickState(GamePadControl.LeftStick, state.X, state.Y);
			SetStickState(GamePadControl.RightStick, state.Z, state.RotationZ);
			SetButtonState(state, GamePadControl.RightStickPressed);
			SetButtonState(state, GamePadControl.LeftStickPressed);
			SetHatState(state, GamePadControl.Hat);

			SetButtonState(state, GamePadControl.LeftIndex);
			SetButtonState(state, GamePadControl.RightIndex);
			SetButtonState(state, GamePadControl.A);
			SetButtonState(state, GamePadControl.B);
			SetButtonState(state, GamePadControl.X);
			SetButtonState(state, GamePadControl.Y);
			SetAnalogState(GamePadControl.LeftTrigger, state.AccelerationX);
			SetAnalogState(GamePadControl.RightTrigger, state.AccelerationY);

			CallEvents();

			bool neutral = LeftStick.Neutral && RightStick.Neutral && LeftIndex.Changed == false && RightIndex.Changed == false &&
				X.Changed == false && Y.Changed == false && A.Changed == false && B.Changed == false && LeftArrow.Changed == false && RightArrow.Changed == false && O.Changed == false &&
				Hat.Neutral &&
				RightStickPressed.Changed == false && LeftStickPressed.Changed == false;

			if(LastGamepadNeutral == false || neutral == false)
			{
				CallGamepadEvent();
			}
			LastGamepadNeutral = neutral;

		}

	}
}
