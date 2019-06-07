#define LOG_STATE_CHANGES
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using RaspiCommon;
using RaspiCommon.Devices.Optics;

namespace TrackBot.Spatial
{
	public abstract class Activity : ThreadBase
	{
		#region Static Properties

		public static ActivityType RunningActivityType { get; private set; }
		public static Activity RunningActivity { get; private set; }

		#endregion

		protected enum StateTransition
		{
			Init,
			Run,
			Stop
		}

		public ActivityType ActivityType { get; private set; }

		public enum ActivityStates
		{
			Init,
			Idle,
			FindDestination,
			FindSecondaryPoint,
			TravelToDest,
			RotateToNewBearing,
			Stuck,
			FindInImage,
			CenterInImage,
			Success,
			ReturnHome,
		}

		public ActivityStates ActivityState { get; protected set; }

		public bool WaitForStateChange { get; private set; }

		public bool QuitRequest { get; set; }

		protected DateTime EnterStateTime { get; set; }

		static MutexEvent _changeStateEvent;

		static Activity()
		{
			RunningActivityType = ActivityType.None;
			_changeStateEvent = new MutexEvent();
		}

		protected Activity(ActivityType type)
			: base(type.ToString())
		{
			ActivityType = type;
			QuitRequest = false;
		}

		public static void StartActivity(ActivityType activityType, object[] args, bool promptForStateChange)
		{
			Log.SysLogText(LogLevel.DEBUG, "Start {0}", activityType);

			try
			{
				StopActivity();

				foreach(Type type in Assembly.GetExecutingAssembly().GetTypes())
				{
					if(type.IsSubclassOf(typeof(Activity)) && type.Name.Equals(activityType.ToString(), StringComparison.InvariantCultureIgnoreCase))
					{
						RunningActivity = Activator.CreateInstance(type) as Activity;
						if(RunningActivity.ParseArgs(args))
						{
							RunningActivityType = RunningActivity.ActivityType;
							RunningActivity.WaitForStateChange = promptForStateChange;
							RunningActivity.Start();
							break;
						}
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("Start activity {0} EXCEPTION: {0}", e.Message);
			}
		}

		public static void StopActivity()
		{
			if(RunningActivityType != ActivityType.None)
			{
				if(RunningActivity.State == ThreadState.Started)
				{
					Log.SysLogText(LogLevel.INFO, "Stopping previous running activity {0}", RunningActivityType);
					RunningActivityType = ActivityType.None;
					RunningActivity.Stop();
					RunningActivity = null;
				}
			}
		}

		protected override bool OnRun()
		{
			bool result = false;

			Log.LogText(LogLevel.DEBUG, "OnRun {0} in state: {1}", this, ActivityState);
			String runMethodName;
			MethodInfo method = GetStateMethod(StateTransition.Run, ActivityState, out runMethodName);
			if(method != null)
			{
				if((result = (bool)method.Invoke(this, null)) == false)
				{
					Log.LogText(LogLevel.DEBUG, "Run method {0} returned false", runMethodName);
				}
			}
			return result;
		}

		protected MethodInfo GetStateMethod(StateTransition transition, Enum state, out String methodName)
		{
			MethodInfo result = null;
			methodName = String.Format("{0}{1}State", transition, state);

			foreach(MethodInfo method in this.GetType().GetRuntimeMethods())
			{
				if(method.Name == methodName)
				{
					result = method;
					break;
				}
			}
			return result;

		}

		protected void SwitchState(ActivityStates state)
		{
			MethodInfo method;
			bool result;

			try
			{
				Log.LogText(LogLevel.DEBUG, "{0} Switching state from {1} to {2}", Name, ActivityState, state);
				if(WaitForStateChange)
				{
					Log.LogText(LogLevel.DEBUG, "Waiting for input to switch state");
					_changeStateEvent.Wait();
					if(QuitRequest)
					{
						throw new TrackBotException("Terminating activity");
					}
				}

				String methodName;

				// deinit last state
				method = GetStateMethod(StateTransition.Stop, ActivityState, out methodName);
				if(method != null)
				{
#if  LOG_STATE_CHANGES
					Log.LogText(LogLevel.DEBUG, "Invoking '{0}'", methodName);
#endif
					if((result = (bool)method.Invoke(this, null)) == false)
					{
						throw new TrackBotException("Method {0} return false", method);
					}
				}
				else
				{
#if LOG_STATE_CHANGES
					Log.LogText(LogLevel.DEBUG, "No method exists called '{0}'... continuing", methodName);
#endif
				}

				ActivityState = state;
				EnterStateTime = DateTime.UtcNow;

				// init new state
				method = GetStateMethod(StateTransition.Init, ActivityState, out methodName);
				if(method != null)
				{
#if LOG_STATE_CHANGES
					Log.LogText(LogLevel.DEBUG, "Invoking '{0}'... continuing", methodName);
#endif
					if((result = (bool)method.Invoke(this, null)) == false)
					{
						throw new TrackBotException("Method {0} return false", method);
					}
				}
				else
				{
#if LOG_STATE_CHANGES
					Log.LogText(LogLevel.DEBUG, "No method exists called '{0}'... continuing", methodName);
#endif
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("EXCEPTION {0}: {1}", GetType().Name, e.Message);
				ActivityState = ActivityStates.Idle;
			}
		}

		public static void ChangeState()
		{
			Log.SysLogText(LogLevel.DEBUG, "Change state request (Quit = {0})", RunningActivity.QuitRequest);
			_changeStateEvent.Set();
		}

		virtual protected bool ParseArgs(object[] args)
		{
			return true;
		}

		/// <summary>
		/// Wait for the given time fo a valid range to show up in the Lidar
		/// </summary>
		/// <param name="howLong"></param>
		/// <returns></returns>
		protected Double WaitForValidRangeExact(Double bearing, TimeSpan howLong)
		{
			Log.LogText(LogLevel.DEBUG, "WaitForValidRangeExact at {0} for {1}", bearing.ToAngleString(), howLong.ToAbbreviatedFormat(true));
			Double distance = 0;
			DateTime startTime = DateTime.UtcNow;
			while(DateTime.UtcNow < startTime + howLong)
			{
				distance = Widgets.Instance.ImageEnvironment.GetRangeAtBearing(bearing);
				Log.LogText(LogLevel.DEBUG, ">>>> Distance at {0} is {1}", bearing.ToAngleString(), distance.ToMetersString());
				if(distance > 0)
				{
					break;
				}
				Sleep(250);
			}
			return distance;
		}

		/// <summary>
		/// Wait for range after adjusting for lidar position
		/// </summary>
		/// <param name="bearing"></param>
		/// <param name="waitTime"></param>
		/// <returns></returns>
		protected Double WaitForAdjustedRangeExact(Double bearing, TimeSpan waitTime)
		{
			Double range = Math.Max(WaitForValidRangeExact(bearing, waitTime) - Widgets.Instance.Chassis.LidarPosition.Y, 0.1);
			return range;
		}

		/// <summary>
		/// Turn to bearing with retries
		/// </summary>
		/// <param name="bearing"></param>
		/// <returns></returns>
		protected bool TryTurnToBearing(Double bearing)
		{
			int tries = 3;
			Direction correct = Direction.Forward;

			do
			{
				if(Widgets.Instance.Tracks.TurnToBearing(bearing, SpinDirection.None, Program.Config.StandardSpeed) == true)
				{
					break;
				}

				Log.LogText(LogLevel.DEBUG, "Failed to turn... correcting ({0} attempts left", tries);
				correct = correct == Direction.Forward ? Direction.Backward : Direction.Forward;
				Sleep(1000);
				Widgets.Instance.Tracks.MoveMeters(correct, .2, Widgets.Instance.Tracks.Slow);
				Sleep(1000);
			} while(--tries > 0);
			return tries > 0;
		}

		protected bool TryGetLEDPosition(Color color,  out LEDPosition ledPosition)
		{
			ledPosition = null;
			Mat image;
			if(!Widgets.Instance.Camera.TryTakeSnapshot(out image))
			{
				throw new ActivityException("Could not get new image");
			}

			Widgets.Instance.LEDImageAnalysis.AnalyzeImage(image, color);

			if(Widgets.Instance.LEDImageAnalysis.HasColor(color))
			{
				ledPosition = Widgets.Instance.LEDImageAnalysis.GetColor(color);
			}
			return ledPosition != null;
		}
	}
}
