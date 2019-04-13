using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Performance;

namespace TrackBot.System
{
	public enum TimerTypes
	{
		FindDestination
	}

	internal class Performance
	{
		public static Dictionary<TimerTypes, PerformanceTimer> Timers { get; private set; }

		static Performance()
		{
			Timers = new Dictionary<TimerTypes, PerformanceTimer>()
			{
				{ TimerTypes.FindDestination,       new PerformanceTimer(TimerTypes.FindDestination) },
			};
		}

	}
}
