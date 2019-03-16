using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;

namespace KanoopCommon.LocationAlgorithms
{
	public class SpanningTree
	{
		#region Protected Member Variables

		protected Dictionary<String, TreePathVertice>		m_Vertices;

		protected List<TreePathVertice>						m_VisitedVertices;
		protected List<TreePathVertice>						m_UnvisitedVertices;

		protected LineList									m_OriginalLines;
		protected LineList									m_Lines;
		protected LineList									m_LinesToConstructor;

		#endregion

		#region Public Properties

		public List<TreePathVertice> Vertices
		{
			get { return new List<TreePathVertice>(m_Vertices.Values); }
		}

		protected TreePathVertice							m_Origin;
		public IPoint Origin
		{
			get { return m_Origin.Position; }
			set { m_Origin = AddAdHocVertice(ConvertPointToPointD(value), TreePathVertice.VerticeType.AdHocOrigin); }
		}

		protected TreePathVertice							m_Destination;
		public IPoint Destination
		{
			get { return m_Destination.Position; }
			set { m_Destination = AddAdHocVertice(ConvertPointToPointD(value), TreePathVertice.VerticeType.AdHocDestination); }
		}

		static bool _DebugLogging;
		public static bool DebugLogging
		{
			get { return _DebugLogging; }
			set
			{
				if(value == true && _DebugLogging == false)
				{
					Log = new Log();
					Log.Open(LogLevel.ALWAYS, @"e:\tmp\stree2.log", OpenFlags.COMBO_VERBOSE | OpenFlags.OUTPUT_TO_FILE);
					_DebugLogging = true;
				}
				else if(value == false && _DebugLogging == true)
				{
					Log.Close();
					Log = null;
					_DebugLogging = false;
				}
			}
		}

		public static Log Log { get; private set; }

		#endregion

		#region Constructor(s)

		static SpanningTree()
		{
			_DebugLogging = false;
		}
		
		public SpanningTree()
		{
		}

		public SpanningTree(LineList lines)
		{
			m_LinesToConstructor = lines;

			Initialize();

		}

		protected virtual void Initialize()
		{
			if(DebugLogging && Log == null)
			{
				DebugLogText(LogLevel.DEBUG, "**************** New Spanning Tree {0} lines", m_LinesToConstructor.Count);
			}

			DumpLines("To constructor", m_LinesToConstructor);

			m_Vertices = new Dictionary<String, TreePathVertice>();
			m_Lines = new LineList();
			m_OriginalLines = new LineList();

			foreach(Line line in m_LinesToConstructor)
			{
				m_OriginalLines.Add(line);
				m_Lines.Add(line);

				/** create objects for each end if they do not already exist */
				TreePathVertice v1, v2;
				if(m_Vertices.TryGetValue(line.P1.HashName, out v1) == false)
				{
					v1 = new TreePathVertice(line.P1 as PointD, TreePathVertice.VerticeType.Standard);
					v1.Name = String.Format("{0} pt1", line.Name);
					m_Vertices.Add(v1.HashName, v1);
				}

				if(m_Vertices.TryGetValue(line.P2.HashName, out v2) == false)
				{
					v2 = new TreePathVertice(line.P2 as PointD, TreePathVertice.VerticeType.Standard);
					v2.Name = String.Format("{0} pt2", line.Name);
					m_Vertices.Add(v2.HashName, v2);
				}

				/** add them as neighbors to each other */
				v1.TryAddNeighbor(v2);
				v2.TryAddNeighbor(v1);
			}

			DumpLines("m_Lines", m_Lines);
			DumpVertices();
		}

		#endregion

		#region Primary Functionality

		public virtual LineList ComputePath()
		{
			/**
			 * Compute the shortest path using spanning tree 
			 * based on Dijkstras Algorithm
			 */
			if(m_Origin == null || m_Destination == null)
			{
				throw new Exception("Spanning Tree must have origin and destination");
			}

			InitializeVertices();

			while(Cycle())	{}

			return new LineList(GetPath());
		}

		public void InitializeVertices()
		{

			/** create empty list of visited vertices */
			m_VisitedVertices = new List<TreePathVertice>();
			
			/** create list of all vertices which we will remove from as we visit */
			m_UnvisitedVertices = new List<TreePathVertice>(m_Vertices.Values);

			/** 
			 * set all initial values for each vertice...
			 * origin distance to zero, and all others to infinity 
			 * state and source
			 */
			foreach(KeyValuePair<String, TreePathVertice> kvp in m_Vertices)
			{
				TreePathVertice vertice = kvp.Value;

				if(vertice == m_Origin)
				{
DebugLogText(LogLevel.DEBUG, "Set origin vertice to {0} ({1})", m_Origin, ConvertPointToNative(m_Origin.Position));
					vertice.Distance = 0;
				}
				else
				{
DebugLogText(LogLevel.DEBUG, "Set other vertice at {0}, ({1})", vertice, ConvertPointToNative(vertice.Position));
					vertice.Distance = Double.PositiveInfinity;
				}
				vertice.State = TreePathVertice.VerticeState.Unvisited;
				vertice.Source = null;
			}
		}

