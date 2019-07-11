using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using KanoopCommon.Database;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.PersistentConfiguration;
using KanoopCommon.Serialization;
using RaspiCommon;
using RaspiCommon.Data.DataSource;
using RaspiCommon.Data.Entities.Facial;
using RaspiCommon.Data.Entities.Track;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.Compass;
using RaspiCommon.Devices.Locomotion;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Extensions;

namespace Radar
{
	static class Program
	{
		public static RaspiConfig Config { get; private set; }
		public static Log Log { get; private set; }
		public static NullCompass Compass { get; private set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			SolidColorAnalysis.Compass = Compass = new NullCompass();

			OpenLog();

			OpenConfig();

			SetConfigDefaults();

			Test();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new RadarForm());
		}

		static void Test()
		{
			String directory = @"c:\pub\tmp\images\unclassified";
			//LoadTrainingImagesIntoDatabase();
			//SaveTrainingImagesToFiles(directory);
			//TrainModel();
			//MoveImages(directory);
			//DetectFaces(directory);
			return;
		}

		private static void DetectFaces(string directory)
		{
			FacialDataSource fds = DataSourceFactory.Create<FacialDataSource>(Program.Config.FacialDBCredentials);
			FaceNameList names;
			fds.GetAllNames(out names);

			LBPHFaceRecognizer recognizer = new LBPHFaceRecognizer();
			recognizer.Read(Program.Config.LBPHRecognizerFile);
//			EigenFaceRecognizer recognizer = new EigenFaceRecognizer();
//			recognizer.Read(Program.Config.EigenRecognizerFile);

			foreach(String file in Directory.GetFiles(directory))
			{
				Mat image = new Mat(file).ToGrayscaleImage();
				FaceRecognizer.PredictionResult result = recognizer.Predict(image);

				String name;
				if(names.TryGetName(result.Label, out name))
				{
					FacePrediction prediction = new FacePrediction(new Rectangle(0, 0, image.Width, image.Height), image, name, result.Label, result.Distance);
					Log.LogText(LogLevel.DEBUG, "{0} is {1}", Path.GetFileName(file), prediction);
				}
			}


		}

		private static void MoveImages(string fromDirectory)
		{
			String name = "annika";
			fromDirectory = $@"c:\pub\tmp\images\raw\{name}";
			String toDirectory = $@"c:\pub\tmp\images\greyscale\{name}";
			if(Directory.Exists(toDirectory) == false)
				Directory.CreateDirectory(toDirectory);
			foreach(String file in Directory.GetFiles(fromDirectory))
			{
				String newFile = DirectoryExtensions.GetNextNumberedFileName(toDirectory, name, ".bmp");
				Mat mat = new Mat(file).ToGrayscaleImage();
				mat.Save(newFile);
				//File.Copy(file, newFile)
			}
		}

		private static void LoadTrainingImagesIntoDatabase()
		{
			FacialDataSource fds = DataSourceFactory.Create<FacialDataSource>(Program.Config.FacialDBCredentials);
			List<String> names = new List<string>()
			{
				"papa", "karina", "annika"
			};
			foreach(String name in names)
			{
				String fromDirectory = $@"c:\pub\tmp\images\greyscale\{name}";
				foreach(String file in Directory.GetFiles(fromDirectory))
				{
					Mat image = new Mat(file).ToGrayscaleImage();
					fds.AddImage(name, image);
					//File.Copy(file, newFile)
				}
			}
		}

		private static void TrainModel()
		{
			FacialDataSource fds = DataSourceFactory.Create<FacialDataSource>(Program.Config.FacialDBCredentials);

			List<FacialImage> faces;
			fds.GetAllFacialImages(out faces);

			FaceNameList names;
			fds.GetAllNames(out names);

			VectorOfMat images = new VectorOfMat();
			VectorOfInt nameIDs = new VectorOfInt();
			foreach(FacialImage image in faces)
			{
				images.Push(image.Image);
				nameIDs.Push(new int[] { (int)image.NameID });
				Log.SysLogText(LogLevel.DEBUG, $"Saved {image.Name} to model");
			}
			LBPHFaceRecognizer _lpRecognizer = new LBPHFaceRecognizer();
			_lpRecognizer.Train(images, nameIDs);
			_lpRecognizer.Write(Program.Config.LBPHRecognizerFile);

			EigenFaceRecognizer _eigenRecognizer = new EigenFaceRecognizer();
			_eigenRecognizer.Train(images, nameIDs);
			_eigenRecognizer.Write(Program.Config.EigenRecognizerFile);

			FisherFaceRecognizer _fisherRecognizer = new FisherFaceRecognizer();
			_fisherRecognizer.Train(images, nameIDs);
			_fisherRecognizer.Write(Program.Config.FisherRecognizerFile);

		}

