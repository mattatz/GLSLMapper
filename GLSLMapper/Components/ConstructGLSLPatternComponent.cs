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

namespace GLSLMapper.Components
{
    public class ConstructGLSLPatternComponent : GH_Component
    {
        /// <summary>
        /// </summary>
        public ConstructGLSLPatternComponent()
          : base("ConstructGLSLPattern", "ConstructGLSLPattern",
            "Pattern generator by GLSL",
            "GLSLMapper", "ConstructGLSLPattern")
        {
        }

        public override void CreateAttributes()
        {
            m_attributes = new ImagePreview(this);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            var shader = @"
#version 330

out vec4 outputColor;
uniform vec2 uSize;

void main(void)
{
    vec2 center = vec2(0.5, 0.5);
    vec2 xy = gl_FragCoord.xy / uSize;
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

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string shader = null;
            double resolution = 0;
            if (
                DA.GetData(0, ref shader) &&
                DA.GetData(1, ref resolution)
            )
            {
                var size = (int)Math.Min(Math.Max(32, resolution), 4098);
                var renderer = shader.Length > 0 ? new Renderer.Renderer(size, size, shader, true) : new Renderer.Renderer(size, size, true);
                renderer.Run();
                var buffer = renderer.GetBuffer();
                ((ImagePreview)m_attributes).UpdateImage(renderer.GetBitmap());
                DA.SetData(0, new PatternMap(buffer));
            }
        }

        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            base.RemovedFromDocument(document);
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