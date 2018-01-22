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
    public partial class RadialIndicator : FrameworkElement
    {
        static RadialIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialIndicator), new FrameworkPropertyMetadata(typeof(RadialIndicator)));
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(double), typeof(RadialIndicator));

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.RegisterAttached("Min", typeof(double), typeof(RadialIndicator));

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.RegisterAttached("Max", typeof(double), typeof(RadialIndicator));

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.RegisterAttached("Foreground", typeof(Brush), typeof(RadialIndicator), 
                new PropertyMetadata(Brushes.Black));

        public double Value
        {
            set
            {
                if (value > Max)
                    value = Max - double.Epsilon;
                else if (value < Min)
                    value = Min + double.Epsilon;

                SetValue(ValueProperty, value);
            }
            get { return (double)GetValue(ValueProperty); }
        }

        public double Min
        {
            set
            {
                if (value >= Max)
                    value = Max - double.Epsilon;

                SetValue(MinProperty, value);
            }

            get { return (double)GetValue(MinProperty); }
        }

        public double Max
        {
            set
            {
                if (value <= Min)
                    value = Min - double.Epsilon;

                SetValue(MaxProperty, value);
            }

            get { return (double)GetValue(MaxProperty); }
        }

        public Brush Foreground
        {
            set { SetValue(ForegroundProperty, value); }
            get { return (Brush)GetValue(ForegroundProperty); }
        }

        public double GetPercentage()
        {
            return (Value - Min) / (Max - Min) * 100;
        }

        protected PathGeometry getIndicatorPie(double cx, double cy, double rx, double ry, double a, double s)
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

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double rx = ActualWidth / 2.0,
                ry = ActualHeight / 2.0,
                valueAngle = -(Value - Min) / (Max - Min) * 360.0;

            drawingContext.DrawGeometry(Brushes.Transparent, new Pen(Foreground, 11), getIndicatorPie(rx, ry,
                rx - 5.5, ry - 5.5, 90, valueAngle));
        }
    }
}