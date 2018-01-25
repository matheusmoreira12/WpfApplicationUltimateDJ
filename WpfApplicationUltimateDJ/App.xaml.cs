using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace WpfApplicationUltimateDJ
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        UltimateDJ.Windows.SplashScreen SplashWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SplashWindow.Show();
        }

        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);

            SplashWindow.Hide();
            MainWindow.Show();
        }

        public App()
        {
            SplashWindow = new UltimateDJ.Windows.SplashScreen();
        }
    }
}
