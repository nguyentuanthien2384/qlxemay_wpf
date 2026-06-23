using QLXeMay.Models;

namespace QLXeMay.ViewModels
{
    /// <summary>An observable line in the shopping cart. Quantity is editable and recomputes the line total.</summary>
    internal sealed class CartItemViewModel : ViewModelBase
    {
        private int quantity;

        public CartItemViewModel(StoreProduct product, int quantity)
        {
            ProductId = product.ProductId;
            Name = product.Name;
            UnitPrice = product.UnitPrice;
            Stock = product.Stock;
            this.quantity = quantity;
        }

        public string ProductId { get; }
        public string Name { get; }
        public double UnitPrice { get; }
        public int Stock { get; }

        public int Quantity
        {
            get => quantity;
            set
            {
                int clamped = value < 1 ? 1 : (Stock > 0 && value > Stock ? Stock : value);
                if (SetProperty(ref quantity, clamped))
                {
                    OnPropertyChanged(nameof(LineTotal));
                    OnPropertyChanged(nameof(LineTotalText));
                }
            }
        }

        public double LineTotal => UnitPrice * quantity;
        public string UnitPriceText => UnitPrice.ToString("#,##0") + " đ";
        public string LineTotalText => LineTotal.ToString("#,##0") + " đ";
    }
}
