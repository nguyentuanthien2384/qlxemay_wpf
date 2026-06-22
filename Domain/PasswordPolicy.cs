using System;
using System.Collections.Generic;

namespace QLXeMay.Domain
{
    public static class PasswordPolicy
    {
        public const int MinimumLength = 10;

        public static IReadOnlyList<string> Validate(string password, string userName, string displayName)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Mật khẩu không được để trống.");
                return errors;
            }

            if (password.Length < MinimumLength)
            {
                errors.Add("Mật khẩu phải có ít nhất " + MinimumLength + " ký tự.");
            }

            if (!Has(password, char.IsUpper))
            {
                errors.Add("Mật khẩu phải có ít nhất một chữ hoa.");
            }

            if (!Has(password, char.IsLower))
            {
                errors.Add("Mật khẩu phải có ít nhất một chữ thường.");
            }

            if (!Has(password, char.IsDigit))
            {
                errors.Add("Mật khẩu phải có ít nhất một chữ số.");
            }

            if (!Has(password, c => !char.IsLetterOrDigit(c)))
            {
                errors.Add("Mật khẩu phải có ít nhất một ký tự đặc biệt.");
            }

            string lowerPassword = password.ToLowerInvariant();
            string normalizedUserName = string.IsNullOrWhiteSpace(userName) ? string.Empty : userName.Trim().ToLowerInvariant();
            if (normalizedUserName.Length >= 3 && lowerPassword.Contains(normalizedUserName))
            {
                errors.Add("Mật khẩu không được chứa tên đăng nhập.");
            }

            string normalizedDisplayName = string.IsNullOrWhiteSpace(displayName) ? string.Empty : displayName.Trim().ToLowerInvariant();
            if (normalizedDisplayName.Length >= 4 && lowerPassword.Contains(normalizedDisplayName))
            {
                errors.Add("Mật khẩu không được chứa tên hiển thị.");
            }

            return errors;
        }

        public static void ThrowIfInvalid(string password, string userName, string displayName)
        {
            IReadOnlyList<string> errors = Validate(password, userName, displayName);
            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join(Environment.NewLine, errors));
            }
        }

        public static string Summary =>
            "Tối thiểu " + MinimumLength + " ký tự, có chữ hoa, chữ thường, số, ký tự đặc biệt và không chứa tên đăng nhập/tên hiển thị.";

        private static bool Has(string value, Func<char, bool> predicate)
        {
            foreach (char c in value)
            {
                if (predicate(c)) return true;
            }

            return false;
        }
    }
}
