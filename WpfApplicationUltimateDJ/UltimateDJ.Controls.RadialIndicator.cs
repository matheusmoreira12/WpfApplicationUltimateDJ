using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class RadialIndicatorRenderer : Renderer
    {
        protected override Geometry GetControlShape()
        {
            RadialIndicator radialIndicator = (RadialIndicator)Target;

            double w = radialIndicator.ActualWidth, h = radialIndicator.ActualHeight,
                cx = w / 2, cy = h / 2,
                rx = w / 2, ry = h / 2;

            EllipseGeometry eg0 = new EllipseGeometry(new Point(cx, cy), rx, ry);
            EllipseGeometry eg1 = new EllipseGeometry(new Point(cx, cy),
                rx - radialIndicator.RingThickness, ry - radialIndicator.RingThickness);

            return new CombinedGeometry(GeometryCombineMode.Exclude, eg0, eg1);
        }

        protected override void RenderBackground(DrawingContext drawingContext)
        {
            RadialIndicator radialIndicator = (RadialIndicator)Target;

            drawingContext.DrawGeometry(radialIndicator.Background, new Pen(), GetControlShape());
        }

        protected override void RenderBorder(DrawingContext drawingContext)
        {
            RadialIndicator radialIndicator = (RadialIndicator)Target;

            drawingContext.DrawGeometry(Brushes.Transparent, new Pen(radialIndicator.BorderBrush, radialIndicator.BorderThickness), 
                GetControlShape());
        }

        private PathGeometry getIndicatorPie(double cx, double cy, double rx, double ry, double a, double s)
        {
            double ar = a / 180.0 * Math.PI, sr = s / 180.0 * Math.PI,
                x0 = cx + Math.Cos(ar) * rx, y0 = cy - Math.Sin(ar) * ry,
                x1 = cx + Math.Cos(ar + sr) * rx, y1 = cy - Math.Sin(ar + sr) * ry;

            PathGeometry pg = new PathGeometry();
            pg.FillRule = FillRule.EvenOdd;

            PathFigure pf = new PathFigure(new Point(cx, cy), new PathSegment[] {
                new LineSegment(new Point(x0, y0), false),
                new ArcSegment(new Point(x1, y1), new Size(rx, ry), 0, Math.Abs(s) > 180,
                s <= 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise, false)
            }, true);

            pg.Figures.Add(pf);

            return pg;
        }

        protected override void RenderContent(DrawingContext drawingContext)
        {
            RadialIndicator radialIndicator = (RadialIndicator)Target;

            double w = radialIndicator.ActualWidth, h = radialIndicator.ActualHeight,
                cx = w / 2, cy = h / 2,
                rx = w / 2, ry = h / 2,
                val = radialIndicator.GetPercentage() / 100;

            Geometry ring = GetControlShape(),
                pie = getIndicatorPie(cx, cy, rx, ry, 90,
                (radialIndicator.Direction == IndicatorDirection.Backward ? val : -val * 360.0));

            drawingContext.DrawGeometry(radialIndicator.Foreground, new Pen(), 
                new CombinedGeometry(GeometryCombineMode.Intersect, ring, pie));
        }

        public RadialIndicatorRenderer(RadialIndicator target) : base(target)
        {
        }
    }

    /// <summary>
    /// Interaction logic for UltimateDJ.xaml
    /// </summary>
    public partial class RadialIndicator : Indicator
    {
        static RadialIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialIndicator), new FrameworkPropertyMetadata(typeof(RadialIndicator)));
        }

        protected RadialIndicatorRenderer Renderer;

        #region Visual Properties

        public static readonly DependencyProperty RingThicknessProperty =
            DependencyProperty.RegisterAttached("RingThickness", typeof(double), typeof(RadialIndicator),
            new PropertyMetadata(5.0));

        /// <summary>
        /// Gets or sets the indicator ring thickness.
        /// </summary>
        public double RingThickness
        {
            set { SetValue(RingThicknessProperty, value); }
            get { return (double)GetValue(RingThicknessProperty); }
        }

        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double rx = ActualWidth / 2.0, ry = ActualHeight / 2.0,
                cx = rx, cy = ry;

            Renderer.Render(drawingContext);
        }

        public RadialIndicator()
        {
            Renderer = new RadialIndicatorRenderer(this);
        }
    }
}