using System;
using System.Windows;
using System.Windows.Controls;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class UserAdminWindow : UserControl
    {
        private readonly UserAdminWindowViewModel viewModel;

        public UserAdminWindow(Action goBack)
        {
            InitializeComponent();
            viewModel = new UserAdminWindowViewModel(new AuthenticationService(), new DialogService(), goBack ?? (() => { }));
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
