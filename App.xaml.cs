using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using QLXeMay.Class;
using QLXeMay.Infrastructure;
using QLXeMay.Services;
using QLXeMay.Windows;

namespace QLXeMay
{
    public partial class App : Application
    {
        private IAuthenticationService authenticationService;
        private bool isSwitchingSession;

        public App()
        {
            DispatcherUnhandledException += HandleDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            TaskScheduler.UnobservedTaskException += HandleUnobservedTaskException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            try
            {
                AppLogger.Info("Application startup.");
                Function.ketnoi();
                Function.RepairVietnameseSeedData();
                authenticationService = new AuthenticationService();
                authenticationService.EnsureSecuritySchema();

                if (!ShowLoginAndMainWindow())
                {
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Application startup failed.", ex);
                MessageBox.Show("Không thể khởi động ứng dụng hoặc kết nối CSDL.\n" + ex.Message,
                    "Lỗi khởi động", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            AppLogger.Info("Application exit.");
            AppSession.SignOut();
            Function.Disconnect();
            base.OnExit(e);
        }

        internal void SignOut(Window currentWindow)
        {
            if (currentWindow == null) return;

            isSwitchingSession = true;
            AppSession.SignOut();
            currentWindow.Close();
            isSwitchingSession = false;

            if (!ShowLoginAndMainWindow())
            {
                Shutdown();
            }
        }

        private bool ShowLoginAndMainWindow()
        {
            LoginWindow loginWindow = new LoginWindow(authenticationService);
            bool? loginResult = loginWindow.ShowDialog();
            if (loginResult != true || !AppSession.IsAuthenticated)
            {
                return false;
            }

            MainWindow mainWindow = new MainWindow();
            MainWindow = mainWindow;
            mainWindow.Closed += HandleMainWindowClosed;
            mainWindow.Show();
            return true;
        }

        private void HandleMainWindowClosed(object sender, EventArgs e)
        {
            if (!isSwitchingSession)
            {
                Shutdown();
            }
        }

        private static void HandleDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            AppLogger.Error("Unhandled UI exception.", e.Exception);
            MessageBox.Show("Ứng dụng gặp lỗi nhưng đã được giữ lại.\n" + e.Exception.Message,
                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            AppLogger.Error("Unhandled application exception.", exception);
        }

        private static void HandleUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            AppLogger.Error("Unhandled background task exception.", e.Exception);
            e.SetObserved();
        }
    }
}
