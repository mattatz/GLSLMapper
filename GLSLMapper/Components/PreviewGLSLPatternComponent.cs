using System;
using System.Collections.Generic;
using GLSLMapper.Attributes;
using GLSLMapper.Misc;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GLSLMapper.Components
{
    public class PreviewGLSLPatternComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PreviewGLSLPatternComponent()
          : base("PreviewGLSLPattern", "PreviewGLSLPattern",
              "Preview GLSL pattern",
              "GLSLMapper", "PreviewGLSLPattern")
        {
        }

        public override void CreateAttributes()
        {
            m_attributes = new ImagePreview(this);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("glsl pattern", "p", "pattern", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            PatternMap pattern = null;
            if (
                DA.GetData(0, ref pattern)
            )
            {
                ((ImagePreview)m_attributes).UpdateImage(pattern.buffer.bitmap);
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("160C1835-747B-4617-B87C-BFD999F86CB7"); }
        }
    }
}