using System;
using System.Data;
using System.Data.SqlClient;
using QLXeMay.Class;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal sealed class ReportService : IReportService
    {
        public ReportResult BuildReport(ReportMode mode, DateTime fromDate, DateTime toDate, string groupKey)
        {
            if (mode == ReportMode.BanHang) return BuildSalesReport(fromDate, toDate);
            if (mode == ReportMode.NhapHang) return BuildPurchaseReport(fromDate, toDate);
            if (mode == ReportMode.KetQuaKinhDoanh) return BuildBusinessResultReport(fromDate, toDate);
            return BuildTopProductReport(groupKey);
        }

        private static ReportResult BuildSalesReport(DateTime fromDate, DateTime toDate)
        {
            DateRangeFilter where = DateWhere("ddh.ngaymua", fromDate, toDate);
            string sql = "SELECT ddh.soddh, ddh.ngaymua, kh.tenkhach, nv.tennv, h.mahang, h.tenhang, ct.soluong, ct.giamgia, ct.thanhtien " +
                         "FROM tbldondathang ddh INNER JOIN tblchitietddh ct ON ddh.soddh=ct.soddh " +
                         "INNER JOIN tbldmhang h ON ct.mahang=h.mahang LEFT JOIN tblkhachhang kh ON ddh.makhach=kh.makhach " +
                         "LEFT JOIN tblnhanvien nv ON ddh.manv=nv.manv WHERE 1=1" + where.Sql + " ORDER BY ddh.ngaymua DESC";
            string summarySql = "SELECT h.tenhang, SUM(ct.soluong) AS SoLuongBan, SUM(ct.thanhtien) AS DoanhThu " +
                                "FROM tbldondathang ddh INNER JOIN tblchitietddh ct ON ddh.soddh=ct.soddh INNER JOIN tbldmhang h ON ct.mahang=h.mahang " +
                                "WHERE 1=1" + where.Sql + " GROUP BY h.tenhang ORDER BY SUM(ct.soluong) DESC";

            DataTable main = Function.GetDataToTable(sql, where.Parameters);
            DataTable summary = Function.GetDataToTable(summarySql, where.Parameters);
            double total = Function.ToDouble(Function.GetFieldValues("SELECT ISNULL(SUM(tongtien),0) FROM tbldondathang ddh WHERE 1=1" + where.Sql, where.Parameters));
            return new ReportResult(main, summary, $"Số dòng chi tiết: {main.Rows.Count} | Tổng doanh thu: {total:N0} VNĐ");
        }

        private static ReportResult BuildPurchaseReport(DateTime fromDate, DateTime toDate)
        {
            DateRangeFilter where = DateWhere("hdn.ngaynhap", fromDate, toDate);
            string sql = "SELECT hdn.sohdn, hdn.ngaynhap, ncc.tenncc, nv.tennv, h.mahang, h.tenhang, ct.soluong, ct.dongia, ct.giamgia, ct.thanhtien " +
                         "FROM tblhoadonnhap hdn INNER JOIN tblchitiethdn ct ON hdn.sohdn=ct.sohdn " +
                         "INNER JOIN tbldmhang h ON ct.mahang=h.mahang LEFT JOIN tblnhacungcap ncc ON hdn.mancc=ncc.mancc " +
                         "LEFT JOIN tblnhanvien nv ON hdn.manv=nv.manv WHERE 1=1" + where.Sql + " ORDER BY hdn.ngaynhap DESC";
            string summarySql = "SELECT h.tenhang, SUM(ct.soluong) AS SoLuongNhap, SUM(ct.thanhtien) AS TongTienNhap " +
                                "FROM tblhoadonnhap hdn INNER JOIN tblchitiethdn ct ON hdn.sohdn=ct.sohdn INNER JOIN tbldmhang h ON ct.mahang=h.mahang " +
                                "WHERE 1=1" + where.Sql + " GROUP BY h.tenhang ORDER BY SUM(ct.soluong) DESC";

            DataTable main = Function.GetDataToTable(sql, where.Parameters);
            DataTable summary = Function.GetDataToTable(summarySql, where.Parameters);
            double total = Function.ToDouble(Function.GetFieldValues("SELECT ISNULL(SUM(tongtien),0) FROM tblhoadonnhap hdn WHERE 1=1" + where.Sql, where.Parameters));
            return new ReportResult(main, summary, $"Số dòng chi tiết: {main.Rows.Count} | Tổng tiền nhập: {total:N0} VNĐ");
        }

        private static ReportResult BuildBusinessResultReport(DateTime fromDate, DateTime toDate)
        {
            DateRangeFilter salesWhere = DateWhere("ddh.ngaymua", fromDate, toDate);
            DateRangeFilter purchaseWhere = DateWhere("hdn.ngaynhap", fromDate, toDate);
            double salesTotal = Function.ToDouble(Function.GetFieldValues("SELECT ISNULL(SUM(tongtien),0) FROM tbldondathang ddh WHERE 1=1" + salesWhere.Sql, salesWhere.Parameters));
            double purchaseTotal = Function.ToDouble(Function.GetFieldValues("SELECT ISNULL(SUM(tongtien),0) FROM tblhoadonnhap hdn WHERE 1=1" + purchaseWhere.Sql, purchaseWhere.Parameters));
            double profit = salesTotal - purchaseTotal;

            DataTable main = new DataTable();
            main.Columns.Add("Chỉ tiêu");
            main.Columns.Add("Giá trị");
            main.Rows.Add("Tổng doanh thu bán", salesTotal.ToString("N0") + " VNĐ");
            main.Rows.Add("Tổng chi phí nhập", purchaseTotal.ToString("N0") + " VNĐ");
            main.Rows.Add("Lợi nhuận ước tính", profit.ToString("N0") + " VNĐ");
            main.Rows.Add("Biên lợi nhuận", salesTotal > 0 ? (profit / salesTotal * 100).ToString("F2") + "%" : "0%");

            string topSql = "SELECT TOP 10 h.tenhang, SUM(ct.soluong) AS SoLuongBan, SUM(ct.thanhtien) AS DoanhThu " +
                            "FROM tbldondathang ddh INNER JOIN tblchitietddh ct ON ddh.soddh=ct.soddh INNER JOIN tbldmhang h ON ct.mahang=h.mahang " +
                            "WHERE 1=1" + salesWhere.Sql + " GROUP BY h.tenhang ORDER BY SUM(ct.thanhtien) DESC";
            DataTable summary = Function.GetDataToTable(topSql, salesWhere.Parameters);
            return new ReportResult(main, summary, $"Doanh thu: {salesTotal:N0} | Chi phí nhập: {purchaseTotal:N0} | Lợi nhuận: {profit:N0} VNĐ");
        }

        private static ReportResult BuildTopProductReport(string groupKey)
        {
            string groupField = "h.tenhang";
            string name = "Sản phẩm";
            string join = "";
            if (groupKey == "theloai") { groupField = "l.tenloai"; name = "Thể loại"; join = " LEFT JOIN tbltheloai l ON h.maloai=l.maloai"; }
            if (groupKey == "hangsx") { groupField = "sx.tenhangsx"; name = "Hãng sản xuất"; join = " LEFT JOIN tblhangsx sx ON h.mahangsx=sx.mahangsx"; }

            string sql = $"SELECT TOP 10 {groupField} AS [{name}], SUM(ct.soluong) AS [Số lượng bán], SUM(ct.thanhtien) AS [Doanh thu] " +
                         "FROM tblchitietddh ct INNER JOIN tbldmhang h ON ct.mahang=h.mahang" + join +
                         $" GROUP BY {groupField} ORDER BY SUM(ct.soluong) DESC";
            DataTable table = Function.GetDataToTable(sql);
            return new ReportResult(table, null, "Top 10 theo " + name.ToLower() + ": " + table.Rows.Count + " dòng dữ liệu.");
        }

        private static DateRangeFilter DateWhere(string column, DateTime fromDate, DateTime toDate)
        {
            return new DateRangeFilter(
                $" AND {column} >= @fromDate AND {column} < @toDate",
                Function.Param("@fromDate", fromDate.Date),
                Function.Param("@toDate", toDate.Date.AddDays(1)));
        }

        private sealed class DateRangeFilter
        {
            public DateRangeFilter(string sql, params SqlParameter[] parameters)
            {
                Sql = sql;
                Parameters = parameters;
            }

            public string Sql { get; }
            public SqlParameter[] Parameters { get; }
        }
    }
}
