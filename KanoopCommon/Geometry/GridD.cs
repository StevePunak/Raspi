using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using KanoopCommon.Conversions;
using System.Drawing;
using KanoopCommon.Database;

namespace KanoopCommon.Geometry
{
	public class GridD : RectangleD, IGrid
	{
		#region Public Properties

        public Int32 NumberOfRows
        {
            get; private set;
        }

        public Int32 NumberOfColumns
        {
            get; private set;
        }

		#endregion

		#region Constructors

		public GridD(Decimal x, Decimal y, Decimal width, Decimal height, Int32 numberOfRows, Int32 numberOfColumns)
			: this((Double)x, (Double)y, (Double)width, (Double)height, numberOfRows, numberOfColumns) {} 

		public GridD(int x, int y, int width, int height, Int32 numberOfRows, Int32 numberOfColumns)
			: this((Double)x, (Double)y, (Double)width, (Double)height, numberOfRows, numberOfColumns) {} 

		public GridD(Double x, Double y, Double width, Double height, Int32 numberOfRows, Int32 numberOfColumns)
			: this(new PointD(x, y), new PointD(x + width, y), new PointD(x + width, y + height), new PointD(x, y + height), numberOfRows, numberOfColumns) {}

		public GridD(PointD p1, PointD p2, PointD p3, PointD p4, Int32 numberOfRows, Int32 numberOfColumns) : this()
		{
			m_Points[0] = p1;
			m_Points[1] = p2;
			m_Points[2] = p3;
			m_Points[3] = p4;

            NumberOfRows = numberOfRows;
            NumberOfColumns = numberOfColumns;
		}

		public GridD ()
		{
			m_Points = new PointDList { PointD.Empty, PointD.Empty, PointD.Empty, PointD.Empty };
		}

		#endregion

		#region Utility
		public override string ToString()
		{
			return String.Format("X: {0} Y: {1} W: {2} H {3} NR {4} NC {5}", UpperLeft.X, UpperLeft.Y, Width, Height, NumberOfRows, NumberOfColumns);
		}
		#endregion
	}
}
