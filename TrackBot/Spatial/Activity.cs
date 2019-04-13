#undef LOG_STATE_CHANGES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

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
			TravelToDest,
			RotateToNewBearing,
			Stuck
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

		public static void StartActivity(ActivityType activityType, bool waitForStateChange)
		{
			try
			{
				StopActivity();

				switch(activityType)
				{
					case ActivityType.GoToDestination:
						RunningActivity = new GoToDestination();
						RunningActivity.Start();
						RunningActivityType = ActivityType.GoToDestination;
						break;
					case ActivityType.TravelLongestPath:
						RunningActivity = new TravelLongestPath();
						RunningActivity.Start();
						RunningActivityType = ActivityType.TravelLongestPath;
						break;
				}

				if(RunningActivity != null)
				{
					RunningActivity.WaitForStateChange = waitForStateChange;
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
					Log.SysLogText(LogLevel.INFO, "Stopping previous running activity {0}", RunningActivity);
					RunningActivity.Stop();
				}
			}
		}

		protected override bool OnRun()
		{
			switch(ActivityState)
			{
				case ActivityStates.Init:
					RunInitState();
					break;
				case ActivityStates.Idle:
					RunIdleState();
					break;
				case ActivityStates.FindDestination:
					RunFindDestinationState();
					break;
				case ActivityStates.RotateToNewBearing:
					RunRotateToNewBearingState();
					break;
				case ActivityStates.TravelToDest:
					RunTravelToDestState();
					break;
				case ActivityStates.Stuck:
					RunStuckState();
					break;
				default:
					break;
			}
			return true;
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

		virtual protected bool RunInitState() { return false; }
		virtual protected bool RunFindDestinationState() { return false; }
		virtual protected bool RunRotateToNewBearingState() { return false; }
		virtual protected bool RunIdleState() { return false; }
		virtual protected bool RunTravelToDestState() { return false; }
		virtual protected bool RunStuckState() { return false; }
	}
}
