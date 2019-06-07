using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using RaspiCommon.Devices.GamePads;

namespace TrackBotCommon.InputDevices.GamePads
{
	public delegate void HatEventHandler(GamePadBase gamePad, Hat hat);
	public delegate void StickEventHandler(GamePadBase gamePad, Stick stick);
	public delegate void ButtonPressedHandler(GamePadBase gamePad, Button state);
	public delegate void AnalogInputChangedHandler(GamePadBase gamePad, AnalogControl value);
	public delegate void GamepadEventHandler(GamePadBase gamePad);
}
