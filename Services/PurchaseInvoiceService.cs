using System;
using System.Data;
using System.Data.SqlClient;
using QLXeMay.Class;
using QLXeMay.Domain;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal sealed class PurchaseInvoiceService : IPurchaseInvoiceService
    {
        public DataTable LoadEmployees() => Function.GetDataToTable("SELECT manv, manv FROM tblnhanvien");
        public DataTable LoadSuppliers() => Function.GetDataToTable("SELECT mancc, mancc FROM tblnhacungcap");
        public DataTable LoadProducts() => Function.GetDataToTable("SELECT mahang, mahang FROM tbldmhang");
        public DataTable LoadInvoiceNumbers() => Function.GetDataToTable("SELECT sohdn, sohdn FROM tblhoadonnhap");

        public string GetEmployeeName(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId)) return string.Empty;
            return Function.GetFieldValues("SELECT tennv FROM tblnhanvien WHERE manv=@manv", Function.Param("@manv", employeeId));
        }

        public PartyInfo GetSupplierInfo(string supplierId)
        {
            if (string.IsNullOrWhiteSpace(supplierId)) return new PartyInfo("");
            DataTable table = Function.GetDataToTable(
                "SELECT tenncc, diachi, sdt FROM tblnhacungcap WHERE mancc=@mancc",
                Function.Param("@mancc", supplierId));
            if (table.Rows.Count == 0) return new PartyInfo("");

            DataRow row = table.Rows[0];
            return new PartyInfo(row["tenncc"].ToString(), row["diachi"].ToString(), row["sdt"].ToString());
        }

        public string GetProductName(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId)) return string.Empty;
            return Function.GetFieldValues("SELECT tenhang FROM tbldmhang WHERE mahang=@mahang", Function.Param("@mahang", productId));
        }

        public PurchaseInvoiceOperationResult SaveLine(PurchaseInvoiceRequest request)
        {
            PurchaseInvoiceOperationResult result = new PurchaseInvoiceOperationResult();

            bool saved = Function.ExecuteTransaction((connection, transaction) =>
            {
                EnsureHeader(connection, transaction, request);

                if (Function.CheckKey(connection, transaction,
                    "SELECT 1 FROM tblchitiethdn WHERE sohdn=@sohdn AND mahang=@mahang",
                    Function.Param("@sohdn", request.InvoiceNo),
                    Function.Param("@mahang", request.ProductId)))
                {
                    throw new InvalidOperationException("Mặt hàng này đã có trong hóa đơn. Hãy xóa dòng cũ nếu muốn nhập lại.");
                }

                result.LineTotal = InvoiceCalculator.CalculateLineTotal(request.Quantity, request.UnitPrice, request.Discount);

                Function.ExecuteSql(connection, transaction,
                    "INSERT INTO tblchitiethdn(sohdn, mahang, soluong, dongia, giamgia, thanhtien) VALUES(@sohdn, @mahang, @soluong, @dongia, @giamgia, @thanhtien)",
                    Function.Param("@sohdn", request.InvoiceNo),
                    Function.Param("@mahang", request.ProductId),
                    Function.Param("@soluong", request.Quantity),
                    Function.Param("@dongia", request.UnitPrice),
                    Function.Param("@giamgia", request.Discount),
                    Function.Param("@thanhtien", result.LineTotal));

                int stock = Function.ToInt(Function.GetFieldValues(connection, transaction,
                    "SELECT soluong FROM tbldmhang WHERE mahang=@mahang",
                    Function.Param("@mahang", request.ProductId)));

                Function.ExecuteSql(connection, transaction,
                    "UPDATE tbldmhang SET soluong=@soluong, dongianhap=@dongianhap, dongiaban=@dongiaban WHERE mahang=@mahang",
                    Function.Param("@soluong", InvoiceCalculator.StockAfterPurchase(stock, request.Quantity)),
                    Function.Param("@dongianhap", request.UnitPrice),
                    Function.Param("@dongiaban", InvoiceCalculator.CalculateSuggestedSellingPrice(request.UnitPrice)),
                    Function.Param("@mahang", request.ProductId));

                UpdateTotal(connection, transaction, request.InvoiceNo, result);
                result.Details = LoadDetails(connection, transaction, request.InvoiceNo);
            }, "Không thể lưu hóa đơn nhập.");

            if (!saved) throw new InvalidOperationException("Không thể lưu hóa đơn nhập.");
            return result;
        }

        public PurchaseInvoiceOperationResult DeleteLine(string invoiceNo, string productId, int quantity)
        {
            PurchaseInvoiceOperationResult result = new PurchaseInvoiceOperationResult();

            bool deleted = Function.ExecuteTransaction((connection, transaction) =>
            {
                Function.ExecuteSql(connection, transaction,
                    "DELETE FROM tblchitiethdn WHERE sohdn=@sohdn AND mahang=@mahang",
                    Function.Param("@sohdn", invoiceNo),
                    Function.Param("@mahang", productId));

                int stock = Function.ToInt(Function.GetFieldValues(connection, transaction,
                    "SELECT soluong FROM tbldmhang WHERE mahang=@mahang",
                    Function.Param("@mahang", productId)));

                Function.ExecuteSql(connection, transaction,
                    "UPDATE tbldmhang SET soluong=@soluong WHERE mahang=@mahang",
                    Function.Param("@soluong", InvoiceCalculator.StockAfterPurchaseRollback(stock, quantity)),
                    Function.Param("@mahang", productId));

                UpdateTotal(connection, transaction, invoiceNo, result);
                result.Details = LoadDetails(connection, transaction, invoiceNo);
            }, "Không thể xóa dòng hàng.");

            if (!deleted) throw new InvalidOperationException("Không thể xóa dòng hàng.");
            return result;
        }

        public void CancelInvoice(string invoiceNo)
        {
            bool cancelled = Function.ExecuteTransaction((connection, transaction) =>
            {
                DataTable details = Function.GetDataToTable(connection, transaction,
                    "SELECT mahang, soluong FROM tblchitiethdn WHERE sohdn=@sohdn",
                    Function.Param("@sohdn", invoiceNo));

                foreach (DataRow row in details.Rows)
                {
                    string productId = row["mahang"].ToString().Trim();
                    int quantity = Function.ToInt(row["soluong"].ToString());
                    int stock = Function.ToInt(Function.GetFieldValues(connection, transaction,
                        "SELECT soluong FROM tbldmhang WHERE mahang=@mahang",
                        Function.Param("@mahang", productId)));

                    Function.ExecuteSql(connection, transaction,
                        "UPDATE tbldmhang SET soluong=@soluong WHERE mahang=@mahang",
                        Function.Param("@soluong", InvoiceCalculator.StockAfterPurchaseRollback(stock, quantity)),
                        Function.Param("@mahang", productId));
                }

                Function.ExecuteSql(connection, transaction,
                    "DELETE FROM tblchitiethdn WHERE sohdn=@sohdn",
                    Function.Param("@sohdn", invoiceNo));
                Function.ExecuteSql(connection, transaction,
                    "DELETE FROM tblhoadonnhap WHERE sohdn=@sohdn",
                    Function.Param("@sohdn", invoiceNo));
            }, "Không thể hủy hóa đơn nhập.");

            if (!cancelled) throw new InvalidOperationException("Không thể hủy hóa đơn nhập.");
        }

        public PurchaseInvoiceSnapshot LoadInvoice(string invoiceNo)
        {
            DataTable table = Function.GetDataToTable(
                "SELECT * FROM tblhoadonnhap WHERE sohdn=@sohdn",
                Function.Param("@sohdn", invoiceNo));
            if (table.Rows.Count == 0) return null;

            DataRow row = table.Rows[0];
            return new PurchaseInvoiceSnapshot
            {
                InvoiceNo = invoiceNo,
                EmployeeId = row["manv"].ToString().Trim(),
                SupplierId = row["mancc"].ToString().Trim(),
                InvoiceDate = Convert.ToDateTime(row["ngaynhap"]),
                Total = Function.ToDouble(row["tongtien"].ToString()),
                Details = LoadDetails(invoiceNo)
            };
        }

        private static void EnsureHeader(SqlConnection connection, SqlTransaction transaction, PurchaseInvoiceRequest request)
        {
            if (Function.CheckKey(connection, transaction,
                "SELECT 1 FROM tblhoadonnhap WHERE sohdn=@sohdn",
                Function.Param("@sohdn", request.InvoiceNo)))
            {
                return;
            }

            Function.ExecuteSql(connection, transaction,
                "INSERT INTO tblhoadonnhap(sohdn, manv, ngaynhap, mancc, tongtien) VALUES(@sohdn, @manv, @ngaynhap, @mancc, 0)",
                Function.Param("@sohdn", request.InvoiceNo),
                Function.Param("@manv", request.EmployeeId),
                Function.Param("@ngaynhap", request.InvoiceDate),
                Function.Param("@mancc", request.SupplierId));
        }

        private static void UpdateTotal(SqlConnection connection, SqlTransaction transaction, string invoiceNo, PurchaseInvoiceOperationResult result)
        {
            result.Total = Function.ToDouble(Function.GetFieldValues(connection, transaction,
                "SELECT ISNULL(SUM(thanhtien),0) FROM tblchitiethdn WHERE sohdn=@sohdn",
                Function.Param("@sohdn", invoiceNo)));

            Function.ExecuteSql(connection, transaction,
                "UPDATE tblhoadonnhap SET tongtien=@tongtien WHERE sohdn=@sohdn",
                Function.Param("@tongtien", result.Total),
                Function.Param("@sohdn", invoiceNo));
        }

        private static DataTable LoadDetails(string invoiceNo)
        {
            return Function.GetDataToTable(
                "SELECT ct.mahang, h.tenhang, ct.soluong, ct.dongia, ct.giamgia, ct.thanhtien FROM tblchitiethdn ct INNER JOIN tbldmhang h ON ct.mahang=h.mahang WHERE ct.sohdn=@sohdn",
                Function.Param("@sohdn", invoiceNo));
        }

        private static DataTable LoadDetails(SqlConnection connection, SqlTransaction transaction, string invoiceNo)
        {
            return Function.GetDataToTable(connection, transaction,
                "SELECT ct.mahang, h.tenhang, ct.soluong, ct.dongia, ct.giamgia, ct.thanhtien FROM tblchitiethdn ct INNER JOIN tbldmhang h ON ct.mahang=h.mahang WHERE ct.sohdn=@sohdn",
                Function.Param("@sohdn", invoiceNo));
        }
    }
}
