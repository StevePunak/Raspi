using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using KanoopCommon.Database;

namespace RaspiCommon.Data.Entities.Facial
{
	public class FacialImage
	{
		[ColumnName("image_id")]
		public UInt32 ImageID { get; set; }
		[ColumnName("image_bytes")]
		internal byte[] FaceBytes { get; set; }
		[ColumnName("height")]
		public int Height { get; set; }
		[ColumnName("width")]
		public int Width { get; set; }
		[ColumnName("channels")]
		public int Channels { get; set; }
		[ColumnName("depth_type")]
		public DepthType DepthType { get; set; }

		// joined columns
		[ColumnName("name")]
		public String Name { get; set; }
		[ColumnName("name_id")]
		public UInt32 NameID { get; set; }

		public Mat Image { get; set; }

		public FacialImage()
		{

		}

		public override string ToString()
		{
			return $"{Name} Image {ImageID} NameID {NameID}";
		}
	}
}
