using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KanoopCommon.Crypto;

namespace KanoopCommon.Geometry
{
	internal static class PointCommon
	{
		public static int ComparePoints(object p1, object p2, int precision)
		{
			if(p1 is IPoint == false || p2 is IPoint == false)
			{
				throw new GeometryException("Both points compared must be of type IPoint");
			}

			int result;

			IPoint point1 = p1 as IPoint;
			IPoint point2 = p2 as IPoint;

			if(	(result = Math.Round(point1.X, precision).CompareTo(Math.Round(point2.X, precision))) == 0 &&
				(result = Math.Round(point1.Y, precision).CompareTo(Math.Round(point2.Y, precision))) == 0 )
			{
				/** result is 0, compare equal */
			}
			else
			{
				/** we will use the first non-equal value */
			}

			return result;
		}

		public static int GetHashCode(IPoint point)
		{
			return (int)MD5.HashStringAsUInt32(point.ToString());
		}

		public static bool Equals(IPoint thisPoint, object other)
		{
			if(other is IPoint == false)
			{
				throw new GeometryException("Other type to Equals must be IPoint");
			}

			return ((IPoint)other).X == thisPoint.X && ((IPoint)other).Y == thisPoint.Y;
		}
	}
}