		private static void SaveTrainingImagesToFiles(String directory)
		{
			FacialDataSource fds = DataSourceFactory.Create<FacialDataSource>(Program.Config.FacialDBCredentials);

			List<FacialImage> faces;
			fds.GetAllFacialImages(out faces);

			FaceNameList names;
			fds.GetAllNames(out names);

			foreach(FacialImage image in faces)
			{
				String filename = DirectoryExtensions.GetNextNumberedFileName(directory, $"{image.Name}-", ".bmp");
				image.Image.Save(filename);
			}

		}

		static void SetConfigDefaults()
		{
			Config.MqttPublicHost = "thufir";
			Config.EigenRecognizerFile = @"c:\pub\classify\faces.eigen";
			Config.LBPHRecognizerFile = @"c:\pub\classify\faces.lbph";
			Config.FisherRecognizerFile = @"c:\pub\classify\faces.fisher";
			Config.RemoteImageDirectory = @"\\raspi\pi\images";
			Config.FaceCascadeFile = @"c:\pub\classify\cascades\haarcascade_frontalface_default.xml";
			Config.BatonCascadeFile = @"c:\pub\classify\cascades\baton_cascade.xml";
			Config.LidarServer = "thufir:5959";
		}

		static void TestImage()
		{
			Program.Config.BlueThresholds = new ColorThreshold(Color.Blue, 150, 70);
			Program.Config.GreenThresholds = new ColorThreshold(Color.Green, 150, 100);
			Program.Config.RedThresholds = new ColorThreshold(Color.Red, 150, 70);
			Config.Save();

			SolidColorAnalysis.Width = 800;
			SolidColorAnalysis.Height = 600;
			SolidColorAnalysis.ColorThresholds[Color.Blue] = Program.Config.BlueThresholds;
			SolidColorAnalysis.ColorThresholds[Color.Green] = Program.Config.GreenThresholds;
			SolidColorAnalysis.ColorThresholds[Color.Red] = Program.Config.RedThresholds;

			String remoteFile = @"\\raspi\pi\images\snap-0003.bmp";
			String workDir = @"c:\pub\tmp\junk";
			String inputFile = Path.Combine(workDir, Path.GetFileName(remoteFile));

			File.Copy(remoteFile, inputFile, true);
			Camera.ImageDirectory = workDir;
			SolidColorAnalysis.ImageAnalysisDirectory = workDir;

			Mat image = new Mat(inputFile);
//		Camera.ConvertImage(outputFile, new Size(LEDImageAnalysis.Width, LEDImageAnalysis.Height), ImageType.RawRGB, ImageType.Bitmap, 0, out outputFile);

			SolidColorAnalysis analysis = new SolidColorAnalysis(new NullCamera());
			analysis.AnalyzeImage(image, new Size(6, 6)); //, workDir, out files, out leds, out candidates);
			//ImageAnalysis analysis = new ImageAnalysis(files, leds, candidates);
			//analysis.Serialize();

			//ledAnalysis.LEDs = leds;

		}

		private static void OpenConfig()
		{
			String      configFileName = RaspiConfig.GetDefaultConfigFileName();

			if(Directory.Exists(Path.GetDirectoryName(configFileName)))
				Directory.SetCurrentDirectory(Path.GetDirectoryName(configFileName));
			else
				Directory.CreateDirectory(Path.GetDirectoryName(configFileName));

			Log.LogText(LogLevel.DEBUG, "Opening config...");

			ConfigFile  configFile = new ConfigFile(configFileName);
			Config = (RaspiConfig)configFile.GetConfiguration(typeof(RaspiConfig));

			Config.Save();
		}

		private static void OpenLog()
		{
			Console.WriteLine("Opening log");
			Log = new Log();
			UInt32 flags = OpenFlags.CONTENT_TIMESTAMP | OpenFlags.OUTPUT_TO_FILE | OpenFlags.OUTPUT_TO_DEBUG;
			Log.Open(LogLevel.ALWAYS, "robo.log", flags);
			Log.SystemLog = Log;
			Console.WriteLine("Log opened");
		}
	}
}
