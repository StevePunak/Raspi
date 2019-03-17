using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackbot.Spatial
{
	internal class GridLocationList : List<GridLocation>
	{

	}

	internal class GridLocation
	{
		public int Row { get { return X; } set { X = value; } }
		public int Column { get { return Y; } set { Y = value; } }

		public int X { get; set; }
		public int Y { get; set; }

		public GridLocation()
			: this(0, 0) { }

		public GridLocation(int row, int column)
		{
			Row = row;
			Column = column;
		}

		public GridLocation Clone()
		{
			return new GridLocation(Row, Column);
		}

		public override string ToString()
		{
			return String.Format("Row: {0}, Col: {1}", Row, Column);
		}

	}
}
