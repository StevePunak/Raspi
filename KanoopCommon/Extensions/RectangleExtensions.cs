using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using KanoopCommon.Geometry;

namespace KanoopCommon.Extensions
{
	public static class RectangleExtensions
	{
		public static Double Area (this Rectangle rect)
		{
			return (Double)rect.Size.Height * (Double)rect.Size.Width;
		}

		public static PointD Centroid(this Rectangle rect)
		{
			return new PointD(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
		}

		public static Rectangle ExpandWithinImage(this Rectangle rect, int pixels, Size imageSize)
		{
			int left = Math.Max(0, rect.Left - pixels / 2);
			int top = Math.Max(0, rect.Top - pixels / 2);
			int right = Math.Min(imageSize.Width - 1, rect.Right + pixels / 2);
			int bottom = Math.Min(imageSize.Height - 1, rect.Bottom + pixels / 2);
			return new Rectangle(left, top, right - left, bottom - top);
		}

		public static Rectangle GetFromPoint(Size imageSize, Point point, Size rectangleSize)
		{
			int left = Math.Max(0, point.X - rectangleSize.Width / 2);
			int top = Math.Max(0, point.Y - rectangleSize.Height / 2);
			int right = Math.Min(imageSize.Width - 1, point.X + rectangleSize.Width / 2);
			int bottom = Math.Min(imageSize.Height - 1, point.Y + rectangleSize.Height / 2);
			return new Rectangle(left, top, right - left, bottom - top);
		}

		public static int GetMidX(this Rectangle rect)
		{
			return rect.X + rect.Width / 2;
		}
	}
}
