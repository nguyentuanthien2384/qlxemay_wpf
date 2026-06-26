using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class LoginWindow : Window
    {
        private readonly IAuthenticationService authenticationService;
        private readonly LoginWindowViewModel viewModel;
        private bool isPasswordVisible;

        internal LoginWindow(IAuthenticationService authenticationService)
        {
            InitializeComponent();
            this.authenticationService = authenticationService;
            viewModel = new LoginWindowViewModel(authenticationService, CloseWithResult, ShowChangePassword, ShowRegisterWindow);
            DataContext = viewModel;
            Loaded += HandleLoaded;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            UserNameTextBox.Focus();
            UpdateCapsLockWarning();
        }

        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!isPasswordVisible)
            {
                viewModel.Password = PasswordInput.Password;
                VisiblePasswordBox.Text = PasswordInput.Password;
            }
            UpdateCapsLockWarning();
        }

        private void VisiblePasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isPasswordVisible)
            {
                viewModel.Password = VisiblePasswordBox.Text;
                PasswordInput.Password = VisiblePasswordBox.Text;
            }
        }

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;
            PasswordInput.Visibility = isPasswordVisible ? Visibility.Collapsed : Visibility.Visible;
            VisiblePasswordBox.Visibility = isPasswordVisible ? Visibility.Visible : Visibility.Collapsed;
            TogglePasswordButton.Content = isPasswordVisible ? "Ẩn mật khẩu" : "Hiện mật khẩu";
            if (isPasswordVisible)
            {
                VisiblePasswordBox.Focus();
                VisiblePasswordBox.CaretIndex = VisiblePasswordBox.Text.Length;
            }
            else
            {
                PasswordInput.Focus();
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateCapsLockWarning();
        }

        private void UpdateCapsLockWarning()
        {
            CapsLockWarning.Visibility = Keyboard.IsKeyToggled(Key.CapsLock) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CloseWithResult(bool result)
        {
            DialogResult = result;
            Close();
        }

        private bool ShowChangePassword(int userId, string currentPassword)
        {
            ChangePasswordWindow window = new ChangePasswordWindow(authenticationService, userId, currentPassword)
            {
                Owner = this
            };
            return window.ShowDialog() == true;
        }

        private void ShowRegisterWindow()
        {
            RegisterWindow window = new RegisterWindow(authenticationService)
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void CustomerRegister_Click(object sender, RoutedEventArgs e)
        {
            CustomerRegisterWindow window = new CustomerRegisterWindow(authenticationService)
            {
                Owner = this
            };
            window.ShowDialog();
        }
    }
}
