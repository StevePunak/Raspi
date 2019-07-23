using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using KanoopCommon.Conversions;
using KanoopCommon.Database;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.PersistentConfiguration;
using KanoopCommon.Queueing;
using KanoopCommon.Threading;
using RaspiCommon;
using RaspiCommon.Data.DataSource;
using RaspiCommon.Data.Entities.Facial;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.GamePads;
using RaspiCommon.Devices.MotorControl;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Devices.Servos;
using RaspiCommon.Devices.Spatial;
using RaspiCommon.Extensions;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Network;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.LidarImaging;
using SharpDX.DirectInput;
using TrackBotCommon;
using TrackBotCommon.InputDevices;
using TrackBotCommon.InputDevices.GamePads;
using TrackBotCommon.Testing;

namespace Testing
{

	class Program
	{
		public static RaspiConfig Config { get; private set; }
		public static Log Log { get; private set; }

		static PointCloud2D PointCloud { get; set; }

		static void Main(string[] args)
		{
			OpenLog();
			OpenConfig();

			Test();

			Console.WriteLine("Done");
		}

		private static void Test()
		{
			//new BMP280Test();
			//new ADS1115Test();
			//new ClawControlTest();
			//new WiringPiTest();
			new LidarClientTest();
			//new PigsTest();
			//new SerialTest();
			//new LidarTest();
		}

		private static void RecTests()
		{
			//LoadTrainingImagesIntoDatabase();
			//SaveTrainingImagesToFiles(directory);
			//TrainModel();
			//MoveImages(directory);
			//DetectFaces(directory);
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
			}
			LBPHFaceRecognizer _lpRecognizer = new LBPHFaceRecognizer();
			_lpRecognizer.Train(images, nameIDs);
			_lpRecognizer.Write(Program.Config.LBPHRecognizerFile);
			Log.SysLogText(LogLevel.DEBUG, $"LBPHFaceRecognizer complete");

			EigenFaceRecognizer _eigenRecognizer = new EigenFaceRecognizer();
			_eigenRecognizer.Train(images, nameIDs);
			_eigenRecognizer.Write(Program.Config.EigenRecognizerFile);
			Log.SysLogText(LogLevel.DEBUG, $"EigenFaceRecognizer complete");

			FisherFaceRecognizer _fisherRecognizer = new FisherFaceRecognizer();
			_fisherRecognizer.Train(images, nameIDs);
			_fisherRecognizer.Write(Program.Config.FisherRecognizerFile);
			Log.SysLogText(LogLevel.DEBUG, $"FisherFaceRecognizer complete");

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
			Log = new Log();
			Log.Open(LogLevel.ALWAYS, "lidar.log", OpenFlags.CONTENT_TIMESTAMP | OpenFlags.OUTPUT_TO_FILE | OpenFlags.OUTPUT_TO_DEBUG | OpenFlags.OUTPUT_TO_CONSOLE);
			Log.SystemLog = Log;
		}
	}
}
