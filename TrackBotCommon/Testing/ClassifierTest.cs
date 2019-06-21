using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Extensions;
using RaspiCommon.Extensions;

namespace TrackBotCommon.Testing
{
	public class ClassifierTest
	{
		public CascadeClassifier Classifier { get; private set; }

		public ClassifierTest(String inputFile)
		{
			ClassifyImage();
			return;
			String annoFile = @"c:\pub\classify\baton\positive\annotations.txt";
			String imageDirectory = Path.Combine(Path.GetDirectoryName(annoFile), "fullimages");
			String outputDirectory = Path.Combine(Path.GetDirectoryName(annoFile), "partimages");
			foreach(String line in File.ReadAllLines(annoFile))
			{
				String[] parts = line.Split(' ');
				String imageFile = Path.Combine(imageDirectory, Path.GetFileName(parts[0]));
				Mat image = new Mat(imageFile);
				int count = int.Parse(parts[1]);
				int partIndex = 2;
				int imageIndex = 0;
				for(;imageIndex < count;imageIndex++)
				{
					int x = int.Parse(parts[partIndex]);
					int y = int.Parse(parts[partIndex + 1]);
					int width = int.Parse(parts[partIndex + 2]);
					int height = int.Parse(parts[partIndex + 3]);
					partIndex += 4;

					Mat partMat = new Mat(image, new Rectangle(x, y, width, height));
					String outputFile = DirectoryExtensions.GetNextNumberedFileName(outputDirectory, "partial-", ".jpg", 4);
					partMat.Save(outputFile);
				}
			}

		}

		void ClassifyImage()
		{
			String imageDirectory = Environment.OSVersion.Platform == PlatformID.Unix
				? @"/home/pi/classify/"
				: @"c:\pub\classify\images";
			String cascadeDirectory = Environment.OSVersion.Platform == PlatformID.Unix
				? @"/home/pi/classify/cascades"
				: @"c:\pub\classify\cascades"; 
			String inputFile = Path.Combine(imageDirectory, "20190616_141209.jpg");
			String cascadeFile = Path.Combine(cascadeDirectory, "haarcascade_frontalface_default.xml");
			if(Environment.OSVersion.Platform == PlatformID.Unix)
			{
				inputFile = @"/home/pi/classify/image0024.jpg";
				cascadeFile = @"/home/pi/classify/cascade.xml";
			}
			Classifier = new CascadeClassifier(cascadeFile);
			Mat image = new Mat(inputFile);
			Rectangle[] rectangles = Classifier.DetectMultiScale(image, 1.1, 3, new Size(1, 1));
			Console.WriteLine($"Found {rectangles.Length} rectangles");
		}
	}
}
