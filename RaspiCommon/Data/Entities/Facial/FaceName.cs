using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Database;

namespace RaspiCommon.Data.Entities.Facial
{
	public class FaceName
	{
		[ColumnName("name_id")]
		public UInt32 NameID { get; set; }
		[ColumnName("name")]
		public String Name { get; set; }

		public FaceName()
		{

		}

		public override string ToString()
		{
			return $"{Name} ({NameID})";
		}
	}

	public class FaceNameList : List<FaceName>
	{
		public bool TryGetName(int id, out String name)
		{
			name = null;
			FaceName faceName = Find(f => f.NameID == (UInt32)id);
			if(faceName != null)
			{
				name = faceName.Name;
			}
			return name != null;
		}
	}
}
