using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Threading;
using SharpDX.DirectInput;
using SharpDX.XInput;
using RaspiCommon.Devices.GamePads;

namespace TrackBotCommon.InputDevices.GamePads
{
	public enum GamepadConnection { USB, Bluetooth, XInput };

	public class SteelSeriesXL : StandardGamePad
	{

		internal SteelSeriesXL(GamepadConnection connectionType)
			: base(typeof(SteelSeriesXL).Name, GamePadType.SteelSeries, connectionType)
		{
		}

		protected override void InterpretStateUSB(JoystickState state)
		{
		}

		protected override void InterpretStateBluetooth(JoystickState state)
		{
		}

		public int ReduceResolution(int value)
		{
			return (value >> 1) << 1;
		}

		public override string ToString()
		{
			return $"{LeftStick} {RightStick} {Hat} LT {LeftTrigger} RT {RightTrigger} LI {LeftIndex} RI {RightIndex} X {X} Y {Y} A {A} B {B}";
		}
	}
}
