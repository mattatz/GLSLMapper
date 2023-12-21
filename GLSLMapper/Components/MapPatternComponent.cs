using System;
using System.Collections.Generic;
using GLSLMapper.Misc;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GLSLMapper.Components
{
    public class MapPatternComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MapPattern class.
        /// </summary>
        public MapPatternComponent()
          : base("MapPattern", "MapPattern",
              "Map pattern to ",
              "GLSLMapper", "Map")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("geometry", "G", "geometry", GH_ParamAccess.item);
            pManager.AddGenericParameter("pattern", "P", "pattern", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("path", "P", "path", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            PatternMap pattern = null;
            if(DA.GetData(1, ref pattern))
            {
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
            get { return new Guid("ACA985B6-658E-49D4-985E-27C8CC4A1A43"); }
        }
    }
}