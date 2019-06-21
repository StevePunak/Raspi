using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Logging;
using RaspiCommon.Extensions;

namespace RaspiCommon.Devices.Optics
{
	public class MJpegStreamerCamera : RaspiCameraV2
	{
		#region Constants

		const String CONFIG_FILE = "/etc/mjpeg-streamer/raspi-options";
		const String RELOAD_COMMAND = "/home/pi/bin/restartmjs";

		#endregion

		#region Local Member Variables

		String _lastParms;

		#endregion

		#region Properties

		public String LastSnapShot { get { return String.Format("{0}/{1}", ImageDirectory, "lastsnap.jpg"); } }

		#endregion

		#region Constructor

		public MJpegStreamerCamera()
			: base(typeof(MJpegStreamerCamera).Name)
		{
			SaveNameEnd = "snap.jpg";
		}

		#endregion

		protected override bool OnStart()
		{
			try
			{
				Log.LogText(LogLevel.DEBUG, "{0} OnStart", this);
				//ModifyConfiguration(MakeParmsString(), Parameters.Width, Parameters.Height);
			}
			catch(Exception e)
			{
				Log.LogText(LogLevel.ERROR, "MotionCamera startup exception: {0}", e.Message);
			}
			_lastParms = MakeParmsString();
			return base.OnStart();
		}

		public override bool SnapPhoto(out Mat image)
		{
			Log.LogText(LogLevel.DEBUG, "{0} SnapPhoto", this);
			image = null;
			bool result = false;
			try
			{
				String filename = String.Format("{0}/{1:0000}-{2}", ImageDirectory, ImageNumber, SaveNameEnd);
				Log.LogText(LogLevel.DEBUG, "{0} SnapPhoto 1", this);
				String parms = MakeParmsString();
				Log.LogText(LogLevel.DEBUG, "{0} SnapPhoto 2 {1}", this, parms);

				String args = String.Format("{0}", parms);
				if(parms != _lastParms)
				{
					Log.LogText(LogLevel.DEBUG, "{0} Waiting while changing image parameters for '{0}'  to: '{1}'", Name, filename, args);
					ModifyConfiguration(args, Parameters.Width, Parameters.Height);
					Sleep(5000);
					Log.LogText(LogLevel.DEBUG, "Process reconfig complete");
				}
				_lastParms = parms;

				Log.LogText(LogLevel.DEBUG, "{0} SnapPhoto 3", this);
				EatX(filename);

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

		void EatX(String filename)
		{
			Log.LogText(LogLevel.DEBUG, "{0} EATX", this);
		}

		void GrabThatSnapshot(String fileName)
		{
			try
			{
				Console.WriteLine("will grab 1");
				Log.LogText(LogLevel.DEBUG, "Grab 1");
				WebRequest request = WebRequest.Create(SnapshotUrl);

				Console.WriteLine("will grab 2");
				Log.LogText(LogLevel.DEBUG, "Grab 2");
				using(WebResponse response = request.GetResponse())
				using(Stream dataStream = response.GetResponseStream())
				using(BinaryReader reader = new BinaryReader(dataStream))
				{
					Console.WriteLine("will grab 3");
					Log.LogText(LogLevel.DEBUG, "Grab 3 reader {0} dataStream {1} reader {2}",
						reader == null ? "NULL" : "not null",
						dataStream == null ? "NULL" : "not null",
						response == null ? "NULL" : "not null");
					byte[] data = reader.ReadBytes(2000000);
					using(MemoryStream ms = new MemoryStream(data))
					{
						Mat image = new Mat();
						image.LoadFromByteArray(data);

						image.Save(fileName);
						LastSavedImage = image;
					}
				}
				Console.WriteLine("will grab 99");
				Log.LogText(LogLevel.DEBUG, "Grab 99");
			}
			catch(Exception e)
			{
				Log.LogText(LogLevel.WARNING, "{0} Exception: {1}", this, e.Message);
			}

		}

		void CopyLastSnapshot()
		{
			String filename = String.Format("{0}/{1:0000}-{2}", ImageDirectory, ImageNumber, SaveNameEnd);
			File.Copy(LastSnapShot, filename);
			LastSavedImageFileName = filename;
			Mat image = new Mat(filename);
			LastSavedImage = image;
			LastSavedImageTime = DateTime.UtcNow;
			InvokeSnapshotTaken(filename, ImageType.Jpeg);
		}

		void ModifyConfiguration(String args, int width, int height)
		{
			File.WriteAllText(CONFIG_FILE, args);
			Log.LogText(LogLevel.INFO, "Parameters modification successful: {0}X{1} ({2})", width, height, args);
			ProcessStartInfo pinfo = new ProcessStartInfo(RELOAD_COMMAND)
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
	}
}
