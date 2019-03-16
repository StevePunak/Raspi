using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public class Ellipse : IEllipse
	{
		#region Public Properties

		public Double Area { get; private set; }

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
				m_Center = value;
			}
		}

		#endregion

		#region Private Members

		/** These member variables actually define the ellipse */
		IPoint m_Center;
		Double m_MajorAxisLength;
		Double m_MinorAxisLength;
		Double m_MajorAxisBearing;

		#endregion

		#region Constructors

		private Ellipse(IPoint center, Double length1, Double length2, Double majorAxisBearing)
		{
			m_Center = center;

			if(MajorAxis == null && MinorAxis == null)
			{
				m_MajorAxisLength = length1 > length2 ? length1 : length2;
				m_MinorAxisLength = length1 < length2 ? length1 : length2;
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

		public Ellipse(PointD center, Double majorAxisLength, Double minorAxisLength, Double majorAxisBearing)
			: this(	LineFromCenterAndBearing(center, majorAxisBearing, majorAxisLength), 
					LineFromCenterAndBearing(center, Angle.Add(majorAxisBearing, 90), minorAxisLength)) {}

		public Ellipse(ILine majorAxis, ILine minorAxis)
			: this(majorAxis.MidPoint, majorAxis.Length, minorAxis.Length, majorAxis.Bearing) 
		{
			MajorAxis = majorAxis;
			MinorAxis = minorAxis;
		}

		public Ellipse(IRectangle rectangle)
			: this(	rectangle.Lines[0].Length > rectangle.Lines[1].Length						// major
						? new Line(rectangle.Lines[1].MidPoint, rectangle.Lines[3].MidPoint)
						: new Line(rectangle.Lines[0].MidPoint, rectangle.Lines[2].MidPoint),
					rectangle.Lines[0].Length < rectangle.Lines[1].Length						// minor
						? new Line(rectangle.Lines[1].MidPoint, rectangle.Lines[3].MidPoint)
						: new Line(rectangle.Lines[0].MidPoint, rectangle.Lines[2].MidPoint) ) {}

		#endregion

		#region Public Geometry Methods

		public void Move(IPoint where)
		{
			m_Center = new PointD(where);

			RecalculateAll();
		}

		#endregion

		#region Private Methods

		void RecalculateAll()
		{
			RecalculateAxes();
			RecalculateFociAndArea();
		}

		void RecalculateAxes()
		{
			MajorAxis = new Line(	
				FlatGeo.GetPoint(m_Center, Angle.Reverse(m_MajorAxisBearing), m_MajorAxisLength / 2),
				FlatGeo.GetPoint(m_Center, m_MajorAxisBearing, m_MajorAxisLength / 2));

			Double minorAxisBearing = Angle.Add(m_MajorAxisBearing, 90);
			MinorAxis = new Line(
				FlatGeo.GetPoint(m_Center, Angle.Reverse(minorAxisBearing), m_MinorAxisLength / 2),
				FlatGeo.GetPoint(m_Center, minorAxisBearing, m_MinorAxisLength / 2));
		}

		void RecalculateFociAndArea()
		{
			//      __________
			//    \/ r1² * r2²
			Double fociDistance = Math.Sqrt(MajorRadius * MajorRadius - MinorRadius * MinorRadius);	
			Foci = new PointD[]
			{
				FlatGeo.GetPoint(Center, Angle.Reverse(MajorAxis.Bearing), fociDistance),
				FlatGeo.GetPoint(Center, MajorAxis.Bearing, fociDistance)
			};

			Eccentricity = new Line(Foci[0], Center).Length / new Line(Foci[0], MinorAxis.P1).Length;

			Area = Math.PI * MajorRadius * MinorRadius;
		}

		static Line LineFromCenterAndBearing(PointD centerPoint, Double bearing, Double distance)
		{
			PointD p1 = FlatGeo.GetPoint(centerPoint, Angle.Reverse(bearing), distance / 2);
			PointD p2 = FlatGeo.GetPoint(centerPoint, bearing, distance / 2);
			return new Line(p1, p2);
		}


		#endregion

		#region Utility

		public override string ToString()
		{
			return String.Format("Ellipse:  {0} Maj {1} Min {2}", Center, MajorAxis, MinorAxis);
		}

		#endregion
	}
}
