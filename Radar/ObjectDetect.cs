using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;

namespace Radar
{
	public class ObjectDetect
	{
		Image<Bgr, byte> originalImage, grayscaleImage, cannyImage, contoursImage, contourOutput, trianglesImage, rectanglesImage, circlesImage, allShapesImage;
//		FullSizeImage fullImage;
		VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
		VectorOfPoint approxContour = new VectorOfPoint();
		List<Triangle2DF> triangleList = new List<Triangle2DF>();
		List<RotatedRect> boxList = new List<RotatedRect>();
		CircleF[] circles;
		int countTriangles = 0, countRectangles = 0, countCircles = 0;

		ImageBox ibCanny, ibTriangles, ibRectangles, ibCircles, ibAllDetectedShapes;
		Label lblTriangles, lblCircles, lblRectangles;

		public ObjectDetect( Mat image, ImageBox canny, ImageBox triangles, ImageBox rectangles, ImageBox circles, ImageBox allDetected,
							Label lTriangles, Label lRectangles, Label lCircles)
		{
			originalImage = image.ToImage<Bgr, byte>();

			ibCanny = canny;
			ibTriangles = triangles;
			ibRectangles = rectangles;
			ibCircles = circles;
			ibAllDetectedShapes = allDetected;
			lblTriangles = lTriangles;
			lblCircles = lCircles;
			lblRectangles = lRectangles;
		}

		public void Detect()
		{
			//Grayscale image from original image
			grayscaleImage = ToGrayscaleImage(originalImage);

			//Canny image from grayscale image
			cannyImage = ToCannyImage(grayscaleImage);
			ibCanny.Image = cannyImage;

			//Get and draw contours from canny image
			contoursImage = GetContours(cannyImage);

			//Get all objects
			for(int i = 0;i < contours.Size;i++)
			{
				getAllObjects(contours, i);
			}

			//Draw all Objects
			drawAllObjects();
		}
		private Image<Bgr, byte> ToGrayscaleImage(Image<Bgr, byte> inputImage)
		{
			UMat uimage = new UMat();
			CvInvoke.CvtColor(inputImage, uimage, ColorConversion.Bgr2Gray);
			var outputImage = uimage.ToImage<Bgr, Byte>();
			return outputImage;
		}

		private Image<Bgr, byte> ToCannyImage(Image<Bgr, byte> inputImage)
		{
			var outputImage = inputImage.Canny(400, 20);
			return outputImage.ToUMat().ToImage<Bgr, byte>();
		}

		private Image<Bgr, byte> GetContours(Image<Bgr, byte> inputImage)
		{
			double cannyThreshold = 180.0;
			double circleAccumulatorThreshold = 120;

			contourOutput = originalImage;
			UMat matInput = inputImage.ToUMat();
			matInput.ConvertTo(matInput, DepthType.Cv8U);
			CvInvoke.CvtColor(matInput, matInput, ColorConversion.Bgr2Gray);
			circles = CvInvoke.HoughCircles(matInput, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);
			CvInvoke.FindContours(matInput, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
			CvInvoke.DrawContours(contourOutput, contours, -1, new MCvScalar(0, 255, 0), 1);
			return contourOutput;
		}

		private void getAllObjects(VectorOfVectorOfPoint contours, int count)
		{
			trianglesImage = cannyImage.CopyBlank();
			rectanglesImage = cannyImage.CopyBlank();
			circlesImage = cannyImage.CopyBlank();
			VectorOfPoint contour = contours[count];
			CvInvoke.ApproxPolyDP(contour, approxContour, 10, true);
			//Triangle detection
			if(approxContour.Size == 3)
			{
				Point[] pts = approxContour.ToArray();
				triangleList.Add(new Triangle2DF(
				   pts[0],
				   pts[1],
				   pts[2]
				   ));
				countTriangles++;
			}

			//Rectangle detection
			if(approxContour.Size == 4)
			{
				bool isRectangle = true;
				Point[] pts = approxContour.ToArray();
				LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

				for(int j = 0;j < edges.Length;j++)
				{
					double angle = Math.Abs(edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
					if(angle < 80 || angle > 100)
					{
						isRectangle = false;
						break;
					}
				}
				if(isRectangle)
				{
					boxList.Add(CvInvoke.MinAreaRect(approxContour));
					countRectangles++;
				}
			}

		}

		private void drawAllObjects()
		{
			allShapesImage = cannyImage.CopyBlank();
			//Draw triangles
			foreach(Triangle2DF triangle in triangleList)
			{
				trianglesImage.Draw(triangle, new Bgr(Color.Orange), 2);
				allShapesImage.Draw(triangle, new Bgr(Color.Orange), 2);
			}

			ibTriangles.Image = trianglesImage;
			lblTriangles.Text = $"Triangles ({countTriangles})";

			//Draw rectangles
			foreach(RotatedRect box in boxList)
			{
				rectanglesImage.Draw(box, new Bgr(Color.DarkOrange), 2);
				allShapesImage.Draw(box, new Bgr(Color.Red), 2);
			}
			ibRectangles.Image = rectanglesImage;
			lblRectangles.Text = $"Rectangles ({countRectangles})";

			//Draw circles
			foreach(CircleF circle in circles)
			{
				circlesImage.Draw(circle, new Bgr(Color.Brown), 2);
				allShapesImage.Draw(circle, new Bgr(Color.Yellow), 2);
				countCircles++;
			}
			ibCircles.Image = circlesImage;
			lblCircles.Text = $"Circles ({countCircles})";

			//Draw all shapes
			ibAllDetectedShapes.Image = allShapesImage;
		}

#if zero
		//------------------Show image full size-------------------
		private void ibOriginal_DoubleClick(object sender, EventArgs e)
		{
			fullImage = new FullSizeImage(originalImage);
			fullImage.Show();
		}

		private void ibGrayscale_DoubleClick(object sender, EventArgs e)
		{
			fullImage = new FullSizeImage(grayscaleImage);
			fullImage.Show();
		}

		private void ibCanny_DoubleClick(object sender, EventArgs e)
		{
			fullImage = new FullSizeImage(cannyImage);
			fullImage.Show();
		}

		private void ibContours_DoubleClick(object sender, EventArgs e)
		{
			fullImage = new FullSizeImage(contourOutput);
			fullImage.Show();
		}

		private void ibTriangles_DoubleClick(object sender, EventArgs e)
		{
			fullImage = new FullSizeImage(trianglesImage);
			fullImage.Show();
		}

		private void ibRectangles_DoubleClick(object sender, EventArgs e)
		{
			fullImage = new FullSizeImage(rectanglesImage);
			fullImage.Show();
		}

		private void ibCircles_DoubleClick(object sender, EventArgs e)
		{
			fullImage = new FullSizeImage(circlesImage);
			fullImage.Show();
		}

		private void ibAllDetectedShapes_DoubleClick(object sender, EventArgs e)
		{
			fullImage = new FullSizeImage(allShapesImage);
			fullImage.Show();
		}
#endif
	}
}
