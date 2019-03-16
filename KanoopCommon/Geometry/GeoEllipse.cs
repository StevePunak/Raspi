using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System;

namespace KanoopCommon.Geometry
{
	public class GeoEllipse : GeoShape, IEllipse
	{
		#region Public Properties

		public virtual Double Area { get; private set; }

		public Double Eccentricity { get; private set; }

		public IPoint[] Foci { get; private set; }

		public ILine MajorAxis { get; private set; }

		public Double MajorRadius { get; private set; }

		public ILine MinorAxis { get; private set; }

		public Double MinorRadius { get; private set; }

		public IPoint Center
		{
			get { return m_Center; }
			private set
			{
				m_Center = new GeoPoint(value);
			}
		}

		public GeoPoint GeoCenter { get { return m_Center as GeoPoint; } }

		public Double SemiMajorAxisLength { get { return MajorAxis.Length / 2; } }

		public Double SemiMinorAxisLength { get { return MinorAxis.Length / 2; } }

		public Double MajorAxisBearing { get { return m_MajorAxisBearing; } }

		#endregion

		#region Private Member Variables

		/** These member variables actually define the ellipse */
		protected GeoPoint		m_Center;
		Double					m_MajorAxisLength;
		Double					m_MinorAxisLength;
		Double					m_MajorAxisBearing;

		#endregion

		#region Constructors

		private GeoEllipse(IPoint center, Double axis1Length, Double axis2Length, Double majorAxisBearing)
		{
			m_Center = center.ToGeoPoint();

			/** may be set if called from chaining constructor */
			if(MajorAxis == null && MinorAxis == null)
			{
				m_MajorAxisLength = axis1Length > axis2Length ? axis1Length : axis2Length;
				m_MinorAxisLength = axis1Length < axis2Length ? axis1Length : axis2Length;
				m_MajorAxisBearing = majorAxisBearing;

				RecalculateAxes();
			}
			else
			{
				m_MajorAxisLength = MajorAxis.Length;
				m_MinorAxisLength = MinorAxis.Length;

				m_MajorAxisBearing = majorAxisBearing;
			}

			RecalculateFociAndArea();
		}

		public GeoEllipse()
			: this(new GeoPoint(), 0, 0, 0) { }

		public GeoEllipse(GeoPoint center, Double majorAxisLength, Double minorAxisLength, Double majorAxisBearing = 0)
			: this(	LineFromCenterAndBearing(center, majorAxisBearing, majorAxisLength),
			        LineFromCenterAndBearing(center, Angle.Add(majorAxisBearing, 90), minorAxisLength)) {}

		public GeoEllipse(ILine majorAxis, ILine minorAxis)
			: this(majorAxis.MidPoint, majorAxis.Length, minorAxis.Length, majorAxis.Bearing)
		{
			MajorAxis = majorAxis;
			MinorAxis = minorAxis;
		}

		public GeoEllipse(IRectangle rectangle)
			: this(	rectangle.Lines[0].Length > rectangle.Lines[1].Length						// major
			        ? new Line(rectangle.Lines[1].MidPoint, rectangle.Lines[3].MidPoint)
					: new Line(rectangle.Lines[0].MidPoint, rectangle.Lines[2].MidPoint),
			        rectangle.Lines[0].Length < rectangle.Lines[1].Length						// minor
			        ? new Line(rectangle.Lines[1].MidPoint, rectangle.Lines[3].MidPoint)
					: new Line(rectangle.Lines[0].MidPoint, rectangle.Lines[2].MidPoint) ) {}


		public GeoEllipse(GeoCircle circle)
			: this(circle.Center, circle.Radius, circle.Radius, 0) { }

		public GeoEllipse(Double latitude, Double longitude, Double radius)
			: this(new GeoPoint(latitude, longitude), radius, radius, 0) { }

		public GeoEllipse(GeoPoint center, Double radius)
			: this(center, radius, radius, 0) { }

		#region Ellipse Based Constructors

		public GeoEllipse(GeoEllipse other)
			: this(other.MajorAxis, other.MinorAxis) {}

		#endregion

		#region Private Methods

		void RecalculateAll()
		{
			RecalculateAxes();
			RecalculateFociAndArea();
		}

