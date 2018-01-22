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
    /// Interaction logic for IndicatorLabel.xaml
    /// </summary>
    public partial class IndicatorLabel : Label
    {
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.RegisterAttached("Unit", typeof(string), typeof(IndicatorLabel));

        public string Unit {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public IndicatorLabel()
        {
            InitializeComponent();
        }
    }
}
