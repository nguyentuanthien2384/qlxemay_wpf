using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using QLXeMay.Class;
using QLXeMay.Infrastructure;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal sealed class AuthenticationService : IAuthenticationService
    {
        private const int PasswordIterations = 120000;

        private static readonly string[] AllPermissions =
        {
            PermissionNames.ManageEmployees,
            PermissionNames.ManageCatalog,
            PermissionNames.ManageCustomers,
            PermissionNames.ManageSuppliers,
            PermissionNames.ManageProducts,
            PermissionNames.SalesInvoice,
            PermissionNames.PurchaseInvoice,
            PermissionNames.Search,
            PermissionNames.Reports,
            PermissionNames.AiAssistant,
            PermissionNames.UserAdmin
        };

        public void EnsureSecuritySchema()
        {
            Function.ExecuteSql(@"
IF OBJECT_ID(N'tblroles', N'U') IS NULL
BEGIN
    CREATE TABLE tblroles (
        roleid INT IDENTITY(1,1) PRIMARY KEY,
        rolename NVARCHAR(50) NOT NULL UNIQUE,
        displayname NVARCHAR(100) NOT NULL,
        description NVARCHAR(255) NULL,
        isbuiltin BIT NOT NULL DEFAULT 0
    );
END");

            Function.ExecuteSql(@"
IF OBJECT_ID(N'tblpermissions', N'U') IS NULL
BEGIN
    CREATE TABLE tblpermissions (
        permissionkey NVARCHAR(80) NOT NULL PRIMARY KEY,
        displayname NVARCHAR(120) NOT NULL,
        description NVARCHAR(255) NULL
    );
END");

            Function.ExecuteSql(@"
IF OBJECT_ID(N'tblusers', N'U') IS NULL
BEGIN
    CREATE TABLE tblusers (
        userid INT IDENTITY(1,1) PRIMARY KEY,
        username NVARCHAR(50) NOT NULL UNIQUE,
        displayname NVARCHAR(100) NOT NULL,
        passwordhash NVARCHAR(200) NOT NULL,
        passwordsalt NVARCHAR(200) NOT NULL,
        passworditerations INT NOT NULL,
        roleid INT NOT NULL FOREIGN KEY REFERENCES tblroles(roleid),
        isactive BIT NOT NULL DEFAULT 1,
        failedlogincount INT NOT NULL DEFAULT 0,
        createdat DATETIME NOT NULL DEFAULT GETDATE(),
        lastloginat DATETIME NULL
    );
END");

            Function.ExecuteSql(@"
IF OBJECT_ID(N'tblrolepermissions', N'U') IS NULL
BEGIN
    CREATE TABLE tblrolepermissions (
        roleid INT NOT NULL FOREIGN KEY REFERENCES tblroles(roleid),
        permissionkey NVARCHAR(80) NOT NULL FOREIGN KEY REFERENCES tblpermissions(permissionkey),
        CONSTRAINT PK_tblrolepermissions PRIMARY KEY(roleid, permissionkey)
    );
END");

            SeedPermissions();
            SeedRoles();
            SeedRolePermissions();
            SeedDefaultUsers();
        }

        public UserSession Authenticate(string userName, string password)
        {
            string normalizedUserName = NormalizeUserName(userName);
            if (string.IsNullOrWhiteSpace(normalizedUserName) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            DataTable table = Function.GetDataToTable(
                @"SELECT TOP 1 u.userid, u.username, u.displayname, u.passwordhash, u.passwordsalt,
                         u.passworditerations, u.isactive, u.roleid, r.rolename, r.displayname AS roledisplayname
                  FROM tblusers u
                  INNER JOIN tblroles r ON u.roleid = r.roleid
                  WHERE u.username=@username",
                Function.Param("@username", normalizedUserName));

            if (table.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = table.Rows[0];
            int userId = Convert.ToInt32(row["userid"]);
            if (!Convert.ToBoolean(row["isactive"]))
            {
                return null;
            }

            string storedHash = row["passwordhash"].ToString();
            string storedSalt = row["passwordsalt"].ToString();
            int iterations = Convert.ToInt32(row["passworditerations"]);

            if (!PasswordHasher.Verify(password, storedSalt, storedHash, iterations))
            {
                Function.ExecuteSql(
                    "UPDATE tblusers SET failedlogincount=failedlogincount+1 WHERE userid=@userid",
                    Function.Param("@userid", userId));
                return null;
            }

            int roleId = Convert.ToInt32(row["roleid"]);
            Function.ExecuteSql(
                "UPDATE tblusers SET failedlogincount=0, lastloginat=GETDATE() WHERE userid=@userid",
                Function.Param("@userid", userId));

            return new UserSession(
                userId,
                row["username"].ToString(),
                row["displayname"].ToString(),
                row["rolename"].ToString(),
                row["roledisplayname"].ToString(),
                LoadPermissionKeys(roleId));
        }

        public IReadOnlyList<UserAccountInfo> LoadUsers()
        {
            DataTable table = Function.GetDataToTable(
                @"SELECT u.userid, u.username, u.displayname, u.isactive, u.failedlogincount,
                         CONVERT(NVARCHAR(19), u.lastloginat, 120) AS lastloginat,
                         r.rolename, r.displayname AS roledisplayname
                  FROM tblusers u
                  INNER JOIN tblroles r ON u.roleid = r.roleid
                  ORDER BY u.username");

            List<UserAccountInfo> users = new List<UserAccountInfo>();
            foreach (DataRow row in table.Rows)
            {
                users.Add(new UserAccountInfo
                {
                    UserId = Convert.ToInt32(row["userid"]),
                    UserName = row["username"].ToString(),
                    DisplayName = row["displayname"].ToString(),
                    IsActive = Convert.ToBoolean(row["isactive"]),
                    FailedLoginCount = Convert.ToInt32(row["failedlogincount"]),
                    LastLoginAt = row["lastloginat"] == DBNull.Value ? "" : row["lastloginat"].ToString(),
                    RoleName = row["rolename"].ToString(),
                    RoleDisplayName = row["roledisplayname"].ToString()
                });
            }

            return users;
        }

        public IReadOnlyList<RoleInfo> LoadRoles()
        {
            DataTable table = Function.GetDataToTable(
                "SELECT roleid, rolename, displayname FROM tblroles ORDER BY roleid");

            List<RoleInfo> roles = new List<RoleInfo>();
            foreach (DataRow row in table.Rows)
            {
                roles.Add(new RoleInfo
                {
                    RoleId = Convert.ToInt32(row["roleid"]),
                    RoleName = row["rolename"].ToString(),
                    DisplayName = row["displayname"].ToString()
                });
            }

            return roles;
        }

        public void CreateUser(string userName, string displayName, string password, int roleId, bool isActive)
        {
            string normalizedUserName = NormalizeUserName(userName);
            ValidateUserInput(normalizedUserName, displayName, roleId);
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự.");
            }

            if (Function.CheckKey("SELECT 1 FROM tblusers WHERE username=@username", Function.Param("@username", normalizedUserName)))
            {
                throw new InvalidOperationException("Tên đăng nhập đã tồn tại.");
            }

            PasswordHash passwordHash = PasswordHasher.Hash(password);
            Function.ExecuteSql(
                @"INSERT INTO tblusers(username, displayname, passwordhash, passwordsalt, passworditerations, roleid, isactive)
                  VALUES(@username, @displayname, @hash, @salt, @iterations, @roleid, @isactive)",
                Function.Param("@username", normalizedUserName),
                Function.Param("@displayname", displayName.Trim()),
                Function.Param("@hash", passwordHash.Hash),
                Function.Param("@salt", passwordHash.Salt),
                Function.Param("@iterations", passwordHash.Iterations),
                Function.Param("@roleid", roleId),
                Function.Param("@isactive", isActive));
        }

        public void UpdateUser(int userId, string displayName, int roleId, bool isActive)
        {
            if (userId <= 0) throw new ArgumentException("Chọn tài khoản cần cập nhật.");
            ValidateUserInput("user", displayName, roleId);

            Function.ExecuteSql(
                "UPDATE tblusers SET displayname=@displayname, roleid=@roleid, isactive=@isactive WHERE userid=@userid",
                Function.Param("@displayname", displayName.Trim()),
                Function.Param("@roleid", roleId),
                Function.Param("@isactive", isActive),
                Function.Param("@userid", userId));
        }

        public void ResetPassword(int userId, string newPassword)
        {
            if (userId <= 0) throw new ArgumentException("Chọn tài khoản cần đổi mật khẩu.");
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự.");
            }

            PasswordHash passwordHash = PasswordHasher.Hash(newPassword);
            Function.ExecuteSql(
                @"UPDATE tblusers
                  SET passwordhash=@hash, passwordsalt=@salt, passworditerations=@iterations, failedlogincount=0
                  WHERE userid=@userid",
                Function.Param("@hash", passwordHash.Hash),
                Function.Param("@salt", passwordHash.Salt),
                Function.Param("@iterations", passwordHash.Iterations),
                Function.Param("@userid", userId));
        }

        private static void SeedPermissions()
        {
            EnsurePermission(PermissionNames.ManageEmployees, "Quản lý nhân viên");
            EnsurePermission(PermissionNames.ManageCatalog, "Quản lý danh mục chung");
            EnsurePermission(PermissionNames.ManageCustomers, "Quản lý khách hàng");
            EnsurePermission(PermissionNames.ManageSuppliers, "Quản lý nhà cung cấp");
            EnsurePermission(PermissionNames.ManageProducts, "Quản lý hàng hóa");
            EnsurePermission(PermissionNames.SalesInvoice, "Lập hóa đơn bán");
            EnsurePermission(PermissionNames.PurchaseInvoice, "Lập hóa đơn nhập");
            EnsurePermission(PermissionNames.Search, "Tìm kiếm dữ liệu");
            EnsurePermission(PermissionNames.Reports, "Xem báo cáo");
            EnsurePermission(PermissionNames.AiAssistant, "Sử dụng trợ lý AI");
            EnsurePermission(PermissionNames.UserAdmin, "Quản trị tài khoản");
        }

        private static void SeedRoles()
        {
            EnsureRole("Administrator", "Quản trị hệ thống", "Toàn quyền hệ thống");
            EnsureRole("Manager", "Quản lý cửa hàng", "Quản lý nghiệp vụ, không quản trị tài khoản");
            EnsureRole("Sales", "Nhân viên bán hàng", "Bán hàng, khách hàng, tìm kiếm và báo cáo");
            EnsureRole("Warehouse", "Thủ kho", "Nhập hàng, nhà cung cấp, hàng hóa, tìm kiếm và báo cáo");
            EnsureRole("Viewer", "Chỉ xem báo cáo", "Chỉ tra cứu và xem báo cáo");
        }

        private static void SeedRolePermissions()
        {
            EnsureRolePermissions("Administrator", AllPermissions);
            EnsureRolePermissions("Manager", new[]
            {
                PermissionNames.ManageCatalog,
                PermissionNames.ManageCustomers,
                PermissionNames.ManageSuppliers,
                PermissionNames.ManageProducts,
                PermissionNames.SalesInvoice,
                PermissionNames.PurchaseInvoice,
                PermissionNames.Search,
                PermissionNames.Reports,
                PermissionNames.AiAssistant
            });
            EnsureRolePermissions("Sales", new[]
            {
                PermissionNames.ManageCustomers,
                PermissionNames.SalesInvoice,
                PermissionNames.Search,
                PermissionNames.Reports,
                PermissionNames.AiAssistant
            });
            EnsureRolePermissions("Warehouse", new[]
            {
                PermissionNames.ManageSuppliers,
                PermissionNames.ManageProducts,
                PermissionNames.PurchaseInvoice,
                PermissionNames.Search,
                PermissionNames.Reports
            });
            EnsureRolePermissions("Viewer", new[]
            {
                PermissionNames.Search,
                PermissionNames.Reports
            });
        }

        private static void SeedDefaultUsers()
        {
            EnsureUser("admin", "Quản trị viên", "Admin@123", "Administrator");
            EnsureUser("manager", "Quản lý cửa hàng", "Manager@123", "Manager");
            EnsureUser("sales", "Nhân viên bán hàng", "Sales@123", "Sales");
            EnsureUser("warehouse", "Nhân viên kho", "Warehouse@123", "Warehouse");
            EnsureUser("viewer", "Tài khoản xem báo cáo", "Viewer@123", "Viewer");
        }

        private static void EnsurePermission(string permissionKey, string displayName)
        {
            Function.ExecuteSql(
                @"IF NOT EXISTS (SELECT 1 FROM tblpermissions WHERE permissionkey=@permissionkey)
                  INSERT INTO tblpermissions(permissionkey, displayname) VALUES(@permissionkey, @displayname)
                  ELSE UPDATE tblpermissions SET displayname=@displayname WHERE permissionkey=@permissionkey",
                Function.Param("@permissionkey", permissionKey),
                Function.Param("@displayname", displayName));
        }

        private static void EnsureRole(string roleName, string displayName, string description)
        {
            Function.ExecuteSql(
                @"IF NOT EXISTS (SELECT 1 FROM tblroles WHERE rolename=@rolename)
                  INSERT INTO tblroles(rolename, displayname, description, isbuiltin) VALUES(@rolename, @displayname, @description, 1)
                  ELSE UPDATE tblroles SET displayname=@displayname, description=@description WHERE rolename=@rolename",
                Function.Param("@rolename", roleName),
                Function.Param("@displayname", displayName),
                Function.Param("@description", description));
        }

        private static void EnsureRolePermissions(string roleName, IEnumerable<string> permissions)
        {
            int roleId = GetRoleId(roleName);
            foreach (string permission in permissions)
            {
                Function.ExecuteSql(
                    @"IF NOT EXISTS (SELECT 1 FROM tblrolepermissions WHERE roleid=@roleid AND permissionkey=@permissionkey)
                      INSERT INTO tblrolepermissions(roleid, permissionkey) VALUES(@roleid, @permissionkey)",
                    Function.Param("@roleid", roleId),
                    Function.Param("@permissionkey", permission));
            }
        }

        private static void EnsureUser(string userName, string displayName, string password, string roleName)
        {
            if (Function.CheckKey("SELECT 1 FROM tblusers WHERE username=@username", Function.Param("@username", userName)))
            {
                return;
            }

            PasswordHash passwordHash = PasswordHasher.Hash(password);
            Function.ExecuteSql(
                @"INSERT INTO tblusers(username, displayname, passwordhash, passwordsalt, passworditerations, roleid, isactive)
                  VALUES(@username, @displayname, @hash, @salt, @iterations, @roleid, 1)",
                Function.Param("@username", userName),
                Function.Param("@displayname", displayName),
                Function.Param("@hash", passwordHash.Hash),
                Function.Param("@salt", passwordHash.Salt),
                Function.Param("@iterations", passwordHash.Iterations),
                Function.Param("@roleid", GetRoleId(roleName)));
        }

        private static int GetRoleId(string roleName)
        {
            string value = Function.GetFieldValues(
                "SELECT roleid FROM tblroles WHERE rolename=@rolename",
                Function.Param("@rolename", roleName));
            int roleId = Function.ToInt(value);
            if (roleId <= 0)
            {
                throw new InvalidOperationException("Không tìm thấy vai trò: " + roleName);
            }

            return roleId;
        }

        private static IReadOnlyList<string> LoadPermissionKeys(int roleId)
        {
            DataTable table = Function.GetDataToTable(
                "SELECT permissionkey FROM tblrolepermissions WHERE roleid=@roleid",
                Function.Param("@roleid", roleId));

            List<string> permissions = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                permissions.Add(row["permissionkey"].ToString());
            }

            return permissions;
        }

        private static string NormalizeUserName(string userName)
        {
            return string.IsNullOrWhiteSpace(userName) ? "" : userName.Trim().ToLowerInvariant();
        }

        private static void ValidateUserInput(string userName, string displayName, int roleId)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Tên đăng nhập không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Tên hiển thị không được để trống.");
            }

            if (roleId <= 0)
            {
                throw new ArgumentException("Chọn vai trò cho tài khoản.");
            }
        }

        private sealed class PasswordHash
        {
            public string Hash { get; set; }
            public string Salt { get; set; }
            public int Iterations { get; set; }
        }

        private static class PasswordHasher
        {
            public static PasswordHash Hash(string password)
            {
                byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
                byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(password),
                    saltBytes,
                    PasswordIterations,
                    HashAlgorithmName.SHA256,
                    32);

                return new PasswordHash
                {
                    Hash = Convert.ToBase64String(hashBytes),
                    Salt = Convert.ToBase64String(saltBytes),
                    Iterations = PasswordIterations
                };
            }

            public static bool Verify(string password, string salt, string expectedHash, int iterations)
            {
                try
                {
                    byte[] saltBytes = Convert.FromBase64String(salt);
                    byte[] expectedBytes = Convert.FromBase64String(expectedHash);
                    byte[] actualBytes = Rfc2898DeriveBytes.Pbkdf2(
                        Encoding.UTF8.GetBytes(password),
                        saltBytes,
                        iterations,
                        HashAlgorithmName.SHA256,
                        expectedBytes.Length);

                    return CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
                }
                catch (Exception ex)
                {
                    AppLogger.Error("Password verification failed.", ex);
                    return false;
                }
            }
        }
    }
}
