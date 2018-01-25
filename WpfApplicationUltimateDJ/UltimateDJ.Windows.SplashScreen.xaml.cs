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
using System.Windows.Threading;

namespace UltimateDJ.Windows
{
    public sealed class DelayTimer : IDisposable
    {
        private DispatcherTimer InternalTimer;

        public EventHandler Done;
        public void OnDone(EventArgs args)
        {
            Done?.Invoke(this, args);
        }

        private void internalTimerTick(object sender, EventArgs args)
        {
            InternalTimer.Stop();

            OnDone(new EventArgs());
        }

        public void Start() { InternalTimer.Start(); }
        public void Stop() { InternalTimer.Stop(); }

        public void Dispose()
        {
            Stop();
        }

        public DelayTimer(TimeSpan delay)
        {
            InternalTimer = new DispatcherTimer { Interval = delay };
            InternalTimer.Tick += internalTimerTick;
            Start();
        }
    }

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private DelayTimer TooLongDelayTimer;

        private void TooLongDelayTimer_onDone(object sender, EventArgs args)
        {
            TooLongMessageLabel.Visibility = Visibility.Visible;
        }

        public SplashScreen()
        {
            InitializeComponent();

            TooLongDelayTimer = new DelayTimer(TimeSpan.FromSeconds(5));
            TooLongDelayTimer.Done += TooLongDelayTimer_onDone;
            TooLongDelayTimer.Start();
        }
    }
}
