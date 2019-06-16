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
	public class RaspiCameraV2 : Camera
	{
		#region Constants

		const String SNAPSHOT_PROGRAM = "raspiyuv";

		#endregion

		#region Private Properties

		protected String SnapshotDelayString { get { return Parameters.SnapshotDelay != TimeSpan.Zero ? String.Format("-t {0}", (int)Parameters.SnapshotDelay.TotalMilliseconds) : String.Empty; } }
		protected String WidthString { get { return Parameters.Width != 0 ? String.Format("-w {0}", Parameters.Width) : String.Empty; } }
		protected String HeightString { get { return Parameters.Height != 0 ? String.Format("-h {0}", Parameters.Height) : String.Empty; } }
		protected String ContrastString { get { return Parameters.Contrast != 0 ? String.Format("-co {0}", Parameters.Contrast) : String.Empty; } }
		protected String SaturationString { get { return Parameters.Saturation != 0 ? String.Format("-sa {0}", Parameters.Saturation) : String.Empty; } }
		protected String ImageEffectString { get { return String.IsNullOrEmpty(Parameters.ImageEffect) ? String.Empty : String.Format("-ifx {0}", Parameters.ImageEffect); } }
		protected String ColorEffectString { get { return String.IsNullOrEmpty(Parameters.ColorEffect) ? String.Empty : String.Format("-cfx {0}", Parameters.ColorEffect); } }
		protected String ExposureString { get { return String.IsNullOrEmpty(Parameters.Exposure) ? String.Empty : String.Format("-ex {0}", Parameters.Exposure); } }
		protected String AutoWhiteBalanceString { get { return String.IsNullOrEmpty(Parameters.AutoWhiteBalance) ? String.Empty : String.Format("-awb {0}", Parameters.AutoWhiteBalance); } }
		protected String MeteringModeString { get { return String.IsNullOrEmpty(Parameters.MeteringMode) ? String.Empty : String.Format("-mm {0}", Parameters.MeteringMode); } }
		protected String ImageTypeString
		{
			get
			{
				String output = String.Empty;
				switch(Parameters.ImageType)
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
		protected String FlipHorizontalString { get { return Parameters.FlipHorizontal ? "-hf" : String.Empty; } }
		protected String FlipVerticalString { get { return Parameters.FlipVertical ? "-vf" : String.Empty; } }
		protected String BrightnessString { get { return Parameters.Brightness > 0 ? String.Format("-br {0}", Parameters.Brightness) : String.Empty; } }

		#endregion

		#region Local Member Variables

		String _lastParms;

		#endregion

		#region Constructor

		public RaspiCameraV2()
			: this(typeof(RaspiCameraV2).Name) {}

		public RaspiCameraV2(String name)
			: base(name)
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

		public virtual bool SnapPhoto(out Mat image)
		{
			image = null;
			bool result = false;
			String filename = String.Format("{0}/{1:0000}-{2}", ImageDirectory, ImageNumber, SaveNameEnd);
			String parms = String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13}",
				SnapshotDelayString, WidthString, HeightString,
				ImageTypeString,
				FlipHorizontalString,
				FlipVerticalString,
				BrightnessString,
				ImageEffectString,
				ExposureString,
				ContrastString,
				SaturationString,
				ColorEffectString,
				AutoWhiteBalanceString,
				MeteringModeString);

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
				if(ConvertTo != ImageType.Unknown && ConvertImage(filename, new Size(Parameters.Width, Parameters.Height), Parameters.ImageType, ConvertTo, ImageNumber, out outputFile, out image))
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

		#region Utility

		protected String MakeParmsString()
		{
			String parms = String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",
				FlipHorizontalString,
				FlipVerticalString,
				BrightnessString,
				ImageEffectString,
				ExposureString,
				ContrastString,
				SaturationString,
				ColorEffectString,
				AutoWhiteBalanceString,
				MeteringModeString);
//			Log.LogText(LogLevel.DEBUG, "{0} Make Parms {1}", Name, parms);
			return parms;
		}

		#endregion

	}
}
