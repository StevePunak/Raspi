using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;

namespace TrackBot.Tracks
{
	class BotTracks : L298N_DC_TankTracks
	{
		const Double BEARING_ADJUST_HIGH_THERSHOLD = 3;
		const Double BEARING_ADJUST_LOW_THERSHOLD = 1;
		const int MAX_SPEED_ADJUST = 5;

		static readonly TimeSpan MAX_TIME_TO_RUN = TimeSpan.FromSeconds(4);
		static readonly TimeSpan MIN_DIRECTION_ADJUST_INTERVAL = TimeSpan.FromMilliseconds(500);

		public int TooSlow { get { return 50; } }
		public int Slow { get { return 70; } }
		public int Medium { get { return 85; } }
		public int Fast { get { return 95; } }
		public int StandardSpeed { get; set; }

		public Double MetersPerSecond = .47;

		public Double StoppingDistance { get; set; }

		public int LastSpeed { get; private set; }

		public TimeSpan RampUp { get; set; }
		public TimeSpan RampDown { get; set; }

		public int Speed
		{
			set { LastSpeed = LeftSpeed = RightSpeed = value; }
		}

		public GridLocation StartLocation { get; private set; }
		public DateTime StartCalcTime { get; private set; }

		DateTime _lastDirectionalAdjustment;

		public BotTracks()
			: base(
				  Program.Config.TracksLeftA1Pin, 
				  Program.Config.TracksLeftA2Pin, 
				  Program.Config.TracksLeftEnaPin, 
				  Program.Config.TracksRightA1Pin, 
				  Program.Config.TracksRightA2Pin, 
				  Program.Config.TracksRightEnaPin)
		{
			Log.SysLogText(LogLevel.DEBUG, "Tracks at L1 {0} L2 {1} LENA {2} R1 {3} R2 {4} RENA {5}",
				  Program.Config.TracksLeftA1Pin,
				  Program.Config.TracksLeftA2Pin,
				  Program.Config.TracksLeftEnaPin,
				  Program.Config.TracksRightA1Pin,
				  Program.Config.TracksRightA2Pin,
				  Program.Config.TracksRightEnaPin);

			RampUp = Program.Config.RampUp;
			RampDown = Program.Config.RampDown;

			StandardSpeed = Program.Config.StandardSpeed == 0 ? 90 : Program.Config.StandardSpeed;
			StoppingDistance = Program.Config.StoppingDistance == 0 ? .25 : Program.Config.StoppingDistance;
		}

		/// <summary>
		/// Spin in place at speed 0 - 100
		/// </summary>
		/// <param name="speed"></param>
		public void Spin(SpinDirection direction, int speed)
		{
			if(direction == SpinDirection.Clockwise)
			{
				LeftSpeed = speed;
				RightSpeed = -speed;
			}
			else
			{
				LeftSpeed = -speed;
				RightSpeed = speed;
			}
		}

		public void SetStart()
		{
			StartLocation = new GridLocation(Widgets.Environment.Location, Widgets.GyMag.Bearing);
			StartCalcTime = DateTime.UtcNow;
		}

		public bool TryCalculateCurrentLocation(out PointD location)
		{
			TimeSpan elapsed = DateTime.UtcNow - StartCalcTime;

			location = null;
			Double metersPerSecond;
			if(Program.Config.MetersPerSecondAtPower.TryGetValue(LastSpeed, out metersPerSecond))
			{
				Double metersTraveled = elapsed.TotalSeconds * metersPerSecond;
				Double diff = Degrees.AngularDifference(StartLocation.Bearing, Widgets.GyMag.Bearing);
				if(diff > 8)
				{
					Console.WriteLine("Unable to calculate with {0:0.00}° difference", diff);
				}
				else
				{
					Line line = new Line(StartLocation.Point, StartLocation.Bearing, metersTraveled);
					location = line.P2 as PointD;
				}
			}
			return location != null;
		}

