using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace Testing
{
	class ImageConvert
	{
		static public void Convert(String sourceDirectory, String wildCard, String outputDirectory)
		{
			foreach(String file in Directory.GetFiles(sourceDirectory, wildCard))
			{
//				Bitmap bitmap = new Bitmap(file);
				Mat infile = new Mat(file);
				String outputFileName = Path.Combine(outputDirectory, String.Format(@"{0}{1}", Path.GetFileNameWithoutExtension(file), ".png"));
//				bitmap.Save(outputFileName, System.Drawing.Imaging.ImageFormat.Png);
				//File.Copy(file, outputFileName);
//				infile.Save(outputFileName);
			}
		}
	}
}
