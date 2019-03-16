using KanoopCommon.CommonObjects;
using KanoopCommon.Conversions;
using KanoopCommon.Database;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System;

namespace KanoopCommon.Geometry
{

    public class PointDListList : List<PointDList> { };

	[Serializable]
	public class PointDList : List<PointD>
	{
		#region Constructors

		public PointDList()
			: base() {}

		public PointDList(IEnumerable<PointD> points)
			: this()
		{
			AddRange(points);
		}

		public PointDList(IEnumerable<IPoint> points)
		{
			foreach(IPoint point in points)
			{
				Add(new PointD(point));
			}
		}

		#endregion

		#region Conversions

		public LineList ToLineList()
		{
			LineList list = new LineList();

			for(int x = 0; x < Count - 1; x++)
			{
				list.Add(new Line(this[x], this[x + 1]));
			}
			return list;
		}

		public static Unescaped ToUnescapedSQLString(PointDList pnts)
		{
			Unescaped retVal = Unescaped.String("NULL");
			if (pnts != null & pnts.Count > 0)
			{
				retVal = Unescaped.String(pnts.ToSQLString());
			}
			return retVal;
		}

		public Unescaped ToSQLString()
		{
			String	strReturn = "PolyFromText('POLYGON((";
			string	strComma = "";

			PointD	ptFirst = this[0];
			strReturn += ptFirst.X + " " + ptFirst.Y;

			strComma = ",";

			for(int i = 1; i < this.Count; i++)
			{
				strReturn += strComma + this[i].X + " " + this[i].Y;
				strComma = (ptFirst == this[i]) ? "" : ", ";
			}
			if (this.Count > 0)
			{
				int intLastIndex = this.Count - 1;
				if ((this[0].X != this[intLastIndex].X) ||
				    (this[0].Y != this[intLastIndex].Y))
				{
					strReturn += strComma + this[0].X + " " + this[0].Y;
				}
			}
			strReturn += "))')";
			return Unescaped.String(strReturn);
		}

		#endregion

		#region Obsolete Methods

		[Obsolete("Replaced by Polygon")]
		public void ClosePolygon()
		{
			// Close off polygon
			if (this.Count > 0)
			{
				int intLastIndex = this.Count - 1;
				if( (this[0].X != this[intLastIndex].X) &&
				    (this[0].Y != this[intLastIndex].Y))
				{
					this.Add(this[0]);
				}
			}
		}

		[Obsolete("Replaced by Polygon")]
		public Double Area
		{
			get
			{
				int		i = 0;
				int		j = this.Count - 1;
				Double	area = 0;

				for (i = 0; i < this.Count; j=i++)
				{
					area += this[i].X * this[j].Y;
					area -= this[i].Y * this[j].X;
				}
				area /= 2.0;

				return (area);
			}
		}

		[Obsolete("Replaced by Polygon")]
		public PointD Centroid
		{
			get
			{
				int		i, j = this.Count-1;
				Double	factor = 0;
				Double	cx = 0, cy = 0;

				for (i = 0; i < this.Count; j=i++)
				{
					factor = (this[i].X * this[j].Y - this[j].X * this[i].Y);
					cx += (this[i].X + this[j].X) * factor;
					cy += (this[i].Y + this[j].Y) * factor;
				}

				factor = Area * 6.0;
				return new PointD(cx / factor, cy / factor);
			}
		}

		[Obsolete("Replaced by Polygon")]
		public void MoveGeo(Double bearing, Double distance)
		{
			GeoPointList list = new GeoPointList();

			foreach(PointD point in this)
			{
				list.Add(new GeoPoint(point));
			}
			Clear();

			foreach(GeoPoint point in list)
			{
				Add(new PointD(point));
				point.Move(bearing, distance);
			}
		}

		#endregion

		#region Utility

		public PointDList Clone()
		{
			PointDList list = new PointDList();
			this.ForEach(p => list.Add(p.Clone() as PointD));
			return list;
		}

		#endregion
	}

	[Serializable]	// Needed for Web Client
	public class PointD : IPoint, IPoint2DReadOnly
	{
		#region Public Properties

		Double	m_X;
		[ColumnName("X_POS","LONGITUDE")]
		public Double X
		{
			get { return m_X; }
			set { m_X = value; }
		}

		Double	m_Y;
		[ColumnName("Y_POS", "LATITUDE")]
		public Double Y
		{
			get { return m_Y; }
			set { m_Y = value; }
		}

		int		m_Precision;
		public int Precision
		{
			get { return m_Precision; }
			set
			{
				m_Precision = value;
				m_X = Math.Round(m_X, value);
				m_X = Math.Round(m_X, value);
			}
		}

		public String Name { get; set; }

		public String HashName { get { return String.Format("{0:0.000000}, {1:0.000000}", m_X, m_Y); } }

		#endregion

		#region Constructors

		public PointD()
			: this(0, 0)	{}