		public void AdjustSpeed(Double desiredBearing, Double actualBearing)
		{
			Double diff = desiredBearing.AngularDifference(actualBearing);
			if(diff > BEARING_ADJUST_HIGH_THERSHOLD)
			{
				if(DateTime.UtcNow > _lastDirectionalAdjustment + MIN_DIRECTION_ADJUST_INTERVAL)
				{
					if(LeftSpeed > TooSlow && RightSpeed > TooSlow)
					{
						SpinDirection direction = GetClosestSpinDirection(actualBearing, desiredBearing);
						if(direction == SpinDirection.Clockwise)
						{
							if(LeftSpeed < LastSpeed + MAX_SPEED_ADJUST)
							{
								LeftSpeed += 1;
							}
							else if(RightSpeed > LastSpeed - MAX_SPEED_ADJUST)
							{
								RightSpeed -= 1;
							}
							else
							{
								Log.SysLogText(LogLevel.WARNING, "Speed adjust in the wind 1");
							}
							_lastDirectionalAdjustment = DateTime.UtcNow;
						}
						else
						{
							if(RightSpeed < LastSpeed + MAX_SPEED_ADJUST)
							{
								RightSpeed += 1;
							}
							else if(LeftSpeed > LastSpeed - MAX_SPEED_ADJUST)
							{
								LeftSpeed -= 1;
							}
							else
							{
								Log.SysLogText(LogLevel.WARNING, "Speed adjust in the wind 2");
							}
							_lastDirectionalAdjustment = DateTime.UtcNow;
						}
					}
					else
					{
						Console.WriteLine("Below speed threshold for adjustment");
					}
				}
			}

		}

		public void ForwardTime(TimeSpan time, int motorSpeed)
		{
			Widgets.Tracks.Speed = 75;
			GpioSharp.Sleep(time);
			Widgets.Tracks.Stop();
		}

		public void BackwardTime(TimeSpan time, int motorSpeed)
		{
			Widgets.Tracks.Speed = -75;
			GpioSharp.Sleep(time);
			Widgets.Tracks.Stop();
		}

		public bool ForwardMeters(Double meters, int motorSpeed)
		{
			return MoveMeters(Direction.Forward, meters, motorSpeed);
		}

		public bool BackwardMeters(Double meters, int motorSpeed)
		{
			return MoveMeters(Direction.Backward, meters, motorSpeed);
		}

		public bool MoveMeters(Direction direction, Double meters, int motorSpeed)
		{
			bool result = false;

			Double startDistance = RangeAtDirection(direction);
			Log.SysLogText(LogLevel.DEBUG, "Start move {0} meters at {1:0.00} distance from nearest obstacle", meters, startDistance);

			DateTime startTime = DateTime.UtcNow;
			Widgets.Tracks.Speed = direction == Direction.Forward ? motorSpeed : -motorSpeed;
			while(true)
			{
				Double range = RangeAtDirection(direction);
				Double distanceTraveled = startDistance - range;
				if(DateTime.UtcNow > startTime + MAX_TIME_TO_RUN)
				{
					Console.WriteLine("Ran out of time");
					break;
				}
				else if(distanceTraveled >= meters)
				{
					Console.WriteLine("Traveled our distance");
					result = true;
					break;
				}
				else if(range < StoppingDistance)
				{
					Console.WriteLine("Ran out of space");
					break;
				}				
				GpioSharp.Sleep(100);
			}
			Widgets.Tracks.Stop();

			return result;
		}

		private Double RangeAtDirection(Direction direction)
		{
			Double bearing = Widgets.GyMag.Bearing;
			if(direction == Direction.Backward)
				bearing = bearing.AddDegrees(180);
			Double range = Widgets.Environment.FuzzyRangeAtBearing(bearing, Widgets.Environment.RangeFuzz);
			Log.SysLogText(LogLevel.DEBUG, "Range at {0} bearing {1:0.00}° is {2:0.000}m", direction, bearing, range);
			return range;
		}

