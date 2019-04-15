using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiCommon.Spatial.DeadReckoning
{
	public class DRMatrix
	{
		public int ByteArraySize { get { return sizeof(int) + sizeof(int) + ((Width * Height) * GridCell.ByteArraySize);  } }

		public int Width { get; set; }
		public int Height { get; set; }

		public GridCell[,] Cells { get; set; }

		public DRGrid Grid { get; set; }

		public DRMatrix(DRGrid grid, int width, int height)
		{
			Width = width;
			Height = height;

			Grid = grid;

			Cells = new GridCell[Width, Height];
			for(int y = 0;y < Height;y++)
			{
				for(int x = 0;x < Width;x++)
				{
					Cells[x, y] = new GridCell(this, x, y);
				}
			}
		}

		public DRMatrix(BinaryReader br)
		{
			Width = br.ReadInt32();
			Height = br.ReadInt32();
			Cells = new GridCell[Width, Height];
			for(int x = 0;x < Width;x++)
			{
				for(int y = 0;y < Height;y++)
				{
					Cells[x, y] = new GridCell(this, x, y)
					{
						State = (CellState)br.ReadInt32()
					};
				}
			}
		}

		public byte[] Serialize()
		{
			byte[] serialized = new byte[ByteArraySize];
			using(BinaryWriter bw = new BinaryWriter(new MemoryStream(serialized)))
			{
				bw.Write(Width);
				bw.Write(Height);
				for(int x = 0;x < Width;x++)
				{
					for(int y = 0;y < Height;y++)
					{
						bw.Write(Cells[x, y].Serialize());
					}
				}
			}
			return serialized;
		}
	}
}
