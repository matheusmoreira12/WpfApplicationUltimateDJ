using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace UltimateDJ.Controls
{
    public abstract class Renderer
    {
        protected UIElement Target = null;

        protected abstract void RenderBackground(DrawingContext drawingContext);
        protected abstract void RenderContent(DrawingContext drawingContext);
        protected abstract void RenderBorder(DrawingContext drawingContext);

        public void Render(DrawingContext drawingContext)
        {
            RenderBackground(drawingContext);
            RenderContent(drawingContext);
            RenderBorder(drawingContext);
        }

        protected abstract Geometry GetControlShape();

        public Renderer(UIElement target)
        {
            Target = target;
        }
    }
}
