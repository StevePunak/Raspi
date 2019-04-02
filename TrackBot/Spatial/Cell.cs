using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;

namespace TrackBot.Spatial
{
	public class CellList : List<Cell>
	{
		public CellList()
			: base() {}

		public CellList(IEnumerable<Cell> other)
			: base(other) {}

		public CellList GetEmptyCells()
		{
			CellList cells = new CellList(FindAll(c => c.IsEmpty));
			return cells;
		}

		public override string ToString()
		{
			return String.Format("{0} cells", Count);
		}
	}

	public class Cell
	{
		public PointD Center { get; private set; }
		public LevelRectangle Square { get; private set; }
		public CellContents Contents { get; set; }
		public bool IsEmpty { get { return Contents == CellContents.Empty; } }
		public object Tag { get; set; }
		public int Row { get; private set; }
		public int Column { get; private set; }
		public bool RoboIsHere { get; set; }
		public bool RoboWasHere { get; set; }

		public Cell(PointD center, Double size, int row, int col)
		{
			Center = center;
			Row = row;
			Column = col;
			Square = new LevelRectangle(center.X - (size / 2), center.Y - (size / 2), size, size);
			Contents = CellContents.Unknown;
			RoboIsHere = false;
			RoboWasHere = false;
		}

		public bool Contains(PointD point)
		{
			return Square.Contains(point);
		}

		public override string ToString()
		{
			return String.Format("{0}  row/col {1}, {2} {3}", Center.ToString(2), Row, Column, Contents);
		}
	}
}
