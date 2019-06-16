using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Extensions;
using RaspiCommon.GraphicalHelp;

namespace Testing
{
	class ImageTest
	{
		public ImageTest()
		{
			String file = @"c:\pub\tmp\image.jpg";
			Mat image = new Mat(file);
			List<String> outputFiles;
			ColoredObjectPositionList objects;
			ObjectCandidateList candidates;
			SolidColorAnalysis.AnalyzeImage(image, new List<Color>() { Color.Red }, new Size(300, 300), @"c:\pub\tmp\analysis", out outputFiles, out objects, out candidates);
		}
	}
}
