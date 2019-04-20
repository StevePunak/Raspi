using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using KanoopCommon.CommonObjects;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

namespace RaspiCommon.Devices.Optics
{
	public delegate void SnapshotTakenHandler(String imageLocation, ImageType imageType);

	public class Camera : ThreadBase
	{
		#region Events

		public event SnapshotTakenHandler SnapshotTaken;

		#endregion

		#region Constants

		public static string ImageDirectory = "/var/robo/images";

		const String SNAPSHOT_PROGRAM = "raspiyuv";
		const String SAVE_NAME_END = "snap.img";
		const String DELETE_WILDCARD = "*-snap*";

		const int COUNT_TO_KEEP = 10;

		#endregion

		#region Public Properties

		public String LastSavedImage { get; private set; }

		public Double FieldOfViewHorizontal { get { return 53.0; } }
		public Double FieldOfViewVertical { get { return 41.0; } }

		public Double AngleOffset { get; set; }

		public TimeSpan WaitTime { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public ImageType ImageType { get; set; }
		public bool FlipHorizontal { get; set; }
		public bool FlipVertical { get; set; }
		public int Brightness { get; set; }
		public String Effect { get; set; }
		public String Exposure { get; set; }

		public ImageType ConvertTo { get; set; }

		#endregion

		#region Private Properties

		String WaitTimeString { get { return WaitTime != TimeSpan.Zero ? String.Format("-t {0}", (int)WaitTime.TotalMilliseconds) : String.Empty; } }
		String WidthString { get { return Width != 0 ? String.Format("-w {0}", Width) : String.Empty; } }
		String HeightString { get { return Height != 0 ? String.Format("-h {0}", Height) : String.Empty; } }
		String EffectString { get { return String.IsNullOrEmpty(Effect) ? String.Empty : String.Format("-ifx {0}", Effect); } }
		String ExposureString { get { return String.IsNullOrEmpty(Exposure) ? String.Empty : String.Format("-ex {0}", Exposure); } }
		String ImageTypeString
		{
			get
			{
				String output = String.Empty;
				switch(ImageType)
				{
					case ImageType.RawRGB:
						output = String.Format("--rgb");
						break;
					case ImageType.RawBGR:
						output = String.Format("--bgr");
						break;
					default:
						break;
				}
				return output;
			}
		}
		String FlipHorizontalString { get { return FlipHorizontal ? "-hf" : String.Empty; } }
		String FlipVerticalString { get { return FlipVertical ? "-vf" : String.Empty; } }
		String BrightnessString { get { return String.Format("-br {0}", Brightness); } }

		#endregion

		#region Local Member Variables

		String _lastParms;
		int _snapshotCount;

		#endregion

		public Camera()
			: base(typeof(Camera).Name)
		{
			Log.SysLogText(LogLevel.INFO, "Creating camera object");

			if(Directory.Exists(ImageDirectory) == false)
			{
				Log.LogText(LogLevel.INFO, "Creating snapshot directory {0}", ImageDirectory);
				Directory.CreateDirectory(ImageDirectory);
			}

			if(TryGetLastSnapshot(out _snapshotCount))
			{
				_snapshotCount++;
			}
			else
			{
				_snapshotCount = 0;
			}

			WaitTime = TimeSpan.FromMilliseconds(1000);
			Width = 800;
			Height = 600;
			ImageType = ImageType.RawRGB;
			FlipHorizontal = true;
			FlipVertical = true;
			Brightness = 30;

			ConvertTo = ImageType.Unknown;

			SnapshotTaken += delegate {};
			_lastParms = String.Empty;

			Interval = TimeSpan.FromSeconds(2);
		}

		protected override bool OnRun()
		{
			String filename = String.Format("{0}/{1:0000}-{2}", ImageDirectory, _snapshotCount, SAVE_NAME_END);
			String parms = String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",
				WaitTimeString, WidthString, HeightString, ImageTypeString, FlipHorizontalString, FlipVerticalString, BrightnessString, EffectString, ExposureString);
			String args = String.Format("{0} -o {1}", parms, filename);
			if(parms != _lastParms)
			{
				Log.LogText(LogLevel.DEBUG, "File '{0}'  args: '{1}'", filename, args);
			}
			_lastParms = parms;
			ProcessStartInfo pinfo = new ProcessStartInfo(SNAPSHOT_PROGRAM, args)
			{

			};

			Process process = Process.Start(pinfo);
			if(process.WaitForExit(10000) && process.ExitCode == 0)
			{
				String outputFile;
				if(ConvertTo != ImageType.Unknown && ConvertImage(filename, new Size(Width, Height), ImageType, ConvertTo, _snapshotCount, out outputFile))
				{
					LastSavedImage = outputFile;
					SnapshotTaken(outputFile, ConvertTo);
				}
				else
				{
					LastSavedImage = filename;
					SnapshotTaken(filename, ImageType.RawRGB);
				}
			}
			else
			{
				Log.LogText(LogLevel.DEBUG, "Process exit {0}", process.ExitCode);
			}

			if(++_snapshotCount > 9999)
			{
				_snapshotCount = 0;
			}

			CleanUp();

			return true;
		}

