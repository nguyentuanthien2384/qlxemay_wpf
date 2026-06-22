using System;
using System.Windows.Input;
using QLXeMay.Infrastructure;
using QLXeMay.Models;
using QLXeMay.Services;

namespace QLXeMay.ViewModels
{
    internal sealed class LoginWindowViewModel : ViewModelBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly Action<bool> closeAction;
        private string userName;
        private string password;
        private string errorMessage;

        public LoginWindowViewModel(IAuthenticationService authenticationService, Action<bool> closeAction)
        {
            this.authenticationService = authenticationService;
            this.closeAction = closeAction;

            LoginCommand = new RelayCommand(_ => Login());
            CancelCommand = new RelayCommand(_ => closeAction(false));
        }

        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public string ErrorMessage
        {
            get => errorMessage;
            private set => SetProperty(ref errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand CancelCommand { get; }

        private void Login()
        {
            ErrorMessage = "";
            UserSession session = authenticationService.Authenticate(UserName, Password);
            if (session == null)
            {
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.";
                return;
            }

            AppSession.SignIn(session);
            closeAction(true);
        }
    }
}
