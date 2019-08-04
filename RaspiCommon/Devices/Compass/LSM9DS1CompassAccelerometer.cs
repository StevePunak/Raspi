using System;
using System.Runtime.InteropServices;
using System.Threading;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using KanoopCommon.Threading;
using RaspiCommon.Spatial;

namespace RaspiCommon.Devices.Compass
{
	public class LSM9DS1CompassAccelerometer : ICompass
	{
		public event NewBearingHandler NewBearing;

		[StructLayout(LayoutKind.Sequential)]
		public struct MAG_INFO
		{
			public float gx;
			public float gy;
			public float gz;
			public float ax;
			public float ay;
			public float az;
			public float mx;
			public float my;
			public float mz;
		}

		[DllImport("libgpiosharp.so")]
		public static extern int LSM9D51_Calibrate();

		[DllImport("libgpiosharp.so")]
		public static extern int GetMagnetometer(ref MAG_INFO info);

		public Double MagneticDeviation { get; set; }

		Double _lastBearing;
		DateTime _lastBearingGetTime;

		static readonly TimeSpan MIN_FETCH_INTERVAL = TimeSpan.FromMilliseconds(1);

		public Double Bearing
		{
			get
			{
				Double value;
				try
				{
					_lock.Lock();

					if(DateTime.UtcNow > _lastBearingGetTime + MIN_FETCH_INTERVAL)
					{
						MAG_INFO info = new MAG_INFO();
						GetMagnetometer(ref info);
						value = Calculate(info.mx, info.my, info.mz);
						_lastBearing = value;

						Log.SysLogText(LogLevel.DEBUG, "Got new bearing from LSM9DS1 {0}", _lastBearing.ToAngleString());
						NewBearing(_lastBearing);

						_lastBearingGetTime = DateTime.UtcNow;
					}
				}
				finally
				{
					_lock.Unlock();
				}
				return _lastBearing;
			}
		}

		public Double XAdjust { get; set; }
		public Double YAdjust { get; set; }

		public Double MinX { get; private set; }
		public Double MinY { get; private set; }
		public Double MaxX { get; private set; }
		public Double MaxY { get; private set; }

		MutexLock _lock;

		public LSM9DS1CompassAccelerometer()
		{
			MinX = 99;
			MaxX = -99;
			MinY = 99;
			MaxY = -99;

			NewBearing += delegate { };

			_lock = new MutexLock();

			LSM9D51_Calibrate();
		}

		public void Calibrate()
		{
			try
			{
				_lock.Lock();

				//lsm9dsl_create();
				LSM9D51_Calibrate();

				for(int count = 0;count < 100;count++)
				{
					MAG_INFO info = new MAG_INFO();
					GetMagnetometer(ref info);
					Console.WriteLine("{0} {1}", info.mx, info.my);
					MinX = Math.Min(MinX, info.mx);
					MaxX = Math.Max(MaxX, info.mx);
					MinY = Math.Min(MinY, info.my);
					MaxY = Math.Max(MaxY, info.my);
					Log.SysLogText(LogLevel.DEBUG, "{0}    mx: {1}  my: {2}  minx {3} maxx {4} miny {5} maxy {6}", 100 - count, info.mx, info.my, MinX, MaxX, MinY, MaxY);
					Thread.Sleep(TimeSpan.FromSeconds(.1));
				}

				Double xrange = (MaxX - MinX) / 2;
				XAdjust = -(MaxX - xrange);
				Double yrange = (MaxY - MinY) / 2;
				YAdjust = -(MaxY - yrange);

				Log.SysLogText(LogLevel.DEBUG, "xrange {0}  X adjust {1}  yrange {2}  Y adjust {3}", xrange, XAdjust, yrange, YAdjust);
			}
			finally
			{
				_lock.Unlock();
			}

		}

		public Double Calculate(Double inx, Double iny, Double inz)
		{
//			Log.SysLogText(LogLevel.DEBUG, "Calc bearing");

			Double adX = inx + XAdjust;
			Double adY = iny + YAdjust;

			Double angle = Math.Atan2(adY, adX);
			Double degrees = (angle * 180) / Math.PI;

			if(degrees < 0)
				degrees += 360;

			degrees = degrees.AddDegrees(MagneticDeviation);

			//Log.SysLogText(LogLevel.DEBUG, "X {0}  Y {1}  AdjX {2}  AdjY {3}  Degrees: {4:0.00}°  MagDev: {5}°   xadj: {6}  yadj: {7}",
			//	inx, iny, adX, adY, degrees, MagneticDeviation, XAdjust, YAdjust);

			return degrees;
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}
	}
}
