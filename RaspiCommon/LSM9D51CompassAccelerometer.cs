using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon
{
	public class LSM9D51CompassAccelerometer
	{
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

		public Double Degrees
		{
			get
			{
				MAG_INFO info = new MAG_INFO();
				GetMagnetometer(ref info);
				return Calculate(info.mx, info.my, info.mz);
			}
		}

		public Double XAdjust { get; set; }
		public Double YAdjust { get; set; }

		public Double MinX { get; private set; }
		public Double MinY { get; private set; }
		public Double MaxX { get; private set; }
		public Double MaxY { get; private set; }

		public LSM9D51CompassAccelerometer()
		{
			MinX = 99;
			MaxX = -99;
			MinY = 99;
			MaxY = -99;

		}

		public void Calibrate()
		{
			//lsm9dsl_create();
			LSM9D51_Calibrate();

			for(int x = 0;x < 100;x++)
			{
				MAG_INFO info = new MAG_INFO();
				GetMagnetometer(ref info);
				MinX = Math.Min(MinX, info.mx);
				MaxX = Math.Max(MaxX, info.mx);
				MinY = Math.Min(MinY, info.my);
				MaxY = Math.Max(MaxY, info.my);
				Console.WriteLine("{0} minx {1} maxx {2} miny {3} maxy {4}", 50 - x, MinX, MaxX, MinY, MaxY);
				GpioSharp.Sleep(TimeSpan.FromSeconds(.1));
			}

			Double xrange = (MaxX - MinX) / 2;
			XAdjust = -(MaxX - xrange);
			Double yrange = (MaxY - MinY) / 2;
			YAdjust = -(MaxY - yrange);

			Console.WriteLine("xrange {0}  X adjust {1}  yrange {2}  Y adjust {3}", xrange, XAdjust, yrange, YAdjust);
		}

		public Double Calculate(Double x, Double y, Double z)
		{
			x += XAdjust;
			y += YAdjust;
			Double angle = Math.Atan2(y, x);
			Double degrees = (angle * 180) / Math.PI;
			degrees += MagneticDeviation;
			if(degrees < 0)
			{
				degrees += 360;
			}
			// Console.WriteLine("y {0}  x{1}  Degrees: {2}", y, x, Degrees);
			return degrees;
		}
	}
}
