using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanoopCommon.Extensions
{
	public static class ListStringExtensions
	{
		public static void TerminateAll(this List<String> list)
		{
			for(int x = 0;x < list.Count;x++)
			{
				list[x] = list[x].Terminate();
			}
		}

		public static String GetString(this List<String> list)
		{
			StringBuilder sb = new StringBuilder();
			foreach(String s in  list)
			{
				sb.AppendFormat("  {0}\n", s);
			}
			return sb.ToString();
		}
	}
}
