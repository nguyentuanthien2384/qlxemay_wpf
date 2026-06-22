using System.Collections.Generic;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal interface IAuthenticationService
    {
        void EnsureSecuritySchema();
        UserSession Authenticate(string userName, string password);
        IReadOnlyList<UserAccountInfo> LoadUsers();
        IReadOnlyList<RoleInfo> LoadRoles();
        void CreateUser(string userName, string displayName, string password, int roleId, bool isActive);
        void UpdateUser(int userId, string displayName, int roleId, bool isActive);
        void ResetPassword(int userId, string newPassword);
    }
}
