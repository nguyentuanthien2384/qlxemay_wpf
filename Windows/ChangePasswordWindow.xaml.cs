using System;
using System.Windows;
using QLXeMay.Services;

namespace QLXeMay.Windows
{
    public partial class ChangePasswordWindow : Window
    {
        private readonly IAuthenticationService authenticationService;
        private readonly int userId;

        internal ChangePasswordWindow(IAuthenticationService authenticationService, int userId, string currentPassword)
        {
            InitializeComponent();
            this.authenticationService = authenticationService;
            this.userId = userId;
            CurrentPasswordBox.Password = currentPassword ?? "";
            Loaded += (s, e) => NewPasswordBox.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBlock.Text = "";
            try
            {
                if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageTextBlock.Text = "Mật khẩu nhập lại không khớp.";
                    return;
                }

                authenticationService.ChangePassword(userId, CurrentPasswordBox.Password, NewPasswordBox.Password);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageTextBlock.Text = ex.Message;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
