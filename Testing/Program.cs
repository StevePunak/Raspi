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
using RaspiCommon;
using RaspiCommon.Devices.Chassis;
using RaspiCommon.Devices.MotorControl;
using RaspiCommon.Devices.Spatial;
using RaspiCommon.Extensions;
using RaspiCommon.Lidar;
using RaspiCommon.Lidar.Environs;
using RaspiCommon.Network;
using RaspiCommon.Spatial;
using RaspiCommon.Spatial.Imaging;

namespace Testing
{

	class Program
	{
		public static Log Log { get; private set; }

		static PointCloud2D PointCloud { get; set; }

		static void Main(string[] args)
		{
			OpenLog();

			PointCloudTest();
//			EMGUTest();

			RunLidar();
		}

		private static void PointCloudTest()
		{
			Double PixelsPerMeter = 50;
			Double Scale = 1 / PixelsPerMeter;

			Chassis chassis = new XiaorTankTracks();
			chassis.Points.Add(ChassisParts.Lidar, new PointD(chassis.Points[ChassisParts.FrontRight].X - .070, chassis.Points[ChassisParts.FrontRight].Y + .220));
			chassis.Points.Add(ChassisParts.FrontRangeFinder, new PointD(chassis.Width / 2, 0));
			chassis.Points.Add(ChassisParts.RearRangeFinder, new PointD(chassis.Width / 2, chassis.Length));

			BearingAndRange toFrontLeft = chassis.GetBearingAndRange(ChassisParts.Lidar, ChassisParts.FrontLeft, 0);
			BearingAndRange toFrontRight = chassis.GetBearingAndRange(ChassisParts.Lidar, ChassisParts.FrontRight, 0);
			Mat input = new Mat(@"c:\pub\tmp\image1.png");

			PointCloud = input.ToPointCloud(.25, 1 / PixelsPerMeter);
			PointCloud2D frontLeft, frontRight;
			FuzzyPath path = LidarEnvironment.MakeFuzzyPath(PointCloud, 30, chassis.FrontLeftLidarVector, chassis.FrontRightLidarVector, 30, out frontLeft, out frontRight);
			double shortest = path.ShortestRange;
			path.Serizalize();

			Mat bitmap = new Mat(input.Size, DepthType.Cv8U, 3);
			FuzzyRangeAtBearing(bitmap, PixelsPerMeter, PointCloud,
				chassis.GetBearingAndRange(ChassisParts.Lidar, ChassisParts.FrontLeft),
				chassis.GetBearingAndRange(ChassisParts.Lidar, ChassisParts.FrontRight),
				90, 45, out frontLeft, out frontRight);

			Mat output = PointCloud.ToBitmap(new Size(500, 500), Color.Blue);
			output.Save(@"c:\pub\tmp\output1.png");

			PointCloud2D cloud2 = PointCloud.Move(new BearingAndRange(45, 10));
			cloud2.PlaceOnBitmap(output, output.Center(), Color.Green);
			output.Save(@"c:\pub\tmp\output2.png");

		}

		public static Double FuzzyRangeAtBearing(
			Mat bitmap,
			Double pixelsPerMeter,
			PointCloud2D vectorsFromLidar, 
			BearingAndRange frontLeftWheelOffset, BearingAndRange frontRightWheelOffset, 
			Double bearingStraightAhead, 
			Double angularWidth, 
			out PointCloud2D frontLeftCloud, out PointCloud2D frontRightCloud)
		{
			PointD center = bitmap.Center();
			bitmap.DrawCross(center, 5, Color.White);

			if(angularWidth == 0)
			{
				angularWidth = 25;
			}

			frontLeftWheelOffset = frontLeftWheelOffset.Rotate(bearingStraightAhead);
			frontRightWheelOffset = frontRightWheelOffset.Rotate(bearingStraightAhead);

			bitmap.DrawCircleCross(center.GetPointAt(frontLeftWheelOffset.Scale(pixelsPerMeter)), 8, Color.Green);
			bitmap.DrawCircleCross(center.GetPointAt(frontRightWheelOffset.Scale(pixelsPerMeter)), 8, Color.Yellow);

			Log.SysLogText(LogLevel.DEBUG, "Getting vectors from lidar");

			frontLeftCloud = vectorsFromLidar.Move(frontLeftWheelOffset);
			frontRightCloud = vectorsFromLidar.Move(frontRightWheelOffset);

			PointD frontLeftLocation = center.GetPointAt(frontLeftWheelOffset);
			PointD frontRightLocation = center.GetPointAt(frontRightWheelOffset);

			PointCloud2DSlice frontLeftSlice = new PointCloud2DSlice(bearingStraightAhead, frontLeftCloud, angularWidth);
			PointCloud2DSlice frontRightSlice = new PointCloud2DSlice(bearingStraightAhead, frontRightCloud, angularWidth);

			bitmap.DrawVectorLines(frontLeftSlice, center.GetPointAt(frontLeftWheelOffset.Scale(pixelsPerMeter)), pixelsPerMeter, Color.LightGreen);
			bitmap.DrawVectorLines(frontRightSlice, center.GetPointAt(frontRightWheelOffset.Scale(pixelsPerMeter)), pixelsPerMeter, Color.LightYellow);

			bitmap.Save(@"c:\pub\tmp\temp.png");

			return Math.Min(frontLeftSlice.MinimumRange, frontRightSlice.MinimumRange);
		}

		private static void EMGUTest()
		{
			String root = Environment.OSVersion.Platform == PlatformID.Unix ? "./" : @"c:\pub\tmp";

			while(true)
			{
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

					Mat output = env.CreateImage(SpatialObjects.Everything);
					String outFile = String.Format(@"{0}\output_{1:000}-a.png", root, index);
					output.Save(outFile);
					output = env.CreateImage(SpatialObjects.Everything);
					outFile = String.Format(@"{0}\output_{1:000}-b.png", root, index);
					output.Save(outFile);

				}
			}
		
			Log.LogText(LogLevel.DEBUG, "Done");
		}

		private static void TestImage()
		{
		}

		static void RunMotor()
		{
			PWMMotorDriver motor = new PWMMotorDriver(GpioPin.Pin12,  GpioPin.Pin26, GpioPin.Pin16, GpioPin.Pin20);
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
