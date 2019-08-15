using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Radar.Scan3D
{
	public class RenderObject : IDisposable
	{
		private bool _initialized;
		private readonly int _vertexArray;
		private readonly int _buffer;
		private readonly int _verticeCount;

		public RenderObject(Vertex[] vertices)
		{
			_verticeCount = vertices.Length;

			GL.GenVertexArrays(1, out _vertexArray);
			_buffer = GL.GenBuffer();

			GL.BindVertexArray(_vertexArray);
			GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);

			//GL.NamedBufferStorage(
			//		_buffer,
			//		Vertex.Size * vertices.Length,			// the size needed by this buffer
			//		vertices,								// data to initialize with
			//		BufferStorageFlags.MapWriteBit);		// at this point we will only write to the buffer

			_initialized = true;
		}
		public void Render()
		{
			GL.BindVertexArray(_vertexArray);
			GL.DrawArrays(PrimitiveType.Triangles, 0, _verticeCount);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_initialized)
				{
					GL.DeleteVertexArray(_vertexArray);
					GL.DeleteBuffer(_buffer);
					_initialized = false;
				}
			}
		}
	}
}
