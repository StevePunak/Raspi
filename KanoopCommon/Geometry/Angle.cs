using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public class Angle
	{
		Double m_Degrees;
		public Double Degrees { get { return m_Degrees; }  set { m_Degrees = value; } }
		
		public static Angle operator+(Angle a1, Angle a2)
		{
			Double d = a2.Degrees + a1.Degrees;
			if(d >= 360) d -= 360;
			return new Angle(d);
		}

		public static Angle operator-(Angle a1, Angle a2)
		{
			Double d = a1.Degrees - a2.Degrees;
			if(d < 0) d += 360;
			return new Angle(d);
		}

		public static Angle operator+(Angle angle, Double degrees)
		{
			return angle + new Angle(degrees);
		}

		public static Angle operator-(Angle angle, Double degrees)
		{
			return angle - new Angle(degrees);
		}

		public static Angle operator+(Angle angle, int degrees)
		{
			return angle + new Angle(degrees);
		}

		public static Angle operator-(Angle angle, int degrees)
		{
			return angle - new Angle(degrees);
		}

		public Angle(Double degrees)
		{
			m_Degrees = degrees;
		}

		public static Double Reverse(Double degrees)
		{
			return Add(degrees, 180);
		}

		public static Double Add(Double degrees, Double amount)
		{
			Double bearing = degrees + amount;
			if(bearing >= 360)
				bearing -= 360;
			return bearing;
		}

		public static Double Subtract(Double degrees, Double amount)
		{
			Double bearing = degrees - amount;
			if(bearing < 0)
				bearing += 360;
			return bearing;
		}

		public override string ToString()
		{
			return(String.Format("{0} deg", m_Degrees));
		}

	}
}
