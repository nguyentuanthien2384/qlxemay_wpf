using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLXeMay.Class;

namespace QLXeMay.Services
{
    internal sealed class GlobalSearchService : IGlobalSearchService
    {
        public DataTable Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new DataTable();

            string term = "%" + keyword.Trim() + "%";
            DataTable result = new DataTable();
            result.Columns.Add("Loại", typeof(string));
            result.Columns.Add("Mã", typeof(string));
            result.Columns.Add("Tên", typeof(string));
            result.Columns.Add("Chi tiết 1", typeof(string));
            result.Columns.Add("Chi tiết 2", typeof(string));
            result.Columns.Add("Chi tiết 3", typeof(string));

            // Search xe máy
            DataTable products = Function.GetDataToTable(
                @"SELECT mahang, tenhang, ISNULL(CAST(dongiaban AS NVARCHAR),'') AS dongiaban,
                         ISNULL(CAST(soluong AS NVARCHAR),'') AS soluong
                  FROM tbldmhang
                  WHERE mahang LIKE @term OR tenhang LIKE @term",
                Function.Param("@term", term));
            foreach (DataRow row in products.Rows)
            {
                result.Rows.Add("🏍 Xe máy", row["mahang"].ToString().Trim(),
                    row["tenhang"].ToString().Trim(),
                    "Giá bán: " + Function.ToDouble(row["dongiaban"].ToString()).ToString("N0"),
                    "Tồn kho: " + row["soluong"].ToString().Trim(), "");
            }

            // Search khách hàng
            DataTable customers = Function.GetDataToTable(
                @"SELECT makhach, tenkhach, ISNULL(diachi,'') AS diachi, ISNULL(sdt,'') AS sdt
                  FROM tblkhachhang
                  WHERE makhach LIKE @term OR tenkhach LIKE @term OR sdt LIKE @term",
                Function.Param("@term", term));
            foreach (DataRow row in customers.Rows)
            {
                result.Rows.Add("👥 Khách hàng", row["makhach"].ToString().Trim(),
                    row["tenkhach"].ToString().Trim(),
                    row["diachi"].ToString().Trim(),
                    row["sdt"].ToString().Trim(), "");
            }

            // Search nhân viên
            DataTable employees = Function.GetDataToTable(
                @"SELECT manv, tennv, ISNULL(sdt,'') AS sdt, ISNULL(diachi,'') AS diachi
                  FROM tblnhanvien
                  WHERE manv LIKE @term OR tennv LIKE @term OR sdt LIKE @term",
                Function.Param("@term", term));
            foreach (DataRow row in employees.Rows)
            {
                result.Rows.Add("👤 Nhân viên", row["manv"].ToString().Trim(),
                    row["tennv"].ToString().Trim(),
                    row["sdt"].ToString().Trim(),
                    row["diachi"].ToString().Trim(), "");
            }

            // Search đơn bán hàng
            DataTable salesOrders = Function.GetDataToTable(
                @"SELECT d.soddh, CONVERT(NVARCHAR, d.ngaymua, 103) AS ngaymua,
                         ISNULL(kh.tenkhach,'') AS tenkhach, ISNULL(nv.tennv,'') AS tennv,
                         ISNULL(CAST(d.tongtien AS NVARCHAR),'') AS tongtien
                  FROM tbldondathang d
                  LEFT JOIN tblkhachhang kh ON d.makhach=kh.makhach
                  LEFT JOIN tblnhanvien nv ON d.manv=nv.manv
                  WHERE d.soddh LIKE @term OR kh.tenkhach LIKE @term OR nv.tennv LIKE @term",
                Function.Param("@term", term));
            foreach (DataRow row in salesOrders.Rows)
            {
                result.Rows.Add("🧾 Đơn bán", row["soddh"].ToString().Trim(),
                    "KH: " + row["tenkhach"].ToString().Trim(),
                    "NV: " + row["tennv"].ToString().Trim(),
                    "Ngày: " + row["ngaymua"].ToString().Trim(),
                    "Tổng: " + Function.ToDouble(row["tongtien"].ToString()).ToString("N0"));
            }

            // Search hóa đơn nhập
            DataTable purchaseOrders = Function.GetDataToTable(
                @"SELECT h.sohdn, CONVERT(NVARCHAR, h.ngaynhap, 103) AS ngaynhap,
                         ISNULL(ncc.tenncc,'') AS tenncc, ISNULL(nv.tennv,'') AS tennv,
                         ISNULL(CAST(h.tongtien AS NVARCHAR),'') AS tongtien
                  FROM tblhoadonnhap h
                  LEFT JOIN tblnhacungcap ncc ON h.mancc=ncc.mancc
                  LEFT JOIN tblnhanvien nv ON h.manv=nv.manv
                  WHERE h.sohdn LIKE @term OR ncc.tenncc LIKE @term OR nv.tennv LIKE @term",
                Function.Param("@term", term));
            foreach (DataRow row in purchaseOrders.Rows)
            {
                result.Rows.Add("📦 HĐ nhập", row["sohdn"].ToString().Trim(),
                    "NCC: " + row["tenncc"].ToString().Trim(),
                    "NV: " + row["tennv"].ToString().Trim(),
                    "Ngày: " + row["ngaynhap"].ToString().Trim(),
                    "Tổng: " + Function.ToDouble(row["tongtien"].ToString()).ToString("N0"));
            }

            // Search nhà cung cấp
            DataTable suppliers = Function.GetDataToTable(
                @"SELECT mancc, tenncc, ISNULL(diachi,'') AS diachi, ISNULL(sdt,'') AS sdt
                  FROM tblnhacungcap
                  WHERE mancc LIKE @term OR tenncc LIKE @term OR sdt LIKE @term",
                Function.Param("@term", term));
            foreach (DataRow row in suppliers.Rows)
            {
                result.Rows.Add("🏭 NCC", row["mancc"].ToString().Trim(),
                    row["tenncc"].ToString().Trim(),
                    row["diachi"].ToString().Trim(),
                    row["sdt"].ToString().Trim(), "");
            }

            return result;
        }
    }
}
