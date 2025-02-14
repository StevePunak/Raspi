using System;
using System.Collections.Generic;
using System.Threading;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.Devices.Locomotion;
using RaspiCommon.Devices.Spatial;
using RaspiCommon.Spatial;

namespace TrackBot.Tracks
{
	public class BotTracks : L298N_DC_TankTracks, IMotorizedChassis
	{
		#region Constants

		const Double BEARING_ADJUST_HIGH_THERSHOLD = 3;
		const Double BEARING_ADJUST_LOW_THERSHOLD = 1;
		const int MAX_SPEED_ADJUST = 5;

		static readonly TimeSpan MAX_TIME_TO_RUN = TimeSpan.FromSeconds(8);
		static readonly TimeSpan MIN_DIRECTION_ADJUST_INTERVAL = TimeSpan.FromMilliseconds(500);

		#endregion

		#region Events

		public event RangeHandler ForwardPrimaryRange;
		public event RangeHandler BackwardPrimaryRange;
		public event RangeHandler ForwardSecondaryRange;
		public event RangeHandler BackwardSecondaryRange;
		public event NewDestinationBearingHandler NewDestinationBearing;
		public event DistanceToTravelHandler DistanceToTravel;
		public event DistanceLeftHandler DistanceLeft;

		#endregion

		public int TooSlow { get { return 50; } }
		public int PrettySlow { get { return 60; } }
		public int Slow { get { return 70; } }
		public int Medium { get { return 85; } }
		public int Fast { get { return 100; } }
		public int StandardSpeed { get; set; }

		public Double MetersPerSecond = .47;

		public Double StoppingDistance { get; set; }

		public int LastSpeed { get; private set; }

		public TimeSpan RampUp { get; set; }
		public TimeSpan RampDown { get; set; }

		public int Speed
		{
			set
			{
				LastSpeed = LeftSpeed = RightSpeed = value;
			}
		}

		public GridLocation StartLocation { get; private set; }
		public DateTime StartCalcTime { get; private set; }

		public TravelThread TravelThread { get; private set; }

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

			ForwardPrimaryRange += delegate { };
			BackwardPrimaryRange += delegate { };
			ForwardSecondaryRange += delegate { };
			BackwardSecondaryRange += delegate { };
			NewDestinationBearing += delegate { };
			DistanceToTravel += delegate { };
			DistanceLeft += delegate { };

		}

		public void StartTracks()
		{
			TravelThread = new TravelThread(this);
			TravelThread.Start();
		}

