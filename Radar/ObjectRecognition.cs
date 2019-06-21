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
using RaspiCommon.Extensions;

namespace Radar
{
	public partial class RadarForm
	{
		BatonFinder BatonFinder { get; set; }

		private void OnNewFrameReceived(object sender, NewFrameEventArgs eventArgs)
		{
			Mat frame = new Image<Bgr, byte>(eventArgs.Frame.Clone() as Bitmap).Mat;
			DetectBatons(ref frame);
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
			if(_faceRecognizer != null)
			{
				List<Rectangle> rectangles = frame.FindObjects(FaceClassifier);
				FacePredictionList predictions = new FacePredictionList();
				foreach(Rectangle rectangle in rectangles)
				{
					Mat part = new Mat(frame, rectangle).Resize(Program.Config.FaceRecognizeSize).ToGrayscaleImage();

					//part.Save(DirectoryExtensions.GetNextNumberedFileName(@"c:\pub\tmp\images\raw", "annika-", ".bmp", 4));
					FaceRecognizer.PredictionResult predictionResult = _faceRecognizer.Predict(part);
					String name;
					if(_faceNames.TryGetName(predictionResult.Label, out name))
					{
						FacePrediction prediction = new FacePrediction(rectangle, part, name, predictionResult.Label, predictionResult.Distance);
						predictions.Add(prediction);
					}
				}

				foreach(FacePrediction prediction in predictions)
				{
					if(prediction.Match < 55000)
					{
						frame.DrawRectangle(prediction.Rectangle, Color.Red);
						frame.DrawTextBelowRectangle($"{prediction.Name} {(int)prediction.Match}", FontFace.HersheyPlain, Color.Green, prediction.Rectangle, 30);
					}
					Log.SysLogText(LogLevel.DEBUG, "{0}", prediction);
				}
			}
		}

		private void LoadEigen()
		{
			_faceRecognizer = new FisherFaceRecognizer();
			_faceRecognizer.Read(Program.Config.EigenRecognizerFile);
			_faceRecognizerReady = true;
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
