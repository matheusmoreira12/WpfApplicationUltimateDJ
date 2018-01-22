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
    public class VinylIndicator : Control
    {
        public double Value
        {
            set
            {
                if (value > _max)
                    _value = _max;
                else if (value < _min)
                    _value = _min;
                else
                    _value = value;
            }
            get { return _value; }
        }
        private double _value = 0;

        public double Min
        {
            set
            {
                if (value >= _max)
                    _min = _max - 1;
                else
                    _min = value;
            }

            get { return _min; }
        }
        private double _min = 0;

        public double Max
        {
            set
            {
                if (value <= _min)
                    _max = _min + 1;
                else
                    _max = value;
            }

            get { return _min; }
        }
        private double _max = 1;

        public double SecondsPerTurn = 2;

        public double GetPercentage()
        {
            return (Value - Min) / (Max - Min) * 100;
        }

        private PathGeometry getIndicatorPie(double radiusX, double radiusY, double angle, double sweep)
        {
            double x0 = radiusX,
                y0 = radiusY,
                x1 = x0 + Math.Cos(angle) * radiusX,
                y1 = y0 - Math.Sin(angle) * radiusY,
                x2 = x0 + Math.Cos(sweep) * radiusX,
                y2 = y0 - Math.Sin(sweep) * radiusY;

            PathGeometry geometry = new PathGeometry();

            PathFigure pf = new PathFigure();
            pf.StartPoint = new Point(x0, y0);
            pf.Segments.Add(new LineSegment(new Point(x1, y1), false));
            pf.Segments.Add(new ArcSegment(new Point(x2, y2), new Size(radiusX * 2, radiusY * 2), 0, false, SweepDirection.Clockwise, false));
            geometry.Figures.Add(pf);

            return geometry;
        }

        private LineGeometry getPointerLine(double radiusX, double radiusY, double angle)
        {
            double x0 = radiusX,
                y0 = radiusY,
                x1 = x0 + Math.Cos(angle),
                y1 = y0 - Math.Sin(angle);

            return new LineGeometry(new Point(x0, y0), new Point(x1, y1));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double rx = Width / 2,
                ry = Height / 2,
                val = GetPercentage() / 100,
                ptr = val % SecondsPerTurn;

            /*drawingContext.DrawGeometry(Brushes.Transparent, new Pen(Brushes.Black, 7), getIndicatorPie(rx, ry, 0,
                 2 * Math.PI * val));
            drawingContext.DrawGeometry(Brushes.Black, new Pen(Brushes.Transparent, 0), getPointerLine(rx, ry, ptr));*/

            drawingContext.DrawRectangle(Brushes.Black, new Pen(), new Rect(0, 0, Width, Height));
        }

        static VinylIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VinylIndicator), new FrameworkPropertyMetadata(typeof(VinylIndicator)));
        }
    }
}