		protected void CleanUp()
		{
			List<String> files = new List<String>(Directory.GetFiles(ImageDirectory, DELETE_WILDCARD));
			int num = 0;
			foreach(String file in files)
			{
				String part = Path.GetFileName(file).Substring(0, 4);
				if(int.TryParse(part, out num))
				{
					if(num < _snapshotCount - COUNT_TO_KEEP || num > _snapshotCount)
					{
						File.Delete(file);
					}
				}
			}
		}

		protected bool TryGetLastSnapshot(out int last)
		{
			bool foundAny = false;
			last = 0;
			List<String> files = new List<String>(Directory.GetFiles(ImageDirectory, "*" + SAVE_NAME_END));
			Log.SysLogText(LogLevel.DEBUG, "Files are:\n{0}", files.GetString());
			String name = null;
			do
			{
				String s = String.Format("{0:0000}", last);
				Log.SysLogText(LogLevel.DEBUG, "Looking for file starts with {0} in {1} files", s, files.Count);

				if((name  = files.Find(f => Path.GetFileName(f).StartsWith(s))) != null)
				{
					foundAny = true;
				}
			} while(name != null && ++last < 9999);
			Log.SysLogText(LogLevel.DEBUG, "Last snapshot is to {0}", last);
			return foundAny;
		}

		public static bool ConvertImage(
			String inputFilename,
			Size size,
			ImageType fromType, 
			ImageType toType, 
			int number, 
			out String outputFilename)
		{
			bool converted = false;
			outputFilename = inputFilename;
			if(fromType == ImageType.RawRGB || fromType == ImageType.RawBGR)
			{
				converted = ConvertRaw(inputFilename, size, fromType, toType, number, out outputFilename);
			}
			else
			{
				throw new CommonException("Invalid input type {0}", fromType);
			}
			return converted;
		}

		static bool ConvertRaw(String inputFilename,
								Size size,
								ImageType fromType,
								ImageType toType,
								int number,
								out String outputFilename)
		{
			String extension = String.Empty;
			switch(toType)
			{
				case ImageType.Bitmap:
					extension = "bmp";
					break;
				default:
					extension = "unknown";
					break;
			}

			outputFilename = Path.Combine(ImageDirectory, String.Format(@"snap-{0:0000}.{1}", number, extension));
			byte[] data = File.ReadAllBytes(inputFilename);
			Mat image = new Mat(size, DepthType.Cv8U, 3);
			unsafe
			{
				IntPtr ptr = image.DataPointer;
				Marshal.Copy(data, 0, ptr, (size.Height * size.Width * 3));
			}
			if(fromType == ImageType.RawRGB)
			{
				Mat[] colors = image.Split();
				CvInvoke.Merge(new VectorOfMat(colors[2], colors[1], colors[0]), image);
			}
			else
			{
			}
			image.Save(outputFilename);
			return true;
		}

	}
}
