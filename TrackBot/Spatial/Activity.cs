using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

		protected abstract override bool OnRun();

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
	}
}
