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
        private string statusMessage;

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
            UpdateUserCommand = new RelayCommand(_ => UpdateUser(), _ => SelectedUser != null);
            ResetPasswordCommand = new RelayCommand(_ => ResetPassword(), _ => SelectedUser != null);
            UnlockUserCommand = new RelayCommand(_ => UnlockUser(), _ => SelectedUser != null);
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
                RaiseCommandState();
                OnPropertyChanged(nameof(SelectedUserSummary));
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

        public string StatusMessage
        {
            get => statusMessage;
            private set => SetProperty(ref statusMessage, value);
        }

        public string SelectedUserSummary
        {
            get
            {
                if (SelectedUser == null) return "Chưa chọn tài khoản.";
                return SelectedUser.UserName + " - " + SelectedUser.StatusText + " - " + SelectedUser.SecurityNote;
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand CreateUserCommand { get; }
        public ICommand UpdateUserCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand UnlockUserCommand { get; }
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

            StatusMessage = "Đã tải " + Users.Count + " tài khoản.";
            RaiseCommandState();
        }

        private void CreateUser()
        {
            try
            {
                authenticationService.CreateUser(NewUserName, DisplayName, Password, SelectedRole == null ? 0 : SelectedRole.RoleId, IsActive);
                StatusMessage = "Đã tạo tài khoản mới. Người dùng sẽ phải đổi mật khẩu ở lần đăng nhập đầu tiên.";
                ClearForm();
                Load();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
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
                StatusMessage = "Đã cập nhật tài khoản.";
                Load();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                dialogService.ShowWarning(ex.Message);
            }
        }

        private void ResetPassword()
        {
            if (SelectedUser == null)
            {
                dialogService.ShowWarning("Chọn tài khoản cần đặt lại mật khẩu.");
                return;
            }

            try
            {
                authenticationService.ResetPassword(SelectedUser.UserId, Password);
                StatusMessage = "Đã đặt lại mật khẩu tạm. User sẽ phải đổi mật khẩu khi đăng nhập.";
                Password = "";
                Load();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                dialogService.ShowWarning(ex.Message);
            }
        }

        private void UnlockUser()
        {
            if (SelectedUser == null)
            {
                dialogService.ShowWarning("Chọn tài khoản cần mở khóa.");
                return;
            }

            try
            {
                authenticationService.UnlockUser(SelectedUser.UserId);
                StatusMessage = "Đã mở khóa tài khoản.";
                Load();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                dialogService.ShowWarning(ex.Message);
            }
        }

        private void ClearForm()
        {
            selectedUser = null;
            OnPropertyChanged(nameof(SelectedUser));
            NewUserName = "";
            DisplayName = "";
            Password = "";
            IsActive = true;
            if (Roles.Count > 0) SelectedRole = Roles[0];
            StatusMessage = "Đang tạo tài khoản mới.";
            OnPropertyChanged(nameof(SelectedUserSummary));
            RaiseCommandState();
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

        private void RaiseCommandState()
        {
            (UpdateUserCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ResetPasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (UnlockUserCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}
