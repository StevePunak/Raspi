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

		public static Point ToPoint(this PointF from)
		{
			return new Point((int)from.X, (int)from.Y);
		}

		public static PointD Center(this Mat bitmap)
		{
			return new PointD(bitmap.Width / 2, bitmap.Height / 2);
		}

		public static void DrawCross(this Mat image, PointD where, MCvScalar color, int lineWidth = 1)
		{
			Cross2DF cross = new Cross2DF(where.ToPoint(), 4, 4);
			CvInvoke.Line(image, cross.Vertical.P1.ToPoint(), cross.Vertical.P2.ToPoint(), color, lineWidth);
			CvInvoke.Line(image, cross.Horizontal.P1.ToPoint(), cross.Horizontal.P2.ToPoint(), color, lineWidth);
		}

		public static void Circle(this Mat image, Circle circle, MCvScalar color, int lineWidth = 1)
		{
			CvInvoke.Circle(image, circle.Center.ToPoint(), (int)circle.Radius, color, lineWidth);
		}

	}
}
