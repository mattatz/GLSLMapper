using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLMapper.Misc
{
    public class PatternMap
    {
        private Buffer buffer;

        public PatternMap(Buffer buffer) {
            this.buffer = buffer;
        }

        public float Sample(float u, float v)
        {
            int x = (int)(u * (float)buffer.width);
            int y = (int)(v * (float)buffer.height);
            x = Math.Max(0, Math.Min(x, buffer.width - 1));
            y = Math.Max(0, Math.Min(y, buffer.height - 1));
            var pix = buffer.pixels;
            return pix[x * y];
        }

        public double Sample(double u, double v)
        {
            return (double)Sample((float)u, (float)v);
        }
    }
}
