using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLMapper.Misc
{
    public class Buffer
    {
        public readonly int width;
        public readonly int height;
        public readonly float[] pixels;
        public readonly Bitmap bitmap;

        public Buffer(int width, int height, float[] pixels, Bitmap bitmap)
        {
            this.width = width;
            this.height = height;
            this.pixels = pixels;
            this.bitmap = bitmap;
        }

        public override string ToString()
        {
            return $"width:{width}, height:{height}, [0,0]:({pixels[0]},{pixels[1]},{pixels[2]})";
        }
    }
}
