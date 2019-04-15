using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Conversions;
using KanoopCommon.Database;
using KanoopCommon.Geometry;
using RaspiCommon.Data.Entities;
using RaspiCommon.Spatial.DeadReckoning;

namespace RaspiCommon.Data.DataSource
{
	[DatabaseVersion("0.1")]
	public class TrackDataSource : DataSourceBase
	{
		public TrackDataSource(SqlDBCredentials credentials)
			: base(credentials) {}

		#region Dead Reckoning

		public virtual DBResult CreateDREnvironment(DeadReckoningEnvironment environment)
		{
			DBResult result = DBResult.InsertionFailure;

			DB.DefaultTimeout = 60;
			QueryString sql = DB.Format(
				"INSERT into dr_grids (name, scale, angular_offset, width, height, origin_x, origin_y) \n" +
				"VALUES ('{0}', {1}, {2}, {3}, {4}, {5}, {6})",
				environment.Name, environment.Scale, 
				environment.AngularOffset, 
				(Double)environment.Grid.Matrix.Width * environment.Scale, 
				(Double)environment.Grid.Matrix.Height * environment.Scale, 
				environment.Origin.X, environment.Origin.Y);
			if((result = DB.Insert(sql)).ResultCode == DBResult.Result.Success)
			{
				UInt32 gridID = result.ItemID;

				StringBuilder sb = new StringBuilder();
				for(int row = 0;row < environment.Grid.Matrix.Height;row++)
				{
					for(int col = 0;col < environment.Grid.Matrix.Width;col++)
					{
						sb.AppendFormat(
							"INSERT into grid_cells (grid_id, x, y, state) \n" +
							"VALUES ({0}, {1}, {2}, {3});\n",
							gridID, environment.Grid.Matrix.Cells[row, col].X, environment.Grid.Matrix.Cells[row, col].Y, (int)environment.Grid.Matrix.Cells[row, col].State);
					}
				}

				sql = DB.Format("{0}", sb);
				String s = sql.ToString();
				result = DB.Insert(sql);
			}
			return result;
		}

		public virtual DBResult DeleteDREnvironment(String name)
		{
			DBResult result = DBResult.InsertionFailure;

			if((result = GetEnvironemtID(name)).ResultCode == DBResult.Result.Success)
			{
				UInt32 gridID = result.ItemID;
				QueryString sql = DB.Format(
					"DELETE FROM grid_cells WHERE grid_id = {0}", gridID);
				if((result = DB.Delete(sql)).ResultCode == DBResult.Result.Success)
				{
					sql = DB.Format(
					"DELETE FROM dr_grids WHERE grid_id = {0}", gridID);
					result = DB.Delete(sql);
				}
			}

			return result;
		}

		public virtual DBResult GetDREnvironment(String name, out DeadReckoningEnvironment environment)
		{
			environment = null;
			DBResult result = DBResult.NoData;

			QueryString sql = DB.Format(
				"SELECT * from dr_grids WHERE name = '{0}'", name);

			DatabaseDataReader reader;
			result = DB.Query(sql, out reader);
			using(reader)
			{
				if(result.ResultCode == DBResult.Result.Success && reader.Read())
				{
					environment = new DeadReckoningEnvironment(
						DBUtil.GetString(reader["name"]),
						DBUtil.GetDouble(reader["width"]),
						DBUtil.GetDouble(reader["width"]),
						DBUtil.GetDouble(reader["scale"]),
						DBUtil.GetDouble(reader["angular_offset"]),
						new PointD(DBUtil.GetDouble(reader["origin_x"]), DBUtil.GetDouble(reader["origin_y"])));
					environment.ID = DBUtil.GetUInt32(reader["grid_id"]);
				}
			}

			if(environment != null)
			{
				sql = DB.Format("SELECT * from grid_cells WHERE grid_id = {0}", environment.ID);
				result = DB.Query(sql, out reader);
				using(reader)
				{
					if(result.ResultCode == DBResult.Result.Success)
					{
						while(reader.Read())
						{
							GridCell cell = DataReaderConverter.CreateClassFromDataReader<GridCell>(reader);
							environment.Grid.Matrix.Cells[cell.X, cell.Y] = cell;
						}
					}
				}
			}

			return result;
		}

		DBResult GetEnvironemtID(String name)
		{
			DBResult result = DBResult.NoData;

			QueryString sql = DB.Format(
				"SELECT * from dr_grids WHERE name = '{0}'", name);

			DatabaseDataReader reader;
			result = DB.Query(sql, out reader);
			using(reader)
			{
				if(result.ResultCode == DBResult.Result.Success && reader.Read())
				{
					result.ItemID = DBUtil.GetUInt32(reader["grid_id"]);
				}
			}
			return result;
		}

		#endregion

		#region Image Landscapes

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

		#endregion
	}
}
