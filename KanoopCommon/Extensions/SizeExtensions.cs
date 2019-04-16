using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanoopCommon.Extensions
{
	public static class SizeExtensions
	{
		public static Size MinimumSize(this Size size)
		{
			int min = Math.Min(size.Height, size.Width);
			return new Size(min, min);
		}

		public static Size Scale(this Size size, Double scale)
		{
			Double width = (Double)size.Width * scale;
			Double height = (Double)size.Height * scale;
			return new Size((int)Math.Round(width), (int)Math.Round(height));
		}

		public static Size Shrink(this Size size, int pixels)
		{
			return new Size(size.Width - pixels, size.Height - pixels);
		}

		public static Size Grow(this Size size, int pixels)
		{
			return new Size(size.Width + pixels, size.Height + pixels);
		}
	}
}
