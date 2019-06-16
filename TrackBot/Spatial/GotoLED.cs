using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Extensions;
using RaspiCommon.Spatial.LidarImaging;

namespace TrackBot.Spatial
{
	class GotoLED : Activity
	{
		const Double FORWARD_COLLISION_WARNING = .15;
		const Double ARRIVAL_RANGE = .5;
		const Double MAX_BEARING_DIFFERENTIAL = 5;
		const Double BEARING_INCREMENT = 20;
		const Double ASSERTIVE_RANGE = 1;
		const Double BACK_STAGGER_DEGREES = 5;
		const int MAX_CENTER_TRIES = 2;
		const int LED_SATURATION_SIZE = 800;

		public Double StartBearing { get; set; }
		public LEDTravelHistoryEntry LastLEDHistory { get; set; }

		public ColoredObjectPosition LED { get; set; }

		public List<Color> Colors { get; set; }
		public Color CurrentColor { get; private set; }
		public List<Color> ColorsTraveled { get; set; }

		Double LegDistanceTraveled { get; set; }
		Double LegBearing { get; set; }

		#region Private Member Variables

		bool _saveAutoSnap;
		int _centerAttemptsForDestination;
		bool _staggerBack;
		int _colorIndex;
		int _centerTriesLeft;
		int _historyIndex;

		SpinDirection SpinDirection { get; set; }

		List<BearingAndRange> History { get; set; }
		
		#endregion

		public GotoLED()
			: base(ActivityType.GotoLED)
		{
			Interval = TimeSpan.FromMilliseconds(100);
			ActivityState = ActivityStates.Init;
		}

		protected override bool ParseArgs(object[] args)
		{
			bool parsed = false;

			Colors = new List<Color>();
			foreach(Object o in args)
			{
				if(o is String && Color.FromName(o.ToString()).ToArgb() != 0)
				{
					Colors.Add(Color.FromName(o.ToString()));
					parsed = true;
				}
				else
				{
					parsed = false;
					break;
				}
			}
			return parsed;
		}

		protected override bool OnStart()
		{
			_staggerBack = false;
			_saveAutoSnap = Widgets.Instance.Camera.AutoSnap;
			_colorIndex = 0;
			_centerAttemptsForDestination = 0;
			SpinDirection = SpinDirection.Clockwise;
			CurrentColor = Colors[_colorIndex];
			ColorsTraveled = new List<Color>();
			LastLEDHistory = null;
			History = new List<BearingAndRange>();
			Widgets.Instance.Camera.AutoSnap = false;
			return base.OnStart();
		}

		protected override bool OnStop()
		{
			Widgets.Instance.Tracks.Stop();
			Widgets.Instance.Camera.AutoSnap = _saveAutoSnap;
			return base.OnStop();
		}

		#region Init State

		protected bool RunInitState()
		{
			Log.LogText(LogLevel.DEBUG, "=============== RunInitState to color {0}", CurrentColor);
			StartBearing = Widgets.Instance.Compass.Bearing;
			Widgets.Instance.Tracks.Stop();
			SwitchState(ActivityStates.FindInImage);
			return true;
		}

		#endregion

		#region Idle State

		bool InitIdleState()
		{
			Widgets.Instance.Tracks.Stop();
			Interval = TimeSpan.FromMilliseconds(500);
			return true;
		}

		protected bool RunIdleState()
		{
			Interval = TimeSpan.FromMilliseconds(500);
			return true;
		}

		#endregion

		#region Find In Image State

