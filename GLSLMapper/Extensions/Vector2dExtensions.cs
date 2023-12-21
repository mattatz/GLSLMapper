using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLMapper.Extensions
{
    public static class Vector2dExtensions
    {
        public static double Dot(this Vector2d src, Vector2d other)
        {
            var x = src.X * other.X;
            var y = src.Y * other.Y;
            return x + y;
        }

        public static Vector2d Perpendicular (this Vector2d src)
        {
            return new Vector2d(
                src.Y,
                -src.X
            );
        }
    }
}
