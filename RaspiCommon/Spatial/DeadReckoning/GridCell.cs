using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Spatial.DeadReckoning
{
	public class GridCell
	{
		public static int ByteArraySize { get { return sizeof(int); } }

		public DRMatrix Matrix { get; set; }

		public int X { get; set; }
		public int Y { get; set; }

		public CellState State { get; set; }

		public GridCell()
			: this(null, 0, 0) {}

		public GridCell(DRMatrix matrix, int x, int y)
		{
			Matrix = matrix;
			X = x;
			Y = y;

			State = CellState.Unknown;
		}

		public byte[] Serialize()
		{
			return BitConverter.GetBytes((int)State);
		}

		public override string ToString()
		{
			return String.Format("{0},{1}  {2}", X, Y, State);
		}
	}
}