		bool InitFindInImageState()
		{
			Interval = TimeSpan.Zero;

			LEDTravelHistoryEntry entry = Program.Config.LEDTravelHistory.GetEntryForColor(CurrentColor, ColorsTraveled);
			if(entry != null)
			{
				Log.LogText(LogLevel.INFO, "Found Color history for our trip to {0}. It is {1} and will turn to {2} to start", 
					CurrentColor.Name, entry, entry.Bearing.ToAngleString());
				SpinDirection direction = Widgets.Instance.Compass.Bearing.ShortestSpinDirectionTo(entry.Bearing);
				TryTurnToBearing(entry.Bearing);
			}
			else if(_staggerBack)
			{
				Double turnTo = SpinDirection == SpinDirection.Clockwise
					? Widgets.Instance.Compass.Bearing.SubtractDegrees(BACK_STAGGER_DEGREES)
					: Widgets.Instance.Compass.Bearing.AddDegrees(BACK_STAGGER_DEGREES);

				if(TryTurnToBearing(turnTo) == true)
				{
					Log.LogText(LogLevel.DEBUG, "Successfully prepared initial bearing at {0}", turnTo.ToAngleString());
				}
				else
				{
					Log.LogText(LogLevel.DEBUG, "DID NOT Successfully prepared initial bearing at {0}", turnTo.ToAngleString());
				}

				Sleep(TimeSpan.FromSeconds(1));
			}
			else
			{
				_staggerBack = true;
			}
			return true;
		}

		protected bool RunFindInImageState()
		{
			bool result = true;

			Log.LogText(LogLevel.DEBUG, "=============== RunFindInImage {0}", CurrentColor);

			try
			{

				ColoredObjectPosition led;
				if(TryGetLEDPosition(CurrentColor, out led))
				{
					LED = led;
					if(LED.Location.Y < 50)
					{
						Log.LogText(LogLevel.DEBUG, "Going to ignore this LED because it's too goddamn high!");

						if(TryTurnToBearing(Widgets.Instance.Compass.Bearing.AddDegrees(BEARING_INCREMENT)) == false)
						{
							throw new ActivityException("Could not turn to bearing");
						}
					}

					Double bearing = Widgets.Instance.Compass.Bearing;

					Log.LogText(LogLevel.DEBUG, "Found {0} and turning from {1} to its bearing", LED, bearing.ToAngleString());
					if(TryTurnToBearing(LED.Bearing) == true)
					{
					}

					Log.LogText(LogLevel.DEBUG, "LED candidate non-zero count is {0}", LED.Candidate.CountNonZero);
					bool saturated;
					if((saturated = led.Candidate.CountNonZero > LED_SATURATION_SIZE))
					{
						Log.LogText(LogLevel.DEBUG, "AREA > Saturation size");
					}

					Double range;
					if(saturated || (range = WaitForAdjustedRangeExact(Widgets.Instance.Compass.Bearing, TimeSpan.FromSeconds(10))) != 0 && range < ARRIVAL_RANGE)
					{
						/** make sure the LED is visible */
						if(TryGetLEDPosition(CurrentColor, out led))
						{
							ArrivedAtDestination();
						}
						else
						{
							Log.LogText(LogLevel.DEBUG, "Can't see the LED... we're going to try some more");
						}
					}
					else
					{
						Double diff = LED.Bearing.AngularDifference(Widgets.Instance.Compass.Bearing);
						if(true || diff > MAX_BEARING_DIFFERENTIAL)
						{
							Log.LogText(LogLevel.DEBUG, "-========== Gonna switch to center. We are {0} away ==============", range.ToMetersString());
							SwitchState(ActivityStates.CenterInImage);
						}
						else
						{
							Log.LogText(LogLevel.DEBUG, "-========== Since bearing is only {0} from LED we're going to hit the road. We are {1} away and mag bearing {2} ==============", 
								diff.ToAngleString(),
								range.ToMetersString(),
								Widgets.Instance.Compass.Bearing);

							SwitchState(ActivityStates.TravelToDest);
						}
					}
				}
				else
				{
					if(TryTurnToBearing(SpinDirection == SpinDirection.Clockwise 
						? Widgets.Instance.Compass.Bearing.AddDegrees(BEARING_INCREMENT)
						: Widgets.Instance.Compass.Bearing.SubtractDegrees(BEARING_INCREMENT)) == false)
					{
						throw new ActivityException("Could not turn to bearing");
					}
					Sleep(TimeSpan.FromSeconds(2.5));
				}
			}
			catch(Exception e)
			{
				Log.LogText(LogLevel.WARNING, "FindInImage ENDING: {0}", e.Message);
				SwitchState(ActivityStates.Idle);
				result = false;
			}

			return result;
		}

