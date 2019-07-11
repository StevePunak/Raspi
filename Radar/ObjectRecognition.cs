using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using RaspiCommon.Data.Entities.Facial;
using RaspiCommon.Devices.Optics;
using RaspiCommon.Devices.Optics.Classifiers;
using RaspiCommon.Extensions;

namespace Radar
{
	public partial class RadarForm
	{
		BatonFinder BatonFinder { get; set; }
		FaceFinder FaceFinder { get; set; }

		private void OnNewFrameReceived(object sender, NewFrameEventArgs eventArgs)
		{
			Mat frame = new Image<Bgr, byte>(eventArgs.Frame.Clone() as Bitmap).Mat;
			DetectBatons(ref frame);
			DetectFaces(ref frame);
			Bitmap bitmap = new Bitmap(frame.Bitmap);
			picVideo.BackgroundImage = bitmap;
		}

		private void DetectBatons(ref Mat frame)
		{
			if(BatonFinder != null)
			{
				FoundObjectList objects = BatonFinder.ProcessFrame(frame);
				foreach(FoundObject rectangle in objects)
				{
					frame.DrawRectangle(rectangle.Rectangle, Color.Chartreuse);
				}
			}
		}

		private void DetectFaces(ref Mat frame)
		{
			if(FaceFinder != null)
			{
				FoundObjectList objects = FaceFinder.ProcessFrame(frame);
				foreach(FoundObject rectangle in objects)
				{
					frame.DrawRectangle(rectangle.Rectangle, Color.Blue);
				}
			}
		}

		private void LoadEigen()
		{
			_faceRecognizer = new FisherFaceRecognizer();
			_faceRecognizer.Read(Program.Config.EigenRecognizerFile);
		}

		private void OnTrainEigenClicked(object sender, EventArgs args)
		{
			try
			{
				List<FacialImage> faces;
				_fds.GetAllFacialImages(out faces);

				VectorOfMat images = new VectorOfMat();
				VectorOfInt nameIDs = new VectorOfInt();
				for(int i = 0;i < faces.Count;i++)
				{
					FacialImage facialImage = faces[i];
					images.Push(faces[i].Image);
					nameIDs.Push(new int[] { (int)faces[i].NameID });
				}
				_faceRecognizer = new EigenFaceRecognizer();
				_faceRecognizer.Train(images, nameIDs);
				_faceRecognizer.Write(Program.Config.EigenRecognizerFile);
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}
	}
}
