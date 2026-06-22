using System;

namespace QLXeMay.Models
{
    internal sealed class AuthenticationResult
    {
        private AuthenticationResult()
        {
        }

        public bool IsSuccess { get; private set; }
        public bool MustChangePassword { get; private set; }
        public AuthFailureReason FailureReason { get; private set; }
        public string Message { get; private set; }
        public DateTime? LockoutEndAt { get; private set; }
        public UserSession Session { get; private set; }

        public static AuthenticationResult Success(UserSession session, bool mustChangePassword)
        {
            return new AuthenticationResult
            {
                IsSuccess = true,
                MustChangePassword = mustChangePassword,
                Session = session,
                FailureReason = AuthFailureReason.None,
                Message = mustChangePassword
                    ? "Bạn cần đổi mật khẩu trước khi vào hệ thống."
                    : "Đăng nhập thành công."
            };
        }

        public static AuthenticationResult Failed(AuthFailureReason reason, string message, DateTime? lockoutEndAt = null)
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                FailureReason = reason,
                Message = message,
                LockoutEndAt = lockoutEndAt
            };
        }
    }
}
