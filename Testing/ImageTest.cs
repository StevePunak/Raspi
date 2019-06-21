using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
			byte[] inputData = File.ReadAllBytes(file);
			Mat output = new Mat();
			output.LoadFromByteArray(inputData);
			output.Save(@"c:\pub\tmp\output.jpg");
		}
	}
}
