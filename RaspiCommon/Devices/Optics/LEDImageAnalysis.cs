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
using KanoopCommon.Threading;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Extensions;
using RaspiCommon.GraphicalHelp;
using RaspiCommon.Network;

namespace RaspiCommon.Devices.Optics
{
	public class LEDImageAnalysis
	{
		public event CameraImagesAnalyzedHandler CameraImagesAnalyzed;

		public Camera Camera { get; set; }
		public static int Width { get; set; }
		public static int Height { get; set; }
		public static Double PixelsPerDegree { get; private set; }
		public String LastImageAnalysisFile { get; set; }
		public static String ImageAnalysisDirectory { get; set; }

		public static Double BearingOffset { get; set; }
		public static ICompass Compass { get; set; }

		public static ColorThresholdList ColorThresholds { get; private set; }

		public bool HasGreen { get { return HasColor(Color.Green); } }
		public LEDPosition GreenLED { get { return GetColor(Color.Green); } }

		public bool HasRed { get { return HasColor(Color.Red); } }
		public LEDPosition RedLED { get { return GetColor(Color.Red); } }

		public bool HasBlue { get { return HasColor(Color.Blue); } }
		public LEDPosition BlueLED { get { return GetColor(Color.Blue); } }

		public LEDPositionList LEDs { get; set; }
		public LEDCandidateList candidates { get; set; }

		public static bool DebugAnalysis { get; set; }

		static LEDImageAnalysis()
		{
			ColorThresholds = new ColorThresholdList()
			{
				{ Color.Blue,	new ColorThreshold() { Color =  Color.Blue, MaximumOtherValue = 128, MinimumValue = 128 } },
				{ Color.Green,	new ColorThreshold() { Color =  Color.Green, MaximumOtherValue = 128, MinimumValue = 128 } },
				{ Color.Red,	new ColorThreshold() { Color =  Color.Red, MaximumOtherValue = 128, MinimumValue = 128 } },
			};
			DebugAnalysis = false;
		}

		public LEDImageAnalysis(Camera camera)
		{
			Camera = camera;
			camera.SnapshotTaken += OnCameraSnapshotTaken;

			Width = Camera.Width;
			Height = Camera.Height;
			PixelsPerDegree = Width / Camera.FieldOfViewHorizontal;

			LEDs = new LEDPositionList();
			Compass = new NullCompass();

			CameraImagesAnalyzed += delegate {};
		}

		public void AnalyzeImage(Mat image = null)
		{
			try
			{
				if(image == null && Camera.LastSavedImageFileName == null)
				{
					throw new RaspiException("No camera image available");
				}
				if(image == null && File.Exists(Camera.LastSavedImageFileName) == false)
				{
					throw new RaspiException("No camera image available at {0}", Camera.LastSavedImageFileName);
				}

				if(!(Camera.ImageType == ImageType.RawRGB || Camera.ImageType == ImageType.RawBGR))
				{
					throw new RaspiException("Can't analyze image type {0}", Camera.ImageType);
				}

				if(image == null)
				{
					image = new Mat(Camera.LastSavedImageFileName);
				}

				List<String> outputFiles;
				LEDPositionList leds;
				LEDCandidateList candidates;
				AnalyzeImage(image, ImageAnalysisDirectory, out outputFiles, out leds, out candidates);

				LEDs = leds;

				List<String> trimmedFilenames = outputFiles.GetFileNames();
				ImageAnalysis analysis = new ImageAnalysis(trimmedFilenames, leds, candidates);
				CameraImagesAnalyzed(analysis);

				LastImageAnalysisFile = Camera.LastSavedImageFileName;
			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.ERROR, "AnalyzeImage EXCEPTION: {0}\n{1}", e.Message, ThreadBase.GetFormattedStackTrace(e));
			}
		}

		/// <summary>
		/// Analyze the given image for LEDs
		/// </summary>
		/// <param name="filename">File containing full color image</param>
		/// <param name="outputDirectory">Directory to put red.bmp, green.mbp and blue.bmp</param>
		/// <param name="lowThreshold">The minimum value for the search color</param>
		/// <param name="highThreshold">The max amount of other colors than the search color which will be allowed</param>
		/// <param name="outputFilenames">Output for the saved files</param>
		/// <param name="leds">Output for found LEDs</param>
		public static void AnalyzeImage(String filename, String outputDirectory, out List<String> outputFilenames, out LEDPositionList leds, out LEDCandidateList candidates)
		{
			Mat image = new Mat(filename);
			AnalyzeImage(image, outputDirectory, out outputFilenames, out leds, out candidates);
		}

