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
    public class VinylTimeIndicator : RadialIndicator
    {
        static VinylTimeIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VinylTimeIndicator), new FrameworkPropertyMetadata(typeof(VinylTimeIndicator)));
        }

        protected LineGeometry getPointerLine(double rx, double ry, double angle)
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
                pointerAngle = -(Value % 2) / 2 * 360.0;

            drawingContext.DrawGeometry(Foreground, new Pen(Brushes.Transparent, 0), new EllipseGeometry(new Point(rx, ry),
                5.5, 5.5));
            drawingContext.DrawGeometry(Foreground, new Pen(Brushes.Black, 7), getPointerLine(rx, ry, pointerAngle + 90));
        }
    }
}
