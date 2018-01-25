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
    public class BarIndicatorRenderer : Renderer
    {
        protected override Geometry GetControlShape()
        {
            throw new NotImplementedException();
        }

        protected override void RenderBackground(DrawingContext drawingContext)
        {
            BarIndicator barIndicator = (BarIndicator)Target;
            double w = barIndicator.ActualWidth, h = barIndicator.ActualHeight;

            drawingContext.DrawRoundedRectangle(barIndicator.Background, new Pen(), new Rect(0, 0, w, h),
                barIndicator.CornerRadiusX, barIndicator.CornerRadiusY);
        }

        protected override void RenderBorder(DrawingContext drawingContext)
        {
            BarIndicator barIndicator = (BarIndicator)Target;
            double w = barIndicator.ActualWidth, h = barIndicator.ActualHeight;

            drawingContext.DrawRoundedRectangle(Brushes.Transparent, new Pen(barIndicator.BorderBrush, barIndicator.BorderThickness), 
                new Rect(0, 0, w, h), barIndicator.CornerRadiusX, barIndicator.CornerRadiusY);
        }

        private void renderVertical(DrawingContext dc)
        {
            BarIndicator barIndicator = (BarIndicator)Target;
            double h = barIndicator.ActualHeight, h1 = h * barIndicator.GetPercentage() / 100.0;

            dc.DrawRoundedRectangle(barIndicator.Foreground, new Pen(Brushes.Transparent, 0), new Rect(0, 
                barIndicator.Direction == IndicatorDirection.Backward ? 0 : (h - h1), barIndicator.ActualWidth, h1), 
                barIndicator.CornerRadiusX, barIndicator.CornerRadiusY);
        }

        private void renderHorizontal(DrawingContext dc)
        {
            BarIndicator barIndicator = (BarIndicator)Target;
            double w = barIndicator.ActualWidth, w1 = w * barIndicator.GetPercentage() / 100.0;

            dc.DrawRoundedRectangle(barIndicator.Foreground, new Pen(Brushes.Transparent, 0), 
                new Rect(barIndicator.Direction == IndicatorDirection.Backward ? (w - w1) : 0, 0, w1, barIndicator.ActualHeight),
                barIndicator.CornerRadiusX, barIndicator.CornerRadiusY);
        }

        protected override void RenderContent(DrawingContext drawingContext)
        {
            BarIndicator barIndicator = (BarIndicator)Target;

            if (barIndicator.Orientation == Orientation.Horizontal)
                renderHorizontal(drawingContext);
            else
                renderVertical(drawingContext);
        }

        public BarIndicatorRenderer(UIElement target) : base(target)
        {
        }
    }

    public class BarIndicator : Indicator
    {
        static BarIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BarIndicator), new FrameworkPropertyMetadata(typeof(BarIndicator)));
        }

        protected BarIndicatorRenderer Renderer;

        #region Layout Properties

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(BarIndicator),
            new PropertyMetadata(Orientation.Horizontal));

        public Orientation Orientation
        {
            set { SetValue(OrientationProperty, value); }
            get { return (Orientation)GetValue(OrientationProperty); }
        }

        #endregion

        #region Visual Properties

        public static readonly DependencyProperty CornerRadiusPropertyX =
            DependencyProperty.RegisterAttached("CornerRadiusX", typeof(double), typeof(BarIndicator),
            new PropertyMetadata(3.0));

        public double CornerRadiusX
        {
            set { SetValue(CornerRadiusPropertyX, value); }
            get { return (double)GetValue(CornerRadiusPropertyX); }
        }

        public static readonly DependencyProperty CornerRadiusPropertyY =
            DependencyProperty.RegisterAttached("CornerRadiusY", typeof(double), typeof(BarIndicator),
            new PropertyMetadata(3.0));

        public double CornerRadiusY
        {
            set { SetValue(CornerRadiusPropertyY, value); }
            get { return (double)GetValue(CornerRadiusPropertyY); }
        }

        #endregion

        #region Rendering

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Renderer.Render(drawingContext);
        }

        #endregion

        public BarIndicator()
        {
            Renderer = new BarIndicatorRenderer(this);
        }
    }
}
