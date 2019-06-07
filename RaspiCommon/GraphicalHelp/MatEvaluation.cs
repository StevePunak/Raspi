using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using RaspiCommon.Extensions;

namespace RaspiCommon.GraphicalHelp
{
	public class PixelDefinition
	{
		public Point Point { get; set; }
		public int Blue { get; set; }
		public int Green { get; set; }
		public int Red { get; set; }
		public int Diff1 { get; private set; }
		public int Diff2 { get; private set; }
		public int TotalDiff { get; private set; }

		public int ColorLevel(Color color)
		{
			if(color == Color.Blue)
				return Blue;
			else if(color == Color.Green)
				return Green;
			else if(color == Color.Red)
				return Red;
			else
				throw new Exception("Invalid color choice");
		}

		public PixelDefinition()
			: this(Point.Empty, Color.Black, 0, 0, 0) { }

		public PixelDefinition(Point point, Color forColor, int blue, int green, int red)
		{
			Point = point;
			Blue = blue;
			Green = green;
			Red = red;

			int myval = 0;
			int other1 = 0;
			int other2 = 0;
			if(forColor == Color.Blue)
			{
				myval = blue;
				other1 = green;
				other2 = red;
			}
			else if(forColor == Color.Green)
			{
				myval = green;
				other1 = blue;
				other2 = red;
			}
			else if(forColor == Color.Red)
			{
				myval = red;
				other1 = green;
				other2 = blue;
			}

			if(myval > other1 && myval > other2)
			{
				Diff1 = myval - other1;
				Diff2 = myval - other2;
				TotalDiff = Diff1 + Diff2;
			}
		}

		public override string ToString()
		{
			return String.Format("{0}  Blue: {1}  Green: {2}  Red: {3}  Total Difference: {4}", Point, Blue, Green, Red, TotalDiff);
		}
	}

	public class MatEvaluation
	{
		public PixelDefinition DiffPixel { get; private set; }


		public MatEvaluation(Mat bitmap, Color color)
		{
			int channel = color.ToBGRChannel();
			DiffPixel = new PixelDefinition();

			Mat[] channels = bitmap.Split();

			int max = 0;
			unsafe
			{
				byte* start = (byte *)bitmap.DataPointer.ToPointer();
				byte* end = start + (bitmap.Rows * bitmap.Cols * bitmap.NumberOfChannels);
				int elementSize = bitmap.ElementSize;
				byte* ptr = start;
				for(int row = 0;row < bitmap.Rows && ptr < end;row++)
				{
					for(int col = 0;col < bitmap.Cols && ptr < end;col++, ptr += elementSize)
					{
						byte blue = *ptr;
						byte green = *(ptr + 1);
						byte red = *(ptr + 2);

						PixelDefinition pixel = new PixelDefinition(new Point(col, row), color, blue, green, red);
						if(pixel.TotalDiff > max)
						{
							DiffPixel = pixel;
							max = pixel.TotalDiff;
						}
					}
				}
			}
		}

		public override string ToString()
		{
			return String.Format("At {0}", DiffPixel);
		}

	}
}