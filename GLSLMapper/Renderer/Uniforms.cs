using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLMapper.Renderer
{

    public abstract class UniformBase
    {
        public readonly string name;

        protected UniformBase(string name) {
            this.name = name;
        }

        protected int GetLocation (int shaderHandle)
        {
            return GL.GetUniformLocation(shaderHandle, name);
        }

        public abstract void Bind(int shaderHandle);
    }

    public abstract class GenericUniformBase<T> : UniformBase
    {

        public readonly T value;

        public GenericUniformBase(string name, T value): base(name) {
            this.value = value;
        }

        public override string ToString()
        {
            return $"{name}: {value}";
        }
    }

    public class BoolUniform : GenericUniformBase<bool>
    {
        public BoolUniform(string name, bool value) : base(name, value)
        {
        }

        public override void Bind(int shaderHandle)
        {
            var location = GetLocation(shaderHandle);
            GL.Uniform1(location, value ? 1.0 : 0.0);
        }
    }

    public class FloatUniform : GenericUniformBase<float>
    {
        public FloatUniform(string name, float value) : base(name, value)
        {
        }

        public override void Bind(int shaderHandle)
        {
            var location = GetLocation(shaderHandle);
            Rhino.RhinoApp.WriteLine(name + " location: " + location);
            Rhino.RhinoApp.WriteLine(name + " value: " + value);
            GL.Uniform1(location, value);
        }
    }

    public class Vector3dUniform : GenericUniformBase<Rhino.Geometry.Vector3d>
    {
        public Vector3dUniform(string name, Rhino.Geometry.Vector3d value) : base(name, value)
        {
        }

        public override void Bind(int shaderHandle)
        {
            var location = GetLocation(shaderHandle);
            GL.Uniform3(location, new Vector3((float)value.X, (float)value.Y, (float)value.Z));
        }
    }

}
