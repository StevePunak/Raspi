using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Performance;

namespace RaspiCommon.System
{
	public enum TimerTypes
	{
		FindDestination,
		FindShortestRange,
	}

	internal class Performance
	{
		public static Dictionary<TimerTypes, PerformanceTimer> Timers { get; private set; }

		static Performance()
		{
			Timers = new Dictionary<TimerTypes, PerformanceTimer>();
			foreach(Enum e in Enum.GetValues(typeof(TimerTypes)))
			{
				Timers.Add((TimerTypes)e, new PerformanceTimer(e));
			}
		}

	}
}
