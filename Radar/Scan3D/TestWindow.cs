using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KanoopCommon.CommonObjects;
using KanoopCommon.Logging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Radar.Scan3D
{
	class TestWindow : GameWindow
	{
		static readonly String Shaders = "c:/pub/opengl/Components/Shaders";
		static readonly String VertexShader = Path.Combine(Shaders, "SimpleVertexShader.vertexshader");
		static readonly String FragmentShader = Path.Combine(Shaders, "SimpleFragmentShader.fragmentshader");

		private int _vertexArray;
		private int _vertexBuffer;
		private int _programID;

		public TestWindow()
			: base(800, 600,
				  GraphicsMode.Default,
				  "3D Rendering",
				  GameWindowFlags.Default,
				  DisplayDevice.Default,
				  4, 0,
				  GraphicsContextFlags.ForwardCompatible)
		{
			Title += ": OpenGl version " + GL.GetString(StringName.Version);

			MakeCurrent();

			GL.GenVertexArrays(1, out _vertexArray);
			GL.BindVertexArray(_vertexArray);


			GL.GenBuffers(1, out _vertexBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);

			float[] vertexData = new float[] {
			   -1.0f, -1.0f, 0.0f,
			   1.0f, -1.0f, 0.0f,
			   0.0f,  1.0f, 0.0f };
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexData.Length * sizeof(float)), vertexData, BufferUsageHint.StaticDraw);

			_programID = LoadShaders();
			GL.UseProgram(_programID);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			Matrix4 m4 = new Matrix4(
				1, 0, 0, 10,
				0, 1, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1);
			Vector4 v4 = new Vector4(10, 10, 10, 1);
			Vector4 transformed = m4 * v4;

			var scaled = transformed * (float).1;

			Vector3 rotateVector = new Vector3(0, 0, 0);
			//Vector3.Transform()

			// 1st attribute buffer : vertices
			GL.EnableVertexAttribArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

			// Draw the triangle !
			GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
			GL.DisableVertexAttribArray(0);
			SwapBuffers();
		}

		int LoadShaders()
		{
			int programID = 0;
			try
			{
				// Create the shaders
				int VertexShaderID = GL.CreateShader(ShaderType.VertexShader);
				int FragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

				// Read the Vertex Shader code from the file


				// Compile Vertex Shader
				GL.ShaderSource(VertexShaderID, File.ReadAllText(VertexShader));
				GL.CompileShader(VertexShaderID);

				// Check Vertex Shader
				String infoLog;
				if((infoLog = GL.GetShaderInfoLog(VertexShaderID)).Length > 0)
				{
					throw new Exception3D($"VertexShader: {infoLog}");
				}

				// Compile Fragment Shader
				GL.ShaderSource(FragmentShaderID, File.ReadAllText(FragmentShader));
				GL.CompileShader(FragmentShaderID);

				// Check Fragment Shader
				if((infoLog = GL.GetShaderInfoLog(FragmentShaderID)).Length > 0)
				{
					throw new Exception3D($"FragmentShader: {infoLog}");
				}

				// Link the program
				programID = GL.CreateProgram();
				GL.AttachShader(programID, VertexShaderID);
				GL.AttachShader(programID, FragmentShaderID);
				GL.LinkProgram(programID);

				// Check the program
				if((infoLog = GL.GetProgramInfoLog(programID)).Length > 0)
				{
					throw new Exception3D($"Program Lik=nk: {infoLog}");
				}

				GL.DetachShader(programID, VertexShaderID);
				GL.DetachShader(programID, FragmentShaderID);

				GL.DeleteShader(VertexShaderID);
				GL.DeleteShader(FragmentShaderID);
			}
			catch(Exception e)
			{
				Log.SysLogText(LogLevel.ERROR, $"EXCEPTION: {e.Message}");
			}
			return programID;
		}

	}
}
