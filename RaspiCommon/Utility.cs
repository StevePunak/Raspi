using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;

namespace RaspiCommon
{
	public static class Utility
	{
		public static SpinDirection GetClosestSpinDirection(Double from, Double to)
		{
			Double clockwiseDiff = Degrees.ClockwiseDifference(from, to);
			Double counterClockwiseDiff = Degrees.CounterClockwiseDifference(from, to);
			SpinDirection direction = clockwiseDiff < counterClockwiseDiff
				? SpinDirection.Clockwise : SpinDirection.CounterClockwise;
			return direction;
		}

		public static void LogSleep(String label, TimeSpan time)
		{
			int seconds = (int)time.TotalSeconds;
			if(seconds > 0)
			{
				do
				{
					Log.SysLogText(LogLevel.DEBUG, "{0}: Sleeping for {1} more seconds", label, seconds);
					Thread.Sleep(1000);
				} while(--seconds > 0);
			}
			else
			{
				Thread.Sleep(time);
			}
		}

	}
}
