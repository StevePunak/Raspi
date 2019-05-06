using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using KanoopCommon.Logging;

namespace RaspiCommon.Devices.Optics
{
	public class RaspiCamCv : Camera
	{
		#region Public Properties

		public VideoCapture VideoCapture { get; protected set; }

		#endregion

		#region Constructor

		public RaspiCamCv()
			: base(typeof(RaspiCamCv).Name)
		{
			VideoCapture = new VideoCapture(0);
			VideoCapture.SetCaptureProperty(CapProp.Fps, 2);
			VideoCapture.SetCaptureProperty(CapProp.FrameWidth, 800);
			VideoCapture.SetCaptureProperty(CapProp.FrameHeight, 600);

			VideoCapture.ImageGrabbed += OnImageGrabbed;

			Log.LogText(LogLevel.INFO, "\n\n\n\nCamera {0} opened {1}", this, VideoCapture.IsOpened);
			VideoCapture.Start(new OpenCvExceptionHandler());

			Interval = TimeSpan.FromSeconds(5);
		}

		private void OnImageGrabbed(object sender, EventArgs e)
		{
		}

		#endregion

		#region Thread Run

		protected override bool OnRun()
		{
			Log.LogText(LogLevel.DEBUG, "Capturing from video stream");

			Mat mat = VideoCapture.QueryFrame();

			String filename = String.Format("{0}/{1:0000}-snap.jpg", ImageDirectory, ImageNumber);
			if(VideoCapture.Grab())
			{
				Log.LogText(LogLevel.DEBUG, "Grab returned true");
			}
			else
			{
				Log.LogText(LogLevel.DEBUG, "Grab returned false");
			}
			//mat.Save(filename);
			//Log.LogText(LogLevel.DEBUG, "Got a {0} frame at {1}", mat.Size, filename);
			//LastSavedImage = filename;
			//InvokeSnapshotTaken(filename, ImageType.Jpeg);

			return true;
		}

		#endregion

		#region Public Methods

		public override bool TryTakeSnapshot(out Mat image)
		{
			image = null;
			return false;
		}
		#endregion
	}
}
