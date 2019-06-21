using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using KanoopCommon.Conversions;
using KanoopCommon.Database;
using RaspiCommon.Data.Entities.Facial;
using RaspiCommon.Extensions;

namespace RaspiCommon.Data.DataSource
{
	[DatabaseVersion("0.1")]
	public class FacialDataSource : DataSourceBase
	{
		public FacialDataSource(SqlDBCredentials credentials)
			: base(credentials) { }

		public virtual DBResult AddFaceName(String name, out FaceName faceName)
		{
			DBResult result = DBResult.InsertionFailure;
			faceName = new FaceName() { Name = name };

			DB.DefaultTimeout = 60;
			QueryString sql = DB.Format("INSERT into facial.names (name) VALUES ('{0}')", name);
			if((result = DB.Insert(sql)).ResultCode == DBResult.Result.Success)
			{
				faceName.NameID = result.ItemID;
			}
			return result;

		}

		public virtual DBResult GetName(String name, out FaceName faceName)
		{
			faceName = null;
			DBResult result = DBResult.NoData;

			QueryString sql = DB.Format(
					"SELECT * from facial.names WHERE name like '{0}'", name);

			DatabaseDataReader reader;
			result = DB.Query(sql, out reader);
			using(reader)
			{
				if(result.ResultCode == DBResult.Result.Success && reader.Read())
				{
					faceName = DataReaderConverter.CreateClassFromDataReader<FaceName>(reader);
				}
			}
			return result;
		}

		public virtual DBResult GetAllNames(out List<String> names)
		{
			names = new List<string>();
			DBResult result = DBResult.NoData;

			QueryString sql = DB.Format("SELECT * from facial.names order by name");

			DatabaseDataReader reader;
			result = DB.Query(sql, out reader);
			using(reader)
			{
				while(result.ResultCode == DBResult.Result.Success && reader.Read())
				{
					String name = DBUtil.GetString(reader["name"]);
					names.Add(name);
				}
			}
			return result;
		}

		public virtual DBResult GetAllNames(out FaceNameList names)
		{
			names = new FaceNameList();
			DBResult result = DBResult.NoData;

			QueryString sql = DB.Format("SELECT * from facial.names order by name");

			DatabaseDataReader reader;
			result = DB.Query(sql, out reader);
			using(reader)
			{
				while(result.ResultCode == DBResult.Result.Success && reader.Read())
				{
					FaceName name = DataReaderConverter.CreateClassFromDataReader<FaceName>(reader);
					names.Add(name);
				}
			}
			return result;
		}

		public virtual DBResult GetAllFacialImages(out List<FacialImage> faces)
		{
			faces = new List<FacialImage>();
			DBResult result = DBResult.NoData;

			QueryString sql = DB.Format(
				"SELECT I.*, N.* \n" +
				"FROM facial.images I \n" +
				"JOIN facial.names N on N.name_id = I.name_id \n");

			DatabaseDataReader reader;
			result = DB.Query(sql, out reader);
			using(reader)
			{
				while(result.ResultCode == DBResult.Result.Success && reader.Read())
				{
					FacialImage face = DataReaderConverter.CreateClassFromDataReader<FacialImage>(reader);
					face.Image = new Mat(face.Height, face.Width, face.DepthType, face.Channels);
					face.Image.LoadFromByteArray(face.FaceBytes);
					faces.Add(face);
				}
			}
			return result;
		}

		public virtual DBResult AddImage(String name, Mat image)
		{
			FacialImage face;
			return AddImage(name, image, out face);
		}

		public virtual DBResult AddImage(String name, Mat image, out FacialImage face)
		{
			DBResult result = DBResult.InsertionFailure;
			face = null;
			FaceName faceName;
			if((result = GetName(name, out faceName)).ResultCode != DBResult.Result.Success)
			{
				AddFaceName(name, out faceName);
			}

			byte[] bytes = image.ToByteArray();

			QueryString sql = DB.Format(
				"INSERT into facial.images (name_id, width, height, channels, depth_type, image_bytes) \n" +
				$"VALUES ({faceName.NameID}, {image.Width}, {image.Height}, {image.NumberOfChannels}, {(int)image.Depth}, ?)");

			OdbcParameterList parameters = new OdbcParameterList();
			parameters.Add(OdbcType.Binary, bytes);

			if((result = DB.Insert(sql, parameters)).ResultCode == DBResult.Result.Success)
			{
				face = new FacialImage() { Name = faceName.Name, NameID = faceName.NameID, ImageID = result.ItemID, Image = image };
			}
			return result;
		}

	}
}
