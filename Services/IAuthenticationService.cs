using System.Collections.Generic;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal interface IAuthenticationService
    {
        void EnsureSecuritySchema();
        AuthenticationResult Authenticate(string userName, string password);
        IReadOnlyList<UserAccountInfo> LoadUsers();
        IReadOnlyList<RoleInfo> LoadRoles();
        void CreateUser(string userName, string displayName, string password, int roleId, bool isActive);
        void RegisterUser(string userName, string displayName, string password);
        void UpdateUser(int userId, string displayName, int roleId, bool isActive);
        void ResetPassword(int userId, string newPassword);
        void ChangePassword(int userId, string currentPassword, string newPassword);
        void UnlockUser(int userId);
    }
}
