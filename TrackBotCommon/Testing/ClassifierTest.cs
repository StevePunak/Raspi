using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace TrackBotCommon.Testing
{
	public class ClassifierTest
	{
		public CascadeClassifier Classifier { get; private set; }

		public ClassifierTest(String inputFile)
		{
			String filename = @"C:\pub\classify\baton\single\trained\cascade.xml";
			Classifier = new CascadeClassifier(filename);
			Mat image = new Mat(inputFile);
			Rectangle[] rectangles = Classifier.DetectMultiScale(image, 1.4, 0);

		}
	}
}
