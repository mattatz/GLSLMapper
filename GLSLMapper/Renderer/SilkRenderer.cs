/*
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Diagnostics;
using System.Linq;

namespace MyGrasshopperAssembly1.Renderer
{
    public class SilkRenderer
    {
        //Vertex shaders are run on each vertex.
        private readonly string vertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec3 position;
        
        void main()
        {
            gl_Position = vec4(position.x, position.y, position.z, 1.0);
        }
        ";

        private readonly string fragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }
        ";

        // Vertex data, uploaded to the VBO.
        private readonly float[] vertices =
        {
            //X    Y      Z
             1.0f,  1.0f, 0.0f,
             1.0f, -1.0f, 0.0f,
            -1.0f, -1.0f, 0.0f,
            -1.0f,  1.0f, 0.0f
        };

        // Index data, uploaded to the EBO.
        private readonly uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private IWindow window;
        private GL gl;
        private uint vbo;
        private uint ebo;
        private uint vao;
        private uint shader;
        private float[] pixels;

        public SilkRenderer(int width, int height, GraphicsAPI api)
        {
            var options = WindowOptions.Default;
            options.API = api;
            options.IsVisible = false;
            options.Size = new Vector2D<int>(width, height);

            // var platform = Window.Platforms.First();
            // var platform = Window.GetWindowPlatform(false);
            // window = platform?.CreateWindow(options);
            window = Window.Create(options);

            window.Load += OnLoad;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.Closing += OnClose;

            window.Initialize();
            window.DoRender();

            window.Dispose();
        }

        public SilkRenderer(int width, int height) : this(width, height, GraphicsAPI.Default)
        {
        }

        unsafe float[] LoadFramebuffer()
        {
            var size = window.Size;
            float[] pixels = new float[size.X * size.Y * 4];
            fixed (void* buffer = pixels)
            {
                gl?.ReadPixels(0, 0, (uint)size.X, (uint)size.Y,
                    GLEnum.Rgba,
                    // GLEnum.UnsignedByte,
                    GLEnum.Float,
                    buffer
                );
            }
            Debug.WriteLine($"{pixels[0]}, {pixels[1]}, {pixels[2]}");

            return pixels;
        }

        unsafe void LoadScene()
        {
            //Getting the opengl api for drawing to the screen.
            gl = GL.GetApi(window);

            //Creating a vertex array.
            vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            //Initializing a vertex buffer that holds the vertex data.
            vbo = gl.GenBuffer(); //Creating the buffer.
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo); //Binding the buffer.
            fixed (void* v = &vertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
            }

            //Initializing a element buffer that holds the index data.
            ebo = gl.GenBuffer(); //Creating the buffer.
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo); //Binding the buffer.
            fixed (void* i = &indices[0])
            {
                gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
            }

            //Creating a vertex shader.
            uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertexShader, vertexShaderSource);
            gl.CompileShader(vertexShader);

            //Checking the shader for compilation errors.
            string infoLog = gl.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling vertex shader {infoLog}");
            }

            //Creating a fragment shader.
            uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fragmentShader, fragmentShaderSource);
            gl.CompileShader(fragmentShader);

            //Checking the shader for compilation errors.
            infoLog = gl.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling fragment shader {infoLog}");
            }

            //Combining the shaders under one shader program.
            shader = gl.CreateProgram();
            gl.AttachShader(shader, vertexShader);
            gl.AttachShader(shader, fragmentShader);
            gl.LinkProgram(shader);

            //Checking the linking for errors.
            gl.GetProgram(shader, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {gl.GetProgramInfoLog(shader)}");
            }

            //Delete the no longer useful individual shaders;
            gl.DetachShader(shader, vertexShader);
            gl.DetachShader(shader, fragmentShader);
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            //Tell opengl how to give the data to the shaders.
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            gl.EnableVertexAttribArray(0);
        }

        private unsafe void OnLoad()
        {
            LoadScene();
        }

        private unsafe void OnRender(double obj) //Method needs to be unsafe due to draw elements.
        {
            // Clear the color channel.
            gl?.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            //Bind the geometry and shader.
            gl?.BindVertexArray(vao);
            gl?.UseProgram(shader);

            //Draw the geometry.
            gl?.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);

            pixels = LoadFramebuffer();
        }

        private void OnUpdate(double obj)
        {
        }

        private void OnClose()
        {
            Console.WriteLine("Close");
            //Remember to delete the buffers.
            gl.DeleteBuffer(vbo);
            gl.DeleteBuffer(ebo);
            gl.DeleteVertexArray(vao);
            gl.DeleteProgram(shader);
        }


    }
}
*/
