using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Threading;

using Rhino.DocObjects;
using System.Diagnostics;
using System.Threading.Tasks;
using GLSLMapper.Attributes;
using GLSLMapper.Renderer;
using GLSLMapper.Misc;
using Grasshopper.Kernel.Parameters;
using System.Numerics;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using Rhino;

namespace GLSLMapper.Components
{
    public class ConstructGLSLPatternComponent : GH_Component, IGH_VariableParameterComponent
    {
        const int kDefaultInputCount = 2;

        /// <summary>
        /// </summary>
        public ConstructGLSLPatternComponent()
          : base("ConstructGLSLPattern", "ConstructGLSLPattern",
            "Pattern generator by GLSL",
            "GLSLMapper", "ConstructGLSLPattern")
        {
        }

        /*
        public override void CreateAttributes()
        {
            m_attributes = new ImagePreview(this);
        }
        */

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            var shader = @"
#version 330

out vec4 outputColor;
uniform vec3 iResolution;

void main(void)
{
    vec2 center = vec2(0.5, 0.5);
    vec2 xy = gl_FragCoord.xy / iResolution;
    float d = distance(center, xy);
    d = clamp(d, 0.0, 1.0);
    outputColor = vec4(d, d, d, 1.0);
}";
            pManager.AddTextParameter("glsl", "g", "glsl", GH_ParamAccess.item, shader);
            pManager.AddNumberParameter("resolution", "r", "resolution", GH_ParamAccess.item, 256);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("pattern", "P", "pattern", GH_ParamAccess.item);
        }

        Renderer.Renderer cachedRenderer;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string shader = null;
            double resolution = 0;

            try
            {
                if (
                    DA.GetData(0, ref shader) &&
                    DA.GetData(1, ref resolution)
                )
                {
                    var size = (int)Math.Min(Math.Max(32, resolution), 4098);
                    if (cachedRenderer == null || cachedRenderer.fragmentShaderSource != shader || cachedRenderer.Width != size)
                    {
                        cachedRenderer = shader.Length > 0 ? new Renderer.Renderer(size, size, shader, true) : new Renderer.Renderer(size, size, true);
                    }
                    cachedRenderer.ClearUniform();

                    var uniforms = Params.Input;
                    for (int i = kDefaultInputCount; i < uniforms.Count; i++)
                    {
                        var u = uniforms[i];
                        var data = GetUniformData(DA, u, i);
                        if (data != null)
                        {
                            cachedRenderer.RegisterUniform(data);
                        }
                    }
                    cachedRenderer.Run();
                    var buffer = cachedRenderer.GetBuffer();
                    DA.SetData(0, new PatternMap(buffer));
                }
            } catch (Exception e)
            {
            }
        }

        UniformBase? GetUniformData(IGH_DataAccess DA, IGH_Param param, int index)
        {
            IGH_Goo input = default;
            if (DA.GetData(index, ref input))
            {
                if (input is GH_Boolean)
                {
                    return new BoolUniform(param.NickName, ((GH_Boolean)input).Value);
                } else if (input is GH_Number)
                {
                    return new FloatUniform(param.NickName, (float)((GH_Number)input).Value);
                } else if (input is GH_Vector)
                {
                    return new Vector3dUniform(param.NickName, ((GH_Vector)input).Value);
                }
            }

            return null;
        }

        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            base.RemovedFromDocument(document);
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            switch (side)
            {
                case GH_ParameterSide.Input:
                    {
                        return index >= kDefaultInputCount;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            switch (side)
            {
                case GH_ParameterSide.Input:
                    {
                        return index >= kDefaultInputCount;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            var param = new Param_GenericObject();
            RenameVariableParameter(param, index);
            param.Optional = false;
            return param;
        }

        protected void RenameVariableParameter (IGH_Param param, int index)
        {
            param.Name = param.NickName = "iData" + (index - kDefaultInputCount);
            param.Description = $"uniform value named `{param.Name}`";
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
            var input = Params.Input;
            for (int i = kDefaultInputCount; i < input.Count; i++)
            {
                RenameVariableParameter(input[i], i);
            }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("b1067f85-089e-4b4c-aba7-fabd76a921bd");
    }
}