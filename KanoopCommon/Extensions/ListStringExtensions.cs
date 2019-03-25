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
	}
}
