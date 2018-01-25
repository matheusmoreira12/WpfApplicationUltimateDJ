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

namespace WpfApplicationUltimateDJ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Windows.Forms.Timer Timer1;

        public void Timer1_OnTick(object sender, EventArgs args)
        {
            TurntableVynil1.Value+=.01;
        }

        public MainWindow()
        {
            Timer1 = new System.Windows.Forms.Timer();
            Timer1.Interval = 10;
            Timer1.Tick += Timer1_OnTick;
            Timer1.Enabled = true;

            InitializeComponent();
        }
    }
}
