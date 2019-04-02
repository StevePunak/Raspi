using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KanoopCommon.Database;
using System.Drawing;
using KanoopCommon.Conversions;

namespace KanoopCommon.Geometry
{


	public interface IPoint : IComparable, IEquatable<IPoint>, IPoint2DReadOnly
	{
		#region Properties

		String HashName { get; }

		String Name { get; }
// Now obtained from IPoint2DReadOnly
//		Double X { get; }

//		Double Y { get; }

		int Precision { get; set; }

		#endregion

		#region Methods

		IPoint Clone();

		bool Equals(IPoint other, int precision);
	
		IPoint GetPointAt(double bearing, double distance);

		void Move(Double bearing, Double distance);

		void Move(IPoint where);

		IPoint Round(Int32 places);

		Point ToPoint();

		PointD ToPointD();

		GeoPoint ToGeoPoint();

		Unescaped ToUnescapedSQLString();

		String ToString(int precision);

		#endregion
	}
}
