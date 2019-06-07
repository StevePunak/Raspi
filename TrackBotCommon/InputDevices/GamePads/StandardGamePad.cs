using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaspiCommon.Devices.GamePads;
using SharpDX.DirectInput;

namespace TrackBotCommon.InputDevices.GamePads
{
	public class StandardGamePad : GamePadBase
	{
		public Joystick Device { get; set; }

		bool[] _lastButtons;

		protected StandardGamePad(string name, GamePadType type, GamepadConnection connectionType) 
			: base(name, type, connectionType)
		{
		}

		protected override bool OnStart()
		{
			DirectInput directInput = new DirectInput();
			foreach(DeviceInstance device in directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices))
			{
				Joystick joystick = new Joystick(directInput, device.InstanceGuid);
				if(joystick.Information.Type == DeviceType.Gamepad && joystick.Information.ProductName.IndexOf(MatchString, StringComparison.InvariantCultureIgnoreCase) >= 0)
				{
					Device = joystick;
					Device.Acquire();

					break;
				}
			}

			Interval = TimeSpan.FromMilliseconds(50);

			return base.OnStart();
		}

		protected override bool OnRun()
		{
			Device.Poll();
			JoystickState state = Device.GetCurrentState();
			if(ConnectionType == GamepadConnection.USB)
				InterpretStateUSB(state);
			else
				InterpretStateBluetooth(state);

			if(DumpButtonChanges && _lastButtons != null)
			{
				for(int x = 0;x < state.Buttons.Length;x++)
				{
					if(state.Buttons[x] != _lastButtons[x])
					{
						Console.WriteLine($"Button {x} = {state.Buttons[x]}");
					}
				}
			}
			_lastButtons = state.Buttons;

			return true;
		}

		public override void Dump()
		{
			Console.WriteLine(Device.GetCurrentState());
		}

	}
}
