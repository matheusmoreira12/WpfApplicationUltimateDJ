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

        protected LineGeometry getPointerLine(double cx, double cy, double rx, double ry, double a)
        {
            double ar = a / 180 * Math.PI,
                x1 = cx + Math.Cos(ar) * rx, y1 = cy - Math.Sin(ar) * ry;

            return new LineGeometry(new Point(cx, cy), new Point(x1, y1));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double rx = ActualWidth / 2.0,
                ry = ActualHeight / 2.0,
                pa = -((Value % 2.0) / 2.0 * 360.0);

            drawingContext.DrawGeometry(Foreground, new Pen(Brushes.Transparent, 0), new EllipseGeometry(new Point(rx, ry),
                5.5, 5.5));

            drawingContext.PushOpacity(0.5);
            drawingContext.DrawGeometry(Brushes.Transparent, new Pen(Foreground, 7), getPointerLine(rx, ry, rx, ry, pa + 90));
        }
    }
}
