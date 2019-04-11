using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using KanoopCommon.Geometry;

namespace RaspiCommon.Extensions
{
	public static class GraphicsExtensions
	{
		public static PointCloud2D ToPointCloud(this Mat bitmap, Double vectorSize, Double scale = 1)
		{
			PointD center = bitmap.Center();

			PointCloud2D cloud = new PointCloud2D(360 / vectorSize);
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

		public static void SetPixel(this Mat mat, PointD point, Color color)
		{
			unsafe
			{
				int offset = ((int)point.Y * mat.Width * mat.NumberOfChannels) + ((int)point.X * mat.NumberOfChannels);
				byte* ptr = (byte *)mat.DataPointer.ToPointer();
				ptr += offset;
				ptr[0] = color.B;
				ptr[1] = color.G;
				ptr[2] = color.R;
			}
		}

		public static Mat ToBitmap(this PointCloud2D cloud, Size size, Color dotColor, Double scale = 1)
		{
			Double vectorSize = cloud.VectorSize;
			Mat mat = new Mat(size, DepthType.Cv8U, 3);
			PointD center = mat.Center();

			for(Double angle = 0;angle < 360;angle += vectorSize)
			{
				BearingAndRange bar = cloud[(int)(angle / vectorSize)];
				if(bar.Range != 0)
				{
					PointD point = center.GetPointAt(angle, bar.Range * scale);
					mat.SetPixel(point, dotColor);
				}
			}

			return mat;
		}

		public static void PlaceOnBitmap(this PointCloud2D cloud, Mat bitmap, PointD origin, Color dotColor, Double scale = 1)
		{
			Double vectorSize = cloud.VectorSize;
			for(Double angle = 0;angle < 360;angle += vectorSize)
			{
				BearingAndRange bar = cloud[(int)(angle / vectorSize)];
				if(bar.Range != 0)
				{
					PointD point = origin.GetPointAt(angle, bar.Range * scale);
					bitmap.SetPixel(point, dotColor);
				}
			}
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

		public static PointD Center(this Mat bitmap)
		{
			return new PointD(bitmap.Width / 2, bitmap.Height / 2);
		}

		public static void DrawCircle(this Mat image, PointD where, int radius, Color color, int lineWidth = 1)
		{
			image.DrawCircle(where, radius, new Bgr(color).MCvScalar, lineWidth);
		}

		public static void DrawCircle(this Mat image, PointD where, int radius, MCvScalar color, int lineWidth = 1)
		{
			CvInvoke.Circle(image, where.ToPoint(), radius, color, lineWidth);
		}

		public static void DrawCross(this Mat image, PointD where, int size, Color color, int lineWidth = 1)
		{
			image.DrawCross(where, size, new Bgr(color).MCvScalar, lineWidth);
		}

		public static void DrawCross(this Mat image, PointD where, int size, MCvScalar color, int lineWidth = 1)
		{
			Cross2DF cross = new Cross2DF(where.ToPoint(), size, size);
			CvInvoke.Line(image, cross.Vertical.P1.ToPoint(), cross.Vertical.P2.ToPoint(), color, lineWidth);
			CvInvoke.Line(image, cross.Horizontal.P1.ToPoint(), cross.Horizontal.P2.ToPoint(), color, lineWidth);
		}

		public static void DrawCircleCross(this Mat image, PointD where, int size, Color color, int lineWidth = 1)
		{
			image.DrawCircleCross(where, size, new Bgr(color).MCvScalar, lineWidth);
		}

		public static void DrawCircleCross(this Mat image, PointD where, int size, MCvScalar color, int lineWidth = 1)
		{
			image.DrawCross(where, size, color, lineWidth);
			image.DrawCircle(where, size / 2, color);
		}

	}
}
