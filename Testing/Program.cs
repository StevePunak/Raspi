using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using KanoopCommon.Extensions;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;
using KanoopCommon.Queueing;
using KanoopCommon.Threading;
using RaspiCommon.Extensions;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Server;

namespace RaspiCommon
{

	class Program
	{
		public static Log Log { get; private set; }

		static void Main(string[] args)
		{
			OpenLog();

			EMGUTest();

			RunLidar();
		}

		private static void EMGUTest()
		{
			String root = Environment.OSVersion.Platform == PlatformID.Unix ? "./" : @"\\raspi\pi\tmp";

			while(true)
			{
				TestImage();

				foreach(String file in Directory.GetFiles(root, "grid*png"))
				{
					//if(file.Contains("000") == false)
					//	continue;

					Log.SysLogText(LogLevel.DEBUG, "\n\n\n\n\n");
					int index = file.IndexOf("grid");
					if(index > 0 && Char.IsDigit(file[index + 4]))
					{
						String part = file.Substring(index + 4, 3);
						index = int.Parse(part);
					}

					String inFile = root + file;

					Double PixelsPerMeter = 50;

					Mat image = new Mat(file);
					PointD currentPoint = new PointD(image.Width / 2, image.Height / 2);

					LidarEnvironment env = new LidarEnvironment(10, PixelsPerMeter);
					env.Location = currentPoint;
					env.ProcessImage(image, 0, PixelsPerMeter);

					Mat output = env.GetEnvironmentImage(false);
					String outFile = String.Format(@"{0}\output_{1:000}-a.png", root, index);
					output.Save(outFile);
					output = env.GetEnvironmentImage(true);
					outFile = String.Format(@"{0}\output_{1:000}-b.png", root, index);
					output.Save(outFile);

				}
			}
		
			Log.LogText(LogLevel.DEBUG, "Done");
		}

		private static void TestImage()
		{
			Double angle = 356.5;
			Double DebugAngle = 0;
			if(angle.AngularDifference(DebugAngle) < 4)
			{

			}

			Mat mat = new Mat(@"\\raspi\pi\tmp\grid008.png");
			PointD center = new PointD(mat.Width / 2, mat.Height / 2);

			MCvScalar c1 = new Bgr(Color.Green).MCvScalar;
			MCvScalar c2 = new Bgr(Color.Blue).MCvScalar;
			MCvScalar c3 = new Bgr(Color.Orange).MCvScalar;
			MCvScalar c4 = new Bgr(Color.Purple).MCvScalar;

			Mat tank = new Mat("tank2.png");
			CvInvoke.Resize(tank, tank, new Size(25, 25));

			PointD tankcenter = tank.CenterPoint();
			Mat rotationMatrix = new Mat();
			CvInvoke.GetRotationMatrix2D(tankcenter.ToPoint(), 270, 1, rotationMatrix);
			Mat rotated = new Mat();
			CvInvoke.WarpAffine(tank, rotated, rotationMatrix, tank.Size);

			PointD drawLocation = new PointD(center.X - tank.Width / 2, center.Y - tank.Height / 2);

			Rectangle roi = new Rectangle((int)drawLocation.X, (int)drawLocation.Y, tank.Width, tank.Height);
			rotated.CopyTo(new Mat(mat, roi));




			//			Mat tmp = new Mat(mat, roi);
			//			tank.CopyTo(tmp);

			//Rectangle r = CvInvoke.cvGetImageROI(mat);
			//CvInvoke.cvSetImageROI(mat, roi);
			//tank.CopyTo(mat);

			//			Line l1 = new Line(new PointD(100, 100), new PointD(200, 200));
			//			CvInvoke.Line(mat, l1.P1.ToPoint(), l1.P2.ToPoint(), c1);

#if z
			Line l2 = new Line(new PointD(205, 205), new PointD(400, 400));
			CvInvoke.Line(mat, l2.P1.ToPoint(), l2.P2.ToPoint(), c2);

			RectangleD rect1, rect2;
			if(l1.LiesAlongThePathOf(l2, 6, out rect1, out rect2, 8))
			{
			}

			VectorOfPoint v1 = new VectorOfPoint(rect1.ToPointDList().ToPointArray());
			CvInvoke.Polylines(mat, v1, true, c3);
			if(rect2 != null)
			{
				VectorOfPoint v2 = new VectorOfPoint(rect2.ToPointDList().ToPointArray());
				CvInvoke.Polylines(mat, v2, true, c4);
			}

#endif
			mat.Save(@"\\raspi\pi\tmp\junk.png");
		}

