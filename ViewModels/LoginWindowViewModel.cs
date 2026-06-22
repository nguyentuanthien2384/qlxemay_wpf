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
        private readonly Func<int, string, bool> changePasswordAction;
        private readonly Action openRegisterAction;
        private string userName;
        private string password;
        private string errorMessage;
        private string helperMessage;

        public LoginWindowViewModel(
            IAuthenticationService authenticationService,
            Action<bool> closeAction,
            Func<int, string, bool> changePasswordAction,
            Action openRegisterAction)
        {
            this.authenticationService = authenticationService;
            this.closeAction = closeAction;
            this.changePasswordAction = changePasswordAction;
            this.openRegisterAction = openRegisterAction;

            LoginCommand = new RelayCommand(_ => Login());
            RegisterCommand = new RelayCommand(_ => openRegisterAction());
            CancelCommand = new RelayCommand(_ => closeAction(false));
            HelperMessage = "Nếu quên mật khẩu, liên hệ quản trị viên để reset. Tài khoản tự đăng ký cần được admin kích hoạt.";
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

        public string HelperMessage
        {
            get => helperMessage;
            private set => SetProperty(ref helperMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        private void Login()
        {
            ErrorMessage = "";
            AuthenticationResult result = authenticationService.Authenticate(UserName, Password);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return;
            }

            if (result.MustChangePassword)
            {
                bool changed = changePasswordAction(result.Session.UserId, Password);
                if (!changed)
                {
                    ErrorMessage = "Bạn phải đổi mật khẩu trước khi sử dụng hệ thống.";
                    return;
                }
            }

            AppSession.SignIn(result.Session);
            closeAction(true);
        }
    }
}