		public bool Cycle()
		{
			/** get the vertice with the lowest distance from the origin */
			TreePathVertice current = null;
			foreach(TreePathVertice vertice in m_UnvisitedVertices)
			{
				if(current == null || vertice.Distance <= current.Distance)
				{
					current = vertice;
				}
			}
DebugLogText(LogLevel.DEBUG, "Cycle set current vertice to {0}", current);

			if(current.Distance != Double.PositiveInfinity)
			{
				/** go through each neighbor of the current */
				foreach(KeyValuePair<String, TreePathVertice> kvp in current.Neighbors)
				{
					TreePathVertice neighbor = kvp.Value;
					if(neighbor.State == TreePathVertice.VerticeState.Unvisited)
					{
						Double nDistance = FlatGeo.Distance(current.Position, neighbor.Position);
						if(current.Distance + nDistance < neighbor.Distance)
						{
							neighbor.Distance = current.Distance + nDistance;
							neighbor.Source = current;
DebugLogText(LogLevel.DEBUG, "---      set neighbor {0} to distance of {1:0.00}", neighbor, GetNativeDistance(neighbor.Distance));
						}
					}
				}

				/** move this node from visited to unvisited */
				current.State = TreePathVertice.VerticeState.Visited;
DebugLogText(LogLevel.DEBUG, "Set vertice {0} to Visited", current);

				m_UnvisitedVertices.Remove(current);
				m_VisitedVertices.Add(current);
			}
			else
			{
				m_UnvisitedVertices.Remove(current);
DebugLogText(LogLevel.ERROR, "VERTICE {0} has no solution!", current);
			}

			return m_UnvisitedVertices.Count > 0;
		}

		public PointDList GetPath()
		{
			/** now work backwards along source paths */
			PointDList path = new PointDList();
			TreePathVertice vertice = m_Destination;
			TreePathVertice lastInserted;
			do
			{
				path.Insert(0, vertice.Position);
				lastInserted = vertice;
				vertice = vertice.Source;
			}while(lastInserted != m_Origin);

			return path;
		}

		protected TreePathVertice AddAdHocVertice(PointD point, TreePathVertice.VerticeType type)
		{
			TreePathVertice V1, V2;
			if(m_Vertices.TryGetValue(point.HashName, out V1) == false)
			{
				/** 
				 * Having ensured that our vertice does not already exist,
				 * we are either adding an origin or destination.
				 * 
				 * we need to add the vertice itself (V1), as well as an additional one (V2) on the closest
				 * line segment (L) to the new vertice.
				 * 
				 * The vertices at the end of (L) will have each other as neighbors. Since we are
				 * essentially splitting (L) into two segments, 
				 */

				/** clear any that exist of the given type */
				ClearAdHocVertices(type);

				/** create our new vertice */
				V1 = new TreePathVertice(point, type);
				m_Vertices.Add(V1.HashName, V1);

				/** find closest point on the closest line for a second vertice */
				Double closestDistance;
				Line L;
				PointD closestPoint = m_Lines.ClosestPointTo(point, out L, out closestDistance);
DebugLogText(LogLevel.DEBUG, "Added ad-hoc vertice of type {0} at point {1} closest to line [[{2}]] at {3} pixels", type, point, L, closestDistance);
				if(closestPoint != null)
				{
					/** if the closest vertice doesn't exist yet, we will create it, and split the line L */
					if(m_Vertices.TryGetValue(closestPoint.HashName, out V2) == false)
					{
						/** get the vertices on the line we are about to split */
						TreePathVertice LV1, LV2;
						if(	m_Vertices.TryGetValue(L.P1.HashName, out LV1) == false ||
							m_Vertices.TryGetValue(L.P2.HashName, out LV2) == false)
						{
							throw new Exception("Vertices on original lines should exist");
						}

						/** create our new vertice on the split line */
						V2 = new TreePathVertice(closestPoint, type);
						m_Vertices.Add(V2.HashName, V2);

						/** fix up neighbors */
						LV1.TryReplaceNeighbor(LV2, V2);
						LV2.TryReplaceNeighbor(LV1, V2);

						V2.TryAddNeighbor(LV1);
						V2.TryAddNeighbor(LV2);
						V2.TryAddNeighbor(V1);

						V1.TryAddNeighbor(V2);

						/** replace L with two new lines L1 and L2 */
						Line l1 = new Line(L.P1, closestPoint);
						Line l2 = new Line(closestPoint, L.P2);

						m_Lines.Remove(L);
						m_Lines.Add(l1);
						m_Lines.Add(l2);
					}
					else
					{
						V1.TryAddNeighbor(V2);
						V2.TryAddNeighbor(V1);
					}
				}
			}

//			DumpVertices();

			return V1;
		}

