using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using QLXeMay.Infrastructure;
using QLXeMay.Models;
using QLXeMay.Services;

namespace QLXeMay.ViewModels
{
    internal sealed class StorefrontViewModel : ViewModelBase
    {
        private readonly IStoreService storeService;
        private readonly IDialogService dialogService;
        private readonly UserSession session;
        private readonly Action signOut;

        private string searchKeyword;
        private StoreOrderSummary selectedOrder;
        private bool isBusy;

        public StorefrontViewModel(IStoreService storeService, IDialogService dialogService, UserSession session, Action signOut)
        {
            this.storeService = storeService;
            this.dialogService = dialogService;
            this.session = session;
            this.signOut = signOut;

            Products = new ObservableCollection<StoreProduct>();
            Cart = new ObservableCollection<CartItemViewModel>();
            Orders = new ObservableCollection<StoreOrderSummary>();
            OrderLines = new ObservableCollection<StoreOrderLine>();

            SearchCommand = new RelayCommand(_ => LoadProducts());
            RefreshCommand = new RelayCommand(_ => { searchKeyword = string.Empty; OnPropertyChanged(nameof(SearchKeyword)); LoadProducts(); });
            AddToCartCommand = new RelayCommand(p => AddToCart(p as StoreProduct), p => CanAddToCart(p as StoreProduct));
            IncreaseCommand = new RelayCommand(p => ChangeQuantity(p as CartItemViewModel, 1));
            DecreaseCommand = new RelayCommand(p => ChangeQuantity(p as CartItemViewModel, -1));
            RemoveCommand = new RelayCommand(p => RemoveFromCart(p as CartItemViewModel));
            ClearCartCommand = new RelayCommand(_ => ClearCart(), _ => Cart.Count > 0);
            CheckoutCommand = new RelayCommand(_ => Checkout(), _ => Cart.Count > 0 && !isBusy);
            RefreshOrdersCommand = new RelayCommand(_ => LoadOrders());
            LogoutCommand = new RelayCommand(_ => Logout());

            LoadCustomer();
            LoadProducts();
            LoadOrders();
        }

        public ObservableCollection<StoreProduct> Products { get; }
        public ObservableCollection<CartItemViewModel> Cart { get; }
        public ObservableCollection<StoreOrderSummary> Orders { get; }
        public ObservableCollection<StoreOrderLine> OrderLines { get; }

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ClearCartCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand RefreshOrdersCommand { get; }
        public ICommand LogoutCommand { get; }

        public string CustomerName { get; private set; }
        public string CustomerContact { get; private set; }
        public string Greeting => "Xin chào, " + (string.IsNullOrWhiteSpace(CustomerName) ? session.DisplayName : CustomerName);

        public string SearchKeyword
        {
            get => searchKeyword;
            set => SetProperty(ref searchKeyword, value);
        }

        public StoreOrderSummary SelectedOrder
        {
            get => selectedOrder;
            set
            {
                if (SetProperty(ref selectedOrder, value))
                {
                    LoadOrderLines();
                }
            }
        }

        public bool IsCartEmpty => Cart.Count == 0;
        public int CartItemCount => Cart.Sum(item => item.Quantity);
        public double CartTotal => Cart.Sum(item => item.LineTotal);
        public string CartTotalText => CartTotal.ToString("#,##0") + " đ";
        public string CartSummaryText => Cart.Count == 0
            ? "Giỏ hàng đang trống"
            : $"{Cart.Count} sản phẩm • {CartItemCount} chiếc";

        private void LoadCustomer()
        {
            try
            {
                PartyInfo info = storeService.GetCustomerInfo(session.CustomerId);
                CustomerName = string.IsNullOrWhiteSpace(info.Name) ? session.DisplayName : info.Name;
                CustomerContact = string.Join("  •  ",
                    new[] { info.Phone, info.Address }.Where(s => !string.IsNullOrWhiteSpace(s)));
            }
            catch (Exception ex)
            {
                CustomerName = session.DisplayName;
                CustomerContact = string.Empty;
                AppLogger.Error("Cannot load customer profile.", ex);
            }

            OnPropertyChanged(nameof(CustomerName));
            OnPropertyChanged(nameof(CustomerContact));
            OnPropertyChanged(nameof(Greeting));
        }

