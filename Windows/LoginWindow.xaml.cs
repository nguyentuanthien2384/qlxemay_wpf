using System.Windows;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class LoginWindow : Window
    {
        private readonly LoginWindowViewModel viewModel;

        internal LoginWindow(IAuthenticationService authenticationService)
        {
            InitializeComponent();
            viewModel = new LoginWindowViewModel(authenticationService, CloseWithResult);
            DataContext = viewModel;
            Loaded += (s, e) => UserNameTextBox.Focus();
        }

        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.Password = PasswordInput.Password;
        }

        private void CloseWithResult(bool result)
        {
            DialogResult = result;
            Close();
        }
    }
}
