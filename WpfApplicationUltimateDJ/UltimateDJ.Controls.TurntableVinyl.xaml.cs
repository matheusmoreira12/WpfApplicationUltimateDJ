using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for TurntableVinyl.xaml
    /// </summary>
    public partial class TurntableVinyl : UserControl
    {
        static readonly string DECIMAL_SEPARATOR = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(double), typeof(TurntableVinyl));

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.RegisterAttached("Min", typeof(double), typeof(TurntableVinyl));

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.RegisterAttached("Max", typeof(double), typeof(TurntableVinyl));

        public static readonly DependencyProperty BPMProperty =
            DependencyProperty.RegisterAttached("BPM", typeof(double), typeof(TurntableVinyl));

        public static readonly DependencyProperty PitchShiftProperty =
            DependencyProperty.RegisterAttached("PitchShift", typeof(double), typeof(TurntableVinyl));

        #region Update Field Methods
        private void updateValue()
        {
            TimeSpan elapsed = TimeSpan.FromSeconds(Value - Min),
                duration = TimeSpan.FromSeconds(Max - Min),
                remaining = duration - elapsed;

            ElapsedLabel.Content = string.Format($"{{0:mm':'ss'{ DECIMAL_SEPARATOR }'f}}", elapsed);
            RemainingLabel.Content = string.Format($"-{{0:mm':'ss'{ DECIMAL_SEPARATOR }'f}}", remaining);
        }

        private void updateBPM()
        {
            BPMLabel.Content = string.Format("{0:0.0}", BPM);
        }

        private void updatePitchShift()
        {
            string signStr = PitchShift > 0 ? "+" : PitchShift < 0 ? "-" : "";

            PitchShiftLabel.Content = signStr + string.Format("{0:0.0}", Math.Abs(PitchShift));
        }
        #endregion

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ValueProperty ||
                e.Property == MinProperty ||
                e.Property == MaxProperty)
                updateValue();
            else if (e.Property == BPMProperty)
                updateBPM();
            else if (e.Property == PitchShiftProperty)
                updatePitchShift();
        }

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

        public double GetPercentage()
        {
            return (Value - Min) / (Max - Min) * 100;
        }

        public double BPM
        {
            get { return (double)GetValue(BPMProperty); }
            set { SetValue(BPMProperty, value); }
        }

        public double PitchShift
        {
            get { return (double)GetValue(PitchShiftProperty); }
            set
            {

                SetValue(PitchShiftProperty, value);
            }
        }

        public TurntableVinyl()
        {
            InitializeComponent();
        }
    }
}