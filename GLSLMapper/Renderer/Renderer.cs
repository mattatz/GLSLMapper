using System;
using System.Drawing;
using System.Drawing.Imaging;
using GLSLMapper.Misc;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace GLSLMapper.Renderer
{
    public class Renderer : GameWindow
    {
        const string vertexShaderSource = @"
            #version 330

            layout(location = 0) in vec4 position;

            void main(void)
            {
                gl_Position = position;
            }
        ";

        // A simple fragment shader. Just a constant red color.
        readonly string fragmentShaderSource = @"
            #version 330

            out vec4 outputColor;
            uniform vec2 uSize;

            void main(void)
            {
                vec2 xy = gl_FragCoord.xy / uSize;
                outputColor = vec4(xy, 1.0, 1.0);
                // outputColor = vec4(1.0, 0.0, 0.0, 1.0);
                // outputColor = vec4(gl_FragCoord.xy, 0.0, 1.0);
            }
        ";

        // Points of a triangle in normalized device coordinates.
        readonly float[] vertices = new float[] {
            // X, Y, Z, W
            -1.0f, -1.0f, 0.0f, 1.0f,
            1.0f, -1.0f, 0.0f, 1.0f,
            -1.0f, 1.0f, 0.0f, 1.0f,
            1.0f, 1.0f, 0.0f, 1.0f,
            -1.0f, 1.0f, 0.0f, 1.0f,
            1.0f, -1.0f, 0.0f, 1.0f,
        };

        int vertexShaderHandle;
        int fragmentShaderHandle;
        int shaderProgramHandle;
        int vbo;
        int vao;
        float[] pixels;
        Bitmap bitmap;
        bool once;

        public Renderer(int width, int height, bool once = false) : base(width, height)
        {
            this.once = once;
        }

        public Renderer(int width, int height, string fragmentShaderSource, bool once = false) : this(width, height, once)
        {
            this.fragmentShaderSource = fragmentShaderSource;
        }

        protected override void OnLoad(EventArgs e)
        {
            // Load the source of the vertex shader and compile it.
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderSource);
            GL.CompileShader(vertexShaderHandle);

            // Load the source of the fragment shader and compile it.
            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);
            GL.CompileShader(fragmentShaderHandle);

            // Create the shader program, attach the vertex and fragment shaders and link the program.
            shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.LinkProgram(shaderProgramHandle);

            // Create the vertex buffer object (VBO) for the vertex data.
            vbo = GL.GenBuffer();
            // Bind the VBO and copy the vertex data into it.
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Retrive the position location from the program.
            var positionLocation = GL.GetAttribLocation(shaderProgramHandle, "position");

            // Create the vertex array object (VAO) for the program.
            vao = GL.GenVertexArray();
            // Bind the VAO and setup the position attribute.
            GL.BindVertexArray(vao);
            GL.VertexAttribPointer(positionLocation, 4, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(positionLocation);

            // Set the clear color to blue
            GL.ClearColor(0.0f, 0.0f, 1.0f, 0.0f);

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);
            GL.DeleteProgram(shaderProgramHandle);
            GL.DeleteShader(fragmentShaderHandle);
            GL.DeleteShader(vertexShaderHandle);

            base.OnUnload(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Clear the color buffer.
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Bind the VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            // Bind the VAO
            GL.BindVertexArray(vao);

            // Use/Bind the program
            GL.UseProgram(shaderProgramHandle);

            var uSizeLocation = GL.GetUniformLocation(shaderProgramHandle, "uSize");
            // Console.WriteLine($"{Width}x{Height}");
            GL.Uniform2(uSizeLocation, new Vector2(Width, Height));

            // This draws the triangle.
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            pixels = new float[Width * Height * 4];
            GL.ReadPixels(0, 0, Width, Height, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.Float, pixels);
            Console.WriteLine($"# of pixels: {pixels.Length}, {pixels[0]},{pixels[1]},{pixels[2]}");

            bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            GL.ReadPixels(0, 0, Width, Height, OpenTK.Graphics.OpenGL4.PixelFormat.Bgr, PixelType.Byte, bmpData.Scan0);
            bitmap.UnlockBits(bmpData);

            // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window.
            Context.SwapBuffers();

            base.OnRenderFrame(e);

            if (once)
            {
                Close();
            }
        }

        public GLSLMapper.Misc.Buffer GetBuffer()
        {
            return new GLSLMapper.Misc.Buffer(Width, Height, pixels);
        }

        public Bitmap GetBitmap()
        {
            return bitmap;
        }
    }
}
