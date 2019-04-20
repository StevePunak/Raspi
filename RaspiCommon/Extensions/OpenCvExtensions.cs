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
	public static class OpenCvExtensions
	{
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

		public static Mat ToBitmap(this PointCloud2D cloud, int squarePixels, Color dotColor, Double scale = 1)
		{
			return cloud.ToBitmap(new Size(squarePixels, squarePixels), dotColor, scale);
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
					if(point.X >= 0 && point.Y < size.Width && point.Y >= 0 && point.Y <= size.Height)
					{
						mat.SetPixel(point, dotColor);
					}
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

		public static MCvScalar ToMCvScalar(this Color color)
		{
			return new Bgr(color).MCvScalar;
		}

		public static PointD Center(this Mat bitmap)
		{
			return new PointD(bitmap.Width / 2, bitmap.Height / 2);
		}

		public static bool FindMatrix(this Mat bitmap, Size size, Double percent, out PointD where)
		{
			where = null;

			Double threshold = (Double)(size.Height * size.Width) * percent;
			if(bitmap.NumberOfChannels != 1)
			{
				throw new FormatException("Image must be single channel");
			}
			unsafe
			{
				List<Rectangle> blackballed = new List<Rectangle>();

				int row, col;
				byte* ptr = (byte*)bitmap.DataPointer.ToPointer();
				for(row = 0;row < bitmap.Rows && where == null;row++)
				{
					for(col = 0;col < bitmap.Cols && where == null;col++, ptr++)
					{
						if(*ptr != 0 && row < bitmap.Rows - size.Height / 2 && col < bitmap.Cols - size.Width / 2)
						{
							Rectangle bestRect = Rectangle.Empty;
							int bestCount = 0;

							// make rectangles along the bottom and count the pixels
							for(int x = Math.Max(col - size.Width, 0);x < Math.Min(col + size.Width, bitmap.Cols - size.Width);x++)
							{
								Rectangle rect = new Rectangle(new Point(x, row), size);
								using(Mat square = new Mat(bitmap, rect))
								{
									int count = CvInvoke.CountNonZero(square);
									if(count > bestCount)
									{
										bestRect = rect;
										bestCount = count;
									}
								}
							}
							if(bestCount >= threshold)
							{
								// make a bigger rectangle (3 * size) going downward and see if
								// there are lots more pixels of this color... if so blacklist this rect
								PointD centroid = bestRect.Centroid();
								Log.SysLogText(LogLevel.DEBUG, "Checking Matrix Centroid of {0} at {1} with count of {2}", bestRect, centroid, bestCount);
								where = centroid;

#if work_on_later
								Size checkSize = new Size(bestRect.Width * 4, bestRect.Height * 4);
								PointD checkPoint = new PointD(Math.Max(0, bestRect.GetMidX() - checkSize.Width / 2), bestRect.Y);
								Rectangle checkRect = new Rectangle(checkPoint.ToPoint(), checkSize);

								using(Mat square = new Mat(bitmap, checkRect))
								{
									int count = CvInvoke.CountNonZero(square);
									if(count > bestCount)
									{
										bestCount = count;
									}
								}
								where = null; // ???
#endif
							}
						}
					}
				}
			}
			return where != null;
		}

		public static void DrawFilledPolygon(this Mat dest, PointDList polygon, Color color)
		{
			PointF[] poly = polygon.ToPointFArray();
			Point[] points = Array.ConvertAll<PointF, Point>(poly, Point.Round);

			using(VectorOfPoint vp = new VectorOfPoint(points))
			using(VectorOfVectorOfPoint vvp = new VectorOfVectorOfPoint(vp))
			{
				CvInvoke.FillPoly(dest, vvp, color.ToMCvScalar());
			}
		}

		public static void DrawText(this Mat dest, String text, FontFace fontFace, Color color, PointD where, int thickness = 1)
		{
			Double scale = 1;

			CvInvoke.PutText(dest, text, where.ToPoint(), fontFace, scale, color.ToMCvScalar(), thickness);
		}

		public static void DrawCenteredText(this Mat dest, String text, FontFace fontFace, Color color, PointD where, int thickness = 1, Double scale = 1)
		{
			int baseLine = 0;
			Size size = CvInvoke.GetTextSize(text, fontFace, scale, thickness, ref baseLine);

			PointD newPoint = new PointD(where.X - size.Width / 2, where.Y + size.Height / 2);
			CvInvoke.PutText(dest, text, newPoint.ToPoint(), fontFace, scale, color.ToMCvScalar(), thickness);

			dest.DrawCross(where, 4, Color.White);
		}

		public static void DrawImage(this Mat dest, Mat source, PointD where)
		{
			Rectangle rect = new Rectangle(where.ToPoint(), source.Size);
			Mat destRoi = new Mat(dest, rect);
			source.CopyTo(destRoi);
		}

		public static void DrawCenteredImage(this Mat dest, Mat source, PointD where)
		{
			PointD adjusted = new PointD(where.X - source.Width / 2, where.Y - source.Height / 2);
			Rectangle rect = new Rectangle(adjusted.ToPoint(), source.Size);
			Mat destRoi = new Mat(dest, rect);
			source.CopyTo(destRoi);
		}

		public static void DrawEllipse(this Mat bitmap, PointD center, Size size, Double startAngle, Double endAngle, Color color, int thickness = 1)
		{
			Double angle = startAngle.SubtractDegrees(90);
			Double start = 0;
			Double end = endAngle.SubtractDegrees(startAngle);
			CvInvoke.Ellipse(bitmap, center.ToPoint(), size, angle, start, end, color.ToMCvScalar(), thickness);
		}

		public static Mat Scale(this Mat source, Double scale)
		{
			Size newSize = source.Size.Scale(scale);
			Mat dest = new Mat(newSize, source.Depth, source.NumberOfChannels);
			CvInvoke.Resize(source, dest, newSize);
			return dest;
		}

		public static Mat Rotate(this Mat source, Double degrees, Double scale = 1)
		{
			Mat dest = new Mat(source.Size, source.Depth, source.NumberOfChannels);

			Mat rotationMatrix = new Mat();
			CvInvoke.GetRotationMatrix2D(source.Center().ToPoint(), 360 - degrees, scale, rotationMatrix);

			CvInvoke.WarpAffine(source, dest, rotationMatrix, dest.Size);
			// dest.Save(@"c:\pub\tmp\image.png");
			return dest;
		}

		public static void DrawVectorLines(this Mat image, BearingAndRangeList vectors, PointD where, Double scale, Color color, int lineWidth = 1)
		{
			foreach(BearingAndRange bar in vectors)
			{
				Line line = new Line(where, where.GetPointAt(bar.Scale(scale)));
				image.DrawLine(line, color, lineWidth);
			}
		}

		public static void DrawMinMaxLines(this Mat image, BearingAndRangeList vectors, PointD where, Double scale, Color color, int lineWidth = 1)
		{
			if(vectors.Count > 0)
			{
				{
					BearingAndRange bar = vectors.First();
					Line line = new Line(where, where.GetPointAt(bar.Scale(scale)));
					image.DrawLine(line, color, lineWidth);
				}
				{
					BearingAndRange bar = vectors.Last();
					Line line = new Line(where, where.GetPointAt(bar.Scale(scale)));
					image.DrawLine(line, color, lineWidth);
				}
			}
		}

		public static void DrawCircle(this Mat image, Circle circle, Color color, int lineWidth = 1)
		{
			image.DrawCircle(circle.Center, (int)circle.Radius, color, lineWidth);
		}

		public static void DrawCircle(this Mat image, Circle circle, MCvScalar color, int lineWidth = 1)
		{
			image.DrawCircle(circle.Center, (int)circle.Radius, color, lineWidth);
		}

		public static void DrawCircle(this Mat image, PointD where, int radius, Color color, int lineWidth = 1)
		{
			image.DrawCircle(where, radius, new Bgr(color).MCvScalar, lineWidth);
		}

		public static void DrawRectangle(this Mat image, Rectangle rectangle, Color color, int lineWidth = 1)
		{
			CvInvoke.Rectangle(image, rectangle, color.ToMCvScalar(), lineWidth);
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

		public static void DrawLine(this Mat image, Line line, Color color, int lineWidth = 1)
		{
			image.DrawLine(line, color.ToMCvScalar(), lineWidth);
		}

		public static void DrawLine(this Mat image, Line line, MCvScalar color, int lineWidth = 1)
		{
			CvInvoke.Line(image, line.P1.ToPoint(), line.P2.ToPoint(), color, lineWidth);
		}
	}
}
