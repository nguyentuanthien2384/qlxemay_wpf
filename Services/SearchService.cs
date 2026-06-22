using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLXeMay.Class;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal sealed class SearchService : ISearchService
    {
        public DataTable Search(SearchMode mode, IReadOnlyDictionary<string, string> criteria)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string sql;

            if (mode == SearchMode.Hang)
            {
                sql = "SELECT h.mahang, h.tenhang, l.tenloai, hs.tenhangsx, m.tenmau, h.namsx, h.soluong, h.dongianhap, h.dongiaban, tt.tentt " +
                      "FROM tbldmhang h LEFT JOIN tbltheloai l ON h.maloai=l.maloai LEFT JOIN tblhangsx hs ON h.mahangsx=hs.mahangsx " +
                      "LEFT JOIN tblmausac m ON h.mamau=m.mamau LEFT JOIN tbltinhtrang tt ON h.matt=tt.matt WHERE 1=1";
                AddLikeFilter(ref sql, parameters, "h.tenhang", "@tenhang", GetValue(criteria, "tenhang"));
                AddEqualsFilter(ref sql, parameters, "h.maloai", "@maloai", GetValue(criteria, "maloai"));
                AddEqualsFilter(ref sql, parameters, "h.mahangsx", "@mahangsx", GetValue(criteria, "mahangsx"));
                AddEqualsFilter(ref sql, parameters, "h.matt", "@matt", GetValue(criteria, "matt"));
            }
            else if (mode == SearchMode.KhachHang)
            {
                sql = "SELECT * FROM tblkhachhang WHERE 1=1";
                AddLikeFilter(ref sql, parameters, "makhach", "@makhach", GetValue(criteria, "makhach"));
                AddLikeFilter(ref sql, parameters, "tenkhach", "@tenkhach", GetValue(criteria, "tenkhach"));
                AddLikeFilter(ref sql, parameters, "diachi", "@diachi", GetValue(criteria, "diachi"));
                AddLikeFilter(ref sql, parameters, "sdt", "@sdt", GetValue(criteria, "sdt"));
            }
            else if (mode == SearchMode.HoaDonNhap)
            {
                sql = "SELECT h.sohdn, h.ngaynhap, ncc.tenncc, nv.tennv, ct.mahang, d.tenhang, ct.soluong, ct.dongia, ct.giamgia, ct.thanhtien " +
                      "FROM tblhoadonnhap h INNER JOIN tblchitiethdn ct ON h.sohdn=ct.sohdn INNER JOIN tbldmhang d ON ct.mahang=d.mahang " +
                      "LEFT JOIN tblnhacungcap ncc ON h.mancc=ncc.mancc LEFT JOIN tblnhanvien nv ON h.manv=nv.manv WHERE 1=1";
                AddEqualsFilter(ref sql, parameters, "h.sohdn", "@sohdn", GetValue(criteria, "sohdn"));
                AddEqualsFilter(ref sql, parameters, "h.mancc", "@mancc", GetValue(criteria, "mancc"));
                AddEqualsFilter(ref sql, parameters, "h.manv", "@manv", GetValue(criteria, "manv"));
            }
            else
            {
                sql = "SELECT h.soddh, h.ngaymua, kh.tenkhach, nv.tennv, ct.mahang, d.tenhang, ct.soluong, ct.giamgia, ct.thanhtien " +
                      "FROM tbldondathang h INNER JOIN tblchitietddh ct ON h.soddh=ct.soddh INNER JOIN tbldmhang d ON ct.mahang=d.mahang " +
                      "LEFT JOIN tblkhachhang kh ON h.makhach=kh.makhach LEFT JOIN tblnhanvien nv ON h.manv=nv.manv WHERE 1=1";
                AddEqualsFilter(ref sql, parameters, "h.soddh", "@soddh", GetValue(criteria, "soddh"));
                AddEqualsFilter(ref sql, parameters, "h.makhach", "@makhach", GetValue(criteria, "makhach"));
                AddEqualsFilter(ref sql, parameters, "h.manv", "@manv", GetValue(criteria, "manv"));
            }

            return Function.GetDataToTable(sql, parameters.ToArray());
        }

        private static string GetValue(IReadOnlyDictionary<string, string> criteria, string key)
        {
            return criteria != null && criteria.ContainsKey(key) ? criteria[key] : string.Empty;
        }

        private static void AddLikeFilter(ref string sql, List<SqlParameter> parameters, string column, string parameterName, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            sql += $" AND {column} LIKE {parameterName}";
            parameters.Add(Function.Param(parameterName, "%" + value.Trim() + "%"));
        }

        private static void AddEqualsFilter(ref string sql, List<SqlParameter> parameters, string column, string parameterName, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            sql += $" AND {column}={parameterName}";
            parameters.Add(Function.Param(parameterName, value.Trim()));
        }
    }
}