		public PointD(Point p)
			: this(p.X, p.Y) {}

		public PointD(int x, int y)
			: this((Double)x, (Double)y) {}

		public PointD(IPoint2DReadOnly p)
			: this(p.X, p.Y) {}

		public PointD(Double x, Double y)
		{
			m_X = x;
			m_Y = y;
			Name = String.Empty;
		}

		#endregion

		#region Public Geometry Methods

		public static PointD RelativeTo(PointD point, Double bearing, Double distance)
		{
			PointD ret = point.Clone() as PointD;
			ret.Move(bearing, distance);
			return ret;
		}

		public IPoint Round(Int32 places)
		{
			return new PointD(Math.Round(X, places), Math.Round(Y, places));
		}

		public void Move(Double bearing, Double distance)
		{
			PointD np = FlatGeo.GetPoint(this, bearing, distance);
			m_X = np.X;
			m_Y = np.Y;
		}

		public void Move(IPoint where)
		{
			m_Y = where.Y;
			m_X = where.X;
		}

		public void Rotate(PointD centroid, Double angle)
		{
			m_X += (0 - centroid.X);
			m_Y += (0 - centroid.Y);

			PointD np = new PointD();
			np.X = ((m_X * Math.Cos(angle * (Math.PI / 180))) - (m_Y * Math.Sin(angle * (Math.PI / 180)))) + centroid.X;
			np.Y = (Math.Sin(angle * (Math.PI / 180)) * m_X + Math.Cos(angle * (Math.PI / 180)) * m_Y) + centroid.Y;

			m_X = np.X;
			m_Y = np.Y;
		}

		public void Scale(Double scale)
		{
			m_X *= scale;
			m_Y *= scale;
		}

		public IPoint GetPointAt(Double bearing, Double distance)
		{
			return FlatGeo.GetPoint(this, bearing, distance);
		}

		public static PointD Scale(PointD point, Double scale)
		{
			return new PointD(point.X * scale, point.Y * scale);
		}

		public bool IsLeftOf(PointD other)
		{
			return m_X < other.m_X;
		}

		public bool IsRightOf(PointD other)
		{
			return m_X > other.m_X;
		}

		public bool IsAbove(PointD other)
		{
			return m_Y < other.m_Y;
		}

		public bool IsLowerThan(PointD other)
		{
			return m_Y > other.m_Y;
		}

		#endregion

		#region Utility

		public static PointD Empty
		{
			get { return new PointD(0, 0); }
		}

		public Unescaped ToUnescapedSQLString()
		{
			return ToUnescapedSQLString(this);
		}

		public static Unescaped ToUnescapedSQLString(PointD pnt)
		{
			Unescaped retVal = Unescaped.String("NULL");
			if (pnt != null)
			{
				retVal = Unescaped.String(pnt.ToSQLString());
			}
			return retVal;
		}

		public string ToSQLString()
		{
			//return String.Format("GeomFromText('POINT({0:0.000000} {1:0.000000})')", m_X, m_Y);
			return "GeomFromText('POINT("+m_X+" "+m_Y+")')";
		}

		public Point ToPoint()
		{
			return new Point((int)Math.Round(X), (int)Math.Round(Y));
		}

		public GeoPoint ToGeoPoint()
		{
			return new GeoPoint(this);
		}

		public PointD ToPointD()
		{
			return new PointD(this);
		}

		public static bool TryParse(String str, out PointD point)
		{
			point = null;

			String[]	parts = str.Split(',');
			Double		x, y;
			if(parts.Length == 2 && Parser.TryParse(parts[0], out x) && Parser.TryParse(parts[1], out y))
			{
				point = new PointD(x, y);
			}

			return point != null;
		}

		public bool Equals(IPoint other)
		{
			return Equals(other, 0);
		}

		public bool Equals(IPoint other, int precision = 0)
		{
			bool result = false;
			if(precision == 0)
				result = X == other.X && Y == other.Y;
			else
				result = Math.Round(X, precision) == Math.Round(other.X, precision) && Math.Round(Y, precision) == Math.Round(other.Y, precision);
			return result;
		}

		public override int GetHashCode()
		{
			return PointCommon.GetHashCode(this);
		}

		public override bool Equals(object other)
		{
			return PointCommon.Equals(this, other);
		}

		public int CompareTo(object other)
		{
			return PointCommon.ComparePoints(this, other, Precision);
		}

		public IPoint Clone()
		{
			return new PointD(m_X, m_Y);
		}

		public String ToString(int precision)
		{
			String	format = String.Format("F{0}", precision);
			String	name = String.IsNullOrEmpty(Name) ? String.Empty : String.Format(" {0}", Name);
			return String.Format("{0}, {1}{2}", m_X.ToString(format), m_Y.ToString(format), name);
		}

		public override string ToString()
		{
			return ToString(0);
		}

		#endregion
	}
}