		PointD FindClosestPointOnOriginalPath(PointD to)
		{
			PointD proximal = null;
			Double shortest = Double.MaxValue;

			foreach(Line line in m_Lines)
			{
				PointD point = line.ClosestPointTo(to);
				Double distance = FlatGeo.GetDistance(point, to);
				if(distance < shortest)
				{
					shortest = distance;
					proximal = point;
				}
			}

			return proximal;
		}

		SortedList<Double, TreePathVertice> GetVerticesInProximalOrder(PointD to)
		{
			SortedList<Double, TreePathVertice> list = new SortedList<Double, TreePathVertice>();

			foreach(TreePathVertice vertice in m_Vertices.Values)
			{
				Double distance = FlatGeo.GetDistance(vertice.Position, to);
				while(list.ContainsKey(distance))
					distance += .001;
				list.Add(distance, vertice);
			}

			return list;
		}

		void ClearAdHocVertices(TreePathVertice.VerticeType type)
		{
			List<String> deleteList = new List<String>();
			foreach(KeyValuePair<String, TreePathVertice> kvp in m_Vertices)
			{
				if(kvp.Value.Type == type)
				{
					deleteList.Add(kvp.Key);
				}
			}
			foreach(String key in deleteList)
			{
				m_Vertices.Remove(key);
			}
		}

		#endregion

		#region Abstract Implementation

		public virtual PointD ConvertPointToPointD(IPoint original)
		{
			return original as PointD;
		}

		public virtual IPoint ConvertPointToNative(IPoint point)
		{
			return point as PointD;
		}

		protected virtual Double GetNativeDistance(Double distance)
		{
			return distance;
		}

		protected virtual Double GetNativeDistance(IPoint p1, IPoint p2)
		{
			return FlatGeo.GetDistance(p1 as PointD, p2 as PointD);
		}

		#endregion

		#region Logging

		protected static void DebugLogText(LogLevel logLevel, String text, params object[] parms)
		{
			if(DebugLogging)
			{
				Log.LogText(logLevel, text, parms);
			}
		}

		#endregion

		#region Utility

		PointD PossiblyCombinePoint(PointD point, Double distance)
		{
			PointD ret = point;
			foreach(KeyValuePair<String, TreePathVertice> kvp in m_Vertices)
			{
				TreePathVertice vertice = kvp.Value;
				if(point.Equals(vertice.Position) == false && FlatGeo.Distance(point, vertice.Position) < distance)
				{
					ret = vertice.Position;
					DebugLogText(LogLevel.DEBUG, "Combining {0} into {1}", point, ret);
					break;
				}
			}
			return ret;
		}

		public void DumpLines(String name, IEnumerable<Line> lines)
		{
			StringBuilder sb = new StringBuilder();
			foreach(Line line in lines)
			{
				sb.AppendFormat("{0} {1} ({2}) - {3} ({4})\n", line.Name, ConvertPointToNative(line.P1), line.P1.HashName, ConvertPointToNative(line.P2), line.P2.HashName);
			}

			DebugLogText(LogLevel.DEBUG, "Line Dump [{0}]\n{1}", name, sb);
		}

		public void DumpVertices()
		{
			StringBuilder sb = new StringBuilder();
			foreach(TreePathVertice vertice in m_Vertices.Values)
			{
				sb.AppendFormat("{0} Pos: {1} Dist: {2} State: {3}  Type: {4}  Source: {5}\n",
					vertice, ConvertPointToNative(vertice.Position), vertice.Distance > 1000000000 ? "infinite" : GetNativeDistance(vertice.Distance).ToString("0.00"), vertice.State, vertice.Type, vertice.Source == null ? "null" : vertice.Source.ToString());
				foreach(TreePathVertice neighbor in vertice.Neighbors.Values)
				{
					sb.AppendFormat("   Neighbor: {0} Pos: {1} Dist: {2:0.00} State: {3}  Type: {4}  Source: {5}\n",
						neighbor, ConvertPointToNative(neighbor.Position), neighbor.Distance  > 1000000000 ? "infinite" : GetNativeDistance(neighbor.Distance).ToString("0.00"), neighbor.State, neighbor.Type, neighbor.Source == null ? "null" : neighbor.Source.ToString());
				}
				sb.AppendLine();
			}
			DebugLogText(LogLevel.DEBUG, "\n{0}", sb);
		}

		public override string ToString()
		{
			return String.Format("SpanningTree: {0} vertices", m_Vertices.Count);
		}

		#endregion

	}

}
