using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Spatial.LidarImaging;

namespace TrackBot.Spatial
{
	class FindTwoLEDs : Activity
	{
		const Double FORWARD_COLLISION_WARNING = .15;
		const Double MAX_BEARING_DIFFERENTIAL = 5;
		const Double BEARING_INCREMENT = 20;

		public LEDPosition Green { get; set; }
		public LEDPosition Blue { get; set; }

		public Double StartBearing { get; set; }

		List<Double> BlueBearings { get; set; }
		List<Double> GreenBearings { get; set; }

		Double LegDistanceTraveled { get; set; }
		Double LegBearing { get; set; }

		public FindTwoLEDs()
			: base(ActivityType.FindTwoLEDs)
		{
			Interval = TimeSpan.FromMilliseconds(100);
			ActivityState = ActivityStates.Init;

			BlueBearings = new List<double>();
			GreenBearings = new List<double>();
		}

		protected override bool OnStop()
		{
			Widgets.Instance.Tracks.Stop();
			return base.OnStop();
		}

		#region Init State

		protected bool RunInitState()
		{
			Log.LogText(LogLevel.DEBUG, "=============== RunInitState");
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
			return true;
		}

		protected bool RunFindInImageState()
		{
			Log.LogText(LogLevel.DEBUG, "=============== RunFindInImage");

			Widgets.Instance.LEDImageAnalysis.AnalyzeImage();

			bool gotGreen = Widgets.Instance.LEDImageAnalysis.HasGreen;
			bool gotBlue = Widgets.Instance.LEDImageAnalysis.HasBlue;
			if(gotGreen && gotBlue)
			{
				Green = Widgets.Instance.LEDImageAnalysis.GreenLED;
				Blue = Widgets.Instance.LEDImageAnalysis.BlueLED;

				Double bearing = Widgets.Instance.Compass.Bearing;
				Double greenBearing, blueBearing;
				if( Widgets.Instance.LEDImageAnalysis.TryGetBearing(Color.Green, bearing, out greenBearing) &&
					Widgets.Instance.LEDImageAnalysis.TryGetBearing(Color.Blue, bearing, out blueBearing))
				{
					Log.LogText(LogLevel.DEBUG, "Found green LED at {0} [{1}]  and a blue LED at {2} [{3}]  Interior Angle {4}", 
						Green, greenBearing.ToAngleString(), Blue, blueBearing.ToAngleString(), 
						greenBearing.AngularDifference(blueBearing).ToAngleString());

					BlueBearings.Add(blueBearing);
					GreenBearings.Add(greenBearing);

					if(BlueBearings.Count == 1)
					{
						SwitchState(ActivityStates.FindSecondaryPoint);
					}
					else
					{
						Log.LogText(LogLevel.DEBUG, "Done for now BB1: {0}  GB1: {1}    BB2: {2}  GB2: {3}",
							BlueBearings[0].ToAngleString(), GreenBearings[0].ToAngleString(), BlueBearings[1].ToAngleString(), GreenBearings[1].ToAngleString());
						SwitchState(ActivityStates.Idle);
					}
				}
			}
			else
			{
				Log.LogText(LogLevel.DEBUG, "Green {0}  Blue {1}", gotGreen, gotBlue);

				if(Widgets.Instance.Tracks.TurnToBearing(Widgets.Instance.Compass.Bearing.AddDegrees(BEARING_INCREMENT)) == true)
				{
					Sleep(TimeSpan.FromSeconds(2.5));
				}
				else
				{
					Log.LogText(LogLevel.DEBUG, "Could not turn to bearing");
					SwitchState(ActivityStates.Idle);
				}
			}

			return true;
		}

		#endregion

		#region Find Secondary Point State

		protected bool InitFindSecondaryPointState()
		{
			Log.SysLogText(LogLevel.DEBUG, "================ InitFindSecondaryPoint");

			Double mid = Degrees.BearingBetween(GreenBearings[0], BlueBearings[0]);
			LegBearing = mid.SubtractDegrees(90);
			Log.SysLogText(LogLevel.DEBUG, "Mid-Bearing between {0} and {1} is {2} - turning to {3}",
				BlueBearings[0].ToAngleString(), GreenBearings[0].ToAngleString(), mid.ToAngleString(), LegBearing.ToAngleString());
			if(Widgets.Instance.Tracks.TurnToBearing(LegBearing) == true)
			{
				Sleep(TimeSpan.FromSeconds(4));

				Log.LogText(LogLevel.DEBUG, "Movin on");
			}
			else
			{
				Log.LogText(LogLevel.DEBUG, "Could not turn to bearing");
				SwitchState(ActivityStates.Idle);
			}
			return true;
		}

		protected bool RunFindSecondaryPointState()
		{
			Log.SysLogText(LogLevel.DEBUG, "================ RunFindSecondaryPoint");

			bool result = true;

			Double startDistance = Widgets.Instance.GetRangeAtDirection(Direction.Forward);
			Log.SysLogText(LogLevel.DEBUG, "Movin slow");
			Widgets.Instance.Tracks.MoveMeters(RaspiCommon.Direction.Forward, .3, Widgets.Instance.Tracks.Slow, true, true);

			Sleep(1000);
			Double endDistance = Widgets.Instance.GetRangeAtDirection(Direction.Forward);

			LegDistanceTraveled = startDistance - endDistance;
			Log.SysLogText(LogLevel.DEBUG, "Traveled {0} on leg", LegDistanceTraveled.ToMetersString());

			SwitchState(ActivityStates.FindInImage);

			return result;
		}

		#endregion

		#region Find Destination State

		protected bool RunFindDestinationState()
		{
			Log.SysLogText(LogLevel.DEBUG, "================ RunFindDestinationState");

			bool result = false;

			return result;
		}


		#endregion

		#region TravelToDest State

		bool InitTravelToDestState()
		{
			return true;
		}

		bool RunTravelToDestState()
		{
			bool result = true;

			return result;
		}

		#endregion

	}
}
