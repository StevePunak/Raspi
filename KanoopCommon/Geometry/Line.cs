using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System;
using System.ComponentModel;

namespace KanoopCommon.Geometry
{
	public class LineList : List<Line>, IEnumerable<Line>
	{
		#region Constructors

		public LineList()
			: base() {}

		public LineList(IEnumerable<Line> lines)
			: base(lines) {}

		public LineList(IEnumerable<ILine> lines)
		{
			foreach(ILine line in lines)
			{
				Add(new Line(line));
			}
		}

		public LineList(PointDList points)
			: this(points.ToLineList()) {}

		#endregion

		#region Public Geometry Methods

		public PointD ClosestPointTo(IPoint other, out Line closestLine, out Double closestDistance)
		{
			closestDistance = Double.MaxValue;
			PointD closestPoint = null;
			closestLine = null;

			foreach(Line line in this)
			{
				Double	distance;
				PointD	p = line.ClosestPointTo(other, out distance);
				if(distance < closestDistance)
				{
					closestDistance = distance;
					closestPoint = p;
					closestLine = line;
				}
			}
			return closestPoint;
		}

		public Line LeftMost
		{
			get
			{
				Line result = null;
				foreach(Line line in this)
				{
					if(result == null || line.IsLeftOf(result))
					{
						result = line;
					}
				}
				return result;
			}
		}

		public Line RightMost
		{
			get
			{
				Line result = null;
				foreach(Line line in this)
				{
					if(result == null || line.IsRightOf(result))
					{
						result = line;
					}
				}
				return result;
			}
		}

		public Line UpperMost
		{
			get
			{
				Line result = null;
				foreach(Line line in this)
				{
					if(result == null || line.IsAbove(result))
					{
						result = line;
					}
				}
				return result;
			}
		}

		public Line LowerMost
		{
			get
			{
				Line result = null;
				foreach(Line line in this)
				{
					if(result == null || line.IsBelow(result))
					{
						result = line;
					}
				}
				return result;
			}
		}

		public Line Shortest
		{
			get
			{
				Line line = this.Aggregate((l1, l2) => l1.Length < l2.Length ? l1 : l2);
				return line;
			}
		}

		#endregion

		#region Utility

		public LineList Clone()
		{
			LineList lines = new LineList();

			foreach(Line line in this)
			{
				lines.Add(new Line(line.P1.Clone(), line.P2.Clone()));
			}
			return lines;
		}

		#endregion
	}

	public class Line : ILine
	{
		#region Public Properties

		protected PointD	_p1;
		public IPoint P1
		{
			get { return _p1; }
			set { _p1 = value as PointD; }
		}

		protected PointD	_p2;
		public IPoint P2
		{
			get { return _p2; }
			set { _p2 = value as PointD; }
		}

		public List<PointD> Points { get { return new List<PointD>(){ _p1, _p2 }; } }

		public IPoint MidPoint
		{
			get
			{
				return new PointD(	_p2.X - ((_p2.X - _p1.X) / 2),
				                    _p2.Y - ((_p2.Y - _p1.Y) / 2));
			}
		}

		public virtual Double Length
		{
			get
			{
				Double	x = _p1.X - _p2.X;
				Double	y = _p1.Y - _p2.Y;
				return(Math.Sqrt((x*x) + (y*y)));
			}
		}

		public String Name { get; set; }

		public Double Slope
		{
			get
			{
				Double	ret = 0;
				Double	y = _p2.Y - _p1.Y;
				Double	x = _p2.X - _p1.X;
				if(y == 0)
				{
					ret = Double.PositiveInfinity;
					m_bVertical = false;
					m_bHorizontal = true;
				}
				else if(x == 0)
				{
					ret = Double.NegativeInfinity;
					m_bVertical = true;
					m_bHorizontal = false;
				}
				else
				{
					ret = y / x;
				}
				return ret;
			}
		}

		public Double Intercept
		{
			get
			{
				Double	ret = Double.NaN;
				Double	slope = Slope;
				/** not vertical */
				if(ret != Double.NegativeInfinity)
				{
					ret = _p1.Y - (_p1.X * slope);
				}
				return ret;
			}
		}

		bool	m_bVertical;
		public bool IsVertical
		{
			get
			{
				if(Slope == Double.PositiveInfinity) {}
				return m_bVertical;
			}
		}

		bool	m_bHorizontal;
		public bool IsHorizontal
		{
			get
			{
				if(Slope == Double.PositiveInfinity) {}
				return m_bHorizontal;
			}
		}

		public Double Bearing
		{
			get
			{
				/**
				 * get a new line and move it down one unit
				 */
				Line	newLine = this.Clone() as Line;
				newLine.Move(180, 1);

				Line	l = new Line(newLine.P1, new PointD(newLine.P1.X, newLine.P1.Y - 1));
				Double	d = FlatGeo.VectorAngle(l, newLine);
				Angle	angle = new Angle(d);
				if(_p2.X < _p1.X)
				{
					angle.Degrees = 360 - angle.Degrees;
				}
				//Line l = new Line(m_P1, new PointD(m_P1.X + 100, m_P1.Y));
				//Angle angle = new Angle(FlatGeo.VectorAngle(l, this));

				/**
				 * add 90 to get it in the right orientation
				 */
				//angle.Degrees += 90;
				//if(m_P1.Y >
				return angle.Degrees;
			}
		}

		public Object Tag { get; set; }

		#endregion

		#region Constructors

