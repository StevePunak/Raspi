using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;

namespace RaspiCommon.Devices.Chassis
{
	public class Chassis
	{
		Double _length;
		public Double Length
		{
			get { return _length; }
			set
			{
				_length = value;
				Recalculate();
			}
		}

		Double _width;
		public Double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				Recalculate();
			}
		}

		public Dictionary<ChassisParts, PointD> Points { get; private set; }
		public Dictionary<ChassisParts, Dictionary<ChassisParts, BearingAndRange>> PartVectors { get; private set; }

		public Chassis()
			: this(0, 0) {}

		public Chassis(Double length, Double width)
		{
			_length = length;
			_width = width;

			Points = new Dictionary<ChassisParts, PointD>();

			Recalculate();

			PartVectors = new Dictionary<ChassisParts, Dictionary<ChassisParts, BearingAndRange>>();
		}

		public BearingAndRange GetBearingAndRange(ChassisParts from, ChassisParts to, Double fromAngle = 0)
		{
			BearingAndRange value;
			Dictionary<ChassisParts, BearingAndRange> listForFromPart;
			if(PartVectors.TryGetValue(from, out listForFromPart) == false)
			{
				listForFromPart = new Dictionary<ChassisParts, BearingAndRange>();
				PartVectors.Add(from, listForFromPart);
			}
			if(listForFromPart.TryGetValue(to, out value) == false)
			{
				value = Points[from].BearingAndRangeTo(Points[to]);
				listForFromPart.Add(to, value);
			}
			if(value != null && fromAngle != 0)
			{
				value.Bearing = value.Bearing.AddDegrees(fromAngle);
			}
			return value;
		}

		private void Recalculate()
		{
			if(_length > 0 && _width > 0)
			{
				// rear left is at 0,0
				Points.Clear();
				Points.Add(ChassisParts.RearLeft, new PointD(0, 0));
				Points.Add(ChassisParts.RearRight, new PointD(_width, 0));
				Points.Add(ChassisParts.FrontLeft, new PointD(0, _length));
				Points.Add(ChassisParts.FrontRight, new PointD(_width, _length));
				PointD centerPoint = new Line(Points[ChassisParts.FrontLeft], Points[ChassisParts.RearRight]).MidPoint;
				Points.Add(ChassisParts.CenterPoint, centerPoint);

				PartVectors = new Dictionary<ChassisParts, Dictionary<ChassisParts, BearingAndRange>>();
				PartVectors.Add(ChassisParts.CenterPoint, new Dictionary<ChassisParts, BearingAndRange>()
				{
					{ ChassisParts.FrontLeft, centerPoint.BearingAndRangeTo(Points[ChassisParts.FrontLeft]) },
					{ ChassisParts.FrontRight, centerPoint.BearingAndRangeTo(Points[ChassisParts.FrontRight]) },
					{ ChassisParts.RearLeft, centerPoint.BearingAndRangeTo(Points[ChassisParts.RearLeft]) },
					{ ChassisParts.RearRight, centerPoint.BearingAndRangeTo(Points[ChassisParts.RearRight]) },
				});
			}
		}
	}
}
