using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Logging;

namespace RaspiCommon.Devices.Optics
{
	public class MotionDaemonCamera : RaspiCameraV2
	{
		#region Constants

		const String MOTION_CONF = "/etc/motion/motion.conf";
		const String RELOAD_CONF_SCRIPT = "/home/pi/bin/mr";

		#endregion

		#region Local Member Variables

		String _lastParms;

		#endregion

		#region Properties

		public String LastSnapShot { get { return String.Format("{0}/{1}", ImageDirectory, "lastsnap.jpg"); } }

		#endregion

		#region Constructor

		public MotionDaemonCamera()
			: base(typeof(MotionDaemonCamera).Name)
		{
			SaveNameEnd = "snap.jpg";
		}

		#endregion

		protected override bool OnStart()
		{
			try
			{
				ModifyMotionConf(MakeParmsString(), Parameters.Width, Parameters.Height);
			}
			catch(Exception e)
			{
				Log.LogText(LogLevel.ERROR, "MotionCamera startup exception: {0}", e.Message);
			}
			return base.OnStart();
		}

		public override bool SnapPhoto(out Mat image)
		{
			image = null;
			bool result = false;
			try
			{
				String filename = String.Format("{0}/{1:0000}-{2}", ImageDirectory, ImageNumber, SaveNameEnd);
				String parms = MakeParmsString();

				String args = String.Format("{0}", parms);
				if(parms != _lastParms)
				{
					Log.LogText(LogLevel.DEBUG, "{0} Waiting while changing image parameters for '{0}'  to: '{1}'", Name, filename, args);
					ModifyMotionConf(args, Parameters.Width, Parameters.Height);
					Sleep(5000);
					Log.LogText(LogLevel.DEBUG, "Process reconfig complete");

				}
				_lastParms = parms;

				CopyLastSnapshot();

				if(++ImageNumber > 9999)
				{
					ImageNumber = 0;
				}

				CleanUp();

				image = LastSavedImage;

				result = true;
			}
			catch(Exception e)
			{
				Log.LogText(LogLevel.ERROR, "{0} EXCEPTION: {1}", this, e.Message);
			}

			return result;
		}

		void CopyLastSnapshot()
		{
			String filename = String.Format("{0}/{1:0000}-{2}", ImageDirectory, ImageNumber, SaveNameEnd);
			File.Copy(LastSnapShot, filename);
			Log.LogText(LogLevel.DEBUG, "Saved ----------------- {0}", filename);
			LastSavedImageFileName = filename;
			Mat image = new Mat(filename);
			LastSavedImage = image;
			LastSavedImageTime = DateTime.UtcNow;
			InvokeSnapshotTaken(filename, ImageType.Jpeg);
		}

		void ModifyMotionConf(String args, int width, int height)
		{
			const String PARMS_LINE = "mmalcam_control_params";
			const String WIDTH_LINE = "width";
			const String HEIGHT_LINE = "height";
			bool argsFound = false;
			bool widthFound = false;
			bool heightFound = false;

			List<String> lines = new List<String>(File.ReadAllLines(MOTION_CONF));
			for(int x = 0;x < lines.Count;x++)
			{
				String line = lines[x];
				if(line.StartsWith(PARMS_LINE))
				{
					line = String.Format("{0} {1}", PARMS_LINE, args);
					lines[x] = line;
					argsFound = true;
				}
				if(line.StartsWith(WIDTH_LINE))
				{
					line = String.Format("{0} {1}", WIDTH_LINE, width);
					lines[x] = line;
					widthFound = true;
				}
				if(line.StartsWith(HEIGHT_LINE))
				{
					line = String.Format("{0} {1}", HEIGHT_LINE, height);
					lines[x] = line;
					heightFound = true;
				}
			}

			File.WriteAllLines(MOTION_CONF, lines);
			if(argsFound && widthFound && heightFound)
			{
				Log.LogText(LogLevel.INFO, "Parameters modification successful: {0}X{1} ({2})", width, height, args);
				ProcessStartInfo pinfo = new ProcessStartInfo(RELOAD_CONF_SCRIPT)
				{
				};
				Process process = Process.Start(pinfo);
				if(process.WaitForExit(10000) && process.ExitCode == 0)
				{
					Log.LogText(LogLevel.DEBUG, "Process reconfigured");
				}
				else
				{
					Log.LogText(LogLevel.DEBUG, "Process exit {0}", process.ExitCode);
				}

			}
			else
			{
				Log.LogText(LogLevel.INFO, "Parameters modification NOT successful: Width: {0}  Height: {1}  Args: {2}", widthFound, heightFound, argsFound);
			}
		}
	}
}