        private void LoadProducts()
        {
            try
            {
                Products.Clear();
                foreach (StoreProduct product in storeService.LoadProducts(SearchKeyword))
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Cannot load storefront products.", ex);
                dialogService.ShowError("Không thể tải danh sách sản phẩm: " + ex.Message);
            }
        }

        private bool CanAddToCart(StoreProduct product)
        {
            return product != null && product.InStock;
        }

        private void AddToCart(StoreProduct product)
        {
            if (product == null) return;
            if (!product.InStock)
            {
                dialogService.ShowWarning("Sản phẩm này hiện đã hết hàng.");
                return;
            }

            CartItemViewModel existing = Cart.FirstOrDefault(item => item.ProductId == product.ProductId);
            if (existing != null)
            {
                if (existing.Quantity >= product.Stock)
                {
                    dialogService.ShowWarning($"Chỉ còn {product.Stock} chiếc \"{product.Name}\" trong kho.");
                    return;
                }

                existing.Quantity += 1;
            }
            else
            {
                Cart.Add(new CartItemViewModel(product, 1));
            }

            NotifyCartChanged();
        }

        private void ChangeQuantity(CartItemViewModel item, int delta)
        {
            if (item == null) return;
            if (delta > 0 && item.Quantity >= item.Stock)
            {
                dialogService.ShowWarning($"Chỉ còn {item.Stock} chiếc \"{item.Name}\" trong kho.");
                return;
            }

            if (delta < 0 && item.Quantity <= 1)
            {
                RemoveFromCart(item);
                return;
            }

            item.Quantity += delta;
            NotifyCartChanged();
        }

        private void RemoveFromCart(CartItemViewModel item)
        {
            if (item == null) return;
            Cart.Remove(item);
            NotifyCartChanged();
        }

        private void ClearCart()
        {
            if (Cart.Count == 0) return;
            if (!dialogService.Confirm("Bạn có muốn xóa toàn bộ giỏ hàng không?", "Xóa giỏ hàng")) return;
            Cart.Clear();
            NotifyCartChanged();
        }

        private void Checkout()
        {
            if (Cart.Count == 0) return;

            string message = $"Xác nhận đặt {CartItemCount} chiếc với tổng tiền {CartTotalText}?";
            if (!dialogService.Confirm(message, "Xác nhận đặt hàng")) return;

            try
            {
                isBusy = true;
                List<CartLine> lines = Cart.Select(item => new CartLine(item.ProductId, item.Quantity)).ToList();
                StoreOrderResult result = storeService.PlaceOrder(session.CustomerId, lines);

                dialogService.ShowInformation(
                    $"Đặt hàng thành công!\n\nMã đơn: {result.InvoiceNo}\nTổng tiền: {result.Total:#,##0} đ\nThuế (10%): {result.Tax:#,##0} đ\nĐặt cọc (50%): {result.Deposit:#,##0} đ\n\nCửa hàng sẽ liên hệ với bạn để hoàn tất giao dịch.",
                    "Đặt hàng thành công");

                Cart.Clear();
                NotifyCartChanged();
                LoadProducts();
                LoadOrders();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Customer checkout failed.", ex);
                dialogService.ShowError(ex.Message, "Không thể đặt hàng");
                LoadProducts();
            }
            finally
            {
                isBusy = false;
            }
        }

        private void LoadOrders()
        {
            try
            {
                Orders.Clear();
                foreach (StoreOrderSummary order in storeService.LoadOrders(session.CustomerId))
                {
                    Orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Cannot load customer orders.", ex);
            }
        }

        private void LoadOrderLines()
        {
            OrderLines.Clear();
            if (selectedOrder == null) return;

            try
            {
                foreach (StoreOrderLine line in storeService.LoadOrderLines(selectedOrder.InvoiceNo))
                {
                    OrderLines.Add(line);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Cannot load order detail.", ex);
            }
        }

        private void Logout()
        {
            if (!dialogService.Confirm("Bạn có muốn đăng xuất không?", "Đăng xuất")) return;
            signOut?.Invoke();
        }

        private void NotifyCartChanged()
        {
            OnPropertyChanged(nameof(IsCartEmpty));
            OnPropertyChanged(nameof(CartItemCount));
            OnPropertyChanged(nameof(CartTotal));
            OnPropertyChanged(nameof(CartTotalText));
            OnPropertyChanged(nameof(CartSummaryText));
            (ClearCartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CheckoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}
