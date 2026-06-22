using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QLXeMay.Infrastructure;

namespace QLXeMay.Class
{
    internal static class Function
    {
        private const string DefaultConnectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=btl;Integrated Security=True;Encrypt=False";

        public static string ConnectionString { get; private set; } = LoadConnectionString();

        private static string LoadConnectionString()
        {
            string value = Environment.GetEnvironmentVariable("QLXEMAY_CONNECTION_STRING");
            return string.IsNullOrWhiteSpace(value) ? DefaultConnectionString : value;
        }

        public static void ketnoi()
        {
            try
            {
                using (SqlConnection connection = CreateConnection())
                {
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Database connection failed.", ex);
                MessageBox.Show("Lỗi kết nối CSDL:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public static void RepairVietnameseSeedData()
        {
            try
            {
                using (SqlConnection connection = CreateConnection())
                {
                    connection.Open();

                    UpdateSeedText(connection, "tblcongviec", "macv", "CV01", "tencv", "Quản lý");
                    UpdateSeedText(connection, "tblcongviec", "macv", "CV02", "tencv", "Nhân viên bán hàng");
                    UpdateSeedText(connection, "tblcongviec", "macv", "CV03", "tencv", "Kế toán");
                    UpdateSeedText(connection, "tblcongviec", "macv", "CV04", "tencv", "Thủ kho");

                    UpdateSeedText(connection, "tblmausac", "mamau", "MS01", "tenmau", "Đen");
                    UpdateSeedText(connection, "tblmausac", "mamau", "MS02", "tenmau", "Trắng");
                    UpdateSeedText(connection, "tblmausac", "mamau", "MS03", "tenmau", "Đỏ");
                    UpdateSeedText(connection, "tblmausac", "mamau", "MS04", "tenmau", "Xanh");
                    UpdateSeedText(connection, "tblmausac", "mamau", "MS05", "tenmau", "Bạc");

                    UpdateSeedText(connection, "tblnuocsx", "manuocsx", "NSX01", "tennuocsx", "Việt Nam");
                    UpdateSeedText(connection, "tblnuocsx", "manuocsx", "NSX02", "tennuocsx", "Nhật Bản");
                    UpdateSeedText(connection, "tblnuocsx", "manuocsx", "NSX03", "tennuocsx", "Ý");
                    UpdateSeedText(connection, "tblnuocsx", "manuocsx", "NSX04", "tennuocsx", "Trung Quốc");

                    UpdateSeedText(connection, "tbldongco", "madongco", "DC01", "tendongco", "4 thì");
                    UpdateSeedText(connection, "tbldongco", "madongco", "DC02", "tendongco", "Phun xăng điện tử");
                    UpdateSeedText(connection, "tbldongco", "madongco", "DC03", "tendongco", "Động cơ điện");

                    UpdateSeedText(connection, "tbltinhtrang", "matt", "TT01", "tentt", "Mới");
                    UpdateSeedText(connection, "tbltinhtrang", "matt", "TT02", "tentt", "Đã qua sử dụng");

                    UpdateSeedText(connection, "tblphanhxe", "maphanh", "PH01", "tenphanh", "Phanh đĩa");
                    UpdateSeedText(connection, "tblphanhxe", "maphanh", "PH02", "tenphanh", "Phanh tang trống");
                    UpdateSeedText(connection, "tblphanhxe", "maphanh", "PH03", "tenphanh", "Phanh ABS");

                    UpdateSeedText(connection, "tblnhacungcap", "mancc", "NCC01", "tenncc", "Honda Việt Nam");
                    UpdateSeedText(connection, "tblnhacungcap", "mancc", "NCC01", "diachi", "Vĩnh Phúc");
                    UpdateSeedText(connection, "tblnhacungcap", "mancc", "NCC02", "tenncc", "Yamaha Motor VN");
                    UpdateSeedText(connection, "tblnhacungcap", "mancc", "NCC02", "diachi", "Hà Nội");

                    UpdateSeedText(connection, "tblnhanvien", "manv", "NV01", "tennv", "Nguyễn Văn An");
                    UpdateSeedText(connection, "tblnhanvien", "manv", "NV01", "gioitinh", "Nam");
                    UpdateSeedText(connection, "tblnhanvien", "manv", "NV01", "diachi", "Hà Nội");
                    UpdateSeedText(connection, "tblnhanvien", "manv", "NV02", "tennv", "Trần Thị Bình");
                    UpdateSeedText(connection, "tblnhanvien", "manv", "NV02", "gioitinh", "Nữ");
                    UpdateSeedText(connection, "tblnhanvien", "manv", "NV02", "diachi", "Hà Nội");

                    UpdateSeedText(connection, "tblkhachhang", "makhach", "KH01", "tenkhach", "Lê Văn Cường");
                    UpdateSeedText(connection, "tblkhachhang", "makhach", "KH01", "diachi", "Hà Nội");
                    UpdateSeedText(connection, "tblkhachhang", "makhach", "KH02", "tenkhach", "Phạm Thị Dung");
                    UpdateSeedText(connection, "tblkhachhang", "makhach", "KH02", "diachi", "Hải Phòng");
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Vietnamese seed data repair failed.", ex);
            }
        }

        public static void Disconnect()
        {
            // Connections are opened per operation and disposed immediately.
        }

        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static SqlParameter Param(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        public static string QuoteIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Tên bảng/cột không hợp lệ.", nameof(identifier));

            string[] parts = identifier.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i].Trim();
                if (!IsSimpleSqlIdentifier(part))
                    throw new ArgumentException("Tên bảng/cột không hợp lệ: " + identifier, nameof(identifier));

                parts[i] = "[" + part + "]";
            }

            return string.Join(".", parts);
        }

        public static DataTable GetDataToTable(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = CreateConnection())
                {
                    connection.Open();
                    return GetDataToTable(connection, null, sql, parameters);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Query failed: " + sql, ex);
                MessageBox.Show("Lỗi truy vấn:\n" + ex.Message + "\n\nSQL: " + sql, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }

        public static DataTable GetDataToTable(SqlConnection connection, SqlTransaction transaction, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = CreateCommand(connection, transaction, sql, parameters))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public static bool CheckKey(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = CreateConnection())
                {
                    connection.Open();
                    return CheckKey(connection, null, sql, parameters);
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckKey(SqlConnection connection, SqlTransaction transaction, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = CreateCommand(connection, transaction, sql, parameters))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                return reader.Read();
            }
        }

        public static int ExecuteSql(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = CreateConnection())
            {
                connection.Open();
                return ExecuteSql(connection, null, sql, parameters);
            }
        }

        public static int ExecuteSql(SqlConnection connection, SqlTransaction transaction, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = CreateCommand(connection, transaction, sql, parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        public static bool ExecuteTransaction(Action<SqlConnection, SqlTransaction> action, string errorMessage)
        {
            using (SqlConnection connection = CreateConnection())
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        action(connection, transaction);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                            // Preserve the original database error for the user.
                        }
                        AppLogger.Error(errorMessage, ex);
                        MessageBox.Show(errorMessage + "\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }
        }

        public static void FillCombo(string sql, ComboBox cbo, string valueMember, string displayMember)
        {
            try
            {
                DataTable table = GetDataToTable(sql);
                cbo.ItemsSource = table.DefaultView;
                cbo.SelectedValuePath = valueMember;
                cbo.DisplayMemberPath = displayMember;
                cbo.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                AppLogger.Error("FillCombo failed: " + sql, ex);
                MessageBox.Show("Lỗi FillCombo:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string GetSelectedValue(ComboBox cbo)
        {
            return cbo.SelectedValue == null ? string.Empty : cbo.SelectedValue.ToString().Trim();
        }

        public static string GetFieldValues(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = CreateConnection())
                {
                    connection.Open();
                    return GetFieldValues(connection, null, sql, parameters);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetFieldValues(SqlConnection connection, SqlTransaction transaction, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = CreateCommand(connection, transaction, sql, parameters))
            {
                object value = command.ExecuteScalar();
                return value == null || value == DBNull.Value ? string.Empty : value.ToString();
            }
        }

        public static string ToSqlDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static string ConvertDateTime(string d)
        {
            try
            {
                string[] p = d.Split('/');
                return $"{p[1]}/{p[0]}/{p[2]}";
            }
            catch { return d; }
        }

        public static string CreateKey(string tiento)
        {
            return tiento + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmss");
        }

        public static bool IsSoNguyen(string d)
        {
            if (string.IsNullOrWhiteSpace(d)) return false;
            int temp;
            return int.TryParse(d.Trim(), out temp);
        }

        public static bool IsSoThuc(string d)
        {
            if (string.IsNullOrWhiteSpace(d)) return false;
            double temp;
            return double.TryParse(d.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out temp);
        }

        public static double ToDouble(string d, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(d)) return defaultValue;
            double temp;
            if (double.TryParse(d.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out temp)) return temp;
            return defaultValue;
        }

        public static int ToInt(string d, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(d)) return defaultValue;
            int temp;
            if (int.TryParse(d.Trim(), out temp)) return temp;
            return defaultValue;
        }

        public static bool IsDate(string d)
        {
            DateTime dt;
            return DateTime.TryParse(d, out dt) && dt <= DateTime.Today;
        }

        public static string ChuyenSoSangChu(string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber) || sNumber == "0") return "Không đồng";
            int mLen, mDigit;
            string sTemp = "";
            sNumber = sNumber.Replace(",", "").Replace(".", "").Trim();
            string[] mNumText = "không,một,hai,ba,bốn,năm,sáu,bảy,tám,chín".Split(',');
            mLen = sNumber.Length - 1;
            for (int i = 0; i <= mLen; i++)
            {
                mDigit = Convert.ToInt32(sNumber.Substring(i, 1));
                sTemp = sTemp + " " + mNumText[mDigit];
                if (mLen == i) break;
                switch ((mLen - i) % 9)
                {
                    case 0: sTemp += " tỷ"; break;
                    case 6: sTemp += " triệu"; break;
                    case 3: sTemp += " nghìn"; break;
                    default:
                        switch ((mLen - i) % 3)
                        {
                            case 2: sTemp += " trăm"; break;
                            case 1: sTemp += " mươi"; break;
                        }
                        break;
                }
            }
            sTemp = sTemp.Replace("không mươi không ", "").Replace("không mươi ", "linh ").Trim();
            if (sTemp.Length > 0)
                sTemp = sTemp.Substring(0, 1).ToUpper() + sTemp.Substring(1) + " đồng";
            return sTemp;
        }

        private static SqlCommand CreateCommand(SqlConnection connection, SqlTransaction transaction, string sql, params SqlParameter[] parameters)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 30;

            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(CloneParameter(parameter));
            }

            return command;
        }

        private static void UpdateSeedText(SqlConnection connection, string table, string keyColumn, string keyValue, string valueColumn, string value)
        {
            ExecuteSql(
                connection,
                null,
                $"UPDATE {QuoteIdentifier(table)} SET {QuoteIdentifier(valueColumn)}=@value WHERE {QuoteIdentifier(keyColumn)}=@key",
                Param("@value", value),
                Param("@key", keyValue));
        }

        private static SqlParameter CloneParameter(SqlParameter parameter)
        {
            SqlParameter clone = new SqlParameter(parameter.ParameterName, parameter.Value ?? DBNull.Value);
            clone.Direction = parameter.Direction;
            clone.IsNullable = parameter.IsNullable;
            clone.Size = parameter.Size;
            clone.Precision = parameter.Precision;
            clone.Scale = parameter.Scale;
            clone.SqlDbType = parameter.SqlDbType;
            return clone;
        }

        private static bool IsSimpleSqlIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool valid = c == '_' || char.IsLetterOrDigit(c);
                if (!valid) return false;
            }

            return true;
        }
    }
}
