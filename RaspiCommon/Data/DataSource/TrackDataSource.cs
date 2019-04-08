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
			return LandmarkInsert("landmarks", landmark);
		}

		public virtual DBResult LandmarkUpdate(Landmark landmark)
		{
			return LandmarkUpdate("landmarks", landmark);
		}

		public virtual DBResult LandmarksClear(Landscape landscape)
		{
			return LandmarksClear("landmarks", landscape);
		}

		public virtual DBResult LandmarksGet(Landscape landscape, out LandmarkList landmarks)
		{
			return LandmarksGet<Landmark>("landmarks", landscape, out landmarks);
		}

		public virtual DBResult PointMarkerInsert(Landmark landmark)
		{
			return LandmarkInsert("point_markers", landmark);
		}

		public virtual DBResult PointMarkerUpdate(Landmark landmark)
		{
			return LandmarkUpdate("point_markers", landmark);
		}

		public virtual DBResult PointMarkersClear(Landscape landscape)
		{
			return LandmarksClear("point_markers", landscape);
		}

		public virtual DBResult PointMarkersGet(Landscape landscape, out LandmarkList landmarks)
		{
			return LandmarksGet<PointMarker>("point_markers", landscape, out landmarks);
		}

		DBResult LandmarkInsert(String table, Landmark landmark)
		{
			DBResult result = new DBResult(DBResult.Result.InsertionFailure);
			QueryString sql = DB.Format(
				"INSERT into {0} (landscape_id, location, label) VALUES \n" +
				"({1}, {2}, '{3}')",
				table,
				landmark.Landscape.LandscapeID, Unescaped.String(landmark.Location.ToSQLString()), landmark.Label);
			result = DB.Insert(sql);
			return result;

		}

		DBResult LandmarkUpdate(String table, Landmark landmark)
		{
			String keyField = String.Format("{0}_id", table.Substring(0, table.Length - 1));
			DBResult result = new DBResult(DBResult.Result.InsertionFailure);
			QueryString sql = DB.Format(
				"UPDATE {0} set label='{1}' where {2} = {3}",
				table, landmark.Label, keyField, landmark.PrimaryID);
			result = DB.Insert(sql);
			return result;

		}

		DBResult LandmarksClear(String table, Landscape landscape)
		{
			DBResult result = new DBResult(DBResult.Result.InsertionFailure);
			QueryString sql = DB.Format(
				"DELETE FROM {0} WHERE landscape_id = {1}", table, landscape.LandscapeID);
			result = DB.Delete(sql);
			return result;

		}

		DBResult LandmarksGet<T>(String table, Landscape landscape, out LandmarkList landmarks) where T : new()
		{
			landmarks = new LandmarkList();

			DBResult retVal = new DBResult(DBResult.Result.NoData);
			QueryString sql = DB.Format(
				"SELECT * from {0} WHERE landscape_id = {1}", table, landscape.LandscapeID);
			DatabaseDataReader reader;

			retVal = DB.Query(sql, out reader);
			using(reader)
			{
				if(retVal.ResultCode == DBResult.Result.Success)
				{
					while(reader.Read())
					{
						T landmark = DataReaderConverter.CreateClassFromDataReader<T>(reader);
						landmarks.Add(landmark as Landmark);
					}
				}
			}
			return retVal;
		}
	}
}
