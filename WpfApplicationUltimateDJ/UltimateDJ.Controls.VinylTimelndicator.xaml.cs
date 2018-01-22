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
    /// <summary>
    /// Interaction logic for UltimateDJ.xaml
    /// </summary>
    public partial class VinylTimeIndicator : FrameworkElement
    {
        static VinylTimeIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VinylTimeIndicator), new FrameworkPropertyMetadata(typeof(VinylTimeIndicator)));
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(double), typeof(VinylTimeIndicator));

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.RegisterAttached("Min", typeof(double), typeof(VinylTimeIndicator));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.RegisterAttached("Max", typeof(double), typeof(VinylTimeIndicator));

        private PathGeometry getIndicatorPie(double cx, double cy, double rx, double ry, double a, double s)
        {
            double ar = a / 180.0 * Math.PI, sr = s / 180.0 * Math.PI,
                x0 = cx + Math.Cos(ar) * rx, y0 = cy - Math.Sin(ar) * ry,
                x1 = cx + Math.Cos(ar + sr) * rx, y1 = cy - Math.Sin(ar + sr) * ry;

            PathGeometry geometry = new PathGeometry();
            geometry.FillRule = FillRule.EvenOdd;

            PathFigure pf = new PathFigure();
            pf.StartPoint = new Point(x0, y0);
            pf.Segments.Add(new ArcSegment(new Point(x1, y1), new Size(rx, ry), 0, Math.Abs(s) > 180, 
                s <= 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise, true));
            geometry.Figures.Add(pf);

            return geometry;
        }

        private LineGeometry getPointerLine(double rx, double ry, double angle)
        {
            double x0 = rx,
                y0 = ry,
                x1 = x0 + Math.Cos(angle) * rx,
                y1 = y0 - Math.Sin(angle) * ry;

            return new LineGeometry(new Point(x0, y0), new Point(x1, y1));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double rx = ActualWidth / 2.0,
                ry = ActualHeight / 2.0,
                valueAngle = -(Value - Min) / (Max - Min) * 360.0,
                pointerAngle = -(Value % 2) / 2 * 360.0,
                pitchAngle = -5.0 / 100.0 * 360.0;

            drawingContext.DrawGeometry(Brushes.Black, new Pen(Brushes.Transparent, 0), new EllipseGeometry(new Point(rx, ry), 
                5.5, 5.5));
            drawingContext.DrawGeometry(Brushes.Transparent, new Pen(Brushes.Black, 7), getPointerLine(rx, ry, pointerAngle + 90));
            drawingContext.DrawGeometry(Brushes.Transparent, new Pen(Brushes.Black, 11), getIndicatorPie(rx, ry, 
                rx - 5.5, ry - 5.5, 90, valueAngle));
            drawingContext.DrawGeometry(Brushes.Transparent, new Pen(Brushes.Firebrick, 11), getIndicatorPie(rx, ry, 
                rx - 16.5, ry - 16.5, 90, pitchAngle));
        }

        public VinylTimeIndicator()
        {
            InitializeComponent();
        }
    }
}