		#endregion

		#region Center In Image State

		protected bool InitCenterInImageState()
		{
			_centerTriesLeft = MAX_CENTER_TRIES;
			return true;
		}

		protected bool RunCenterInImageState()
		{
			Interval = TimeSpan.Zero;
			bool result = true;

			Log.LogText(LogLevel.DEBUG, "=============== RunCenterInImageState {0}", CurrentColor);

			try
			{
				Mat image;
				if(!Widgets.Instance.Camera.TryTakeSnapshot(out image))
				{
					throw new ActivityException("Could not get new image");
				}

				Widgets.Instance.LEDImageAnalysis.AnalyzeImage(image, RaspiCommon.Devices.Optics.LED.LEDSize);

				if(Widgets.Instance.LEDImageAnalysis.HasColor(CurrentColor))
				{
					LED = Widgets.Instance.LEDImageAnalysis.GetColor(CurrentColor);
					Double bearing = Widgets.Instance.Compass.Bearing;

					Double diff = bearing.AngularDifference(LED.Bearing);
					bool turnSomeMore = diff > MAX_BEARING_DIFFERENTIAL;

					Log.LogText(LogLevel.DEBUG, "CENTERING ======    The image says our LED is at {0} and our bearing is currently {1}... we {2} turn    ====== CENTERING",
						LED.Bearing.ToAngleString(), bearing.ToAngleString(), turnSomeMore ? "WILL" : "WILL NOT");
					if(turnSomeMore)
					{
						if(TryTurnToBearing(LED.Bearing) == true)
						{
						}

						bearing = Widgets.Instance.Compass.Bearing;
						Double range;
						if((range = WaitForAdjustedRangeExact(bearing, TimeSpan.FromSeconds(10))) != 0 && range < ARRIVAL_RANGE)
						{
							ArrivedAtDestination();
						}
						else if(--_centerTriesLeft == 0)
						{
							Log.LogText(LogLevel.DEBUG, "Forget this centering stuff... This is gonna have to be good enough");
							if(_centerAttemptsForDestination == 0)
							{
								AddHistoryEntry(bearing);
								_centerAttemptsForDestination++;
							}
							SwitchState(ActivityStates.TravelToDest);
						}
					}
					else
					{
						if(_centerAttemptsForDestination == 0)
						{
							AddHistoryEntry(bearing);
							_centerAttemptsForDestination++;
						}
						SwitchState(ActivityStates.TravelToDest);
					}
				}
				else
				{
					Log.LogText(LogLevel.DEBUG, "GOD DAMMIT! WE LOST IT!");
					Sleep(TimeSpan.FromSeconds(2.5));
					SwitchState(ActivityStates.FindInImage);
				}
			}
			catch(Exception e)
			{
				Log.LogText(LogLevel.WARNING, "FindInImage ENDING: {0}", e.Message);
				SwitchState(ActivityStates.Idle);
				result = false;
			}

			return result;
		}

		#endregion

		#region Travel To Destination

