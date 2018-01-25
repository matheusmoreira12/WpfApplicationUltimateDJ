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
    /// Interaction logic for UltimateDJ.Controls.ReportBugMessage.xaml
    /// </summary>
    public partial class ReportBugMessageLabel : Label
    {
        public static readonly DependencyProperty ReportUriProperty = DependencyProperty.RegisterAttached("ReportUri", typeof(string),
            typeof(SplashScreen), new PropertyMetadata("http://www.google.com"));

        public string ReportUri
        {
            get { return (string)GetValue(ReportUriProperty); }
            set { SetValue(ReportUriProperty, value); }
        }

        public ReportBugMessageLabel()
        {
            InitializeComponent();
        }
    }
}
