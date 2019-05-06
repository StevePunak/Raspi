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

	public abstract class Camera : ThreadBase
	{
		#region Events

		public event SnapshotTakenHandler SnapshotTaken;

		#endregion

		#region Constants

		public static string ImageDirectory = "/var/robo/images";

		protected const String DELETE_WILDCARD = "*-snap*";
		protected const String SAVE_NAME_END = "snap.img";

		const int COUNT_TO_KEEP = 10;

		#endregion

		#region Public Properties

		public String LastSavedImageFileName { get; protected set; }
		public Mat LastSavedImage { get; protected set; }
		public DateTime LastSavedImageTime { get; protected set; }

		public Double FieldOfViewHorizontal { get { return 53.0; } }
		public Double FieldOfViewVertical { get { return 41.0; } }

		public Double AngleOffset { get; set; }

		public TimeSpan SnapshotDelay { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public ImageType ImageType { get; set; }
		public bool FlipHorizontal { get; set; }
		public bool FlipVertical { get; set; }
		public int Brightness { get; set; }
		public String ImageEffect { get; set; }
		public String ColorEffect { get; set; }
		public String Exposure { get; set; }
		public int Contrast { get; set; }
		public int Saturation { get; set; }

		public ImageType ConvertTo { get; set; }

		public bool AutoSnap { get; set; }

		public int ImageNumber { get; protected set; }

		#endregion

		#region Constructor

		protected Camera(String name)
			: base(name)
		{
			Log.SysLogText(LogLevel.INFO, "Creating camera object");

			if(Directory.Exists(ImageDirectory) == false)
			{
				Log.LogText(LogLevel.INFO, "Creating snapshot directory {0}", ImageDirectory);
				Directory.CreateDirectory(ImageDirectory);
			}

			int imageNumber;
			if(TryGetLastSnapshot(out imageNumber))
			{
				ImageNumber = imageNumber + 1;
			}
			else
			{
				ImageNumber = 0;
			}

			SnapshotDelay = TimeSpan.FromMilliseconds(1000);
			Width = 800;
			Height = 600;
			ImageType = ImageType.RawRGB;
			FlipHorizontal = true;
			FlipVertical = true;
			Brightness = 30;

			ConvertTo = ImageType.Unknown;

			SnapshotTaken += delegate {};

			Interval = TimeSpan.FromSeconds(2);
		}

		#endregion

		#region Public Properties

		public abstract bool TryTakeSnapshot(out Mat image);

		#endregion

		#region File Handling

		protected void CleanUp()
		{
			List<String> files = new List<String>(Directory.GetFiles(ImageDirectory, DELETE_WILDCARD));
			int num = 0;
			foreach(String file in files)
			{
				String part = Path.GetFileName(file).Substring(0, 4);
				if(int.TryParse(part, out num))
				{
					if(num < ImageNumber - COUNT_TO_KEEP || num > ImageNumber)
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

				if((name = files.Find(f => Path.GetFileName(f).StartsWith(s))) != null)
				{
					foundAny = true;
				}
			} while(name != null && ++last < 9999);
			Log.SysLogText(LogLevel.DEBUG, "Last snapshot is to {0}", last);
			return foundAny;
		}

		#endregion

		#region Event Invocation

		protected void InvokeSnapshotTaken(String filename, ImageType imageType)
		{
			SnapshotTaken(filename, imageType);
		}

		#endregion

		#region Image Conversion

		public static bool ConvertImage(
			String inputFilename,
			Size size,
			ImageType fromType, 
			ImageType toType, 
			int number, 
			out String outputFilename, out Mat image)
		{
			image = null;
			bool converted = false;
			outputFilename = inputFilename;
			if(fromType == ImageType.RawRGB || fromType == ImageType.RawBGR)
			{
				converted = ConvertRaw(inputFilename, size, fromType, toType, number, out outputFilename, out image);
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
								out String outputFilename, out Mat image)
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
			image = new Mat(size, DepthType.Cv8U, 3);
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

		#endregion
	}
}
