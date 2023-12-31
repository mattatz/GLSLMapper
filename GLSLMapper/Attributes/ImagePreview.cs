﻿using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using OpenTK.Graphics.OpenGL;
using Rhino;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLMapper.Attributes
{
    public class ImagePreview : GH_ComponentAttributes
    {
        private Bitmap bitmap = new Bitmap(128, 128);
        private Pen pen;

        public ImagePreview(IGH_Component component) : base(component)
        {
            pen = new Pen(Color.Black, 2);
        }

        public void UpdateImage (Bitmap next)
        {
            bitmap = next;
        }

        protected override void Layout()
        {
            Pivot = GH_Convert.ToPoint(Pivot);
            m_innerBounds = LayoutComponentBox(base.Owner);

            var size = 128;
            var gripOffset = 24;
            m_innerBounds.Size = new Size(size - gripOffset, size);
            LayoutInputParams(base.Owner, m_innerBounds);
            LayoutOutputParams(base.Owner, m_innerBounds);
            Bounds = LayoutBounds(base.Owner, m_innerBounds);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Objects)
            {
                RenderComponentCapsule(canvas, graphics, drawComponentBaseBox: true, drawComponentNameBox: false, drawJaggedEdges: true, drawParameterGrips: true, drawParameterNames: false, drawZuiElements: true);

                var offset = 0;
                var box = Bounds;
                graphics.DrawRectangle(pen, Rectangle.Round(box));
                var r = new RectangleF(box.X + offset, box.Y + offset, box.Width - offset * 2, box.Height - offset * 2);
                graphics.DrawImage(bitmap, r);
            } else
            {
                base.Render(canvas, graphics, channel);
            }
        }
    }

}
