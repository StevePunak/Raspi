using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace Testing
{
	class FrameGrabTest
	{
		public FrameGrabTest()
		{
			String url = "http://raspi:8085/?action=snapshot";

			WebRequest request = WebRequest.Create(url);

			using(WebResponse response = request.GetResponse())
			using(Stream dataStream = response.GetResponseStream())
			using(BinaryReader reader = new BinaryReader(dataStream))
			{
				byte[] data = reader.ReadBytes(10000000);
				using(MemoryStream ms = new MemoryStream(data))
				{
					Mat mat = new Mat();
					CvInvoke.Imdecode(data, ImreadModes.Unchanged, mat);
					mat.Save(@"c:\pub\tmp\matimage.jpg");
					Bitmap bitmap = new Bitmap(ms);
					bitmap.Save(@"c:\pub\tmp\image.jpg");
				}
			}

		}
	}
}
