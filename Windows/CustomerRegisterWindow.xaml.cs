using System;
using System.Windows;
using System.Windows.Media;
using QLXeMay.Services;

namespace QLXeMay.Windows
{
    public partial class CustomerRegisterWindow : Window
    {
        private readonly IAuthenticationService authenticationService;

        internal CustomerRegisterWindow(IAuthenticationService authenticationService)
        {
            InitializeComponent();
            this.authenticationService = authenticationService;
            Loaded += (s, e) => FullNameTextBox.Focus();
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

                authenticationService.RegisterCustomer(
                    FullNameTextBox.Text,
                    PhoneTextBox.Text,
                    AddressTextBox.Text,
                    UserNameTextBox.Text,
                    PasswordBox.Password);

                MessageBox.Show(
                    "Đăng ký thành công! Bạn có thể đăng nhập ngay bằng tài khoản vừa tạo để mua hàng.",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
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
