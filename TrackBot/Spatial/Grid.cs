using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.Geometry;
using KanoopCommon.Logging;

namespace TrackBot.Spatial
{
	public class Grid
	{
		public String Name { get; set; }

		public Double HeightMeters { get; private set; }
		public Double WidthMeters { get; private set; }
		public Double CellSize { get; private set; }
		public int CellsWide { get; private set; }
		public int CellsHigh { get; private set; }
		public List<CellList> Rows { get; private set; }
		public List<CellList> Columns { get; private set; }
		public CellList AllCells { get; private set; }

		#region Private Solution Variables


		#endregion

		public Grid()
			: this(String.Empty, 1, 1, 1) {}

		internal Grid(String name, Double width, Double height, Double cellSize)
		{
			Name = name;
			WidthMeters = width;
			HeightMeters = height;
			CellSize = cellSize;

			Clear();

			/** Grid is fully populated with empty cells */
		}


		#region Populate

		private void PopulateRowsAndColumns()
		{
			/** populate rows by index */
			for(int row = 0;row < CellsHigh;row++)
			{
				CellList rowCells = new CellList();
				Rows.Add(rowCells);
				for(int col = 0;col < CellsWide;col++)
				{
					Cell cell = GetCellAtLocation(row, col);
					rowCells.Add(cell);
				}
			}

			/** populate cols by index */
			for(int col = 0;col < CellsWide;col++)
			{
				CellList colCells = new CellList();
				Columns.Add(colCells);
				for(int row = 0;row < CellsHigh;row++)
				{
					Cell cell = GetCellAtLocation(row, col);
					colCells.Add(cell);
				}
			}
		}

		#endregion

		internal Cell GetCellAtLocation(PointD point)
		{
			bool foundRow = false, foundCol = false;

			int row;
			for(row = 0;row < Rows.Count;row++)
			{
				if(point.Y >= Rows[row][0].Square.Location.Y && point.Y <= Rows[row][0].Square.Location.Y + Rows[row][0].Square.Height)
				{
					foundRow = true;
					break;
				}
			}

			int col = 0;
			if(foundRow)
			{
				for(col = 0;col < Columns.Count;col++)
				{
					if(point.X >= Columns[col][row].Square.Location.X && point.X <= Columns[col][row].Square.Location.X + Columns[col][row].Square.Width)
					{
						foundCol = true;
						break;
					}
				}
			}

			Cell cell = null;
			if(foundRow && foundCol)
				 cell = Rows[row][col];

			return cell;
		}

		internal Cell GetCellAtLocation(int row, int col)
		{
			Cell cell = AllCells.Find(c => c.Row == row && c.Column == col);
			return cell;
		}

		#region Graphics

		public void Clear()
		{
			Rows = new List<CellList>();
			Columns = new List<CellList>();
			AllCells = new CellList();

			CellsWide = (int)(WidthMeters / CellSize);
			CellsHigh = (int)(HeightMeters / CellSize);

			for(Double x = CellSize / 2, col = 0;x < WidthMeters;x += CellSize, col++)
			{
				for(Double y = CellSize / 2, row = 0;y < HeightMeters;y += CellSize, row++)
				{
					Cell cell = new Cell(new PointD(x, y), CellSize, (int)row, (int)col);
					AllCells.Add(cell);
				}
			}

			PopulateRowsAndColumns();
		}

		public Bitmap ConvertToBitmap(int pixelsPerCell)
		{
			const int MIN_SIZE = 25;
			pixelsPerCell = Math.Max(pixelsPerCell, MIN_SIZE);

			int textOffset = MIN_SIZE;
			int sizeX = textOffset + (pixelsPerCell * CellsWide);
			int sizeY = textOffset + (pixelsPerCell * CellsHigh);
			Bitmap bm = new Bitmap(sizeX, sizeY);

			using(Graphics g = Graphics.FromImage(bm))
			{
				Font drawFont = new Font("Arial", 6);
				SolidBrush drawBrush = new SolidBrush(Color.Black);

				String text = "88";
				SizeF textSize = g.MeasureString(text, drawFont);

				int textX = 0, textY = 0;
				for(int x = 0;x < CellsWide;x++)
				{
					Cell cell = Rows[x][0];
					textX = 2;
					textY = textOffset + (x * pixelsPerCell);
					text = String.Format("{0:0.00}", cell.Center.Y);
					g.DrawString(text, drawFont, drawBrush, new PointF(textX, textY));

					cell = Columns[x][0];
					textX = textOffset + (x * pixelsPerCell);
					textY = 2;
					text = String.Format("{0:0.00}", cell.Center.X);
					g.DrawString(text, drawFont, drawBrush, new PointF(textX, textY));
				}

				for(int row = 0;row < CellsWide;row++)
				{
					for(int col = 0;col < CellsHigh;col++)
					{
						int x = (col * pixelsPerCell) + textOffset;
						int y = (row * pixelsPerCell) + textOffset;
						Point point = new Point(x, y);

						Pen borderPen = new Pen(Color.Black, 3);
						Rectangle rect = new Rectangle(point, new Size(pixelsPerCell, pixelsPerCell));
						g.DrawRectangle(borderPen, rect);

						Cell cell = Rows[row][col];
						SolidBrush brush = new SolidBrush(Color.GreenYellow);


						if(cell.Contents != CellContents.Unknown)
						{
							//Console.WriteLine("Drawing {0} {1}", cell.Contents, cell);
						}

						if(cell.RoboIsHere)
						{
							brush = new SolidBrush(Color.Red);
						}
						if(cell.RoboWasHere)
						{
							brush = new SolidBrush(Color.LightSalmon);
						}
						else if(cell.Contents == CellContents.Barrier)
						{
							brush = new SolidBrush(Color.DarkGray);
						}
						else if(cell.Contents == CellContents.Unknown)
						{
							brush = new SolidBrush(Color.Black);
						}
						else if(cell.Contents == CellContents.Empty)
						{
							brush = new SolidBrush(Color.DarkTurquoise);
						}
						else
						{
							Console.WriteLine("Drawing unknown  {0}", cell);
							brush = new SolidBrush(Color.GreenYellow);
						}

						rect.Inflate(-3, -3);
						g.FillRectangle(brush, rect);
					}
				}
			}

			return bm;
		}

		#endregion

		public override string ToString()
		{
			return String.Format("Grid {0}", Name);
		}
	}
}
