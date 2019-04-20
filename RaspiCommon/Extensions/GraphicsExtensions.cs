using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;

namespace RaspiCommon.Extensions
{
	public static class GraphicsExtensions
	{
		public static PointCloud2D ToPointCloud(this Mat bitmap, Double vectorSize, Double scale = 1)
		{
			PointD center = bitmap.Center();

			PointCloud2D cloud = new PointCloud2D(360 / vectorSize);
			int fullPixels = 0;
			unsafe
			{
				IntPtr startPtr = bitmap.DataPointer;
				byte* ptr = (byte*)startPtr.ToPointer();
				for(int y = 0;y < bitmap.Height;y++)
				{
					for(int x = 0;x < bitmap.Width;x++, ptr += bitmap.NumberOfChannels)
					{
//						ptr = (byte*)startPtr.ToPointer() + (x * y * bitmap.NumberOfChannels);
						byte b = ptr[0];
						byte g = ptr[1];
						byte r = ptr[2];
						if(r != 0 || g != 0 || b != 0)
						{
							fullPixels++;
							if(x != (int)center.X && y != (int)center.Y)
							{
								BearingAndRange bar = center.BearingAndRangeTo(new PointD(x, y));
								int offset = (int)(bar.Bearing / vectorSize);
								cloud[offset].Bearing = (Double)offset * vectorSize; /*bar.Bearing;*/
								cloud[offset].Range = bar.Range * scale;
							}
						}
					}
				}
			}

			return cloud;
		}

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

	}
}
