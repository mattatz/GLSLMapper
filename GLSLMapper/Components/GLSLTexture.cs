using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Threading;

using Rhino.DocObjects;
using System.Diagnostics;
using System.Threading.Tasks;
using GLSLMapper.Attributes;
using GLSLMapper.Renderer;

namespace GLSLMapper.Components
{
    public class GLSLTexture : GH_Component
    {
        /// <summary>
        /// </summary>
        public GLSLTexture()
          : base("GLSLTexture", "GLSLTexture",
            "Texture generator by GLSL",
            "GLSLMapper", "GLSLTexture")
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
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("test", "O", "test", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override async void SolveInstance(IGH_DataAccess DA)
        {
            /*
            var t = new Task(() =>
            {
                Rhino.RhinoApp.WriteLine("init renderer");
                try
                {
                    // var renderer = new Renderer.Renderer(512, 512);
                    // var buffer = renderer.GetBuffer();
                    // Rhino.RhinoApp.WriteLine("buffer: " + buffer.pixels.Length);
                } catch (Exception e)
                {
                    Rhino.RhinoApp.WriteLine(e.ToString());
                }
            });
            t.Start(TaskScheduler.FromCurrentSynchronizationContext());
            */

            /*
            var pixels = await Task.Run(() =>
            {
                // var api = GraphicsAPI.Default;
                // api.Version = new APIVersion(4, 6);
                var renderer = new Renderer.Renderer(512, 512);
                var buffer = renderer.GetBuffer();
                return buffer != null ? buffer.pixels : null;
            });
            */

            var size = 256;
            var renderer = new Renderer.Renderer(size, size, true);
            renderer.Run();
            var buffer = renderer.GetBuffer();
            // Rhino.RhinoApp.WriteLine("buffer: " + buffer.ToString());

            ((ImagePreview)m_attributes).UpdateImage(renderer.GetBitmap());

            DA.SetDataList(0, buffer.pixels);
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