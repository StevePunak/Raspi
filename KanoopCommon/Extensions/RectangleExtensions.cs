using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace KanoopCommon.Extensions
{
	public static class RectangleExtensions
	{
		public static Double Area (this Rectangle rect)
		{
			return (Double)rect.Size.Height * (Double)rect.Size.Width;
		}
	}
}
