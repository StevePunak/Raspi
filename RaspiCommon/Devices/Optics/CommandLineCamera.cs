using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Logging;

namespace RaspiCommon.Devices.Optics
{
	public class CommandLineCamera : Camera
	{
		#region Constants

		const String SNAPSHOT_PROGRAM = "raspiyuv";

		#endregion

		#region Private Properties

		String SnapshotDelayString { get { return SnapshotDelay != TimeSpan.Zero ? String.Format("-t {0}", (int)SnapshotDelay.TotalMilliseconds) : String.Empty; } }
		String WidthString { get { return Width != 0 ? String.Format("-w {0}", Width) : String.Empty; } }
		String HeightString { get { return Height != 0 ? String.Format("-h {0}", Height) : String.Empty; } }
		String ContrastString { get { return Contrast != 0 ? String.Format("-co {0}", Contrast) : String.Empty; } }
		String SaturationString { get { return Saturation != 0 ? String.Format("-sa {0}", Saturation) : String.Empty; } }
		String ImageEffectString { get { return String.IsNullOrEmpty(ImageEffect) ? String.Empty : String.Format("-ifx {0}", ImageEffect); } }
		String ColorEffectString { get { return String.IsNullOrEmpty(ColorEffect) ? String.Empty : String.Format("-cfx {0}", ColorEffect); } }
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
		String BrightnessString { get { return Brightness > 0 ? String.Format("-br {0}", Brightness) : String.Empty; } }

		#endregion

		#region Local Member Variables

		String _lastParms;

		#endregion

		#region Constructor

		public CommandLineCamera()
			: base(typeof(CommandLineCamera).Name)
		{
			_lastParms = String.Empty;
		}

		#endregion

		#region Main Run Loop

		protected override bool OnRun()
		{
			if(AutoSnap)
			{
				Mat image;
				SnapPhoto(out image);
			}
			return true;
		}

		public override bool TryTakeSnapshot(out Mat image)
		{
			return SnapPhoto(out image);
		}

		public bool SnapPhoto(out Mat image)
		{
			image = null;
			bool result = false;
			String filename = String.Format("{0}/{1:0000}-{2}", ImageDirectory, ImageNumber, SAVE_NAME_END);
			String parms = String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
				SnapshotDelayString, WidthString, HeightString,
				ImageTypeString,
				FlipHorizontalString,
				FlipVerticalString,
				BrightnessString,
				ImageEffectString,
				ExposureString,
				ContrastString,
				SaturationString,
				ColorEffectString);

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
				String outputFile = String.Empty;
				ImageType type = ImageType.RawRGB;
				if(ConvertTo != ImageType.Unknown && ConvertImage(filename, new Size(Width, Height), ImageType, ConvertTo, ImageNumber, out outputFile, out image))
				{
					type = ConvertTo;
				}
				else
				{
					outputFile = filename;
				}
				LastSavedImageFileName = outputFile;
				LastSavedImage = image;
				LastSavedImageTime = DateTime.UtcNow;
				InvokeSnapshotTaken(filename, type);
				result = true;
			}
			else
			{
				Log.LogText(LogLevel.DEBUG, "Process exit {0}", process.ExitCode);
			}

			if(++ImageNumber > 9999)
			{
				ImageNumber = 0;
			}

			CleanUp();

			return result;
		}

		#endregion
	}
}
