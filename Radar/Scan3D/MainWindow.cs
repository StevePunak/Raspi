using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Radar.Scan3D
{
	class MainWindow : GameWindow
	{
		static readonly String Shaders = "c:/pub/opengl/Components/Shaders";
		static readonly String VertexShader = Path.Combine(Shaders, "vertexShader");
		static readonly String FragmentShader = Path.Combine(Shaders, "fragmentShader");

		public MainWindow()
			: base(800, 600, 
				  GraphicsMode.Default,
				  "3D Rendering",
				  GameWindowFlags.Default,
				  DisplayDevice.Default,
				  4, 0,
				  GraphicsContextFlags.ForwardCompatible)
		{
			Title += ": OpenGl version " + GL.GetString(StringName.Version);
		}

		private int _program;
		private int _vertexArray;
		private int _buffer;
		protected override void OnLoad(EventArgs e)
		{
			CursorVisible = true;
			_program = CompileShaders();
			Closed += OnClosed;
			MakeCurrent();
		
		}

		int _pointSize = 10;
		bool _bigger = true;
		private double _time;
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			Title = $"{Title}: (Vsync: {VSync}) FPS: {1f / e.Time:0}";
			_time += e.Time;

			Color4 backColor;
			backColor.A = 1.0f;
			backColor.R = 0.1f;
			backColor.G = 0.1f;
			backColor.B = 0.3f;
			GL.ClearColor(backColor);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.UseProgram(_program);

			// set parm to vertex shader
			GL.VertexAttrib1(0, _time);

			// set parm to vertex shader
			Vector4 position;
			position.X = (float)Math.Sin(_time) * 0.5f;
			position.Y = (float)Math.Cos(_time) * 0.5f;
			position.Z = 0.0f;
			position.W = 1.0f;
			GL.VertexAttrib4(1, position);

			GL.DrawArrays(PrimitiveType.Points, 0, 1);
			GL.PointSize(_pointSize);
			if(_bigger)
			{
				if(++_pointSize > 300)
				{
					_bigger = false;
				}
			}
			else
			{
				if(--_pointSize <= 5)
				{
					_bigger = true;
				}
			}
			SwapBuffers();
		}

		private int CompileShader(ShaderType type, string path)
		{
			int shader = GL.CreateShader(type);
			GL.ShaderSource(shader, File.ReadAllText(path));
			GL.CompileShader(shader);
			String info = GL.GetShaderInfoLog(shader);
			if(!string.IsNullOrWhiteSpace(info))
				Debug.WriteLine($"GL.CompileShader [{type}] had info log: {info}");
			return shader;
		}

		private int CompileShaders()
		{
			var program = GL.CreateProgram();
			var shaders = new List<int>();
			shaders.Add(CompileShader(ShaderType.VertexShader, VertexShader));
			shaders.Add(CompileShader(ShaderType.FragmentShader, FragmentShader));

			foreach(var shader in shaders)
				GL.AttachShader(program, shader);
			GL.LinkProgram(program);
			foreach(var shader in shaders)
			{
				GL.DetachShader(program, shader);
				GL.DeleteShader(shader);
			}

			return program;
		}

		protected override void OnResize(EventArgs e)
		{
			GL.Viewport(0, 0, Width, Height);
		}

		private void OnClosed(object sender, EventArgs eventArgs)
		{
			Exit();
		}

		public override void Exit()
		{
			GL.DeleteVertexArrays(1, ref _vertexArray);
			GL.DeleteProgram(_program);
			base.Exit();
		}

	}
}