		public Line(PointD p1, PointD p2)
		{
			_p1 = p1;
			_p2 = p2;
			Tag = null;
		}

		public Line(ILine line)
			: this(line.P1, line.P2) {}

		public Line(IPoint p1, IPoint p2)
			: this(p1.X, p1.Y, p2.X, p2.Y) {}

		public Line(Double x1, Double y1, Double x2, Double y2)
			: this(new PointD(x1, y1), new PointD(x2, y2)) {}

		public Line(PointD origin, Double bearing, Double distance)
			: this(origin, FlatGeo.GetPoint(origin, bearing, distance)) {}

		public Line(Point p1, Point p2)
			: this(new PointD(p1), new PointD(p2)) {}

		public Line()
			: this(new PointD(0, 0), new PointD(0, 0))	{}

		#endregion

		#region Public Geometery Metods

		public Double DistanceTo(PointD to)
		{
			Double	ret = Double.PositiveInfinity;

			/** make a single leg of a triangle between the remote point and one point on this line */
			Line	leg = new Line(P1, to);

			/** get the interior angle */
			Double	angle = FlatGeo.Angle(to, leg.P1, leg.P2).Degrees;
			if(angle >= 180)
			{
				angle -= 180;
			}

			if(angle != 0)
			{
				/** find the distance to the closest point on the line */
				ret = Math.Abs(Math.Sin(angle) * leg.Length);
			}
			else
			{
				/** the point is on the line */
				ret = 0;
			}

			return ret;
		}

		public PointD ClosestPointTo(IPoint pt)
		{
			Double distance;
			return ClosestPointTo(pt, out distance);
		}

		public PointD ClosestPointTo(IPoint pt, out Double distance)
		{
			PointD	closest;
			Double	dx = P2.X - P1.X;
			Double	dy = P2.Y - P1.Y;

			// Calculate the t that minimizes the distance.
			Double	t = ((pt.X - P1.X) * dx + (pt.Y - P1.Y) * dy) / (dx * dx + dy * dy);

			// See if this represents one of the segment's
			// end points or a point in the middle.
			if (t < 0)
			{
				closest = new PointD(P1.X, P1.Y);
				dx = pt.X - P1.X;
				dy = pt.Y - P1.Y;
			}
			else if (t > 1)
			{
				closest = new PointD(P2.X, P2.Y);
				dx = pt.X - P2.X;
				dy = pt.Y - P2.Y;
			}
			else
			{
				closest = new PointD(P1.X + t * dx, P1.Y + t * dy);
				dx = pt.X - closest.X;
				dy = pt.Y - closest.Y;
			}

			distance = Math.Sqrt(dx * dx + dy * dy);

			return closest;
		}

		public void Move(Double bearing, Double distance)
		{
			_p1.Move(bearing, distance);
			_p2.Move(bearing, distance);
		}

		public void Rotate(PointD centroid, Double angle)
		{
			_p1.Rotate(centroid, angle);
			_p2.Rotate(centroid, angle);
		}

		public bool Intersects(Line other, out PointD intersection)
		{
			intersection = null;
			return FlatGeo.GetIntersection(this, other, out intersection);
		}

		public bool Intersects(Line other)
		{
			PointD intersection;
			return Intersects(other, out intersection);
		}

		public bool Intersects(Circle circle)
		{
			return circle.Intersects(this);
		}

		public bool SharesEndPointWith(ILine other, int precision = 0)
		{
			return
			    other.P1.Equals(P1, precision) ||
			    other.P1.Equals(P2, precision) ||
			    other.P2.Equals(P1, precision) ||
			    other.P2.Equals(P2, precision);
		}

		public bool IsEndPoint(IPoint point, int precision = 0)
		{
			return _p1.Equals(point, precision) || _p2.Equals(point, precision);
		}

		public void GetABC(out Double A, out Double B, out Double C)
		{
			A = B = C = 0;

			A = P2.Y - P1.Y;
			B = P1.X - P2.X;
			C = A * P1.X + B * P1.Y;
		}

		public bool IsLeftOf(Line other)
		{
			Double minL1 = Math.Min(P1.X, P2.X);
			Double minL2 = Math.Min(other.P1.X, other.P2.X);

			return minL1 < minL2;
		}

		public bool IsRightOf(Line other)
		{
			Double maxL1 = Math.Max(P1.X, P2.X);
			Double maxL2 = Math.Max(other.P1.X, other.P2.X);

			return maxL1 > maxL2;
		}

		public bool IsAbove(Line other)
		{
			Double minL1 = Math.Min(P1.Y, P2.Y);
			Double minL2 = Math.Min(other.P1.Y, other.P2.Y);

			return minL1 < minL2;
		}

		public bool IsBelow(Line other)
		{
			Double maxL1 = Math.Max(P1.Y, P2.Y);
			Double maxL2 = Math.Max(other.P1.Y, other.P2.Y);

			return maxL1 > maxL2;
		}

		#endregion

		#region Utility

		public ILine Clone()
		{
			return new Line(_p1.Clone(), _p2.Clone());
		}

		public bool Equals(ILine l)
		{
			return (l.P1.X == P1.X && l.P1.Y == P1.Y && l.P2.X == P2.X && l.P2.Y == P2.Y);
		}

		public String ToString(int precision)
		{
			return String.Format("{0} - {1}", _p1.ToString(precision), _p2.ToString(precision));
		}

		public override string ToString()
		{
			return ToString(0);
		}

		#endregion
	}
}
