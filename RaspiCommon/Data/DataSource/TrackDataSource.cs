using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Conversions;
using KanoopCommon.Database;
using RaspiCommon.Data.Entities;

namespace RaspiCommon.Data.DataSource
{
	[DatabaseVersion("0.1")]
	public class TrackDataSource : DataSourceBase
	{
		public TrackDataSource(SqlDBCredentials credentials)
			: base(credentials) {}

		public virtual DBResult LandscapeGet<T>(String name, out T landscape) where T: new()
		{
			landscape = default(T);

			DBResult retVal = new DBResult(DBResult.Result.NoData);
			QueryString sql = DB.Format(
				"SELECT * from landscapes WHERE name = '{0}'", name);
			DatabaseDataReader reader;

			retVal = DB.Query(sql, out reader);
			using(reader)
			{
				if(retVal.ResultCode == DBResult.Result.Success)
				{
					if(reader.Read())
					{
						landscape = DataReaderConverter.CreateClassFromDataReader<T>(reader);
						if(landscape is Landscape)
						{
							Object l = landscape;
							LandmarkList landmarks;
							if(LandmarksGet(landscape as Landscape, out landmarks).ResultCode == DBResult.Result.Success)
							{
								((Landscape)l).Landmarks = landmarks;
							}
						}
					}
				}
			}
			return retVal;
		}

		public virtual DBResult LandmarkInsert(Landmark landmark)
		{
			DBResult result = new DBResult(DBResult.Result.InsertionFailure);
			QueryString sql = DB.Format(
				"INSERT into landmarks (landscape_id, location, label) VALUES \n" +
				"({0}, {1}, '{2}')",
				landmark.Landscape.LandscapeID, Unescaped.String(landmark.Location.ToSQLString()), landmark.Label);
			result = DB.Insert(sql);
			return result;

		}

		public virtual DBResult LandmarksClear(Landscape landscape)
		{
			DBResult result = new DBResult(DBResult.Result.InsertionFailure);
			QueryString sql = DB.Format(
				"DELETE FROM landmarks WHERE landscape_id = {0}", landscape.LandscapeID);
			result = DB.Delete(sql);
			return result;

		}

		public virtual DBResult LandmarksGet(Landscape landscape, out LandmarkList landmarks)
		{
			landmarks = new LandmarkList();

			DBResult retVal = new DBResult(DBResult.Result.NoData);
			QueryString sql = DB.Format(
				"SELECT * from landmarks WHERE landscape_id = {0}", landscape.LandscapeID);
			DatabaseDataReader reader;

			retVal = DB.Query(sql, out reader);
			using(reader)
			{
				if(retVal.ResultCode == DBResult.Result.Success)
				{
					while(reader.Read())
					{
						Landmark landmark = DataReaderConverter.CreateClassFromDataReader<Landmark>(reader);
						landmarks.Add(landmark);
					}
				}
			}
			return retVal;
		}
	}
}