		protected bool RunTravelToDestState()
		{
			Log.SysLogText(LogLevel.DEBUG, "================ RunTravelToDestState {0}", CurrentColor);

			bool result = true;

			Double startBearing = Widgets.Instance.Compass.Bearing;
			Double startDistance = WaitForAdjustedRangeExact(startBearing, TimeSpan.FromSeconds(10));
			Log.SysLogText(LogLevel.DEBUG, "Movin slow {0}", startDistance.ToMetersString());
			Double metersToTravel = Math.Max(startDistance - ASSERTIVE_RANGE, ASSERTIVE_RANGE);
			TravelResult travelResult = Widgets.Instance.Tracks.MoveMeters(Direction.Forward, metersToTravel, Widgets.Instance.Tracks.Slow);
			if(travelResult == TravelResult.TravelTimeComplete)
			{
				History.Add(new BearingAndRange(startBearing, metersToTravel));
			}

			Sleep(1000);
			Double endDistance = Widgets.Instance.ImageEnvironment.GetRangeAtBearing(Widgets.Instance.Compass.Bearing) - Widgets.Instance.Chassis.LidarPosition.Y;

			Double range = 0;
			if((range = WaitForValidRangeExact(Widgets.Instance.Compass.Bearing, TimeSpan.FromSeconds(10))) != 0 && range < ARRIVAL_RANGE)
			{
				ArrivedAtDestination();
			}
			else
			{
				Log.LogText(LogLevel.DEBUG, "RANGE is now -------------- {0} -----------------", range.ToMetersString());
				SwitchState(ActivityStates.FindInImage);
			}

			return result;
		}

		#endregion

		#region Center In Image State

		protected bool InitReturnHomeState()
		{
			bool result = false;
			if(History.Count > 0)
			{
				_historyIndex = History.Count - 1;
				result = true;
			}
			return result;
		}

		protected bool RunReturnHomeState()
		{
			Interval = TimeSpan.Zero;
			bool result = true;

			Log.LogText(LogLevel.DEBUG, "=============== RunReturnHomeState");

			try
			{
				Double reciprocal = History[_historyIndex].Bearing.Reciprocal();
				Double distance = History[_historyIndex].Range;

				if(TryTurnToBearing(reciprocal) == false)
				{
					throw new ActivityException("Could not turn to bearing");
				}

				TravelResult travelResult = Widgets.Instance.Tracks.MoveMeters(Direction.Forward, distance, Widgets.Instance.Tracks.Slow);
				if(travelResult == TravelResult.TravelTimeComplete)
				{
					if(--_historyIndex < 0)
					{
						Log.SysLogText(LogLevel.DEBUG, "HEY... we think we are here!");
						SwitchState(ActivityStates.Success);
					}
				}
				else
				{
					Log.SysLogText(LogLevel.DEBUG, "Don't know what to do now");
					result = false;
				}

			}
			catch(Exception e)
			{
				Log.LogText(LogLevel.WARNING, "Return home ENDING: {0}", e.Message);
				SwitchState(ActivityStates.Idle);
				result = false;
			}

			return result;
		}

		#endregion

		#region Success

		protected bool RunSuccessState()
		{
			Log.LogText(LogLevel.DEBUG, "Success");
			return false;
		}


		#endregion

		#region Utility

		private void ArrivedAtDestination()
		{
			Log.LogText(LogLevel.DEBUG, "!!!!!!!!!!!!! We arrived at destination while in state {0}  !!!!!!!!!!!!!", ActivityState);
			if(++_colorIndex < Colors.Count)
			{
				ColorsTraveled.Add(CurrentColor);
				CurrentColor = Colors[_colorIndex];
				_centerAttemptsForDestination = 0;
				SpinDirection = SpinDirection.Opposite();
				SwitchState(ActivityStates.Init);
			}
			else
			{
				Log.LogText(LogLevel.DEBUG, "Goin home!");
				SwitchState(ActivityStates.ReturnHome);
				// SwitchState(ActivityStates.Success);
			}
		}

		private void AddHistoryEntry(Double bearing)
		{
			LEDTravelHistoryEntry entry = Program.Config.LEDTravelHistory.GetEntryForColor(CurrentColor, ColorsTraveled);
			if(entry == null)
			{
				entry = new LEDTravelHistoryEntry(bearing, CurrentColor, ColorsTraveled);
				Log.LogText(LogLevel.DEBUG, "Adding new travel history entry {0}", entry);
				Program.Config.LEDTravelHistory.Add(entry);
			}
			else
			{
				Log.LogText(LogLevel.DEBUG, "Changing travel history intial bearing from {0} to {1}", entry.Bearing.ToAngleString(), bearing.ToAngleString());
				entry.Bearing = bearing;
			}
			Program.Config.Save();

		}

		#endregion

	}
}
