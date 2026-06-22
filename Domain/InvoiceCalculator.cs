using System;

namespace QLXeMay.Domain
{
    public static class InvoiceCalculator
    {
        public static double CalculateLineTotal(int quantity, double unitPrice, double discountPercent)
        {
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice));

            double discount = ClampPercent(discountPercent);
            return quantity * unitPrice * (1 - discount / 100.0);
        }

        public static double CalculateSalesTax(double total)
        {
            if (total < 0) throw new ArgumentOutOfRangeException(nameof(total));
            return total * 0.1;
        }

        public static double CalculateSalesDeposit(double total)
        {
            if (total < 0) throw new ArgumentOutOfRangeException(nameof(total));
            return total * 0.5;
        }

        public static double CalculateSuggestedSellingPrice(double purchasePrice)
        {
            if (purchasePrice < 0) throw new ArgumentOutOfRangeException(nameof(purchasePrice));
            return purchasePrice * 1.1;
        }

        public static int StockAfterSale(int currentStock, int quantity)
        {
            if (currentStock < 0) throw new ArgumentOutOfRangeException(nameof(currentStock));
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            if (quantity > currentStock) throw new InvalidOperationException("Số lượng bán vượt quá tồn kho.");
            return currentStock - quantity;
        }

        public static int StockAfterSaleRollback(int currentStock, int quantity)
        {
            if (currentStock < 0) throw new ArgumentOutOfRangeException(nameof(currentStock));
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            return currentStock + quantity;
        }

        public static int StockAfterPurchase(int currentStock, int quantity)
        {
            if (currentStock < 0) throw new ArgumentOutOfRangeException(nameof(currentStock));
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            return currentStock + quantity;
        }

        public static int StockAfterPurchaseRollback(int currentStock, int quantity)
        {
            if (currentStock < 0) throw new ArgumentOutOfRangeException(nameof(currentStock));
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            return Math.Max(0, currentStock - quantity);
        }

        private static double ClampPercent(double value)
        {
            if (value < 0) return 0;
            if (value > 100) return 100;
            return value;
        }
    }
}
