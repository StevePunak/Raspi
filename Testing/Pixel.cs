﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
	class Pixel
	{
		public int X { get; set; }
		public int Y { get; set; }
		public Color Color { get; set; }

		public override string ToString()
		{
			return String.Format("{0}, {1}  {2}", X, Y, Color);
		}
	}
}
