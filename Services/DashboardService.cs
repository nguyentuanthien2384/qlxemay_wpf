using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using QLXeMay.Class;
using QLXeMay.Infrastructure;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal sealed class DashboardService : IDashboardService
    {
        private const double MaxBarHeight = 150.0;
        private const int LowStockThreshold = 5;
        private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

        public DashboardSnapshot Load()
        {
            DashboardSnapshot snapshot = new DashboardSnapshot();

            snapshot.Revenue = Scalar("SELECT ISNULL(SUM(tongtien),0) FROM tbldondathang");
            snapshot.Purchase = Scalar("SELECT ISNULL(SUM(tongtien),0) FROM tblhoadonnhap");
            snapshot.OrderCount = (int)Scalar("SELECT COUNT(*) FROM tbldondathang");
            snapshot.Stock = (int)Scalar("SELECT ISNULL(SUM(soluong),0) FROM tbldmhang");
            snapshot.CustomerCount = (int)Scalar("SELECT COUNT(*) FROM tblkhachhang");
            snapshot.ProductCount = (int)Scalar("SELECT COUNT(*) FROM tbldmhang");
            snapshot.MonthlyRevenue = LoadMonthlyRevenue();
            snapshot.FeaturedProducts = LoadFeaturedProducts();
            snapshot.TopSellingProducts = LoadTopSellingProducts();
            snapshot.LowStockProducts = LoadLowStockProducts();

            return snapshot;
        }

        private static double Scalar(string sql)
        {
            try
            {
                return Function.ToDouble(Function.GetFieldValues(sql), 0);
            }
            catch (Exception ex)
            {
                AppLogger.Error("Dashboard scalar query failed: " + sql, ex);
                return 0;
            }
        }

        private static IReadOnlyList<RevenueBar> LoadMonthlyRevenue()
        {
            // Build the last 6 month buckets (oldest -> newest)
            DateTime now = DateTime.Now;
            List<(int Year, int Month)> buckets = new List<(int, int)>();
            for (int i = 5; i >= 0; i--)
            {
                DateTime d = now.AddMonths(-i);
                buckets.Add((d.Year, d.Month));
            }

            Dictionary<string, double> totals = new Dictionary<string, double>();
            try
            {
                DataTable table = Function.GetDataToTable(
                    @"SELECT YEAR(ngaymua) AS y, MONTH(ngaymua) AS m, SUM(tongtien) AS total
                      FROM tbldondathang
                      WHERE ngaymua IS NOT NULL
                      GROUP BY YEAR(ngaymua), MONTH(ngaymua)");

                foreach (DataRow row in table.Rows)
                {
                    int y = Convert.ToInt32(row["y"]);
                    int m = Convert.ToInt32(row["m"]);
                    double total = row["total"] == DBNull.Value ? 0 : Convert.ToDouble(row["total"]);
                    totals[y + "-" + m] = total;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Dashboard monthly revenue query failed.", ex);
            }

            List<RevenueBar> bars = new List<RevenueBar>();
            double max = 0;
            foreach (var b in buckets)
            {
                double value = totals.TryGetValue(b.Year + "-" + b.Month, out double v) ? v : 0;
                if (value > max) max = value;
                bars.Add(new RevenueBar
                {
                    Label = "T" + b.Month,
                    Value = value
                });
            }

            foreach (RevenueBar bar in bars)
            {
                bar.BarHeight = max <= 0 ? 2 : Math.Max(2, bar.Value / max * MaxBarHeight);
                bar.ValueText = bar.Value >= 1_000_000
                    ? (bar.Value / 1_000_000).ToString("0.#") + "tr"
                    : bar.Value.ToString("#,##0");
            }

            return bars;
        }

        private static IReadOnlyList<ProductShowcaseItem> LoadFeaturedProducts()
        {
            List<ProductShowcaseItem> list = new List<ProductShowcaseItem>();
            try
            {
                DataTable table = Function.GetDataToTable(
                    @"SELECT mahang, tenhang, ISNULL(dongiaban,0) AS dongiaban, ISNULL(soluong,0) AS soluong
                      FROM tbldmhang
                      ORDER BY ISNULL(dongiaban,0) DESC, tenhang");

                foreach (DataRow row in table.Rows)
                {
                    int stock = Function.ToInt(row["soluong"].ToString());
                    list.Add(new ProductShowcaseItem
                    {
                        ProductId = row["mahang"].ToString().Trim(),
                        ProductName = row["tenhang"].ToString().Trim(),
                        PriceText = FormatCurrency(Function.ToDouble(row["dongiaban"].ToString())),
                        StockText = "Tồn kho: " + stock.ToString("#,##0"),
                        Subtitle = "Giá bán cao - tiềm năng lợi nhuận tốt",
                        Stock = stock,
                        IsLowStock = stock <= LowStockThreshold
                    });
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Dashboard featured products query failed.", ex);
            }

            return list;
        }

        private static IReadOnlyList<ProductShowcaseItem> LoadTopSellingProducts()
        {
            List<ProductShowcaseItem> list = new List<ProductShowcaseItem>();
            try
            {
                DataTable table = Function.GetDataToTable(
                    @"SELECT TOP 6 h.mahang, h.tenhang, ISNULL(h.dongiaban,0) AS dongiaban, ISNULL(h.soluong,0) AS tonkho, SUM(ISNULL(ct.soluong,0)) AS luotban
                      FROM tblchitietddh ct
                      INNER JOIN tbldmhang h ON h.mahang=ct.mahang
                      GROUP BY h.mahang, h.tenhang, h.dongiaban, h.soluong
                      ORDER BY SUM(ISNULL(ct.soluong,0)) DESC, h.tenhang");

                foreach (DataRow row in table.Rows)
                {
                    int sold = Function.ToInt(row["luotban"].ToString());
                    int stock = Function.ToInt(row["tonkho"].ToString());
                    list.Add(new ProductShowcaseItem
                    {
                        ProductId = row["mahang"].ToString().Trim(),
                        ProductName = row["tenhang"].ToString().Trim(),
                        PriceText = FormatCurrency(Function.ToDouble(row["dongiaban"].ToString())),
                        StockText = "Đã bán: " + sold.ToString("#,##0") + " xe",
                        Subtitle = "Tồn hiện tại: " + stock.ToString("#,##0"),
                        SoldQuantity = sold,
                        Stock = stock,
                        IsLowStock = stock <= LowStockThreshold
                    });
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Dashboard top-selling products query failed.", ex);
            }

            return list;
        }

        private static IReadOnlyList<ProductShowcaseItem> LoadLowStockProducts()
        {
            List<ProductShowcaseItem> list = new List<ProductShowcaseItem>();
            try
            {
                DataTable table = Function.GetDataToTable(
                    @"SELECT TOP 6 mahang, tenhang, ISNULL(dongiaban,0) AS dongiaban, ISNULL(soluong,0) AS soluong
                      FROM tbldmhang
                      WHERE ISNULL(soluong,0) <= @threshold
                      ORDER BY ISNULL(soluong,0) ASC, tenhang",
                    Function.Param("@threshold", LowStockThreshold));

                foreach (DataRow row in table.Rows)
                {
                    int stock = Function.ToInt(row["soluong"].ToString());
                    list.Add(new ProductShowcaseItem
                    {
                        ProductId = row["mahang"].ToString().Trim(),
                        ProductName = row["tenhang"].ToString().Trim(),
                        PriceText = FormatCurrency(Function.ToDouble(row["dongiaban"].ToString())),
                        StockText = "Tồn kho còn: " + stock.ToString("#,##0"),
                        Subtitle = "Cần nhập thêm để tránh thiếu hàng",
                        Stock = stock,
                        IsLowStock = true
                    });
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Dashboard low-stock products query failed.", ex);
            }

            return list;
        }

        private static string FormatCurrency(double value)
        {
            return value.ToString("N0", ViCulture) + " VNĐ";
        }
    }
}
