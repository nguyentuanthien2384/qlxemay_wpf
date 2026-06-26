using System.Collections.Generic;

namespace QLXeMay.Models
{
    internal sealed class DashboardSnapshot
    {
        public double Revenue { get; set; }
        public double Purchase { get; set; }
        public int OrderCount { get; set; }
        public int Stock { get; set; }
        public int CustomerCount { get; set; }
        public int ProductCount { get; set; }
        public IReadOnlyList<RevenueBar> MonthlyRevenue { get; set; } = new List<RevenueBar>();
        public IReadOnlyList<ProductShowcaseItem> FeaturedProducts { get; set; } = new List<ProductShowcaseItem>();
        public IReadOnlyList<ProductShowcaseItem> TopSellingProducts { get; set; } = new List<ProductShowcaseItem>();
        public IReadOnlyList<ProductShowcaseItem> LowStockProducts { get; set; } = new List<ProductShowcaseItem>();

        public double Profit => Revenue - Purchase;
    }

    internal sealed class RevenueBar
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public double BarHeight { get; set; }
        public string ValueText { get; set; }
    }

    internal sealed class ProductShowcaseItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string PriceText { get; set; }
        public string StockText { get; set; }
        public string Subtitle { get; set; }
        public int SoldQuantity { get; set; }
        public int Stock { get; set; }
        public bool IsLowStock { get; set; }
    }
}
