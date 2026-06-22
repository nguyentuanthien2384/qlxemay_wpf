using System;
using System.Collections.Generic;
using System.Data;
using QLXeMay.Class;
using QLXeMay.Infrastructure;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal sealed class DashboardService : IDashboardService
    {
        private const double MaxBarHeight = 150.0;

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
    }
}
