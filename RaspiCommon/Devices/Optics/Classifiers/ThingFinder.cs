using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using RaspiCommon.Extensions;

namespace RaspiCommon.Devices.Optics.Classifiers
{
	public class ThingFinder
	{
		public TimeSpan MinimumInterval { get; set; }
		public DateTime LastDetectTime { get; protected set; }
		public CascadeClassifier Classifier { get; private set; }
		public String FileName { get; set; }

		FoundObjectList Objects { get; set; }

		public ThingFinder(String cascadeFile)
		{
			FileName = cascadeFile;
			Classifier = new CascadeClassifier(cascadeFile);
			Objects = new FoundObjectList();
		}

		public FoundObjectList ProcessFrame(Mat frame)
		{
			FoundObjectList objects = new FoundObjectList();
			if(DateTime.UtcNow > LastDetectTime + MinimumInterval)
			{
				List<Rectangle> rectangles = frame.FindObjects(Classifier, 1.1, 3);
				foreach(Rectangle rectangle in rectangles)
				{
					objects.Add(new FoundObject() { DetectTime = DateTime.UtcNow, Rectangle = rectangle });
				}
				LastDetectTime = DateTime.UtcNow;
				Objects = objects;
			}
			return Objects;
		}
	}
}
