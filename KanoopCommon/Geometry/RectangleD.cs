using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using KanoopCommon.Conversions;
using System.Drawing;
using KanoopCommon.Database;

namespace KanoopCommon.Geometry
{
	public class RectangleD : Polygon, IPolygon, IRectangle
	{
		#region Public Properties

		public LineList LineList { get { return new LineList(Lines); } }

		public PointDList PointList { get { return new PointDList(Points); } }

		public virtual Double Height 
		{ 
			get 
			{
				LineList lines = GetLinesForPoint(UpperLeft);
				return lines.LowerMost.Length;
			} 
		}

		public virtual Double Width 
		{ 
			get 
			{
				LineList lines = GetLinesForPoint(UpperLeft);
				return lines.RightMost.Length;
			}
		}

		public PointD UpperLeft
		{
			get
			{
				return LeftMost.OrderBy(p => p.Y).FirstOrDefault();
			}
		}

		public PointD LowerLeft
		{
			get
			{
				return LeftMost.OrderByDescending(p => p.Y).FirstOrDefault();
			}
		}

		public PointD UpperRight
		{
			get
			{
				return RightMost.OrderBy(p => p.Y).FirstOrDefault();
			}
		}

		public PointD LowerRight
		{
			get
			{
				return RightMost.OrderByDescending(p => p.Y).FirstOrDefault();
			}
		}

		public PointDList LeftMost
		{
			get
			{
				PointDList ordered = new PointDList(m_Points.OrderBy(p => p.X));
				PointDList list = new PointDList();
				list.Add(ordered[0]);
				if(ordered[1].X == ordered[0].X)
				{
					list.Add(ordered[1]);
				}
				return list;
			}
		}

		public PointDList RightMost
		{
			get
			{
				PointDList ordered = new PointDList(m_Points.OrderByDescending(p => p.X));
				PointDList list = new PointDList();
				list.Add(ordered[0]);
				if(ordered[1].X == ordered[0].X)
				{
					list.Add(ordered[1]);
				}
				return list;
			}
		}

		public PointDList UpperMost
		{
			get
			{
				PointDList ordered = new PointDList(m_Points.OrderBy(p => p.Y));
				PointDList list = new PointDList();
				list.Add(ordered[0]);
				if(ordered[1].Y == ordered[0].Y)
				{
					list.Add(ordered[1]);
				}
				return list;
			}
		}

		public PointDList LowerMost
		{
			get
			{
				PointDList ordered = new PointDList(m_Points.OrderByDescending(p => p.Y));
				PointDList list = new PointDList();
				list.Add(ordered[0]);
				if(ordered[1].Y == ordered[0].Y)
				{
					list.Add(ordered[1]);
				}
				return list;
			}
		}

		public ILine ShortestLeg
		{
			get
			{
				Line l1 = new Line(m_Points[0], m_Points[1]);
				Line l2 = new Line(m_Points[1], m_Points[2]);
				return l1.Length > l2.Length ? l2 : l1;
			}
		}

		public ILine LongestLeg
		{
			get
			{
				Line l1 = new Line(m_Points[0], m_Points[1]);
				Line l2 = new Line(m_Points[1], m_Points[2]);
				return l1.Length > l2.Length ? l1 : l2;
			}
		}


		#endregion

		#region Constructors

		public RectangleD(Rectangle rectangle)
			: this((Double)rectangle.X, (Double)rectangle.Y, (Double)rectangle.Width, (Double)rectangle.Height) {} 

		public RectangleD(Decimal x, Decimal y, Decimal width, Decimal height)
			: this((Double)x, (Double)y, (Double)width, (Double)height) {} 

		public RectangleD(int x, int y, int width, int height)
			: this((Double)x, (Double)y, (Double)width, (Double)height) {} 

		public RectangleD(Double x, Double y, Double width, Double height)
			: this(new PointD(x, y), new PointD(x + width, y), new PointD(x + width, y + height), new PointD(x, y + height)) {}

		public RectangleD(PointD p1, PointD p2, PointD p3, PointD p4)
			: this()
		{
			m_Points[0] = p1;
			m_Points[1] = p2;
			m_Points[2] = p3;
			m_Points[3] = p4;
		}

		public RectangleD()
		{
			m_Points = new PointDList { PointD.Empty, PointD.Empty, PointD.Empty, PointD.Empty };
		}

		#endregion

		#region Public Access Methods

		public static RectangleD Inflate(RectangleD original, Double xAmount, Double yAmount)
		{
			PointDList points = new PointDList();
			foreach(PointD point in original.PointList)
			{
				points.Add(new PointD(point.X * xAmount, point.Y * yAmount));
			}
			RectangleD newRect = new RectangleD(points[0], points[1], points[2], points[3]);
			return newRect;
		}

		#endregion

		#region Private Methods

		protected virtual void Recalculate()
		{
			/** build lines */
			m_Lines = new LineList()
			{ 
				new Line(m_Points[0], m_Points[1]),
				new Line(m_Points[1], m_Points[2]),
				new Line(m_Points[2], m_Points[3]),
				new Line(m_Points[3], m_Points[4])
			};

		}

		#endregion

		#region Utility

		public LineList GetLinesForPoint(PointD point)
		{
			LineList lines = new LineList();
			foreach(Line line in Lines)
			{
				if(line.P1.Equals(point) || line.P2.Equals(point))
				{
					lines.Add(line);
				}
			}
			return lines;
		}

		public Rectangle ToRectangle()
		{
			if(	Points[0].Y != Points[1].Y ||
				Points[0].X != Points[3].X ||
				Points[1].X != Points[2].X ||
				Points[3].Y != Points[2].Y)
			{
				throw new GeometryException("Can not convert a rectangle on an angle to a Windows rectangle");
			}

			return new Rectangle((int)Points[0].X, (int)Points[0].Y, (int)(Points[1].X - Points[0].X), (int)(Points[2].Y - Points[1].Y));
		}

		public RectangleD Clone()
		{
			PointD p0 = Points[0].Clone() as PointD;
			PointD p1 = Points[1].Clone() as PointD;
			PointD p2 = Points[2].Clone() as PointD;
			PointD p3 = Points[3].Clone() as PointD;
			return new RectangleD(p0, p1, p2, p3);
		}

		public Unescaped ToUnescapedSqlString()
		{
			return Unescaped.String(string.Format("GeomFromText('Polygon(({0} {1}, {2} {3}, {4} {5}, {6} {7}, {0} {1}))')",
				UpperLeft.X, UpperLeft.Y, UpperRight.X, UpperRight.Y, LowerRight.X, LowerRight.Y, LowerLeft.X, LowerLeft.Y));
		}

		public override string ToString()
		{
			return String.Format("X: {0} Y: {1} W: {2} H {3}", UpperLeft.X, UpperLeft.Y, Width, Height);
		}

		#endregion
	}
}
