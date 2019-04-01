using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using KanoopCommon.Geometry;

namespace RaspiCommon.Extensions
{
	public static class GraphicsExtensions
	{
		public static LineList ToLineList(this LineSegment2D[] segments)
		{
			LineList lines = new LineList();
			foreach(LineSegment2D segment in segments)
			{
				lines.Add(new Line(segment.P1, segment.P2));
			}
			return lines;
		}

		public static PointD CenterPoint(this Mat matrix)
		{
			return new PointD(matrix.Width / 2, matrix.Height / 2);
		}

		public static Point ToPoint(this PointF from)
		{
			return new Point((int)from.X, (int)from.Y);
		}

		public static PointD Center(this Mat bitmap)
		{
			return new PointD(bitmap.Width / 2, bitmap.Height / 2);
		}
	}
}
