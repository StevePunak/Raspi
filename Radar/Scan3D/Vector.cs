using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace Radar.Scan3D
{
	public class Vertex
	{
		public static readonly int Size = 16; // size of struct in bytes (where from ?)

		public Vector4 Position { get; set; }
		public Color4 Color { get; set; }

		public Vertex()
			: this(new Vector4(), new Color4()) { }

		public Vertex(Vector4 position, Color4 color)
		{
			Position = position;
			Color = color;
		}
	}
}
