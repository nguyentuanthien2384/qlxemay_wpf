namespace QLXeMay.Models
{
    internal sealed class UserAccountInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string RoleName { get; set; }
        public string RoleDisplayName { get; set; }
        public bool IsActive { get; set; }
        public int FailedLoginCount { get; set; }
        public string LastLoginAt { get; set; }
    }
}
