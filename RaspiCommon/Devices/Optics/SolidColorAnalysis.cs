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
	public class SolidColorAnalysis
	{
		public static readonly List<Color> AllLEDColors = new List<Color>() { Color.Blue, Color.Red, Color.Green };

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
		public ColoredObjectPosition GreenLED { get { return GetColor(Color.Green); } }

		public bool HasRed { get { return HasColor(Color.Red); } }
		public ColoredObjectPosition RedLED { get { return GetColor(Color.Red); } }

		public bool HasBlue { get { return HasColor(Color.Blue); } }
		public ColoredObjectPosition BlueLED { get { return GetColor(Color.Blue); } }

		public ColoredObjectPositionList ColorObjects { get; set; }
		public ObjectCandidateList candidates { get; set; }

		public static bool DebugAnalysis { get; set; }

		static SolidColorAnalysis()
		{
			ColorThresholds = new ColorThresholdList()
			{
				{ Color.Blue,	new ColorThreshold() { Color =  Color.Blue, MaximumOtherValue = 128, MinimumValue = 128 } },
				{ Color.Green,	new ColorThreshold() { Color =  Color.Green, MaximumOtherValue = 128, MinimumValue = 128 } },
				{ Color.Red,	new ColorThreshold() { Color =  Color.Red, MaximumOtherValue = 128, MinimumValue = 128 } },
			};
			DebugAnalysis = false;
		}

		public SolidColorAnalysis(Camera camera)
		{
			Camera = camera;
			camera.SnapshotTaken += OnCameraSnapshotTaken;

			Width = Camera.Parameters.Width;
			Height = Camera.Parameters.Height;
			PixelsPerDegree = Width / Camera.FieldOfViewHorizontal;

			ColorObjects = new ColoredObjectPositionList();
			Compass = new NullCompass();

			CameraImagesAnalyzed += delegate {};
		}

		public void AnalyzeImage(Size size)
		{
			AnalyzeImage(null, size);
		}

		public void AnalyzeImage(Mat image, Size size)
		{
			AnalyzeImage(image, AllLEDColors, size);
		}

		public void AnalyzeImage(Mat image, Color color, Size size)
		{
			AnalyzeImage(image, new List<Color>() { color }, size);
		}

		public void AnalyzeImage(Mat image, List<Color> colors, Size size)
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

				if(!(Camera.Parameters.ImageType == ImageType.RawRGB || Camera.Parameters.ImageType == ImageType.RawBGR || Camera.Parameters.ImageType == ImageType.Jpeg))
				{
					throw new RaspiException("Can't analyze image type {0}", Camera.Parameters.ImageType);
				}

				if(image == null)
				{
					image = new Mat(Camera.LastSavedImageFileName);
				}

				List<String> outputFiles;
				ColoredObjectPositionList coloredObjects;
				ObjectCandidateList candidates;
				AnalyzeImage(image, colors, size, ImageAnalysisDirectory, out outputFiles, out coloredObjects, out candidates);

				ColorObjects = coloredObjects;

				List<String> trimmedFilenames = outputFiles.GetFileNames();
				ImageAnalysis analysis = new ImageAnalysis(trimmedFilenames, coloredObjects, candidates);

				CameraImagesAnalyzed(analysis);

				LastImageAnalysisFile = Camera.LastSavedImageFileName;
			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.ERROR, "AnalyzeImage EXCEPTION: {0}\n{1}", e.Message, ThreadBase.GetFormattedStackTrace(e));
			}
		}

		/// <summary>
		/// Analyze the given image for ColoredObjects
		/// </summary>
		/// <param name="filename">File containing full color image</param>
		/// <param name="colors">Object colors to look for</param>
		/// <param name="outputDirectory">Directory to put red.bmp, green.mbp and blue.bmp</param>
		/// <param name="lowThreshold">The minimum value for the search color</param>
		/// <param name="highThreshold">The max amount of other colors than the search color which will be allowed</param>
		/// <param name="outputFilenames">Output for the saved files</param>
		/// <param name="coloredObjects">Output for found LEDs</param>
		public static void AnalyzeImage(String filename, List<Color> colors, Size size, String outputDirectory, out List<String> outputFilenames, out ColoredObjectPositionList coloredObjects, out ObjectCandidateList candidates)
		{
			Mat image = new Mat(filename);
			AnalyzeImage(image, colors, size, outputDirectory, out outputFilenames, out coloredObjects, out candidates);
		}

		/// <summary>
		/// Analyze the given image for colored objects
		/// </summary>
		/// <param name="filename">File containing full color image</param>
		/// <param name="colors">Object colors to look for</param>
		/// <param name="outputDirectory">Directory to put red.bmp, green.mbp and blue.bmp</param>
		/// <param name="lowThreshold">The minimum value for the search color</param>
		/// <param name="highThreshold">The max amount of other colors than the search color which will be allowed</param>
		/// <param name="outputFilenames">Output for the saved files</param>
		/// <param name="coloredObjects">Output for found LEDs</param>
		public static void AnalyzeImage(Mat image, List<Color> colors, Size size, String outputDirectory, out List<String> outputFilenames, out ColoredObjectPositionList coloredObjects, out ObjectCandidateList candidates)
		{
			outputFilenames = new List<string>();
			coloredObjects = new ColoredObjectPositionList();

			if(Directory.Exists(outputDirectory) == false)
			{
				Directory.CreateDirectory(outputDirectory);
			}

			String fullImage = Path.Combine(outputDirectory, "fullimage.bmp");
			Log.SysLogText(LogLevel.DEBUG, "AnalyzeImage saving {0}", fullImage);
			image.Save(fullImage);

			Width = image.Width;
			Height = image.Height;

			outputFilenames.Add(fullImage);

			//
			//  Save Split Images
			//
			Dictionary<Color, Mat> mats = new Dictionary<Color, Mat>();
			foreach(Color color in colors)
			{
				Mat mat = SplitColor(image, outputDirectory, color, ref outputFilenames);
				mats.Add(color, mat);
			}
			candidates = new ObjectCandidateList();
			coloredObjects = FindColoredObjects(mats, size, ref candidates);
		}

		/// <summary>
		/// Split the full color image into its parts
		/// </summary>
		/// <param name="image"></param>
		/// <param name="outputDirectory"></param>
		/// <param name="color"></param>
		/// <param name="filenames"></param>
		/// <returns></returns>
		static Mat SplitColor(Mat image, String outputDirectory, Color color, ref List<String> filenames)
		{
			String filename = Path.Combine(outputDirectory, String.Format("{0}.bmp", color.Name.ToLower()));
			Mat outputImage = new Mat(image.Size, DepthType.Cv8U, 1);

			MatEvaluation eval = null;
			if(DebugAnalysis)
			{
				eval = new MatEvaluation(image, color);
				Log.SysLogText(LogLevel.DEBUG, "The highest {0} pixel is {1}", color.Name, eval);
			}

			MCvScalar lowRange, topRange;
			ColorThresholds[color].MakeMCvScalarArrays(out lowRange, out topRange);
			CvInvoke.InRange(image, new ScalarArray(lowRange), new ScalarArray(topRange), outputImage);

			/// if we got no data, try with the eval
			int count = CvInvoke.CountNonZero(outputImage);
			const int EVAL_DIFF_THRESHOLD = 150;
			if(count == 0 && eval != null && eval.DiffPixel.TotalDiff > EVAL_DIFF_THRESHOLD)
			{
				outputImage.SetPixel(new PointD(eval.DiffPixel.Point), 0, 255);
				count = CvInvoke.CountNonZero(outputImage);
			}
			Log.SysLogText(LogLevel.DEBUG, "There are {0} non-zero pixels in {1}, Thresholds === {2}.. saving {3}", count, color, ColorThresholds[color], filename);
			outputImage.Save(filename);
			filenames.Add(filename);
			return outputImage;
		}

		public static ColoredObjectPositionList FindColoredObjects(Dictionary<Color, Mat> mats, Size size, ref ObjectCandidateList candidates)
		{
			ColoredObjectPositionList leds = new ColoredObjectPositionList();
			ColoredObjectPosition position;

			foreach(Color color in mats.Keys)
			{
				ObjectCandidateList additionalCandidates;
				if(TryFindPosition(mats[color], color, size, out additionalCandidates, out position))
				{
					leds.Add(position);
					candidates.AddRange(additionalCandidates);
				}
			}
			return leds;
		}

		public static bool TryFindPosition(Mat bitmap, Color color, Size size, out ObjectCandidateList candidates, out ColoredObjectPosition position)
		{
			position = null;
			candidates = new ObjectCandidateList();

			ObjectCandidateList allCandidates;
			if(bitmap.FindObjectMatrix(color, size, out allCandidates))
			{
				candidates = allCandidates;
				ColoredObjectCandidate winner = candidates.Find(c => c.Concentration == allCandidates.Max(c1 => c1.Concentration));
				if(winner == null)
				{
					Log.SysLogText(LogLevel.DEBUG, "THERE IS NO WINNER in {0} elements", allCandidates.Count);
					allCandidates.DumpToLog();
				}
				position = new ColoredObjectPosition()
				{
					Color = color,
					Location = new PointD(winner.Center),
					Bearing = Compass != null ? Compass.Bearing.AddDegrees((winner.Center.X - (Width / 2)) / PixelsPerDegree + BearingOffset) : 0,
					Size = winner.BoundingRectangle.Size,
					Candidate = winner,
				};
				Log.SysLogText(LogLevel.DEBUG, "Made object position {0} w/bearing offset {1}", position, BearingOffset.ToAngleString());
			}

			return position != null;
		}

		public bool TryGetBearing(Color color, Double fromBearing, out Double bearing)
		{
			bearing = 0;
			bool result = false;
			if(HasColor(color))
			{
				ColoredObjectPosition led = GetColor(color);

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
			return ColorObjects.Find(l => l.Color == color) != null;
		}

		public ColoredObjectPosition GetColor(Color color)
		{
			return ColorObjects.Find(l => l.Color == color);
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
