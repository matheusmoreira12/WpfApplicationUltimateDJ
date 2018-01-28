using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UltimateDJ.Controls
{
    public class CuePointDisplayRenderer : Renderer
    {
        public CuePointDisplayRenderer(UIElement target) : base(target)
        {
        }

        protected override Geometry GetControlShape()
        {
            CuePointDisplay cuePointDisplay = (CuePointDisplay)Target;

            return new RectangleGeometry(new Rect(0, 0, cuePointDisplay.ActualWidth, cuePointDisplay.ActualHeight));
        }

        private PathGeometry getCueArrow(double aw, double x)
        {
            return new PathGeometry(new PathFigure[] { new PathFigure(new Point(x - aw / 2.0, 0), new PathSegment[] {
                new LineSegment(new Point(x + aw / 2.0, 0), false),
                new LineSegment(new Point(x + 0, aw / 1.2), false)
            }, true) });
        }

        private void drawAllCueArrows(DrawingContext dc)
        {
            CuePointDisplay cuePointDisplay = (CuePointDisplay)Target;

            double pw = cuePointDisplay.PointerWidth,
                w = cuePointDisplay.ActualWidth;

            foreach (var cuePoint in cuePointDisplay.CuePoints)
            {
                double x = cuePointDisplay.GetPercentage(cuePoint) / 100 * w;

                dc.DrawGeometry(new SolidColorBrush(cuePoint.Color), new Pen(), getCueArrow(pw, x));
            }
        }

        protected override void RenderBackground(DrawingContext drawingContext) { }

        protected override void RenderBorder(DrawingContext drawingContext) { }

        protected override void RenderContent(DrawingContext drawingContext)
        {
            drawingContext.PushClip(GetControlShape());

            drawAllCueArrows(drawingContext);

            drawingContext.Pop();
        }
    }

    public class CuePoint : DependencyObject
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position",
            typeof(double), typeof(CuePoint));

        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.RegisterAttached("Color",
            typeof(Color), typeof(CuePoint));

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
    }

    public class CuePointCollection : List<CuePoint>, IList<CuePoint>
    {

    }

    public class CuePointDisplay : FrameworkElement
    {
        static CuePointDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CuePointDisplay), new FrameworkPropertyMetadata(typeof(CuePointDisplay)));
        }

        protected CuePointDisplayRenderer Renderer;

        public static readonly DependencyProperty PointerWidthProperty = DependencyProperty.RegisterAttached("PointerWidth",
            typeof(double), typeof(CuePointDisplay), new PropertyMetadata(13.0));

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public double PointerWidth
        {
            get { return (double)GetValue(PointerWidthProperty); }
            set { SetValue(PointerWidthProperty, value); }
        }

        public static readonly DependencyProperty MinProperty = DependencyProperty.RegisterAttached("Min",
            typeof(double), typeof(CuePointDisplay), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty = DependencyProperty.RegisterAttached("Max",
            typeof(double), typeof(CuePointDisplay), new PropertyMetadata(1.0));

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty CuePointsProperty = DependencyProperty.RegisterAttached("CuePoints",
            typeof(CuePointCollection), typeof(CuePointDisplay), new PropertyMetadata(new CuePointCollection()));

        /// <summary>
        /// Gets all the cue points.
        /// </summary>
        public CuePointCollection CuePoints
        {
            get { return (CuePointCollection)GetValue(CuePointsProperty); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Renderer.Render(drawingContext);
        }

        public double GetPercentage(CuePoint cuePoint)
        {
            return (cuePoint.Position - Min) / (Max - Min) * 100;
        }

        public CuePointDisplay()
        {
            Renderer = new CuePointDisplayRenderer(this);
        }
    }
}
