using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace QLXeMay.Infrastructure
{
    /// <summary>
    /// Formats database/DTO column names into user-facing Vietnamese headers.
    /// Keeps DataTable column names unchanged so existing CRUD/search logic still works.
    /// </summary>
    internal static class GridHeaderFormatter
    {
        private static readonly IReadOnlyDictionary<string, string> HeaderMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["macv"] = "Mã công việc",
            ["tencv"] = "Tên công việc",
            ["luongthang"] = "Lương tháng",

            ["manv"] = "Mã nhân viên",
            ["tennv"] = "Tên nhân viên",
            ["gioitinh"] = "Giới tính",
            ["ngaysinh"] = "Ngày sinh",
            ["sdt"] = "Số điện thoại",
            ["diachi"] = "Địa chỉ",

            ["makhach"] = "Mã khách hàng",
            ["tenkhach"] = "Tên khách hàng",

            ["mancc"] = "Mã nhà cung cấp",
            ["tenncc"] = "Tên nhà cung cấp",

            ["mahang"] = "Mã xe",
            ["tenhang"] = "Tên xe",
            ["maloai"] = "Mã thể loại",
            ["tenloai"] = "Thể loại",
            ["mahangsx"] = "Mã hãng sản xuất",
            ["tenhangsx"] = "Hãng sản xuất",
            ["mamau"] = "Mã màu",
            ["tenmau"] = "Màu sắc",
            ["namsx"] = "Năm sản xuất",
            ["maphanh"] = "Mã phanh",
            ["tenphanh"] = "Loại phanh",
            ["madongco"] = "Mã động cơ",
            ["tendongco"] = "Động cơ",
            ["manuocsx"] = "Mã nước sản xuất",
            ["tennuocsx"] = "Nước sản xuất",
            ["matt"] = "Mã tình trạng",
            ["tentt"] = "Tình trạng",
            ["dungtichbinhxang"] = "Dung tích bình xăng",
            ["anh"] = "Đường dẫn ảnh",
            ["thoigianbaohanh"] = "Thời gian bảo hành",
            ["soluong"] = "Số lượng",
            ["dongianhap"] = "Đơn giá nhập",
            ["dongiaban"] = "Đơn giá bán",
            ["dongia"] = "Đơn giá",
            ["giamgia"] = "Giảm giá (%)",
            ["thanhtien"] = "Thành tiền",
            ["tongtien"] = "Tổng tiền",

            ["soddh"] = "Mã đơn bán",
            ["ngaymua"] = "Ngày bán",
            ["sohdn"] = "Mã hóa đơn nhập",
            ["ngaynhap"] = "Ngày nhập",

            ["soluongban"] = "Số lượng bán",
            ["doanhthu"] = "Doanh thu",
            ["soluongnhap"] = "Số lượng nhập",
            ["tongtiennhap"] = "Tổng tiền nhập",

            ["chitieu"] = "Chỉ tiêu",
            ["giatri"] = "Giá trị",

            ["username"] = "Tên đăng nhập",
            ["displayname"] = "Tên hiển thị",
            ["roledisplayname"] = "Vai trò",
            ["statustext"] = "Trạng thái",
            ["securitynote"] = "Bảo mật",
            ["failedlogincount"] = "Số lần sai",
            ["lastloginat"] = "Đăng nhập gần nhất",
            ["passwordchangedat"] = "Đổi mật khẩu gần nhất",
            ["createdattext"] = "Thời gian",
            ["eventtype"] = "Sự kiện",
            ["usertext"] = "Tài khoản",
            ["detail"] = "Chi tiết"
        };

        private static readonly HashSet<string> NumericColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "luongthang", "namsx", "dungtichbinhxang", "thoigianbaohanh", "soluong",
            "dongianhap", "dongiaban", "dongia", "giamgia", "thanhtien", "tongtien",
            "soluongban", "doanhthu", "soluongnhap", "tongtiennhap", "failedlogincount"
        };

        private static readonly HashSet<string> DateColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ngaysinh", "ngaymua", "ngaynhap", "lastloginat", "passwordchangedat", "createdattext"
        };

        public static void Apply(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string key = Normalize(e.PropertyName);
            e.Column.Header = Format(e.PropertyName);
            e.Column.MinWidth = InferMinWidth(key);

            if (e.Column is DataGridBoundColumn boundColumn)
            {
                boundColumn.ElementStyle = CreateCellTextStyle(NumericColumns.Contains(key));
            }
        }

        public static string Format(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return string.Empty;

            string key = Normalize(columnName);
            if (HeaderMap.TryGetValue(key, out string header)) return header;

            // Keep already-friendly aliases such as "Số lượng bán" as-is.
            if (columnName.IndexOf(' ') >= 0) return columnName.Trim();

            return SplitPascalCase(columnName.Trim());
        }

        private static int InferMinWidth(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return 110;
            if (key.Contains("diachi") || key.Contains("anh") || key.Contains("detail")) return 180;
            if (DateColumns.Contains(key)) return 125;
            if (NumericColumns.Contains(key)) return 115;
            if (key.StartsWith("ma", StringComparison.OrdinalIgnoreCase) || key.Contains("code")) return 105;
            return 135;
        }

        private static Style CreateCellTextStyle(bool alignRight)
        {
            Style style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center));
            style.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(10, 0, 10, 0)));
            style.Setters.Add(new Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis));
            if (alignRight)
            {
                style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            }

            return style;
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return value.Replace(" ", string.Empty).Replace("_", string.Empty).Trim().ToLowerInvariant();
        }

        private static string SplitPascalCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                char current = value[i];
                if (i > 0 && char.IsUpper(current) && !char.IsWhiteSpace(value[i - 1]))
                {
                    builder.Append(' ');
                }
                builder.Append(current);
            }

            string result = builder.ToString();
            return result.Length <= 1 ? result.ToUpperInvariant() : char.ToUpperInvariant(result[0]) + result.Substring(1);
        }
    }
}
