using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QLXeMay.Class;
using QLXeMay.Domain;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    /// <summary>
    /// Backs the customer self-service storefront: browsing the catalog and placing orders.
    /// Orders are written to the same tables staff sales use (tbldondathang/tblchitietddh),
    /// but with the seeded online-sales employee so existing reports keep working unchanged.
    /// </summary>
    internal sealed class StoreService : IStoreService
    {
        public IReadOnlyList<StoreProduct> LoadProducts(string keyword)
        {
            string sql = @"
SELECT h.mahang, h.tenhang, h.namsx, h.thoigianbaohanh, h.soluong, h.dongiaban, h.anh,
       l.tenloai, s.tenhangsx, m.tenmau
FROM tbldmhang h
LEFT JOIN tbltheloai l ON h.maloai = l.maloai
LEFT JOIN tblhangsx s ON h.mahangsx = s.mahangsx
LEFT JOIN tblmausac m ON h.mamau = m.mamau";

            DataTable table;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                sql += " ORDER BY h.tenhang";
                table = Function.GetDataToTable(sql);
            }
            else
            {
                sql += " WHERE h.tenhang LIKE @kw OR s.tenhangsx LIKE @kw OR l.tenloai LIKE @kw ORDER BY h.tenhang";
                table = Function.GetDataToTable(sql, Function.Param("@kw", "%" + keyword.Trim() + "%"));
            }

            List<StoreProduct> products = new List<StoreProduct>();
            foreach (DataRow row in table.Rows)
            {
                products.Add(new StoreProduct
                {
                    ProductId = row["mahang"].ToString().Trim(),
                    Name = row["tenhang"].ToString(),
                    Category = AsString(row["tenloai"]),
                    Brand = AsString(row["tenhangsx"]),
                    Color = AsString(row["tenmau"]),
                    Year = Function.ToInt(row["namsx"].ToString()),
                    WarrantyMonths = Function.ToInt(row["thoigianbaohanh"].ToString()),
                    UnitPrice = Function.ToDouble(row["dongiaban"].ToString()),
                    Stock = Function.ToInt(row["soluong"].ToString()),
                    ImagePath = AsString(row["anh"])
                });
            }

            return products;
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

        public StoreOrderResult PlaceOrder(string customerId, IEnumerable<CartLine> lines)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new InvalidOperationException("Không xác định được tài khoản khách hàng.");
            }

            Dictionary<string, int> quantities = AggregateLines(lines);
            if (quantities.Count == 0)
            {
                throw new InvalidOperationException("Giỏ hàng đang trống.");
            }

            StoreOrderResult result = new StoreOrderResult { InvoiceNo = CreateInvoiceNo() };

            bool placed = Function.ExecuteTransaction((connection, transaction) =>
            {
                Function.ExecuteSql(connection, transaction,
                    "INSERT INTO tbldondathang(soddh, manv, ngaymua, makhach, datcoc, thue, tongtien) VALUES(@soddh, @manv, @ngaymua, @makhach, 0, 0, 0)",
                    Function.Param("@soddh", result.InvoiceNo),
                    Function.Param("@manv", AccessControl.OnlineSalesEmployeeId),
                    Function.Param("@ngaymua", DateTime.Today),
                    Function.Param("@makhach", customerId));

                foreach (KeyValuePair<string, int> entry in quantities)
                {
                    string productId = entry.Key;
                    int quantity = entry.Value;

                    int stock = Function.ToInt(Function.GetFieldValues(connection, transaction,
                        "SELECT soluong FROM tbldmhang WHERE mahang=@mahang",
                        Function.Param("@mahang", productId)));

                    string productName = Function.GetFieldValues(connection, transaction,
                        "SELECT tenhang FROM tbldmhang WHERE mahang=@mahang",
                        Function.Param("@mahang", productId));

                    if (quantity > stock)
                    {
                        throw new InvalidOperationException(
                            $"Sản phẩm \"{productName}\" chỉ còn {stock} chiếc, không đủ cho {quantity} chiếc.");
                    }

                    double price = Function.ToDouble(Function.GetFieldValues(connection, transaction,
                        "SELECT dongiaban FROM tbldmhang WHERE mahang=@mahang",
                        Function.Param("@mahang", productId)));
                    double lineTotal = InvoiceCalculator.CalculateLineTotal(quantity, price, 0);

                    Function.ExecuteSql(connection, transaction,
                        "INSERT INTO tblchitietddh(soddh, mahang, soluong, giamgia, thanhtien) VALUES(@soddh, @mahang, @soluong, 0, @thanhtien)",
                        Function.Param("@soddh", result.InvoiceNo),
                        Function.Param("@mahang", productId),
                        Function.Param("@soluong", quantity),
                        Function.Param("@thanhtien", lineTotal));

                    Function.ExecuteSql(connection, transaction,
                        "UPDATE tbldmhang SET soluong=@soluong WHERE mahang=@mahang",
                        Function.Param("@soluong", InvoiceCalculator.StockAfterSale(stock, quantity)),
                        Function.Param("@mahang", productId));
                }

                result.Total = Function.ToDouble(Function.GetFieldValues(connection, transaction,
                    "SELECT ISNULL(SUM(thanhtien),0) FROM tblchitietddh WHERE soddh=@soddh",
                    Function.Param("@soddh", result.InvoiceNo)));
                result.Tax = InvoiceCalculator.CalculateSalesTax(result.Total);
                result.Deposit = InvoiceCalculator.CalculateSalesDeposit(result.Total);

                Function.ExecuteSql(connection, transaction,
                    "UPDATE tbldondathang SET tongtien=@tongtien, thue=@thue, datcoc=@datcoc WHERE soddh=@soddh",
                    Function.Param("@tongtien", result.Total),
                    Function.Param("@thue", result.Tax),
                    Function.Param("@datcoc", result.Deposit),
                    Function.Param("@soddh", result.InvoiceNo));
            }, "Không thể đặt hàng.");

            if (!placed)
            {
                throw new InvalidOperationException("Không thể đặt hàng.");
            }

            return result;
        }

        public IReadOnlyList<StoreOrderSummary> LoadOrders(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId)) return Array.Empty<StoreOrderSummary>();

            DataTable table = Function.GetDataToTable(
                @"SELECT d.soddh, CONVERT(NVARCHAR(10), d.ngaymua, 103) AS ngaymua,
                         d.tongtien, d.thue, d.datcoc,
                         ISNULL((SELECT SUM(ct.soluong) FROM tblchitietddh ct WHERE ct.soddh = d.soddh), 0) AS soluong
                  FROM tbldondathang d
                  WHERE d.makhach=@makhach
                  ORDER BY d.ngaymua DESC, d.soddh DESC",
                Function.Param("@makhach", customerId));

            List<StoreOrderSummary> orders = new List<StoreOrderSummary>();
            foreach (DataRow row in table.Rows)
            {
                orders.Add(new StoreOrderSummary
                {
                    InvoiceNo = row["soddh"].ToString().Trim(),
                    OrderDate = row["ngaymua"].ToString(),
                    ItemCount = Function.ToInt(row["soluong"].ToString()),
                    Total = Function.ToDouble(row["tongtien"].ToString()),
                    Tax = Function.ToDouble(row["thue"].ToString()),
                    Deposit = Function.ToDouble(row["datcoc"].ToString())
                });
            }

            return orders;
        }

        public IReadOnlyList<StoreOrderLine> LoadOrderLines(string invoiceNo)
        {
            if (string.IsNullOrWhiteSpace(invoiceNo)) return Array.Empty<StoreOrderLine>();

            DataTable table = Function.GetDataToTable(
                @"SELECT ct.mahang, h.tenhang, ct.soluong, ct.thanhtien
                  FROM tblchitietddh ct
                  INNER JOIN tbldmhang h ON ct.mahang = h.mahang
                  WHERE ct.soddh=@soddh",
                Function.Param("@soddh", invoiceNo));

            List<StoreOrderLine> lines = new List<StoreOrderLine>();
            foreach (DataRow row in table.Rows)
            {
                lines.Add(new StoreOrderLine
                {
                    ProductId = row["mahang"].ToString().Trim(),
                    ProductName = row["tenhang"].ToString(),
                    Quantity = Function.ToInt(row["soluong"].ToString()),
                    LineTotal = Function.ToDouble(row["thanhtien"].ToString())
                });
            }

            return lines;
        }

        private static Dictionary<string, int> AggregateLines(IEnumerable<CartLine> lines)
        {
            Dictionary<string, int> quantities = new Dictionary<string, int>();
            if (lines == null) return quantities;

            foreach (CartLine line in lines)
            {
                if (line == null || string.IsNullOrWhiteSpace(line.ProductId) || line.Quantity <= 0) continue;
                string key = line.ProductId.Trim();
                quantities[key] = quantities.TryGetValue(key, out int existing) ? existing + line.Quantity : line.Quantity;
            }

            return quantities;
        }

        private static string CreateInvoiceNo()
        {
            string baseKey = Function.CreateKey("DDH");
            string candidate = baseKey;
            int suffix = 1;
            while (Function.CheckKey("SELECT 1 FROM tbldondathang WHERE soddh=@soddh", Function.Param("@soddh", candidate)))
            {
                string tail = suffix.ToString();
                candidate = baseKey.Length + tail.Length > 20
                    ? baseKey.Substring(0, 20 - tail.Length) + tail
                    : baseKey + tail;
                suffix++;
            }

            return candidate;
        }

        private static string AsString(object value)
        {
            return value == null || value == DBNull.Value ? string.Empty : value.ToString().Trim();
        }
    }
}
