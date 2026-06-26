using System;
using System.Data;
using System.Data.SqlClient;
using QLXeMay.Class;
using QLXeMay.Domain;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal sealed class SalesInvoiceService : ISalesInvoiceService
    {
        public DataTable LoadEmployees() => Function.GetDataToTable("SELECT manv, manv FROM tblnhanvien");
        public DataTable LoadCustomers() => Function.GetDataToTable(@"
            SELECT makhach,
                   CONCAT(makhach, ' - ', ISNULL(tenkhach, N'Chưa có tên')) AS CustomerDisplay
            FROM tblkhachhang
            ORDER BY makhach");
        public DataTable LoadProducts() => Function.GetDataToTable("SELECT mahang, mahang FROM tbldmhang");
        public DataTable LoadInvoiceNumbers() => Function.GetDataToTable("SELECT soddh, soddh FROM tbldondathang");

        public string GetEmployeeName(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId)) return string.Empty;
            return Function.GetFieldValues("SELECT tennv FROM tblnhanvien WHERE manv=@manv", Function.Param("@manv", employeeId));
        }

        public PartyInfo GetCustomerInfo(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId)) return new PartyInfo("");
            DataTable table = Function.GetDataToTable(
                "SELECT tenkhach, diachi, sdt FROM tblkhachhang WHERE makhach=@makhach",
                Function.Param("@makhach", customerId));
            if (table.Rows.Count == 0) return new PartyInfo("");

            DataRow row = table.Rows[0];
            return new PartyInfo(row["tenkhach"].ToString(), row["diachi"].ToString(), row["sdt"].ToString());
        }

        public string GetProductName(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId)) return string.Empty;
            return Function.GetFieldValues("SELECT tenhang FROM tbldmhang WHERE mahang=@mahang", Function.Param("@mahang", productId));
        }

        public SalesInvoiceOperationResult SaveLine(SalesInvoiceRequest request)
        {
            SalesInvoiceOperationResult result = new SalesInvoiceOperationResult();

            bool saved = Function.ExecuteTransaction((connection, transaction) =>
            {
                int stock = Function.ToInt(Function.GetFieldValues(connection, transaction,
                    "SELECT soluong FROM tbldmhang WHERE mahang=@mahang",
                    Function.Param("@mahang", request.ProductId)));

                if (request.Quantity > stock)
                {
                    throw new InvalidOperationException($"Số lượng tồn kho không đủ. Hiện còn {stock} sản phẩm.");
                }

                EnsureHeader(connection, transaction, request);

                if (Function.CheckKey(connection, transaction,
                    "SELECT 1 FROM tblchitietddh WHERE soddh=@soddh AND mahang=@mahang",
                    Function.Param("@soddh", request.InvoiceNo),
                    Function.Param("@mahang", request.ProductId)))
                {
                    throw new InvalidOperationException("Mặt hàng này đã có trong hóa đơn. Hãy xóa dòng cũ nếu muốn nhập lại.");
                }

                double price = Function.ToDouble(Function.GetFieldValues(connection, transaction,
                    "SELECT dongiaban FROM tbldmhang WHERE mahang=@mahang",
                    Function.Param("@mahang", request.ProductId)));
                result.LineTotal = InvoiceCalculator.CalculateLineTotal(request.Quantity, price, request.Discount);

                Function.ExecuteSql(connection, transaction,
                    "INSERT INTO tblchitietddh(soddh, mahang, soluong, giamgia, thanhtien) VALUES(@soddh, @mahang, @soluong, @giamgia, @thanhtien)",
                    Function.Param("@soddh", request.InvoiceNo),
                    Function.Param("@mahang", request.ProductId),
                    Function.Param("@soluong", request.Quantity),
                    Function.Param("@giamgia", request.Discount),
                    Function.Param("@thanhtien", result.LineTotal));

                Function.ExecuteSql(connection, transaction,
                    "UPDATE tbldmhang SET soluong=@soluong WHERE mahang=@mahang",
                    Function.Param("@soluong", InvoiceCalculator.StockAfterSale(stock, request.Quantity)),
                    Function.Param("@mahang", request.ProductId));

                UpdateTotals(connection, transaction, request.InvoiceNo, result);
                result.Details = LoadDetails(connection, transaction, request.InvoiceNo);
            }, "Không thể lưu hóa đơn.");

            if (!saved) throw new InvalidOperationException("Không thể lưu hóa đơn.");
            return result;
        }

        public SalesInvoiceOperationResult DeleteLine(string invoiceNo, string productId, int quantity)
        {
            SalesInvoiceOperationResult result = new SalesInvoiceOperationResult();

            bool deleted = Function.ExecuteTransaction((connection, transaction) =>
            {
                Function.ExecuteSql(connection, transaction,
                    "DELETE FROM tblchitietddh WHERE soddh=@soddh AND mahang=@mahang",
                    Function.Param("@soddh", invoiceNo),
                    Function.Param("@mahang", productId));

                int stock = Function.ToInt(Function.GetFieldValues(connection, transaction,
                    "SELECT soluong FROM tbldmhang WHERE mahang=@mahang",
                    Function.Param("@mahang", productId)));

                Function.ExecuteSql(connection, transaction,
                    "UPDATE tbldmhang SET soluong=@soluong WHERE mahang=@mahang",
                    Function.Param("@soluong", InvoiceCalculator.StockAfterSaleRollback(stock, quantity)),
                    Function.Param("@mahang", productId));

                UpdateTotals(connection, transaction, invoiceNo, result);
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
                    "SELECT mahang, soluong FROM tblchitietddh WHERE soddh=@soddh",
                    Function.Param("@soddh", invoiceNo));

                foreach (DataRow row in details.Rows)
                {
                    string productId = row["mahang"].ToString().Trim();
                    int quantity = Function.ToInt(row["soluong"].ToString());
                    int stock = Function.ToInt(Function.GetFieldValues(connection, transaction,
                        "SELECT soluong FROM tbldmhang WHERE mahang=@mahang",
                        Function.Param("@mahang", productId)));

                    Function.ExecuteSql(connection, transaction,
                        "UPDATE tbldmhang SET soluong=@soluong WHERE mahang=@mahang",
                        Function.Param("@soluong", InvoiceCalculator.StockAfterSaleRollback(stock, quantity)),
                        Function.Param("@mahang", productId));
                }

                Function.ExecuteSql(connection, transaction,
                    "DELETE FROM tblchitietddh WHERE soddh=@soddh",
                    Function.Param("@soddh", invoiceNo));
                Function.ExecuteSql(connection, transaction,
                    "DELETE FROM tbldondathang WHERE soddh=@soddh",
                    Function.Param("@soddh", invoiceNo));
            }, "Không thể hủy hóa đơn.");

            if (!cancelled) throw new InvalidOperationException("Không thể hủy hóa đơn.");
        }

        public SalesInvoiceSnapshot LoadInvoice(string invoiceNo)
        {
            DataTable table = Function.GetDataToTable(
                "SELECT * FROM tbldondathang WHERE soddh=@soddh",
                Function.Param("@soddh", invoiceNo));
            if (table.Rows.Count == 0) return null;

            DataRow row = table.Rows[0];
            return new SalesInvoiceSnapshot
            {
                InvoiceNo = invoiceNo,
                EmployeeId = row["manv"].ToString().Trim(),
                CustomerId = row["makhach"].ToString().Trim(),
                InvoiceDate = Convert.ToDateTime(row["ngaymua"]),
                Total = Function.ToDouble(row["tongtien"].ToString()),
                Tax = Function.ToDouble(row["thue"].ToString()),
                Deposit = Function.ToDouble(row["datcoc"].ToString()),
                Details = LoadDetails(invoiceNo)
            };
        }

        private static void EnsureHeader(SqlConnection connection, SqlTransaction transaction, SalesInvoiceRequest request)
        {
            if (Function.CheckKey(connection, transaction,
                "SELECT 1 FROM tbldondathang WHERE soddh=@soddh",
                Function.Param("@soddh", request.InvoiceNo)))
            {
                return;
            }

            Function.ExecuteSql(connection, transaction,
                "INSERT INTO tbldondathang(soddh, manv, ngaymua, makhach, datcoc, thue, tongtien) VALUES(@soddh, @manv, @ngaymua, @makhach, 0, 0, 0)",
                Function.Param("@soddh", request.InvoiceNo),
                Function.Param("@manv", request.EmployeeId),
                Function.Param("@ngaymua", request.InvoiceDate),
                Function.Param("@makhach", request.CustomerId));
        }

        private static void UpdateTotals(SqlConnection connection, SqlTransaction transaction, string invoiceNo, SalesInvoiceOperationResult result)
        {
            result.Total = Function.ToDouble(Function.GetFieldValues(connection, transaction,
                "SELECT ISNULL(SUM(thanhtien),0) FROM tblchitietddh WHERE soddh=@soddh",
                Function.Param("@soddh", invoiceNo)));
            result.Tax = InvoiceCalculator.CalculateSalesTax(result.Total);
            result.Deposit = InvoiceCalculator.CalculateSalesDeposit(result.Total);

            Function.ExecuteSql(connection, transaction,
                "UPDATE tbldondathang SET tongtien=@tongtien, thue=@thue, datcoc=@datcoc WHERE soddh=@soddh",
                Function.Param("@tongtien", result.Total),
                Function.Param("@thue", result.Tax),
                Function.Param("@datcoc", result.Deposit),
                Function.Param("@soddh", invoiceNo));
        }

        private static DataTable LoadDetails(string invoiceNo)
        {
            return Function.GetDataToTable(
                "SELECT ct.mahang, h.tenhang, ct.soluong, ct.giamgia, ct.thanhtien FROM tblchitietddh ct INNER JOIN tbldmhang h ON ct.mahang=h.mahang WHERE ct.soddh=@soddh",
                Function.Param("@soddh", invoiceNo));
        }

        private static DataTable LoadDetails(SqlConnection connection, SqlTransaction transaction, string invoiceNo)
        {
            return Function.GetDataToTable(connection, transaction,
                "SELECT ct.mahang, h.tenhang, ct.soluong, ct.giamgia, ct.thanhtien FROM tblchitietddh ct INNER JOIN tbldmhang h ON ct.mahang=h.mahang WHERE ct.soddh=@soddh",
                Function.Param("@soddh", invoiceNo));
        }
    }
}
