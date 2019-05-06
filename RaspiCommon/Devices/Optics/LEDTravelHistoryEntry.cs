using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Extensions;
using KanoopCommon.Logging;
using KanoopCommon.Serialization;

namespace RaspiCommon.Devices.Optics
{
	[IsSerializable]
	public class LEDTravelHistoryEntry
	{
		public Double Bearing { get; set; }
		public Color GoingTo { get; set; }
		public List<Color> ColorHistory { get; set; }

		public LEDTravelHistoryEntry()
			: this(0, Color.Empty, new List<Color>()) { }

		public LEDTravelHistoryEntry(Double bearing, Color goingTo, List<Color> history)
		{
			Bearing = bearing;
			GoingTo = goingTo;
			ColorHistory = history;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0} going to {1} ", Bearing.ToAngleString(), GoingTo.Name);
			sb.AppendFormat("(");
			foreach(Color color in ColorHistory)
			{
				sb.AppendFormat("{0},", color.Name);
			}
			String ret = sb.ToString().Trim(new char[] { ' ', ',' }) + ")";
			return ret;
		}
	}

	public class LEDTravelHistoryEntryList : List<LEDTravelHistoryEntry>
	{
		public LEDTravelHistoryEntry GetEntryForColor(Color goingTo, List<Color> history)
		{
			LEDTravelHistoryEntry retValue = null;
			foreach(LEDTravelHistoryEntry entry in this)
			{
				if(entry.GoingTo == goingTo && entry.ColorHistory.Count == history.Count)
				{
					int x;
					for(x = 0;x < history.Count && history[x] == entry.ColorHistory[x];x++) ;
					if(x == history.Count)
					{
						retValue = entry;
						break;
					}
				}
			}
			return retValue;
		}

		public void DumpToLog()
		{
			foreach(LEDTravelHistoryEntry entry in this)
			{
				Log.SysLogText(LogLevel.DEBUG, "  {0}", entry);
			}
		}
	}
}
