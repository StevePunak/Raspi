using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanoopCommon.Geometry
{
	public class Triangle : Polygon, ITriangle
	{
		public IPoint A	{ get { return Lines[0].P1 as PointD; } }
		public IPoint B	{ get { return Lines[1].P1 as PointD; } }
		public IPoint C	{ get { return Lines[2].P1 as PointD; } }

		public ILine AtoB { get { return new Line(A, B); } }
		public ILine AtoC { get { return new Line(A, C); } }
		public ILine BtoC { get { return new Line(B, C); } }
		public ILine BtoA { get { return new Line(B, A); } }
		public ILine CtoA { get { return new Line(C, A); } }
		public ILine CtoB { get { return new Line(C, B); } }

		public Triangle(IPoint pA, IPoint pB, IPoint pC)
			: base(new LineList()
			{ 
				new Line(pA, pB),
				new Line(pB, pC),
				new Line(pC, pA)
			})
		{
		}

		public Angle InteriorAngle(IPoint point)
		{
			LineList lines = GetLinesAdjacentTo(point as PointD);
			Angle ret = FlatGeo.Angle(lines[0], lines[1]);
			if(ret.Degrees > 180)
			{
				ret.Degrees = 360 - ret.Degrees;
			}
			return ret;
		}


	}
}
