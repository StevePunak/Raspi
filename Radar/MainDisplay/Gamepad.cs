using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon.Devices.GamePads;
using TrackBotCommon.InputDevices.GamePads;

namespace Radar.MainDisplay
{
	public partial class RadarForm
	{
		private void OnJoystickSpeedChanged(int left, int right)
		{
			Log.SysLogText(LogLevel.DEBUG, "Left {0} Right: {1}", left, right);
			//if(_mqqtController != null && _mqqtController.Client != null && _mqqtController.Client.Connected)
			//{
			//	Log.SysLogText(LogLevel.DEBUG, "Left {0} Right: {1}", left, right);
			//	_mqqtController.SendSpeed(left, right);
			//}
		}

		private void OnGamepadEvent(GamePadBase gamePad)
		{
			const int CLAW_ZONE = 3000;

			if(gamePad.LeftStick.Press == false && gamePad.RightStick.Press == false)
			{
				Double leftSpeed = (int)(((Double)gamePad.LeftStick.Y / 65535 * 200 - 100) * -1);
				Double rightSpeed = (int)(((Double)gamePad.RightStick.Y / 65535 * 200 - 100) * -1);

				if(gamePad.RightStick.X > GamePadTypes.MAX_ANALOG - CLAW_ZONE)
				{
					_clawControl.SetClaw(100);
				}
				else if(gamePad.RightStick.X < CLAW_ZONE)
				{
					_clawControl.SetClaw(0);
				}
				//Log.SysLogText(LogLevel.DEBUG, "Would set L: {0}  R: {1}", leftSpeed, rightSpeed);
				_mqqtController.SendSpeed((int)leftSpeed, (int)rightSpeed);
			}

		}

		private void OnGamepadLeftStickPressedChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button state)
		{
			_clawControl.Rotation = 50;
		}

		private void OnGamepadRightStickPressedChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button state)
		{
		}

		const int CLAW_CONTROL_QUANTUM = 2;
		const int PAN_TILT_CONTROL_QUANTUM = 2;
		private void OnGamepadXButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			if(button.State)
				_mqqtController.SendSpinStepLeftTime(TimeSpan.FromMilliseconds(50));
		}

		private void OnGamepadBButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			if(button.State)
				_mqqtController.SendSpinStepRightTime(TimeSpan.FromMilliseconds(50));
		}

		private void OnGamepadLeftIndexButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			if(button.State)
				_clawControl.ChangeRotation(-CLAW_CONTROL_QUANTUM);
		}

		private void OnGamepadRightIndexButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			if(button.State)
				_clawControl.ChangeRotation(CLAW_CONTROL_QUANTUM);
		}

		private void OnGampadHatChanged(GamePadBase gamePad, Hat hat)
		{
			if(hat.Up.State)
				_panTilt.TiltDown(PAN_TILT_CONTROL_QUANTUM);
			if(hat.Down.State)
				_panTilt.TiltUp(PAN_TILT_CONTROL_QUANTUM);
			if(hat.Right.State)
				_panTilt.PanUp(PAN_TILT_CONTROL_QUANTUM);
			if(hat.Left.State)
				_panTilt.PanDown(PAN_TILT_CONTROL_QUANTUM);
		}

		private void OnGamepadLeftTriggerChanged(GamePadBase gamePad, AnalogControl control)
		{
			int value = AdjustToPercent(control.Value, false);
			Log.SysLogText(LogLevel.DEBUG, "Set Left To {0}", value);
			_clawControl.Elevation = value;
		}

		private void OnGampadRightTriggerChanged(GamePadBase gamePad, AnalogControl control)
		{
			int value = AdjustToPercent(control.Value, true);
			Log.SysLogText(LogLevel.DEBUG, "Set Right To {0}", value);
			_clawControl.Thrust = value;
		}

		int AdjustToPercent(Double value, bool reverse)
		{
			Double v = value / (Double)GamePadTypes.MAX_ANALOG * (Double)100;
			if(reverse)
				v = Math.Abs(100 - v);
			return (int)v;
		}

		/// <summary>
		/// Y - Step Forward
		/// </summary>
		/// <param name="gamePad"></param>
		/// <param name="button"></param>
		private void OnGamepadYButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			Log.SysLogText(LogLevel.DEBUG, "YButton {0}", button);
			if(button.State)
				_mqqtController.SendMoveStep(Direction.Forward, TimeSpan.FromMilliseconds(50), 80);
		}

		/// <summary>
		/// A - Step backward
		/// </summary>
		/// <param name="gamePad"></param>
		/// <param name="button"></param>
		private void OnGampadAButtonChanged(GamePadBase gamePad, RaspiCommon.Devices.GamePads.Button button)
		{
			Log.SysLogText(LogLevel.DEBUG, "AButton {0}", button);
			if(button.State)
				_mqqtController.SendMoveStep(Direction.Backward, TimeSpan.FromMilliseconds(50), 80);
		}

	}
}
