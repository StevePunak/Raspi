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
		public int Slow { get { return 70; } }
		public int StandardSpeed { get { return 90;  } }

		public Double MetersPerSecond = .47;

		public int LastSpeed { get; private set; }

		public TimeSpan RampUp { get; set; }
		public TimeSpan RampDown { get; set; }

		public int Speed
		{
			set { LastSpeed = LeftSpeed = RightSpeed = value; }
		}

		public GridLocation StartLocation { get; private set; }
		public DateTime StartCalcTime { get; private set; }

		public BotTracks()
			: base(
				  Program.Config.TracksLeftA1Pin, 
				  Program.Config.TracksLeftA2Pin, 
				  Program.Config.TracksLeftEnaPin, 
				  Program.Config.TracksRightA1Pin, 
				  Program.Config.TracksRightA2Pin, 
				  Program.Config.TracksRightEnaPin)
		{
			RampUp = Program.Config.RampUp;
			RampDown = Program.Config.RampDown;
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
			bool result = true;

			TimeSpan timeToRun;
			Double speedMPS;

			Log.SysLogText(LogLevel.DEBUG, "Start move {0} meters at {1:0.00} distance from nearest obstacle", meters, Widgets.RangeFinder.Range);
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

					Widgets.Environment.Location = FlatGeo.GetPoint(Widgets.Environment.Location, Widgets.GyMag.Bearing, metersPerInterval);
//					Log.SysLogText(LogLevel.DEBUG, "Moved to {0}", Widgets.Environment.Location);

					if(direction == Direction.Forward && Widgets.RangeFinder.Range <= Program.Config.StopDistance)
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

			Log.SysLogText(LogLevel.DEBUG, "Stopped move {0} meters at {1:0.00} distance from nearest obstacle", meters, Widgets.RangeFinder.Range);
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

		public bool TurnToBearing(Double to)
		{
			const Double THRESHOLD = 2;

			Double from = Widgets.GyMag.Bearing;

			Double clockwiseDiff = Degrees.ClockwiseDifference(from, to);
			Double counterClockwiseDiff = Degrees.CounterClockwiseDifference(from, to);
			SpinDirection direction = clockwiseDiff < counterClockwiseDiff
				? SpinDirection.Clockwise : SpinDirection.CounterClockwise;

			Console.WriteLine("Will turn {0} to from {1} to {2}", direction, from, to);

			Widgets.Tracks.Spin(direction, Widgets.Tracks.StandardSpeed);

			DateTime start = DateTime.UtcNow;
			Double diff = 99;
			while(DateTime.UtcNow < start + TimeSpan.FromSeconds(10))
			{
				Double bearing = Widgets.GyMag.Bearing;
				diff = Degrees.AngularDifference(to, bearing);
				if(diff < THRESHOLD)
				{
					break;
				}
				GpioSharp.Sleep(TimeSpan.FromMilliseconds(10));
			}

			Widgets.Tracks.Stop();

			return diff < THRESHOLD;
		}

	}
}
