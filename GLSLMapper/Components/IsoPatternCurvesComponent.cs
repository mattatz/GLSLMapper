using System;
using System.Collections.Generic;
using System.Linq;
using GLSLMapper.Extensions;
using GLSLMapper.Misc;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace GLSLMapper.Components
{
    public class IsoPatternCurvesComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IsoPatternCurvesComponent class.
        /// </summary>
        public IsoPatternCurvesComponent()
          : base("IsoPatternCurves", "IsoPatternCurves",
              "Iso curves by pattern map",
              "GLSLMapper", "IsoPatternCurves")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("pattern", "P", "pattern", GH_ParamAccess.item);
            pManager.AddNumberParameter("resolution", "R", "resolution", GH_ParamAccess.item, 0.05);
            pManager.AddNumberParameter("step", "S", "step", GH_ParamAccess.item, 0.05);
            var direction = new Vector3d(1, 0, 0);
            pManager.AddVectorParameter("uv direction", "V", "uv direction", GH_ParamAccess.item, direction);
            var scale = new Vector3d(1, 1, 1);
            pManager.AddVectorParameter("scale", "S", "scale", GH_ParamAccess.item, scale);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("c", "C", "c", GH_ParamAccess.list);
            pManager.AddPointParameter("p", "P", "p", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            PatternMap pattern = null;
            double resolution = 0.05;
            double step = 0.05;
            Vector3d direction = default;
            Vector3d scale = default;
            if(
                DA.GetData(0, ref pattern) &&
                DA.GetData(1, ref resolution) &&
                DA.GetData(2, ref step) && 
                DA.GetData(3, ref direction) &&
                DA.GetData(4, ref scale)
            )
            {
                var isoCurves = new List<Curve>();
                var intersectionPoints = new List<Point3d>();

                var uv = new Vector2d(direction.X, direction.Y);
                uv.Unitize();
                var points = new List<Vector2d>(){
                    new Vector2d(0, 0),
                    new Vector2d(1, 0),
                    new Vector2d(1, 1),
                    new Vector2d(0, 1),
                    new Vector2d(0, 0),
                };
                var projected = points.Select((p) =>
                {
                    return uv.Dot(p);
                });
                var min = projected.Min();
                var max = projected.Max();
                var start = uv * min;
                var perp = uv.Perpendicular();
                var t = 0.0;

                var rectangle = new PolylineCurve(points.Select((p) =>
                {
                    return new Point3d(p.X, p.Y, 0);
                }));

                double epsilon = 1e-12;
                while (t < max)
                {
                    var p = start + uv * t;
                    var pt0 = p - perp * 1e5;
                    var pt1 = p + perp * 1e5;
                    var line = new Line(new Point3d(pt0.X, pt0.Y, 0), new Point3d(pt1.X, pt1.Y, 0));
                    var intersections = Intersection.CurveLine(rectangle, line, epsilon, epsilon);

                    foreach(var i in intersections)
                    {
                        intersectionPoints.Add(i.PointA);
                    }

                    var count = intersections.Count();
                    if (count == 2)
                    {
                        var a = intersections[0].PointA;
                        var b = intersections[1].PointA;

                        var dir = b - a;
                        var n = (int)Math.Floor(dir.Length / resolution);
                        var input = new List<Point3d>();

                        var tangent = dir;
                        tangent.Unitize();
                        for (int i = 0; i < n; i++)
                        {
                            var coord = a + tangent * resolution;
                            var height = pattern.Sample(coord.X, coord.Y);
                            input.Add(new Point3d(coord.X * scale.X, coord.Y * scale.Y, height * scale.X));
                        }
                        var h = pattern.Sample(b.X, b.Y);
                        input.Add(new Point3d(b.X * scale.X, b.Y * scale.Y, h * scale.X));
                        var curve = new PolylineCurve(input);
                        isoCurves.Add(curve);
                    }

                    t += step;
                }

                DA.SetDataList(0, isoCurves);
                DA.SetDataList(1, intersectionPoints);
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
            get { return new Guid("E0057726-C597-48FF-9693-AC0FCC5678CA"); }
        }
    }
}