		static void RunMotor()
		{
			MotorDriver motor = new MotorDriver(GpioPin.Pin12,  GpioPin.Pin26, GpioPin.Pin16, GpioPin.Pin20);
			motor.Speed = MotorSpeed.Fast;
			motor.Start();

			motor.Rotate(Direction.Forward, 400);

			GpioSharp.Sleep(10000);

			motor.Stop();

			Console.WriteLine("done");
		}

		static void RunLidar()
		{

//			RPLidar lidar = new RPLidar("/dev/ttyUSB0");
			RPLidar lidar = new RPLidar("COM5", .25);
			lidar.Start();

#if zero
			{
			{
				LidarCommand command = new ResetCommand();
				lidar.SendCommand(command);

				LidarResponse response;
				if(lidar.TryGetResponse(TimeSpan.FromSeconds(1), out response))
				{
					Log.SysLogText(LogLevel.DEBUG, "Reset Response successful");
				}
				else
				{
					Log.SysLogText(LogLevel.DEBUG, "Reset Response Unsuccessful");
				}
			}
			Thread.Sleep(1000);

			{
				LidarCommand command = new GetSampleRateCommand();
				lidar.SendCommand(command);

				LidarResponse response;
				if(lidar.TryGetResponse(TimeSpan.FromSeconds(1), out response))
				{
					Log.SysLogText(LogLevel.DEBUG, "Response successful");
				}
				else
				{
					Log.SysLogText(LogLevel.DEBUG, "Response Unsuccessful");
				}
			}
			Thread.Sleep(1000);

			{
				LidarCommand command = new GetDeviceInfoCommand();
				lidar.SendCommand(command);

				LidarResponse response;
				if(lidar.TryGetResponse(TimeSpan.FromSeconds(1), out response))
				{
					Log.SysLogText(LogLevel.DEBUG, "Next Response successful");
				}
			}
			Thread.Sleep(1000);
				lidar.SendCommand(new StartExpressScanCommand());

				Log.SysLogText(LogLevel.DEBUG, "I'm anout to die!!");
				Thread.Sleep(100);
//				Environment.FailFast("I'm dying");
//				return;
				LidarResponse response;

				if(lidar.TryGetResponse(TimeSpan.FromSeconds(10), out response))
				{
					Log.SysLogText(LogLevel.DEBUG, "Response successful");
				}
			}
#else
			{
				lidar.SendCommand(new StartScanCommand());

				Thread.Sleep(100);
				LidarResponse response;

				if(lidar.TryGetResponse(TimeSpan.FromSeconds(10), out response))
				{
					Log.SysLogText(LogLevel.DEBUG, "Response successful");
				}
			}
#endif

			Thread.Sleep(1000);
			Console.WriteLine("Sending stop");
			{
				LidarCommand command = new StopCommand();
				lidar.SendCommand(command);

				LidarResponse response;
				if(lidar.TryGetResponse(TimeSpan.FromSeconds(1), out response))
				{
					Log.SysLogText(LogLevel.DEBUG, "Stop Response successful");
				}
			}

			Thread.Sleep(1000);
			Console.WriteLine("Sending stop motor");
			{
				LidarCommand command = new SetMotorPwm(0);
				lidar.SendCommand(command);

				LidarResponse response;
				if(lidar.TryGetResponse(TimeSpan.FromSeconds(1), out response))
				{
					Log.SysLogText(LogLevel.DEBUG, "Stop Response successful");
				}
			}

			Thread.Sleep(5000);

		}

		private static void OpenLog()
		{
			Log = new Log();
			Log.Open(LogLevel.ALWAYS, "lidar.log", OpenFlags.CONTENT_TIMESTAMP | OpenFlags.OUTPUT_TO_FILE | OpenFlags.OUTPUT_TO_DEBUG | OpenFlags.OUTPUT_TO_CONSOLE);
			Log.SystemLog = Log;
		}
	}
}
