using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using KanoopCommon.CommonObjects;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon.GraphicalHelp;

namespace RaspiCommon.Extensions
{
	public static class OpenCvExtensions
	{
		public static int ValidX(this Mat image, int x)
		{
			if(x >= image.Width)
				x = image.Width - 1;
			else if(x < 0)
				x = 0;
			return x;
		}

		public static int ValidY(this Mat image, int y)
		{
			if(y >= image.Height)
				y = image.Height - 1;
			else if(y < 0)
				y = 0;
			return y;
		}

		public static Rectangle ValidRectangle(this Mat image, Point upperLeft, Size size)
		{
			Size adjustedSize = new Size(Math.Min(size.Width, image.Width - upperLeft.X), Math.Min(size.Height, image.Height - upperLeft.Y));
			return new Rectangle(upperLeft, adjustedSize);
		}

		public static Mat GetROIWithinImage(this Mat image, Point origin, Size size, int xoffset, int yoffset)
		{
			int x = Math.Max(0, origin.X + xoffset);
			int y = Math.Max(0, origin.Y + yoffset);
			int width = Math.Min(image.Width - 1, origin.X + xoffset + size.Width) - x;
			int height = Math.Min(image.Height - 1, origin.Y + yoffset + size.Height) - y;
			Rectangle roi = new Rectangle(x, y, width, height);
			return new Mat(image, roi);
		}

		public static void SetPixelBGR(this Mat mat, PointD point, Color color)
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

		public static void SetPixel(this Mat mat, PointD point, int channel, int value)
		{
			unsafe
			{
				int offset = ((int)point.Y * mat.Width * mat.NumberOfChannels) + ((int)point.X * mat.NumberOfChannels);
				byte* ptr = (byte *)mat.DataPointer.ToPointer();
				ptr += offset;
				ptr[channel] = (byte)value;
			}
		}

		public static int GetPixel(this Mat mat, PointD point, int channel)
		{
			int ret = 0;
			unsafe
			{
				int offset = ((int)point.Y * mat.Width * mat.NumberOfChannels) + ((int)point.X * mat.NumberOfChannels);
				byte* ptr = (byte *)mat.DataPointer.ToPointer();
				ptr += offset;
				ret = ptr[channel];
			}
			return ret;
		}

