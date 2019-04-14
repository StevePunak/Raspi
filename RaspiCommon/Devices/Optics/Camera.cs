using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using KanoopCommon.Threading;

namespace RaspiCommon.Devices.Optics
{
	public class Camera : ThreadBase
	{
		const string SnapshotDirectory = "/var/robo";
		const String SNAPSHOT_PROGRAM = "raspistill";

		const int COUNT_TO_KEEP = 10;

		public Mat LastSnapshot { get; private set; }

		int _snapshotCount;

		public Camera()
			: base(typeof(Camera).Name)
		{
			Log.SysLogText(LogLevel.INFO, "Creating camera object");

			if(TryGetLastSnapshot(out _snapshotCount))
			{
				_snapshotCount++;
			}
			else
			{
				_snapshotCount = 0;
			}

			Interval = TimeSpan.FromSeconds(2);
		}

		protected override bool OnRun()
		{
			String args = String.Format("-vf -w 640 -h 480 -t 15 -o {0}/{1:0000}-snap.jpg", SnapshotDirectory, _snapshotCount);
			ProcessStartInfo pinfo = new ProcessStartInfo(SNAPSHOT_PROGRAM, args)
			{

			};

			Process process = Process.Start(pinfo);
			process.WaitForExit(10000);

			if(++_snapshotCount > 9999)
			{
				_snapshotCount = 0;
			}

			CleanUp();

			return true;
		}

		protected void CleanUp()
		{
			List<String> files = new List<String>(Directory.GetFiles(SnapshotDirectory, "*-snap.jpg"));
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
			List<String> files = new List<String>(Directory.GetFiles(SnapshotDirectory, "*snap.jpg"));
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
	}
}
