#define DEBUG_LOG
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
using RaspiCommon.Devices.Locomotion;
using RaspiCommon.Devices.Spatial;

namespace TrackBot.Tracks
{
	public class TravelThread : ThreadBase
	{
		const Double STOP_FOR_OBSTACLE_RANGE = .2;

		public int LeftSpeed { get; private set; }
		public int RightSpeed { get; private set; }

		public bool InControl { get; private set; }

		public DateTime LastControlMessage { get; private set; }
		public BotTracks Tracks { get; private set; }

		DateTime _lastEStopTime;
		bool _eStopped;

		Double _spinStepDegrees;
		TimeSpan _spinStepTime;
		SpinDirection _spinStepDirection;

		MoveStep _moveStep;

		public TravelThread(BotTracks tracks)
			: base(typeof(TravelThread).Name)
		{
			Tracks = tracks;
			Interval = TimeSpan.FromMilliseconds(250);

			_spinStepDirection = SpinDirection.None;
			_spinStepDegrees = 0;

			_moveStep = null;

			_eStopped = false;
		}

		public void SpinStepDegrees(SpinDirection direction, Double degrees)
		{
			_spinStepDirection = direction;
			_spinStepDegrees = degrees;
		}

		public void SpinStepTime(SpinDirection direction, TimeSpan time)
		{
			_spinStepDirection = direction;
			_spinStepTime = time;
		}

		public void MoveStepTime(MoveStep step)
		{
			_moveStep = step;
		}

		public void SetSpeed(int left, int right)
		{
			int actualLeft = AdjustedSpeed(left);
			int actualRight = AdjustedSpeed(right);

			LeftSpeed = (int)actualLeft;
			RightSpeed = (int)actualRight;

			InControl = true;
			LastControlMessage = DateTime.UtcNow;
			Interval = TimeSpan.FromMilliseconds(250);
		}

		int AdjustedSpeed(int original)
		{
			return original;
		}

		void SetTracksSpeed()
		{
			if(DateTime.UtcNow - _lastEStopTime < TimeSpan.FromSeconds(3) && (LeftSpeed != 0 || RightSpeed != 0))
			{
				_eStopped = true;
#if DEBUG_LOG
				Log.LogText(LogLevel.DEBUG, "Won't move because of ESTOP");
#endif
			}
			else
			{
				if(_eStopped)
				{
					Log.LogText(LogLevel.DEBUG, "Releasing E-Stop");
					_eStopped = false;
				}
				Tracks.LeftSpeed = LeftSpeed;
				Tracks.RightSpeed = RightSpeed;
			}
		}

		int _lastSpeedLeft;
		int _lastSpeedRight;

		protected override bool OnRun()
		{
			if(InControl && DateTime.UtcNow - LastControlMessage > TimeSpan.FromSeconds(2))
			{
				InControl = false;
				Log.LogText(LogLevel.DEBUG, "{0} giving up control", Name);
				Interval = TimeSpan.FromSeconds(1);
			}
			else if(InControl)
			{
				if(LeftSpeed != _lastSpeedLeft || RightSpeed != _lastSpeedRight)
				{
					Log.LogText(LogLevel.DEBUG, "{0} Setting speed L: {1}  R: {2}  {3} since last message", Name, LeftSpeed, RightSpeed, DateTime.UtcNow - LastControlMessage);
				}
				_lastSpeedLeft = LeftSpeed;
				_lastSpeedRight = RightSpeed;
				
				if(_moveStep != null)
				{
					MoveStep();
				}
				else if(_spinStepDirection != SpinDirection.None)
				{
					SpinStep();
				}
				else if(LeftSpeed != 0 || RightSpeed != 0)
				{
					Travel();
				}
				else
				{
					Tracks.Stop();
				}
			}

			return true;
		}

		private void MoveStep()
		{
			if(_moveStep != null)
			{
				Tracks.MoveTime(_moveStep.Time, _moveStep.Direction, _moveStep.Speed);
			}
			_moveStep = null;
		}

		private void SpinStep()
		{
			if(_spinStepDegrees != 0)
			{
				Double bearing = Widgets.Instance.Compass.Bearing;
				bearing = _spinStepDirection == SpinDirection.Clockwise
					? bearing.AddDegrees(_spinStepDegrees)
					: bearing.SubtractDegrees(_spinStepDegrees);
				Tracks.TurnToBearing(bearing, _spinStepDirection);
			}
			else
			{
				Tracks.Spin(_spinStepDirection, _spinStepTime);
			}
			_spinStepDirection = SpinDirection.None;
		}

		private void Travel()
		{
			bool spinning = (LeftSpeed < 0 && RightSpeed > 0) || (RightSpeed < 0 && LeftSpeed > 0);

			Direction direction = LeftSpeed + RightSpeed > 0 ? Direction.Forward : Direction.Backward;
			Double rangeToObstacle = Widgets.Instance.GetRangeAtDirection(direction, true);
			HCSR04_RangeFinder rangeFinder;
			if(Widgets.Instance.RangeFinders.TryGetValue(direction == Direction.Forward ? RFDir.Front : RFDir.Rear, out rangeFinder))
			{
				if(rangeFinder.Range != 0 && rangeFinder.Range < .3)
				{
					rangeToObstacle = Math.Min(rangeToObstacle, rangeFinder.Range);
				}
			}

#if DEBUG_LOG
			Log.LogText(LogLevel.DEBUG, "{0} Range {1} {2}  Spinning = {3}", Name, direction, rangeToObstacle.ToMetersString(), spinning);
#endif
			if(spinning == false && rangeToObstacle < STOP_FOR_OBSTACLE_RANGE)
			{
#if DEBUG_LOG
				Log.LogText(LogLevel.DEBUG, "{0} Stopping for obstacle {1} at {2}", Name, direction, rangeToObstacle.ToMetersString());
#endif
				Tracks.Stop();
				_lastEStopTime = DateTime.UtcNow;
			}
			else if(spinning)
			{
				_lastEStopTime = DateTime.MinValue;
			}
			SetTracksSpeed();
		}
	}
}
