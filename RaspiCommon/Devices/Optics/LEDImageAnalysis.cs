using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using RaspiCommon.Extensions;
using RaspiCommon.Network;

namespace RaspiCommon.Devices.Optics
{
	public class LEDImageAnalysis
	{
		public event CameraImagesAnalyzedHandler CameraImagesAnalyzed;

		public Camera Camera { get; set; }
		public static int Width { get; set; }
		public static int Height { get; set; }
		public static int LedLowThreshold { get; set; }
		public static int LedHighThreshold { get; set; }
		public String LastImageAnalysisFile { get; set; }
		public static String ImageAnalysisDirectory { get; set; }

		public Double BearingOffset { get; set; }

		public bool HasGreen { get { return HasLedColor(Color.Green); } }
		public LEDPosition GreenLED { get { return GetLedColor(Color.Green); } }

		public bool HasRed { get { return HasLedColor(Color.Red); } }
		public LEDPosition RedLED { get { return GetLedColor(Color.Red); } }

		public bool HasBlue { get { return HasLedColor(Color.Blue); } }
		public LEDPosition BlueLED { get { return GetLedColor(Color.Blue); } }

		public LEDPositionList LEDs { get; set; }

		public LEDImageAnalysis(Camera camera)
		{
			Camera = camera;
			camera.SnapshotTaken += OnCameraSnapshotTaken;

			Width = Camera.Width;
			Height = Camera.Height;
			LedLowThreshold = 5;
			LedHighThreshold = 50;

			LEDs = new LEDPositionList();

			CameraImagesAnalyzed += delegate {};
		}

		public void AnalyzeImage()
		{
			try
			{
				if(Camera.LastSavedImage == null)
				{
					throw new RaspiException("No camera image available");
				}
				if(File.Exists(Camera.LastSavedImage) == false)
				{
					throw new RaspiException("No camera image available at {0}", Camera.LastSavedImage);
				}

				if(!(Camera.ImageType == ImageType.RawRGB || Camera.ImageType == ImageType.RawBGR))
				{
					throw new RaspiException("Can't analyze image type {0}", Camera.ImageType);
				}

				List<String> outputFiles;
				LEDPositionList leds;
				AnalyzeImage(Camera.LastSavedImage, ImageAnalysisDirectory, LedLowThreshold, LedHighThreshold, out outputFiles, out leds);

				LEDs = leds;

				List<String> trimmedFilenames = outputFiles.GetFileNames();
				ImageAnalysis analysis = new ImageAnalysis(trimmedFilenames, leds);
				CameraImagesAnalyzed(analysis);

				Log.SysLogText(LogLevel.DEBUG, "Output {0}, {1}, {2}, {3}\n{4}", outputFiles[0], outputFiles[1], outputFiles[2], outputFiles[3], leds.ToString());
				LastImageAnalysisFile = Camera.LastSavedImage;
			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.ERROR, "AnalyzeImage EXCEPTION: {0}", e.Message);
			}
		}

		public static void AnalyzeImage(String filename, String outputDirectory, int lowThreshold, int highThreshold, out List<String> outputFilenames, out LEDPositionList leds)
		{
			outputFilenames = new List<string>();
			leds = new LEDPositionList();

			Mat image = new Mat(filename);

			String fullImage = Path.Combine(outputDirectory, "fullimage.bmp");
			image.Save(fullImage);
			Width = image.Width;
			Height = image.Height;

			outputFilenames.Add(fullImage);
			Log.SysLogText(LogLevel.DEBUG, "Saved {0}  Low Threshold: {1} High Threshold {2}", fullImage, lowThreshold, highThreshold);

			Mat blue, green, red;
			//
			//  Save Split Images
			//
			{
				String file = Path.Combine(outputDirectory, "blue.bmp");
				MCvScalar lowRange = new MCvScalar(lowThreshold, 0, 0);
				MCvScalar topRange = new MCvScalar(highThreshold, 0, 0);
				Mat outputImage = new Mat(image.Size, DepthType.Cv8U, 1);
				CvInvoke.InRange(image, new ScalarArray(lowRange), new ScalarArray(topRange), outputImage);
				outputImage.Save(file);
				outputFilenames.Add(file);
				blue = outputImage;
			}
			{
				String file = Path.Combine(outputDirectory, "red.bmp");
				MCvScalar lowRange = new MCvScalar(0, 0, lowThreshold);
				MCvScalar topRange = new MCvScalar(0, 0, highThreshold);
				Mat outputImage = new Mat(image.Size, DepthType.Cv8U, 1);
				CvInvoke.InRange(image, new ScalarArray(lowRange), new ScalarArray(topRange), outputImage);
				outputImage.Save(file);
				outputFilenames.Add(file);
				green = outputImage;
			}
			{
				String file = Path.Combine(outputDirectory, "green.bmp");
				MCvScalar lowRange = new MCvScalar(0, lowThreshold, 0);
				MCvScalar topRange = new MCvScalar(0, highThreshold, 0);
				Mat outputImage = new Mat(image.Size, DepthType.Cv8U, 1);
				CvInvoke.InRange(image, new ScalarArray(lowRange), new ScalarArray(topRange), outputImage);
				outputImage.Save(file);
				outputFilenames.Add(file);
				red = outputImage;
			}

			leds = FindLEDs(blue, green, red);
		}

		public static LEDPositionList FindLEDs(Mat blue, Mat red, Mat green)
		{
			LEDPositionList leds = new LEDPositionList();
			LEDPosition position;
			if(TryFindPosition(blue, Color.Blue, out position))
				leds.Add(position);
			if(TryFindPosition(red, Color.Red, out position))
				leds.Add(position);
			if(TryFindPosition(green, Color.Green, out position))
				leds.Add(position);
			return leds;
		}

		public static bool TryFindPosition(Mat bitmap, Color color, out LEDPosition position)
		{
			position = null;

			PointD location;
			Log.SysLogText(LogLevel.DEBUG, "Analyze {0}", color);
			if(bitmap.FindMatrix(new Size(6, 6), .1, out location))
			{
				position = new LEDPosition()
				{
					Color = color,
					Location = location,
				};
				Log.SysLogText(LogLevel.DEBUG, "Made LED position {0}", position);
			}

			return position != null;
		}

		public bool TryGetBearing(Color color, Double fromBearing, out Double bearing)
		{
			bearing = 0;
			bool result = false;
			if(HasLedColor(color))
			{
				LEDPosition led = GetLedColor(color);

				Double x = led.Location.X;
				Double pixelsPerDegree = Width / Camera.FieldOfViewHorizontal;
				bearing = fromBearing.AddDegrees((x - (Width / 2)) / pixelsPerDegree);
				result = true;
			}
			return result;
		}

		private void OnCameraSnapshotTaken(string imageLocation, ImageType imageType)
		{
		}

		bool HasLedColor(Color color)
		{
			return LEDs.Find(l => l.Color == color) != null;
		}

		LEDPosition GetLedColor(Color color)
		{
			return LEDs.Find(l => l.Color == color);
		}

	}
}