		public static int MaxValue(this Mat bitmap, int channel)
		{
			int max = 0;
			unsafe
			{
				byte* start = (byte *)bitmap.DataPointer.ToPointer();
				byte* end = start + (bitmap.Rows * bitmap.Cols * bitmap.NumberOfChannels);
				int elementSize = bitmap.ElementSize;
				for(byte* ptr = start + channel;ptr < end;ptr += elementSize)
				{
					if(*ptr > max)
						max = *ptr;
				}
			}
			return max;
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
						mat.SetPixelBGR(point, dotColor);
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
					bitmap.SetPixelBGR(point, dotColor);
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

		public static Rectangle ShrinkROIToNonZero(this Mat image, Rectangle originalRegion)
		{
			int minY, maxY, minX, maxX;

			for(minY = originalRegion.Top;minY < originalRegion.Bottom;minY++)
			{
				Mat roi = new Mat(image, new Rectangle(new Point(originalRegion.X, minY), new Size(originalRegion.Width, 1)));
				if(CvInvoke.CountNonZero(roi) > 0)
					break;
			}
			for(maxY = originalRegion.Bottom - 1;maxY > originalRegion.Top;maxY--)
			{
				Mat roi = new Mat(image, new Rectangle(new Point(originalRegion.X, maxY), new Size(originalRegion.Width, 1)));
				if(CvInvoke.CountNonZero(roi) > 0)
					break;
			}
			for(minX = originalRegion.Left;minX < originalRegion.Right;minX++)
			{
				Mat roi = new Mat(image, new Rectangle(new Point(minX, originalRegion.Y), new Size(1, originalRegion.Height)));
				if(CvInvoke.CountNonZero(roi) > 0)
					break;
			}
			for(maxX = originalRegion.Right - 1;maxX > originalRegion.Left;maxX--)
			{
				Mat roi = new Mat(image, new Rectangle(new Point(maxX, originalRegion.Y), new Size(1, originalRegion.Height)));
				if(CvInvoke.CountNonZero(roi) > 0)
					break;
			}

			return new Rectangle(minX, minY, (maxX - minX) + 1, (maxY - minY) + 1);
		}

		public static bool FindObjectMatrix(this Mat bitmap, Color color, Size size, out ObjectCandidateList candidates)
		{
			if(bitmap.NumberOfChannels > 1)
			{
				throw new ImageException("Can not FindMatrix with more than one color channel");
			}

			candidates = new ObjectCandidateList();

			int totalcount = CvInvoke.CountNonZero(bitmap);
			if(totalcount > 999999)
			{
				Log.SysLogText(LogLevel.DEBUG, "Image is too noisy with {0} pixels", totalcount);
				return false;
			}
			Log.SysLogText(LogLevel.DEBUG, "{0} Image has a total of {1} full pixels", color.Name, totalcount);

			if(bitmap.NumberOfChannels != 1)
			{
				throw new IllegalFormatException("Image must be single channel");
			}
			unsafe
			{
				int row, col;
				byte* ptr = (byte*)bitmap.DataPointer.ToPointer();
				for(row = 0;row < bitmap.Rows;row++)
				{
					byte[] rowArray = new byte[bitmap.Cols];
					for(col = 0;col < bitmap.Cols;col++, ptr++)
					{
						if(*ptr != 0 && row < bitmap.Rows - size.Height / 2 && col < bitmap.Cols - size.Width / 2)
						{
							if(candidates.Contains(new Point(col, row)) == false)
							{
								ColoredObjectCandidate candidate;
								if(ColoredObjectCandidate.TryGetCandidate(bitmap, color, new Point(col, row), size, out candidate))
								{
									candidates.Add(candidate);
								}
							}
						}
					}
				}
			}

			return candidates.Count > 0;
		}

		public static Mat ToSingleChannel(this Mat source)
		{
			Mat output = new Mat();
			CvInvoke.ExtractChannel(source, output, 0);
			return output;
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

		public static void DrawTextAboveLine(this Mat dest, String text, FontFace fontFace, Color color, Line line, int howFarAbove = 0, int thickness = 1, Double scale = 1)
		{
			int baseLine = 0;
			Size size = CvInvoke.GetTextSize(text, fontFace, scale, thickness, ref baseLine);
			PointD centerPoint = line.MidPoint.Offset(0, -(size.Height + howFarAbove));
			DrawCenteredText(dest, text, fontFace, color, centerPoint, thickness, scale);
		}

		public static void DrawTextBelowRectangle(this Mat dest, String text, FontFace fontFace, Color color, Rectangle rectangle, int howFarBelow = 0, int thickness = 1, Double scale = 1)
		{
			Line line = new Line(new Point(rectangle.Left, rectangle.Bottom), new Point(rectangle.Right, rectangle.Bottom));
			dest.DrawTextBelowLine(text, fontFace, color, line, howFarBelow, thickness, scale);
		}

		public static void DrawTextBelowLine(this Mat dest, String text, FontFace fontFace, Color color, Line line, int howFarBelow = 0, int thickness = 1, Double scale = 1)
		{
			int baseLine = 0;
			Size size = CvInvoke.GetTextSize(text, fontFace, scale, thickness, ref baseLine);
			PointD centerPoint = line.MidPoint.Offset(0, size.Height + howFarBelow);
			DrawCenteredText(dest, text, fontFace, color, centerPoint, thickness, scale);
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

		public static void DrawCenteredRectangle(this Mat image, Point center, Size size, Color color, int lineWidth = 1)
		{
			Rectangle rectangle = new Rectangle(new Point(center.X - size.Width / 2, center.Y - size.Height / 2), size);
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

		public static Mat ToGrayscaleImage(this Mat inputImage)
		{
			Mat returnImage;
			if(inputImage.NumberOfChannels > 1)
			{
				returnImage = new Mat();
				CvInvoke.CvtColor(inputImage, returnImage, ColorConversion.Bgr2Gray);
			}
			else
			{
				returnImage = inputImage.Clone();
			}
			return returnImage;
		}

		public static Mat ToCannyImage(this Mat inputImage)
		{
			return new Mat();
			//var outputImage = CvInvoke.Canny(inputImage, inputImage.Canny(400, 20);
			//return outputImage.ToUMat().ToImage<Bgr, byte>();
		}

		public static void LoadFromByteArray(this Mat image, byte[] data, ImreadModes mode = ImreadModes.Unchanged)
		{
			unsafe
			{
				byte* ptr = (byte*)image.DataPointer.ToPointer();
				Marshal.Copy(data, 0, image.DataPointer, data.Length);
			}
		}

		public static byte[] ToByteArray(this Mat image)
		{
			byte[] data = new byte[image.NumberOfChannels * image.Rows * image.Cols];
			unsafe
			{
				byte* ptr = (byte*)image.DataPointer.ToPointer();
				Marshal.Copy(image.DataPointer, data, 0, data.Length);
			}
			return data;
		}

		public static Mat Resize(this Mat image, Size size, float fx = 0, float fy = 0, Inter interpolation = Inter.Linear)
		{
			Mat output = new Mat();
			CvInvoke.Resize(image, output, size, fx, fy, interpolation);
			return output;
		}

		public static List<Rectangle> FindObjects(this Mat image, CascadeClassifier classifier, Double scaleFactor = 1.1, int minNeighbors = 3, Size minimumSize = default(Size), Size maximumSize = default(Size))
		{
			Rectangle[] rectangles = classifier.DetectMultiScale(image, scaleFactor, minNeighbors, minimumSize, maximumSize);
			List<Rectangle> result = new List<Rectangle>(rectangles);
			return result;
		}

	}
}