		void RecalculateAxes()
		{
			MajorAxis = new GeoLine(
			    EarthGeo.GetPoint(m_Center, Angle.Reverse(m_MajorAxisBearing), m_MajorAxisLength / 2),
			    EarthGeo.GetPoint(m_Center, m_MajorAxisBearing, m_MajorAxisLength / 2));

			Double minorAxisBearing = Angle.Add(m_MajorAxisBearing, 90);
			MinorAxis = new GeoLine(
			    EarthGeo.GetPoint(m_Center, Angle.Reverse(minorAxisBearing), m_MinorAxisLength / 2),
			    EarthGeo.GetPoint(m_Center, minorAxisBearing, m_MinorAxisLength / 2));
		}

		void RecalculateFociAndArea()
		{
			//      __________
			//    \/ r1² * r2²
			Double fociDistance = Math.Sqrt(MajorRadius * MajorRadius - MinorRadius * MinorRadius);
			Foci = new IPoint[]
			{
				EarthGeo.GetPoint(Center as GeoPoint, Angle.Reverse(MajorAxis.Bearing), fociDistance),
				EarthGeo.GetPoint(Center as GeoPoint, MajorAxis.Bearing, fociDistance)
			};

			Eccentricity = new Line(Foci[0], Center).Length / new Line(Foci[0], MinorAxis.P1).Length;

			Area = Math.PI * MajorRadius * MinorRadius;
		}

		static GeoLine LineFromCenterAndBearing(GeoPoint centerPoint, Double bearing, Double distance)
		{
			GeoPoint p1 = EarthGeo.GetPoint(centerPoint, Angle.Reverse(bearing), distance / 2);
			GeoPoint p2 = EarthGeo.GetPoint(centerPoint, bearing, distance / 2);
			return new GeoLine(p1, p2);
		}

		#endregion

		public override IRectangle GetMinimumBoundingRectangle()
		{
			/** offset minor axis to half the major axis distance in either direction */
			Double		offset = MajorAxis.Length / 2;
			GeoPoint	p1 = EarthGeo.GetPoint(MinorAxis.P1 as GeoPoint, MajorAxis.Bearing, offset);
			GeoPoint	p2 = EarthGeo.GetPoint(MinorAxis.P2 as GeoPoint, MajorAxis.Bearing, offset);
			GeoPoint	p3 = EarthGeo.GetPoint(MinorAxis.P2 as GeoPoint, Angle.Reverse(MajorAxis.Bearing), offset);
			GeoPoint	p4 = EarthGeo.GetPoint(MinorAxis.P1 as GeoPoint, Angle.Reverse(MajorAxis.Bearing), offset);
			return new GeoRectangle(new GeoPointList() { p1, p2, p3, p4 } );
		}

		public CoordinateMap GetCoordinateMap(Double ppm, Double centerX = 0, Double centerY = 0)
		{
			return new CoordinateMap(new PointD(centerX, centerY), this.GeoCenter, 0, ppm);
		}

		public virtual bool Contains(GeoPoint point)
		{
			//1. Convert points to flat geometry in the cartesian plane.
			CoordinateMap	map = GetCoordinateMap(1);
			PointD			checkPt = map.GetPixelPoint(point);

			//2. Check points against formula for points within the ellipse
			Double			res = (Math.Pow(checkPt.X, 2) / Math.Pow(SemiMajorAxisLength, 2)) + (Math.Pow(checkPt.Y,2)/Math.Pow(SemiMinorAxisLength, 2));

			return res <=1;
		}

		public override void Move(Double bearing, Double distance)
		{
			GeoPoint newCenter = EarthGeo.GetPoint(GeoCenter, bearing, distance);
			Move(newCenter);
		}

		public virtual void Move(IPoint to)
		{
			GeoPoint center = new GeoPoint(to);
			m_Center = center;

			RecalculateAll();
		}

		public virtual bool Intersects(GeoPolygon polygon)
		{
			throw new NotImplementedException();
		}

		public virtual bool Intersects(GeoLine line)
		{
			throw new NotImplementedException();
		}

		public virtual bool Intersects(GeoLine line, out GeoPoint intersection1, out GeoPoint intersection2, out int intersectionsCalculated)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Utility

		public virtual void SetPrecision(int precision)
		{
			Center.Round(precision);
		}

		public override string ToString()
		{
			return String.Format("{0} Rx {1} Ry A", Center, MajorAxis.Length, MinorAxis.Length, m_MajorAxisBearing);
		}

		public override object Clone()
		{
			return new GeoEllipse(this);
		}

		#endregion
	}
}
