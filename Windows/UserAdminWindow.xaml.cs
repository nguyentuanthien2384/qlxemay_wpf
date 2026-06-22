using System.Windows;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class UserAdminWindow : Window
    {
        private readonly UserAdminWindowViewModel viewModel;

        public UserAdminWindow()
        {
            InitializeComponent();
            viewModel = new UserAdminWindowViewModel(new AuthenticationService(), new DialogService(), Close);
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            DataContext = viewModel;
        }

        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.Password = PasswordInput.Password;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserAdminWindowViewModel.Password) && string.IsNullOrEmpty(viewModel.Password) && PasswordInput.Password.Length > 0)
            {
                PasswordInput.Clear();
            }
        }
    }
}
