using System.Collections.Generic;

namespace QLXeMay.Models
{
    /// <summary>A product as shown to a self-service shopper in the storefront catalog.</summary>
    internal sealed class StoreProduct
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public string Brake { get; set; }
        public string Engine { get; set; }
        public string OriginCountry { get; set; }
        public string Condition { get; set; }
        public int Year { get; set; }
        public double FuelTankCapacity { get; set; }
        public int WarrantyMonths { get; set; }
        public double UnitPrice { get; set; }
        public int Stock { get; set; }
        public string ImagePath { get; set; }

        public bool InStock => Stock > 0;
        public string StockText => Stock > 0 ? $"Còn {Stock} chiếc" : "Hết hàng";
        public string PriceText => UnitPrice.ToString("#,##0") + " đ";
        public string Specs
        {
            get
            {
                List<string> parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(Brand)) parts.Add(Brand.Trim());
                if (!string.IsNullOrWhiteSpace(Color)) parts.Add(Color.Trim());
                if (Year > 0) parts.Add("Đời " + Year);
                if (WarrantyMonths > 0) parts.Add("BH " + WarrantyMonths + " tháng");
                return string.Join("  •  ", parts);
            }
        }

        public string CategoryText => string.IsNullOrWhiteSpace(Category) ? "Chưa cập nhật" : Category.Trim();
        public string BrandText => string.IsNullOrWhiteSpace(Brand) ? "Chưa cập nhật" : Brand.Trim();
        public string ColorText => string.IsNullOrWhiteSpace(Color) ? "Chưa cập nhật" : Color.Trim();
        public string BrakeText => string.IsNullOrWhiteSpace(Brake) ? "Chưa cập nhật" : Brake.Trim();
        public string EngineText => string.IsNullOrWhiteSpace(Engine) ? "Chưa cập nhật" : Engine.Trim();
        public string OriginCountryText => string.IsNullOrWhiteSpace(OriginCountry) ? "Chưa cập nhật" : OriginCountry.Trim();
        public string ConditionText => string.IsNullOrWhiteSpace(Condition) ? "Chưa cập nhật" : Condition.Trim();
        public string FuelTankText => FuelTankCapacity > 0 ? FuelTankCapacity.ToString("0.#") + " lít" : "Chưa cập nhật";
        public string WarrantyText => WarrantyMonths > 0 ? WarrantyMonths + " tháng" : "Chưa cập nhật";
        public string YearText => Year > 0 ? Year.ToString() : "Chưa cập nhật";
    }

    /// <summary>A single product+quantity line submitted when placing a self-service order.</summary>
    internal sealed class CartLine
    {
        public CartLine(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public string ProductId { get; }
        public int Quantity { get; }
    }

    /// <summary>Result returned after a self-service order is committed.</summary>
    internal sealed class StoreOrderResult
    {
        public string InvoiceNo { get; set; }
        public double Total { get; set; }
        public double Tax { get; set; }
        public double Deposit { get; set; }
    }

    /// <summary>A header row in a shopper's order history.</summary>
    internal sealed class StoreOrderSummary
    {
        public string InvoiceNo { get; set; }
        public string OrderDate { get; set; }
        public int ItemCount { get; set; }
        public double Total { get; set; }
        public double Tax { get; set; }
        public double Deposit { get; set; }

        public string TotalText => Total.ToString("#,##0") + " đ";
        public string DepositText => Deposit.ToString("#,##0") + " đ";
    }

    /// <summary>A detail row for one order in the shopper's history.</summary>
    internal sealed class StoreOrderLine
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double LineTotal { get; set; }

        public string LineTotalText => LineTotal.ToString("#,##0") + " đ";
    }
}
