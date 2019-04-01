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
			Stuck
		}

		public ActivityStates ActivityState { get; protected set; }

		protected DateTime EnterStateTime { get; set; }

		static Activity()
		{
			RunningActivityType = ActivityType.None;
		}

		protected Activity(ActivityType type)
			: base(type.ToString())
		{
			ActivityType = type;
		}

		public static void StartActivity(ActivityType activityType)
		{
			try
			{
				StopActivity();

				switch(activityType)
				{
					case ActivityType.RoamAndSeek:
						RunningActivity = new RoamAndSeek();
						RunningActivity.Start();
						RunningActivityType = ActivityType.RoamAndSeek;
						break;
					case ActivityType.TravelLongestPath:
						RunningActivity = new TravelLongestPath();
						RunningActivity.Start();
						RunningActivityType = ActivityType.TravelLongestPath;
						break;
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

			Console.WriteLine("{0} Switching state from {1} to {2}", Name, ActivityState, state);

			try
			{
				String methodName;

				// deinit last state
				method = GetStateMethod(StateTransition.Stop, ActivityState, out methodName);
				if(method != null)
				{
					Log.LogText(LogLevel.DEBUG, "Invoking '{0}'", methodName);
					if((result = (bool)method.Invoke(this, null)) == false)
					{
						throw new TrackBotException("Method {0} return false", method);
					}
				}
				else
				{
					Log.LogText(LogLevel.DEBUG, "No method exists called '{0}'... continuing", methodName);
				}

				ActivityState = state;
				EnterStateTime = DateTime.UtcNow;

				// init new state
				method = GetStateMethod(StateTransition.Init, ActivityState, out methodName);
				if(method != null)
				{
					Log.LogText(LogLevel.DEBUG, "Invoking '{0}'... continuing", methodName);
					if((result = (bool)method.Invoke(this, null)) == false)
					{
						throw new TrackBotException("Method {0} return false", method);
					}
				}
				else
				{
					Log.LogText(LogLevel.DEBUG, "No method exists called '{0}'... continuing", methodName);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("EXCEPTION {0}: {1}", GetType().Name, e.Message);
				ActivityState = ActivityStates.Idle;
			}
		}

		virtual protected bool RunInitState() { return false; }
		virtual protected bool RunFindDestinationState() { return false; }
		virtual protected bool RunIdleState() { return false; }
		virtual protected bool RunTravelToDestState() { return false; }
		virtual protected bool RunStuckState() { return false; }
	}
}