		public void StopTracks()
		{
			TravelThread.Stop();
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

		/// <summary>
		/// Spin in place at speed 0 - 100
		/// </summary>
		/// <param name="speed"></param>
		public void Spin(SpinDirection direction, TimeSpan time, int speed = 0)
		{
			Log.SysLogText(LogLevel.DEBUG, "Will spin {0} for {1}ms", direction, time.TotalMilliseconds);
			if(speed == 0)
				speed = Program.Config.StandardSpeed;
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
			Thread.Sleep(time);
			Stop();
		}

		public void SetStart()
		{
			StartLocation = new GridLocation(Widgets.Instance.ImageEnvironment.Location, Widgets.Instance.Compass.Bearing);
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
				Double diff = Degrees.AngularDifference(StartLocation.Bearing, Widgets.Instance.Compass.Bearing);
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
						SpinDirection direction = Utility.GetClosestSpinDirection(actualBearing, desiredBearing);
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

		/// <summary>
		/// Go forwards for the given time
		/// </summary>
		/// <param name="time"></param>
		/// <param name="motorSpeed"></param>
		public void ForwardTime(TimeSpan time, int motorSpeed)
		{
			Log.SysLogText(LogLevel.INFO, "Starting forward move {0}", (int)time.TotalMilliseconds);
			Widgets.Instance.Tracks.Speed = StandardSpeed;
			Thread.Sleep(time);
			Widgets.Instance.Tracks.Stop();
			Log.SysLogText(LogLevel.INFO, "Ending forward move {0}", (int)time.TotalMilliseconds);
		}

		/// <summary>
		/// Go backwards for the given time
		/// </summary>
		/// <param name="time"></param>
		/// <param name="motorSpeed"></param>
		public void BackwardTime(TimeSpan time, int motorSpeed)
		{
			Log.SysLogText(LogLevel.INFO, "Starting backward move {0}", (int)time.TotalMilliseconds);
			Widgets.Instance.Tracks.Speed = -StandardSpeed;
			Thread.Sleep(time);
			Widgets.Instance.Tracks.Stop();
			Log.SysLogText(LogLevel.INFO, "Ending backward move {0}", (int)time.TotalMilliseconds);
		}

		/// <summary>
		/// Move for the given amount of time in the given direction with range detection
		/// </summary>
		/// <param name="time"></param>
		/// <param name="direction"></param>
		/// <param name="motorSpeed"></param>
		/// <returns></returns>
		public TravelResult MoveTime(TimeSpan time, Direction direction, int motorSpeed)
		{
			TravelResult result = TravelResult.UnknownFailure;

			TimeSpan maxTimeToTravel = time;

			Double startDistance = Widgets.Instance.GetRangeAtDirection(direction);
			Log.SysLogText(LogLevel.DEBUG, "Start move {0} at {1:0.00} distance from nearest obstacle", time, startDistance);

			DateTime startTime = DateTime.UtcNow;
			int trackSpeed = direction == Direction.Forward ? motorSpeed : -motorSpeed;
			Widgets.Instance.Tracks.Speed = trackSpeed;

			while(true)
			{
				Double forwardRange = Widgets.Instance.GetRangeAtDirection(Direction.Forward, false);
				Double backwardRange = Widgets.Instance.GetRangeAtDirection(Direction.Backward, false);

				ForwardPrimaryRange(forwardRange, true);
				BackwardPrimaryRange(backwardRange, true);

				if(Widgets.Instance.RangeFinders.Count > 0)
				{
					forwardRange = Math.Min(Widgets.Instance.RangeFinders[RFDir.Front].Range, forwardRange);
					backwardRange = Math.Min(Widgets.Instance.RangeFinders[RFDir.Rear].Range, backwardRange);

					ForwardSecondaryRange(forwardRange, Widgets.Instance.RangeFinders[RFDir.Front].Valid);
					BackwardSecondaryRange(backwardRange, Widgets.Instance.RangeFinders[RFDir.Rear].Valid);
				}

				Double range = direction == Direction.Forward ? forwardRange : backwardRange;

				Log.SysLogText(LogLevel.DEBUG, "There is {0:0.000}m left at {1}", range, direction);

				DistanceLeft(range);

				if(DateTime.UtcNow >= startTime + maxTimeToTravel)
				{
					result = TravelResult.TravelTimeComplete;
					break;
				}
				Thread.Sleep(100);
			}
			Widgets.Instance.Tracks.Stop();

			Log.SysLogText(LogLevel.DEBUG, "MoveTime returning {0}", result);
			return result;
		}


		public TravelResult ForwardMeters(Double meters, int motorSpeed, bool stagger = false, bool ignoreCollision = false)
		{
			return MoveMeters(Direction.Forward, meters, motorSpeed, stagger, ignoreCollision);
		}

		public TravelResult BackwardMeters(Double meters, int motorSpeed, bool stagger = false, bool ignoreCollision = false)
		{
			return MoveMeters(Direction.Backward, meters, motorSpeed, stagger, ignoreCollision);
		}

		public TravelResult MoveMeters(Direction direction, Double meters, int motorSpeed, bool stagger = false, bool ignoreCollision = false, bool useFuzzyRange = true)
		{
			TravelResult result = TravelResult.UnknownFailure;

			DistanceToTravel(meters);

			TimeSpan maxTimeToTravel = TimeToTravel(meters, motorSpeed);

			Double startDistance = Widgets.Instance.GetRangeAtDirection(direction);
			Double travelBearing = direction == Direction.Forward ? Widgets.Instance.Compass.Bearing : Widgets.Instance.Compass.Bearing.AddDegrees(180);
			Log.SysLogText(LogLevel.DEBUG, "Start move {0} meters at {1:0.00} distance from nearest obstacle", meters, startDistance);

			DateTime startTime = DateTime.UtcNow;
			int trackSpeed = direction == Direction.Forward ? motorSpeed : -motorSpeed;
			Widgets.Instance.Tracks.Speed = trackSpeed;

			
			while(true)
			{
				Double forwardRange = Widgets.Instance.GetRangeAtDirection(Direction.Forward, useFuzzyRange);
				Double backwardRange = Widgets.Instance.GetRangeAtDirection(Direction.Backward, useFuzzyRange);

				ForwardPrimaryRange(forwardRange, true);
				BackwardPrimaryRange(backwardRange, true);

				if(Widgets.Instance.RangeFinders.Count > 0)
				{
					ForwardSecondaryRange(Widgets.Instance.RangeFinders[RFDir.Front].Range, Widgets.Instance.RangeFinders[RFDir.Front].Valid);
					BackwardSecondaryRange(Widgets.Instance.RangeFinders[RFDir.Rear].Range, Widgets.Instance.RangeFinders[RFDir.Rear].Valid);
				}

				Double range = direction == Direction.Forward ? forwardRange : backwardRange;

				Log.SysLogText(LogLevel.DEBUG, "There is {0:0.000}m left at {1}", range, direction);

				DistanceLeft(range);

				Double distanceTraveled = startDistance - range;
				if(stagger == false && DateTime.UtcNow > startTime + maxTimeToTravel)
				{
					result = TravelResult.TravelTimeComplete;
					break;
				}
				else if(distanceTraveled >= meters)
				{
					Log.SysLogText(LogLevel.DEBUG, "Traveled our distance (actually went {0})", distanceTraveled.ToMetersString());
					Widgets.Instance.DeadReckoningEnvironment.Move(travelBearing, distanceTraveled);
					result = TravelResult.TravelDistanceComplete;
					break;
				}
				else if(ignoreCollision == false && range < StoppingDistance)
				{
					Log.SysLogText(LogLevel.DEBUG, "Ran out of space (actually went {0})", distanceTraveled.ToMetersString());
					Widgets.Instance.DeadReckoningEnvironment.Move(travelBearing, distanceTraveled);
					result = TravelResult.OutOfSpace;
					break;
				}
				if(stagger)
				{
					Thread.Sleep(250);	// go for 250ms
					Stop();
					Thread.Sleep(1500);  // stop and settle down

					Widgets.Instance.Tracks.Speed = trackSpeed;		// go again
				}
				else
				{
					Thread.Sleep(100);
				}
			}
			Widgets.Instance.Tracks.Stop();

			Log.SysLogText(LogLevel.DEBUG, "MoveMeters returning {0}", result);
			return result;
		}

		public bool MoveMetersUsingUltraSound(Direction direction, Double meters, int motorSpeed)
		{
			bool result = true;

			TimeSpan timeToRun;
			Double speedMPS;

			Log.SysLogText(LogLevel.DEBUG, "Start move {0} meters at {1:0.00} distance from nearest obstacle", meters, Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.Chassis, Widgets.Instance.Compass.Bearing, Widgets.Instance.ImageEnvironment.RangeFuzz));
			if(TryGetTimeForDistance(meters, motorSpeed, out timeToRun, out speedMPS))
			{
				timeToRun += direction == Direction.Forward ? RampUp : -RampDown;
				Line line = new Line(Widgets.Instance.ImageEnvironment.Location, FlatGeo.GetPoint(Widgets.Instance.ImageEnvironment.Location, Widgets.Instance.Compass.Bearing, meters));

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
					Thread.Sleep(interval);
					metersTraveled += metersPerInterval;

					Widgets.Instance.ImageEnvironment.RelativeLocation = FlatGeo.GetPoint(Widgets.Instance.ImageEnvironment.Location, Widgets.Instance.Compass.Bearing, metersPerInterval);
//					Log.SysLogText(LogLevel.DEBUG, "Moved to {0}", Widgets.Instance.Environment.Location);

					if(direction == Direction.Forward && Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.Chassis, Widgets.Instance.Compass.Bearing, Widgets.Instance.ImageEnvironment.RangeFuzz) <= Program.Config.StopDistance)
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

			Log.SysLogText(LogLevel.DEBUG, "Stopped move {0} meters at {1:0.00} distance from nearest obstacle", 
				meters, Widgets.Instance.ImageEnvironment.FuzzyRangeAtBearing(Widgets.Instance.Chassis, Widgets.Instance.Compass.Bearing, Widgets.Instance.ImageEnvironment.RangeFuzz));
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

		public bool TurnToBearing(Double to, SpinDirection direction = SpinDirection.None, int speed = 0)
		{
			const Double THRESHOLD = 2;

			int spinSpeed = speed == 0 ? Widgets.Instance.Tracks.Fast : speed;
			NewDestinationBearing(to);

			Double from = Widgets.Instance.Compass.Bearing;
			Double diff = 0;

			if(direction == SpinDirection.None)
			{
				direction = Utility.GetClosestSpinDirection(from, to);
			}

			Log.SysLogText(LogLevel.DEBUG, "Will turn {0} to from {1} to {2}  (speed: {3})", direction, from.ToAngleString(), to.ToAngleString(), spinSpeed);

			Widgets.Instance.Tracks.Spin(direction, spinSpeed);

			DateTime start = DateTime.UtcNow;

			Double bearing = 0;
			while(DateTime.UtcNow < start + TimeSpan.FromSeconds(5))
			{
				bearing = Widgets.Instance.Compass.Bearing;
				diff = Degrees.AngularDifference(to, bearing);
				if(bearing.AngularDifference(to, direction) > 180)
				{
					Log.SysLogText(LogLevel.WARNING, "Shit... we're turning the wrong way.. diff from {0} to {1} {2} is {3}. Reversing direction",
						bearing.ToAngleString(), to.ToAngleString(), direction, bearing.AngularDifference(to, direction).ToAngleString());
					direction = direction == SpinDirection.Clockwise ? SpinDirection.CounterClockwise : SpinDirection.Clockwise;
					Widgets.Instance.Tracks.Spin(direction, spinSpeed);
				}
				//Log.SysLogText(LogLevel.DEBUG, "Difference from {0:00}° to {1:00}° is {2:00}°", bearing, to, diff);
				if(diff < THRESHOLD)
				{
					break;
				}
				Thread.Sleep(TimeSpan.FromMilliseconds(10));
			}

			Log.SysLogText(LogLevel.DEBUG, "Turn To Bearing finished at {0:0.00}°", bearing);
			Widgets.Instance.Tracks.Stop();

			return diff < THRESHOLD;
		}

	}
}
