using System;
using System.Windows;
using System.Windows.Media;
using QLXeMay.Services;

namespace QLXeMay.Windows
{
    public partial class RegisterWindow : Window
    {
        private readonly IAuthenticationService authenticationService;

        internal RegisterWindow(IAuthenticationService authenticationService)
        {
            InitializeComponent();
            this.authenticationService = authenticationService;
            Loaded += (s, e) => UserNameTextBox.Focus();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBlock.Text = "";
            try
            {
                if (PasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    ShowMessage("Mật khẩu nhập lại không khớp.", true);
                    return;
                }

                authenticationService.RegisterUser(UserNameTextBox.Text, DisplayNameTextBox.Text, PasswordBox.Password);
                ShowMessage("Đăng ký thành công. Tài khoản đang chờ quản trị viên kích hoạt.", false);
                UserNameTextBox.Text = "";
                DisplayNameTextBox.Text = "";
                PasswordBox.Password = "";
                ConfirmPasswordBox.Password = "";
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, true);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowMessage(string message, bool isError)
        {
            MessageTextBlock.Foreground = isError ? Brushes.DarkRed : Brushes.DarkGreen;
            MessageTextBlock.Text = message;
        }
    }
}
