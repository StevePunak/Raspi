using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon
{
	public class RaspiPaths
	{
		public static String FileRoot { get {
				return Environment.OSVersion.Platform == PlatformID.Unix
					? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
					: @"c:\pub"; } }

		public static String ClassifyRoot { get { return Path.Combine(FileRoot, "classify"); } }
		public static String Cascades { get { return Path.Combine(ClassifyRoot, "cascades"); } }
	}
}
