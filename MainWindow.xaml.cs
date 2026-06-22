using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer idleTimer;
        private DateTime lastActivityAt;
        private readonly TimeSpan idleTimeout;
        private bool isSigningOut;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(new WindowService(this), new DialogService());

            if (DataContext is System.ComponentModel.INotifyPropertyChanged npc)
            {
                npc.PropertyChanged += HandleViewModelPropertyChanged;
            }

            idleTimeout = ResolveIdleTimeout();
            lastActivityAt = DateTime.Now;
            InputManager.Current.PreProcessInput += HandlePreProcessInput;

            idleTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            idleTimer.Tick += HandleIdleTimerTick;
            idleTimer.Start();

            Closed += HandleClosed;
        }

        private void HandleViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CurrentView") return;

            DoubleAnimation fade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(220),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            ContentRegion.BeginAnimation(OpacityProperty, fade);
        }

        private void HandlePreProcessInput(object sender, PreProcessInputEventArgs e)
        {
            if (e.StagingItem == null || e.StagingItem.Input == null) return;
            if (e.StagingItem.Input is MouseEventArgs || e.StagingItem.Input is KeyboardEventArgs)
            {
                lastActivityAt = DateTime.Now;
            }
        }

        private void HandleIdleTimerTick(object sender, EventArgs e)
        {
            if (isSigningOut || idleTimeout <= TimeSpan.Zero) return;
            if (DateTime.Now - lastActivityAt < idleTimeout) return;

            isSigningOut = true;
            idleTimer.Stop();
            MessageBox.Show("Phiên làm việc đã hết hạn do không có thao tác. Vui lòng đăng nhập lại.",
                "Hết phiên", MessageBoxButton.OK, MessageBoxImage.Information);

            if (Application.Current is App app)
            {
                app.SignOut(this);
            }
        }

        private void HandleClosed(object sender, EventArgs e)
        {
            idleTimer.Stop();
            idleTimer.Tick -= HandleIdleTimerTick;
            InputManager.Current.PreProcessInput -= HandlePreProcessInput;
        }

        private static TimeSpan ResolveIdleTimeout()
        {
            string value = Environment.GetEnvironmentVariable("QLXEMAY_IDLE_TIMEOUT_MINUTES");
            int minutes;
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out minutes) && minutes > 0)
            {
                return TimeSpan.FromMinutes(minutes);
            }

            return TimeSpan.FromMinutes(20);
        }
    }
}
