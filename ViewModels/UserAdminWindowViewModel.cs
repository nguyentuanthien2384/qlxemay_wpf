using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using QLXeMay.Infrastructure;
using QLXeMay.Models;
using QLXeMay.Services;

namespace QLXeMay.ViewModels
{
    internal sealed class UserAdminWindowViewModel : ViewModelBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDialogService dialogService;
        private readonly Action closeAction;
        private UserAccountInfo selectedUser;
        private RoleInfo selectedRole;
        private string newUserName;
        private string displayName;
        private string password;
        private bool isActive = true;

        public UserAdminWindowViewModel(
            IAuthenticationService authenticationService,
            IDialogService dialogService,
            Action closeAction)
        {
            this.authenticationService = authenticationService;
            this.dialogService = dialogService;
            this.closeAction = closeAction;

            Users = new ObservableCollection<UserAccountInfo>();
            Roles = new ObservableCollection<RoleInfo>();
            RefreshCommand = new RelayCommand(_ => Load());
            CreateUserCommand = new RelayCommand(_ => CreateUser());
            UpdateUserCommand = new RelayCommand(_ => UpdateUser());
            ResetPasswordCommand = new RelayCommand(_ => ResetPassword());
            ClearCommand = new RelayCommand(_ => ClearForm());
            CloseCommand = new RelayCommand(_ => closeAction());

            Load();
        }

        public ObservableCollection<UserAccountInfo> Users { get; }
        public ObservableCollection<RoleInfo> Roles { get; }

        public UserAccountInfo SelectedUser
        {
            get => selectedUser;
            set
            {
                if (!SetProperty(ref selectedUser, value)) return;
                ApplySelectedUser(value);
            }
        }

        public RoleInfo SelectedRole
        {
            get => selectedRole;
            set => SetProperty(ref selectedRole, value);
        }

        public string NewUserName
        {
            get => newUserName;
            set => SetProperty(ref newUserName, value);
        }

        public string DisplayName
        {
            get => displayName;
            set => SetProperty(ref displayName, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public bool IsActive
        {
            get => isActive;
            set => SetProperty(ref isActive, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand CreateUserCommand { get; }
        public ICommand UpdateUserCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CloseCommand { get; }

        private void Load()
        {
            Users.Clear();
            foreach (UserAccountInfo user in authenticationService.LoadUsers())
            {
                Users.Add(user);
            }

            Roles.Clear();
            foreach (RoleInfo role in authenticationService.LoadRoles())
            {
                Roles.Add(role);
            }

            if (SelectedRole == null && Roles.Count > 0)
            {
                SelectedRole = Roles[0];
            }
        }

        private void CreateUser()
        {
            try
            {
                authenticationService.CreateUser(NewUserName, DisplayName, Password, SelectedRole == null ? 0 : SelectedRole.RoleId, IsActive);
                dialogService.ShowInformation("Đã tạo tài khoản mới.");
                ClearForm();
                Load();
            }
            catch (Exception ex)
            {
                dialogService.ShowWarning(ex.Message);
            }
        }

        private void UpdateUser()
        {
            if (SelectedUser == null)
            {
                dialogService.ShowWarning("Chọn tài khoản cần cập nhật.");
                return;
            }

            try
            {
                authenticationService.UpdateUser(SelectedUser.UserId, DisplayName, SelectedRole == null ? 0 : SelectedRole.RoleId, IsActive);
                dialogService.ShowInformation("Đã cập nhật tài khoản.");
                Load();
            }
            catch (Exception ex)
            {
                dialogService.ShowWarning(ex.Message);
            }
        }

        private void ResetPassword()
        {
            if (SelectedUser == null)
            {
                dialogService.ShowWarning("Chọn tài khoản cần đổi mật khẩu.");
                return;
            }

            try
            {
                authenticationService.ResetPassword(SelectedUser.UserId, Password);
                dialogService.ShowInformation("Đã đặt lại mật khẩu.");
                Password = "";
            }
            catch (Exception ex)
            {
                dialogService.ShowWarning(ex.Message);
            }
        }

        private void ClearForm()
        {
            SelectedUser = null;
            NewUserName = "";
            DisplayName = "";
            Password = "";
            IsActive = true;
            if (Roles.Count > 0) SelectedRole = Roles[0];
        }

        private void ApplySelectedUser(UserAccountInfo user)
        {
            if (user == null) return;

            NewUserName = user.UserName;
            DisplayName = user.DisplayName;
            Password = "";
            IsActive = user.IsActive;
            foreach (RoleInfo role in Roles)
            {
                if (role.RoleName == user.RoleName)
                {
                    SelectedRole = role;
                    break;
                }
            }
        }
    }
}
