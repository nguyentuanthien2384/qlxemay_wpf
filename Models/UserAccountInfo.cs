using System;

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
        public DateTime? LockoutEndAt { get; set; }
        public bool MustChangePassword { get; set; }
        public string PasswordChangedAt { get; set; }

        public bool IsLockedOut => LockoutEndAt.HasValue && LockoutEndAt.Value > DateTime.Now;

        public string StatusText
        {
            get
            {
                if (!IsActive) return "Tạm ngưng";
                if (IsLockedOut) return "Đang khóa";
                if (MustChangePassword) return "Cần đổi mật khẩu";
                return "Hoạt động";
            }
        }

        public string SecurityNote
        {
            get
            {
                if (IsLockedOut) return "Khóa đến " + LockoutEndAt.Value.ToString("dd/MM/yyyy HH:mm");
                if (FailedLoginCount > 0) return FailedLoginCount + " lần đăng nhập sai";
                if (MustChangePassword) return "Mật khẩu tạm, cần đổi khi đăng nhập";
                return "Ổn định";
            }
        }
    }
}
