using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Threading;

namespace RaspiCommon
{
	public class GpioSharp
	{
		[DllImport("libgpiosharp.so")]
		public static extern void CloseLibrary();

		public static void Init()
		{

		}

		public static void DeInit()
		{
			CloseLibrary();

			foreach(ThreadBase thread in ThreadBase.GetRunningThreads())
			{
				if(thread.GetType() == typeof(HCSR04_RangeFinder.RangeFinderThread))
				{
					((HCSR04_RangeFinder.RangeFinderThread)thread).Parent.Stop();
				}
			}

			if(MotorDriver.Running)
			{
				MotorDriver.Stop();
			}
			if(Pigs.Running)
			{
				Pigs.Stop();
			}
		}

		[DllImport("libgpiosharp.so")]
		public static extern void DelayMicroseconds(UInt32 microseconds);

		public static void Sleep(TimeSpan time)
		{
			UInt64 microseconds = (UInt64)(time.Ticks / TimeSpanExtensions.MicrosecondsPerTick);
			DelayMicroseconds((UInt32)microseconds);
		}

		public static void Sleep(int milliseconds)
		{
			TimeSpan time = TimeSpan.FromMilliseconds(milliseconds);
			UInt64 microseconds = (UInt64)(time.Ticks / TimeSpanExtensions.MicrosecondsPerTick);
			DelayMicroseconds((UInt32)microseconds);
		}


	}
}