		/// <summary>
		/// Analyze the given image for LEDs
		/// </summary>
		/// <param name="filename">File containing full color image</param>
		/// <param name="outputDirectory">Directory to put red.bmp, green.mbp and blue.bmp</param>
		/// <param name="lowThreshold">The minimum value for the search color</param>
		/// <param name="highThreshold">The max amount of other colors than the search color which will be allowed</param>
		/// <param name="outputFilenames">Output for the saved files</param>
		/// <param name="leds">Output for found LEDs</param>
		public static void AnalyzeImage(Mat image, String outputDirectory, out List<String> outputFilenames, out LEDPositionList leds, out LEDCandidateList candidates)
		{
			outputFilenames = new List<string>();
			leds = new LEDPositionList();

			if(Directory.Exists(outputDirectory) == false)
			{
				Directory.CreateDirectory(outputDirectory);
			}

			String fullImage = Path.Combine(outputDirectory, "fullimage.bmp");
			image.Save(fullImage);
			Width = image.Width;
			Height = image.Height;

			outputFilenames.Add(fullImage);

			Mat blue, green, red;
			//
			//  Save Split Images
			//
			blue = SplitColor(image, outputDirectory, Color.Blue, outputFilenames);
			green = SplitColor(image, outputDirectory, Color.Green, outputFilenames);
			red = SplitColor(image, outputDirectory, Color.Red, outputFilenames);

			leds = FindLEDs(blue, green, red, out candidates);
		}

		static Mat SplitColor(Mat image, String outputDirectory, Color color, List<String> filenames)
		{
			String filename = Path.Combine(outputDirectory, String.Format("{0}.bmp", color.Name.ToLower()));
			Mat outputImage = new Mat(image.Size, DepthType.Cv8U, 1);

			if(DebugAnalysis)
			{
				MatEvaluation eval = new MatEvaluation(image, color);
				Log.SysLogText(LogLevel.DEBUG, "The highest {0} pixel is {1}", color.Name, eval);
			}

			MCvScalar lowRange, topRange;
			ColorThresholds[color].MakeMCvScalarArrays(out lowRange, out topRange);
			CvInvoke.InRange(image, new ScalarArray(lowRange), new ScalarArray(topRange), outputImage);
			int count = CvInvoke.CountNonZero(outputImage);
			Log.SysLogText(LogLevel.DEBUG, "There are {0} non-zero pixels in {1}, Thresholds === {2}", count, color, ColorThresholds[color]);
			outputImage.Save(filename);
			filenames.Add(filename);
			return outputImage;
		}

		public static LEDPositionList FindLEDs(Mat blue, Mat green, Mat red, out LEDCandidateList candidates)
		{
			LEDPositionList leds = new LEDPositionList();
			LEDPosition position;

			candidates = new LEDCandidateList();
			LEDCandidateList additionalCandidates;
			if(TryFindPosition(blue, Color.Blue, out additionalCandidates, out position))
			{
				leds.Add(position);
				candidates.AddRange(additionalCandidates);
			}
			if(TryFindPosition(red, Color.Red, out additionalCandidates, out position))
			{
				leds.Add(position);
				candidates.AddRange(additionalCandidates);
			}
			if(TryFindPosition(green, Color.Green, out additionalCandidates, out position))
			{
				leds.Add(position);
				candidates.AddRange(additionalCandidates);
			}
			return leds;
		}

		public static bool TryFindPosition(Mat bitmap, Color color, out LEDCandidateList candidates, out LEDPosition position)
		{
			position = null;
			candidates = new LEDCandidateList();

			LEDCandidateList allCandidates;
			if(bitmap.FindLEDMatrix(color, new Size(6, 6), out allCandidates))
			{
				candidates = allCandidates;
				LEDCandidate winner = candidates.Find(c => c.Concentration == allCandidates.Max(c1 => c1.Concentration));
				if(winner == null)
				{
					Log.SysLogText(LogLevel.DEBUG, "THERE IS NO WINNER in {0} elements", allCandidates.Count);
					allCandidates.DumpToLog();
				}
				position = new LEDPosition()
				{
					Color = color,
					Location = new PointD(winner.Center),
					Bearing = Compass.Bearing.AddDegrees((winner.Center.X - (Width / 2)) / PixelsPerDegree + BearingOffset),
					Size = winner.BoundingRectangle.Size,
				};
				Log.SysLogText(LogLevel.DEBUG, "Made LED position {0} w/bearing offset {1}", position, BearingOffset.ToAngleString());
			}

			return position != null;
		}

		public bool TryGetBearing(Color color, Double fromBearing, out Double bearing)
		{
			bearing = 0;
			bool result = false;
			if(HasColor(color))
			{
				LEDPosition led = GetColor(color);

				Double x = led.Location.X;
				bearing = fromBearing.AddDegrees((x - (Width / 2)) / PixelsPerDegree);
				result = true;
			}
			return result;
		}

		private void OnCameraSnapshotTaken(string imageLocation, ImageType imageType)
		{
		}

		public bool HasColor(Color color)
		{
			return LEDs.Find(l => l.Color == color) != null;
		}

		public LEDPosition GetColor(Color color)
		{
			return LEDs.Find(l => l.Color == color);
		}

		public static void SetThreshold(Color color, int minimumValue, int maxOtherColorValue)
		{
			ColorThresholds[color] = new ColorThreshold() { Color = color, MinimumValue = minimumValue, MaximumOtherValue = maxOtherColorValue };
		}

		public static void SetThreshold(ColorThreshold threshold)
		{
			ColorThresholds[threshold.Color] = threshold;
		}

	}
}
