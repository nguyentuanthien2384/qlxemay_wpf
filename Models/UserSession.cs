using System.Collections.Generic;

namespace QLXeMay.Models
{
    internal sealed class UserSession
    {
        private readonly HashSet<string> permissions;

        public UserSession(int userId, string userName, string displayName, string roleName, string roleDisplayName, IEnumerable<string> permissions)
        {
            UserId = userId;
            UserName = userName;
            DisplayName = displayName;
            RoleName = roleName;
            RoleDisplayName = roleDisplayName;
            this.permissions = new HashSet<string>(permissions ?? new string[0]);
        }

        public int UserId { get; }
        public string UserName { get; }
        public string DisplayName { get; }
        public string RoleName { get; }
        public string RoleDisplayName { get; }

        public string DisplayText => $"{DisplayName} ({RoleDisplayName})";

        public bool HasPermission(string permission)
        {
            return permissions.Contains(permission);
        }
    }
}
