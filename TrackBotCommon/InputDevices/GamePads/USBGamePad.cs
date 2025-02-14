﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Devices.GamePads;
using SharpDX.DirectInput;

namespace TrackBotCommon.InputDevices.GamePads
{
	class USBGamePad : StandardGamePad
	{
		internal USBGamePad()
			: base(typeof(USBGamePad).Name, GamePadType.USB, GamepadConnection.USB)
		{
			States.MapButton(GamePadControl.A, 0);
			States.MapButton(GamePadControl.B, 1);
			States.MapButton(GamePadControl.X, 2);
			States.MapButton(GamePadControl.Y, 3);
			States.MapButton(GamePadControl.LeftIndex, 4);
			States.MapButton(GamePadControl.RightIndex, 5);
			States.MapButton(GamePadControl.LeftStickPressed, 8);
			States.MapButton(GamePadControl.RightStickPressed, 9);

			LeftStick.DeadZoneX = LeftStick.DeadZoneY = RightStick.DeadZoneX = RightStick.DeadZoneY = 500;
		}

		protected override void InterpretStateUSB(JoystickState state)
		{

			CallEvents();

			bool neutral = LeftStick.Neutral && RightStick.Neutral && LeftIndex.Changed == false && RightIndex.Changed == false &&
				X.Changed == false && Y.Changed == false && A.Changed == false && B.Changed == false && LeftArrow.Changed == false && RightArrow.Changed == false && O.Changed == false &&
				RightStickPressed.Changed == false && LeftStickPressed.Changed == false;
			if(LastGamepadNeutral == false || neutral == false)
			{
				CallGamepadEvent();
			}
			LastGamepadNeutral = neutral;
		}

	}
}