		public bool MoveMetersUsingUltraSound(Direction direction, Double meters, int motorSpeed)
		{
			bool result = true;

			TimeSpan timeToRun;
			Double speedMPS;

			Log.SysLogText(LogLevel.DEBUG, "Start move {0} meters at {1:0.00} distance from nearest obstacle", meters, Widgets.Environment.FuzzyRangeAtBearing(Widgets.GyMag.Bearing, Widgets.Environment.RangeFuzz));
			if(TryGetTimeForDistance(meters, motorSpeed, out timeToRun, out speedMPS))
			{
				timeToRun += direction == Direction.Forward ? RampUp : -RampDown;
				Line line = new Line(Widgets.Environment.Location, FlatGeo.GetPoint(Widgets.Environment.Location, Widgets.GyMag.Bearing, meters));

				DateTime start = DateTime.UtcNow;
				TimeSpan interval = TimeSpan.FromMilliseconds(10);

				Double intervalsToRun = timeToRun.TotalMilliseconds / interval.TotalMilliseconds;
				Double metersPerInterval = meters / intervalsToRun;

				Console.WriteLine("Will go to {1:0.00} meters at {2:0.00}m/ps will take {3}  ({4} intervals at {5:0.00} meters per interval)",
					line.P2, meters, speedMPS, timeToRun.ToAbbreviatedFormat(true), intervalsToRun, metersPerInterval);

				Double metersTraveled = 0;
				Speed = direction == Direction.Forward ? motorSpeed : -motorSpeed;
				while(DateTime.UtcNow < start + timeToRun)
				{
					GpioSharp.Sleep(interval);
					metersTraveled += metersPerInterval;

					Widgets.Environment.RelativeLocation = FlatGeo.GetPoint(Widgets.Environment.Location, Widgets.GyMag.Bearing, metersPerInterval);
//					Log.SysLogText(LogLevel.DEBUG, "Moved to {0}", Widgets.Environment.Location);

					if(direction == Direction.Forward && Widgets.Environment.FuzzyRangeAtBearing(Widgets.GyMag.Bearing, Widgets.Environment.RangeFuzz) <= Program.Config.StopDistance)
					{
						Console.WriteLine("Stopping due to range");
						result = false;
						break;
					}
				}

				Stop();
			}
			else
			{
				Log.SysLogText(LogLevel.WARNING, "Could not get time for distance at {0} power", motorSpeed);
				result = false;
			}

			Log.SysLogText(LogLevel.DEBUG, "Stopped move {0} meters at {1:0.00} distance from nearest obstacle", meters, Widgets.Environment.FuzzyRangeAtBearing(Widgets.GyMag.Bearing, Widgets.Environment.RangeFuzz));
			return result;
		}

		public bool TryGetTimeForDistance(Double distance, int motorSpeed, out TimeSpan time, out Double speedMPS)
		{
			time = TimeSpan.Zero;

			speedMPS = 0;
			if(Program.Config.MetersPerSecondAtPower.TryGetValue(motorSpeed, out speedMPS))
			{
				time = TimeSpan.FromSeconds(distance / speedMPS);
			}
			else
			{
				Console.WriteLine("Could not get MPS at {0}", motorSpeed);
			}

			return time != TimeSpan.Zero;
		}

		public static SpinDirection GetClosestSpinDirection(Double from, Double to)
		{
			Double clockwiseDiff = Degrees.ClockwiseDifference(from, to);
			Double counterClockwiseDiff = Degrees.CounterClockwiseDifference(from, to);
			SpinDirection direction = clockwiseDiff < counterClockwiseDiff
				? SpinDirection.Clockwise : SpinDirection.CounterClockwise;
			return direction;
		}

		public bool TurnToBearing(Double to)
		{
			const Double THRESHOLD = 2;

			Double from = Widgets.GyMag.Bearing;
			Double diff = 0;

			SpinDirection direction = GetClosestSpinDirection(from, to);

			Console.WriteLine("Will turn {0} to from {1} to {2}", direction, from, to);

			Widgets.Tracks.Spin(direction, Widgets.Tracks.Fast);

			DateTime start = DateTime.UtcNow;

			while(DateTime.UtcNow < start + TimeSpan.FromSeconds(5))
			{
				Double bearing = Widgets.GyMag.Bearing;
				diff = Degrees.AngularDifference(to, bearing);
				Log.SysLogText(LogLevel.DEBUG, "Difference from {0:00}° to {1:00}° is {2:00}°", bearing, to, diff);
				if(diff < THRESHOLD)
				{
					break;
				}
				GpioSharp.Sleep(TimeSpan.FromMilliseconds(10));
			}

			Log.SysLogText(LogLevel.DEBUG, "Stopping");
			Widgets.Tracks.Stop();

			return diff < THRESHOLD;
		}

	}
}
