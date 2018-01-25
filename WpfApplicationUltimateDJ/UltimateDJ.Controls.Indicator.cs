using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    public enum IndicatorDirection
    {
        Forward = 0,
        Backward = 1
    }

    public class Indicator : FrameworkElement
    {
        static Indicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Indicator), new FrameworkPropertyMetadata(typeof(Indicator)));
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(double), typeof(Indicator),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the indicator value.
        /// </summary>
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

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.RegisterAttached("Min", typeof(double), typeof(Indicator),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the indicator minimum value.
        /// </summary>
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

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.RegisterAttached("Max", typeof(double), typeof(Indicator),
                new PropertyMetadata(1.0));

        /// <summary>
        /// Gets or sets the indicator maximum value.
        /// </summary>
        public double Max
        {
            set
            {
                if (value <= Min)
                    value = Min + double.Epsilon;

                SetValue(MaxProperty, value);
            }

            get { return (double)GetValue(MaxProperty); }
        }

        public double GetPercentage()
        {
            return (Value - Min) / (Max - Min) * 100;
        }

        #region Visual Properties

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.RegisterAttached("FlowDirection", typeof(IndicatorDirection), typeof(Indicator),
            new PropertyMetadata(IndicatorDirection.Forward));

        /// <summary>
        /// Gets or sets the indicator direction.
        /// </summary>
        public IndicatorDirection Direction
        {
            set { SetValue(DirectionProperty, value); }
            get { return (IndicatorDirection)GetValue(DirectionProperty); }
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.RegisterAttached("Foreground", typeof(Brush), typeof(Indicator),
                new PropertyMetadata(Brushes.Orange));

        /// <summary>
        /// Gets or sets the indicator foreground brush.
        /// </summary>
        public Brush Foreground
        {
            set { SetValue(ForegroundProperty, value); }
            get { return (Brush)GetValue(ForegroundProperty); }
        }

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.RegisterAttached("Background", typeof(Brush), typeof(Indicator),
                new PropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Gets or sets the indicator background brush.
        /// </summary>
        public Brush Background
        {
            set { SetValue(BackgroundProperty, value); }
            get { return (Brush)GetValue(BackgroundProperty); }
        }

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.RegisterAttached("BorderThickness", typeof(double), typeof(Indicator),
            new PropertyMetadata(2.0));

        /// <summary>
        /// Gets or sets the indicator border thickness.
        /// </summary>
        public double BorderThickness
        {
            set { SetValue(BorderThicknessProperty, value); }
            get { return (double)GetValue(BorderThicknessProperty); }
        }

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.RegisterAttached("BorderBrush", typeof(Brush), typeof(Indicator),
            new PropertyMetadata(Brushes.DimGray));

        /// <summary>
        /// Gets or sets the indicator border brush.
        /// </summary>
        public Brush BorderBrush
        {
            set { SetValue(BorderBrushProperty, value); }
            get { return (Brush)GetValue(BorderBrushProperty); }
        }

        #endregion

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ValueProperty ||
                e.Property == MinProperty ||
                e.Property == MaxProperty ||
                e.Property == BorderThicknessProperty ||
                e.Property == BorderBrushProperty)
                InvalidateVisual();
        }
    }
}
