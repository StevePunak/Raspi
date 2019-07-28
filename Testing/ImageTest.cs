using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Extensions;
using RaspiCommon.GraphicalHelp;

namespace Testing
{
	class ImageTest : TestBase
	{
		protected override void Run()
		{
			byte[] data = File.ReadAllBytes(@"c:\tmp\bytes.bin");

			Mat image = new Mat(new Size(800, 600), DepthType.Cv8U, 3);
			image.LoadFromByteArray(data);
			image.Save(@"c:\tmp\image.jpg");
		}
	}
}
