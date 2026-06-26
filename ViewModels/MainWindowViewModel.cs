using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;
using QLXeMay.Infrastructure;
using QLXeMay.Models;
using QLXeMay.Services;
using QLXeMay.Windows;

namespace QLXeMay.ViewModels
{
    internal sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly IWindowService windowService;
        private readonly IDialogService dialogService;
        private readonly IGlobalSearchService globalSearchService;
        private readonly UserSession currentUser;
        private readonly HomeView homeView;

        private object currentView;
        private bool isNotHome;
        private string currentTitle = "Bảng điều khiển";
        private string activeNavKey = "Home";
        private string globalSearchKeyword = "";
        private DataView globalSearchResults;
        private bool hasSearchResults;

        private string revenueText = "—";
        private string profitText = "—";
        private string orderCountText = "—";
        private string stockText = "—";
        private string customerText = "—";
        private System.Collections.Generic.IReadOnlyList<RevenueBar> monthlyRevenue = new System.Collections.Generic.List<RevenueBar>();
        private System.Collections.Generic.IReadOnlyList<ProductShowcaseItem> featuredProducts = new System.Collections.Generic.List<ProductShowcaseItem>();
        private System.Collections.Generic.IReadOnlyList<ProductShowcaseItem> topSellingProducts = new System.Collections.Generic.List<ProductShowcaseItem>();
        private System.Collections.Generic.IReadOnlyList<ProductShowcaseItem> lowStockProducts = new System.Collections.Generic.List<ProductShowcaseItem>();

        public MainWindowViewModel(IWindowService windowService, IDialogService dialogService)
        {
            this.windowService = windowService;
            this.dialogService = dialogService;
            this.globalSearchService = new GlobalSearchService();
            currentUser = AppSession.CurrentUser;
            homeView = new HomeView();
            homeView.DataContext = this;
            LoadDashboard();

            OpenCongViecCommand = CreateAuthorizedCommand(PermissionNames.ManageEmployees, () => Show("Danh mục công việc", new DanhMucWindow(new DanhMucConfig("DANH MỤC CÔNG VIỆC", "tblcongviec", new List<FieldConfig>
            {
                new FieldConfig("macv", "Mã công việc", FieldKind.Text, true, true),
                new FieldConfig("tencv", "Tên công việc", FieldKind.Text, true, false),
                new FieldConfig("luongthang", "Lương tháng", FieldKind.Number, true, false)
            }), GoHome), "NhanVien"));

            OpenNhanVienCommand = CreateAuthorizedCommand(PermissionNames.ManageEmployees, () => Show("Nhân viên", new DanhMucWindow(new DanhMucConfig("DANH MỤC NHÂN VIÊN", "tblnhanvien", new List<FieldConfig>
            {
                new FieldConfig("manv", "Mã nhân viên", FieldKind.Text, true, true),
                new FieldConfig("tennv", "Tên nhân viên", FieldKind.Text, true, false),
                new FieldConfig("gioitinh", "Giới tính", FieldKind.Text, true, false),
                new FieldConfig("ngaysinh", "Ngày sinh", FieldKind.Date, true, false),
                new FieldConfig("sdt", "Số điện thoại", FieldKind.Text, true, false),
                new FieldConfig("diachi", "Địa chỉ", FieldKind.Text, true, false),
                new FieldConfig("macv", "Công việc", FieldKind.Combo, true, false,
                    "SELECT macv, macv + ' - ' + tencv AS hienthi FROM tblcongviec", "macv", "hienthi")
            }), GoHome), "NhanVien"));

            OpenKhachHangCommand = CreateAuthorizedCommand(PermissionNames.ManageCustomers, () => Show("Khách hàng", new DanhMucWindow(new DanhMucConfig("DANH MỤC KHÁCH HÀNG", "tblkhachhang", new List<FieldConfig>
            {
                new FieldConfig("makhach", "Mã khách hàng", FieldKind.Text, true, true),
                new FieldConfig("tenkhach", "Tên khách hàng", FieldKind.Text, true, false),
                new FieldConfig("diachi", "Địa chỉ", FieldKind.Text, true, false),
                new FieldConfig("sdt", "Số điện thoại", FieldKind.Text, true, false)
            }), GoHome), "KhachHang"));

            OpenNhaCungCapCommand = CreateAuthorizedCommand(PermissionNames.ManageSuppliers, () => Show("Nhà cung cấp", new DanhMucWindow(new DanhMucConfig("DANH MỤC NHÀ CUNG CẤP", "tblnhacungcap", new List<FieldConfig>
            {
                new FieldConfig("mancc", "Mã nhà cung cấp", FieldKind.Text, true, true),
                new FieldConfig("tenncc", "Tên nhà cung cấp", FieldKind.Text, true, false),
                new FieldConfig("diachi", "Địa chỉ", FieldKind.Text, true, false),
                new FieldConfig("sdt", "Số điện thoại", FieldKind.Text, true, false)
            }), GoHome), "HoaDonNhap"));

            OpenHangHoaCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Xe máy", new DanhMucWindow(new DanhMucConfig("DANH MỤC HÀNG HÓA", "tbldmhang", new List<FieldConfig>
            {
                new FieldConfig("mahang", "Mã xe", FieldKind.Text, true, true),
                new FieldConfig("tenhang", "Tên xe", FieldKind.Text, true, false),
                new FieldConfig("maloai", "Thể loại", FieldKind.Combo, true, false, "SELECT maloai, maloai + ' - ' + tenloai AS ht FROM tbltheloai", "maloai", "ht"),
                new FieldConfig("mahangsx", "Hãng sản xuất", FieldKind.Combo, true, false, "SELECT mahangsx, mahangsx + ' - ' + tenhangsx AS ht FROM tblhangsx", "mahangsx", "ht"),
                new FieldConfig("mamau", "Màu", FieldKind.Combo, true, false, "SELECT mamau, mamau + ' - ' + tenmau AS ht FROM tblmausac", "mamau", "ht"),
                new FieldConfig("namsx", "Năm sản xuất", FieldKind.Number, true, false),
                new FieldConfig("maphanh", "Phanh", FieldKind.Combo, true, false, "SELECT maphanh, maphanh + ' - ' + tenphanh AS ht FROM tblphanhxe", "maphanh", "ht"),
                new FieldConfig("madongco", "Động cơ", FieldKind.Combo, true, false, "SELECT madongco, madongco + ' - ' + tendongco AS ht FROM tbldongco", "madongco", "ht"),
                new FieldConfig("manuocsx", "Nước sản xuất", FieldKind.Combo, true, false, "SELECT manuocsx, manuocsx + ' - ' + tennuocsx AS ht FROM tblnuocsx", "manuocsx", "ht"),
                new FieldConfig("matt", "Tình trạng", FieldKind.Combo, true, false, "SELECT matt, matt + ' - ' + tentt AS ht FROM tbltinhtrang", "matt", "ht"),
                new FieldConfig("dungtichbinhxang", "Dung tích bình xăng", FieldKind.Number, true, false),
                new FieldConfig("anh", "Đường dẫn ảnh", FieldKind.Text, false, false),
                new FieldConfig("thoigianbaohanh", "Thời gian bảo hành", FieldKind.Number, true, false),
                new FieldConfig("soluong", "Số lượng", FieldKind.Number, true, false),
                new FieldConfig("dongianhap", "Đơn giá nhập", FieldKind.Number, true, false),
                new FieldConfig("dongiaban", "Đơn giá bán", FieldKind.Number, true, false)
            }), GoHome), "HangHoa"));

            OpenTheLoaiCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Thể loại", new DanhMucWindow(TwoField("DANH MỤC THỂ LOẠI", "tbltheloai", "maloai", "Mã loại", "tenloai", "Tên loại"), GoHome), "HangHoa"));
            OpenMauSacCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Màu sắc", new DanhMucWindow(TwoField("DANH MỤC MÀU SẮC", "tblmausac", "mamau", "Mã màu", "tenmau", "Tên màu"), GoHome), "HangHoa"));
            OpenHangSXCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Hãng sản xuất", new DanhMucWindow(TwoField("DANH MỤC HÃNG SẢN XUẤT", "tblhangsx", "mahangsx", "Mã hãng sản xuất", "tenhangsx", "Tên hãng sản xuất"), GoHome), "HangHoa"));
            OpenNuocSXCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Nước sản xuất", new DanhMucWindow(TwoField("DANH MỤC NƯỚC SẢN XUẤT", "tblnuocsx", "manuocsx", "Mã nước sản xuất", "tennuocsx", "Tên nước sản xuất"), GoHome), "HangHoa"));
            OpenPhanhCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Phanh xe", new DanhMucWindow(TwoField("DANH MỤC PHANH XE", "tblphanhxe", "maphanh", "Mã phanh", "tenphanh", "Tên phanh"), GoHome), "HangHoa"));
            OpenDongCoCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Động cơ", new DanhMucWindow(TwoField("DANH MỤC ĐỘNG CƠ", "tbldongco", "madongco", "Mã động cơ", "tendongco", "Tên động cơ"), GoHome), "HangHoa"));
            OpenTinhTrangCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Tình trạng", new DanhMucWindow(TwoField("DANH MỤC TÌNH TRẠNG", "tbltinhtrang", "matt", "Mã tình trạng", "tentt", "Tên tình trạng"), GoHome), "HangHoa"));

            OpenHoaDonNhapCommand = CreateAuthorizedCommand(PermissionNames.PurchaseInvoice, () => Show("Hóa đơn nhập hàng", new HoaDonNhapWindow(GoHome), "HoaDonNhap"));
            OpenHoaDonBanCommand = CreateAuthorizedCommand(PermissionNames.SalesInvoice, () => Show("Hóa đơn bán hàng", new HoaDonBanWindow(GoHome), "HoaDonBan"));
            OpenTimHangCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Show("Tìm kiếm", new SearchWindow(SearchMode.Hang, GoHome), "TimKiem"));
            OpenTimKhachHangCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Show("Tìm kiếm khách hàng", new SearchWindow(SearchMode.KhachHang, GoHome), "TimKiem"));
            OpenTimHDNCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Show("Tìm hóa đơn nhập", new SearchWindow(SearchMode.HoaDonNhap, GoHome), "TimKiem"));
            OpenTimDDHCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Show("Tìm đơn đặt hàng", new SearchWindow(SearchMode.DonDatHang, GoHome), "TimKiem"));
            OpenBaoCaoBanCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Show("Báo cáo bán hàng", new ReportWindow(ReportMode.BanHang, GoHome), "BaoCao"));
            OpenBaoCaoNhapCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Show("Báo cáo nhập hàng", new ReportWindow(ReportMode.NhapHang, GoHome), "BaoCao"));
            OpenBaoCaoKQKDCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Show("Doanh thu", new ReportWindow(ReportMode.KetQuaKinhDoanh, GoHome), "BaoCao"));
            OpenBaoCaoTopCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Show("Top sản phẩm", new ReportWindow(ReportMode.TopSanPham, GoHome), "BaoCao"));
            OpenAiCommand = CreateAuthorizedCommand(PermissionNames.AiAssistant, () => Show("Trợ lý AI", new AIAssistantWindow(), "TroLyAI"));
            OpenUserAdminCommand = CreateAuthorizedCommand(PermissionNames.UserAdmin, () => Show("Quản trị tài khoản", new UserAdminWindow(GoHome), "UserAdmin"));
            OpenAuditLogCommand = CreateAuthorizedCommand(PermissionNames.AuditLog, () => Show("Nhật ký hệ thống", new AuditLogWindow(GoHome), "AuditLog"));
            BuyFeaturedProductCommand = new RelayCommand(
                p => BuyFeaturedProduct(p?.ToString()),
                p => CanSalesInvoice && !string.IsNullOrWhiteSpace(p?.ToString()));
            GlobalSearchCommand = new RelayCommand(_ => ExecuteGlobalSearch());
            ClearSearchCommand = new RelayCommand(_ => ClearGlobalSearch());
            HomeCommand = new RelayCommand(_ => GoHome());
            BackCommand = new RelayCommand(_ => GoHome());
            LogoutCommand = new RelayCommand(_ => Logout());
            ExitCommand = new RelayCommand(_ => Exit());

            GoHome();
        }

        public string CurrentUserText => currentUser == null ? "Chưa đăng nhập" : currentUser.DisplayText;
        public string AccountMenuHeader => currentUser == null ? "Tài khoản" : "Tài khoản: " + currentUser.UserName;
        public string RoleText => currentUser == null ? "" : currentUser.RoleDisplayName;

        // Visibility flags so the sidebar / home only show what the current role can use.
        public bool CanManageEmployees => HasPermission(PermissionNames.ManageEmployees);
        public bool CanManageCustomers => HasPermission(PermissionNames.ManageCustomers);
        public bool CanManageSuppliers => HasPermission(PermissionNames.ManageSuppliers);
        public bool CanManageProducts => HasPermission(PermissionNames.ManageProducts);
        public bool CanSalesInvoice => HasPermission(PermissionNames.SalesInvoice);
        public bool CanPurchaseInvoice => HasPermission(PermissionNames.PurchaseInvoice);
        public bool CanSearch => HasPermission(PermissionNames.Search);
        public bool CanReports => HasPermission(PermissionNames.Reports);
        public bool CanAi => HasPermission(PermissionNames.AiAssistant);
        public bool CanUserAdmin => HasPermission(PermissionNames.UserAdmin);
        public bool CanAuditLog => HasPermission(PermissionNames.AuditLog);

        public object CurrentView
        {
            get => currentView;
            private set => SetProperty(ref currentView, value);
        }

        public bool IsNotHome
        {
            get => isNotHome;
            private set => SetProperty(ref isNotHome, value);
        }

        public string CurrentTitle
        {
            get => currentTitle;
            private set => SetProperty(ref currentTitle, value);
        }

        public string ActiveNavKey
        {
            get => activeNavKey;
            private set => SetProperty(ref activeNavKey, value);
        }

        public string RevenueText { get => revenueText; private set => SetProperty(ref revenueText, value); }
        public string ProfitText { get => profitText; private set => SetProperty(ref profitText, value); }
        public string OrderCountText { get => orderCountText; private set => SetProperty(ref orderCountText, value); }
        public string StockText { get => stockText; private set => SetProperty(ref stockText, value); }
        public string CustomerText { get => customerText; private set => SetProperty(ref customerText, value); }
        public System.Collections.Generic.IReadOnlyList<RevenueBar> MonthlyRevenue
        {
            get => monthlyRevenue;
            private set => SetProperty(ref monthlyRevenue, value);
        }
        public System.Collections.Generic.IReadOnlyList<ProductShowcaseItem> FeaturedProducts
        {
            get => featuredProducts;
            private set => SetProperty(ref featuredProducts, value);
        }
        public System.Collections.Generic.IReadOnlyList<ProductShowcaseItem> TopSellingProducts
        {
            get => topSellingProducts;
            private set => SetProperty(ref topSellingProducts, value);
        }
        public System.Collections.Generic.IReadOnlyList<ProductShowcaseItem> LowStockProducts
        {
            get => lowStockProducts;
            private set => SetProperty(ref lowStockProducts, value);
        }

        private void LoadDashboard()
        {
            try
            {
                DashboardSnapshot s = new DashboardService().Load();
                RevenueText = FormatMoney(s.Revenue);
                ProfitText = FormatMoney(s.Profit);
                OrderCountText = s.OrderCount.ToString("#,##0");
                StockText = s.Stock.ToString("#,##0");
                CustomerText = s.CustomerCount.ToString("#,##0");
                MonthlyRevenue = s.MonthlyRevenue;
                FeaturedProducts = s.FeaturedProducts;
                TopSellingProducts = s.TopSellingProducts;
                LowStockProducts = s.LowStockProducts;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Cannot load dashboard data.", ex);
            }
        }

        private static string FormatMoney(double value)
        {
            if (value >= 1_000_000_000) return (value / 1_000_000_000).ToString("0.##") + " tỷ";
            if (value >= 1_000_000) return (value / 1_000_000).ToString("0.#") + " tr";
            return value.ToString("#,##0") + " đ";
        }

        public ICommand OpenNhanVienCommand { get; }
        public ICommand OpenCongViecCommand { get; }
        public ICommand OpenKhachHangCommand { get; }
        public ICommand OpenNhaCungCapCommand { get; }
        public ICommand OpenHangHoaCommand { get; }
        public ICommand OpenTheLoaiCommand { get; }
        public ICommand OpenMauSacCommand { get; }
        public ICommand OpenHangSXCommand { get; }
        public ICommand OpenNuocSXCommand { get; }
        public ICommand OpenPhanhCommand { get; }
        public ICommand OpenDongCoCommand { get; }
        public ICommand OpenTinhTrangCommand { get; }
        public ICommand OpenHoaDonNhapCommand { get; }
        public ICommand OpenHoaDonBanCommand { get; }
        public ICommand OpenTimHangCommand { get; }
        public ICommand OpenTimKhachHangCommand { get; }
        public ICommand OpenTimHDNCommand { get; }
        public ICommand OpenTimDDHCommand { get; }
        public ICommand OpenBaoCaoBanCommand { get; }
        public ICommand OpenBaoCaoNhapCommand { get; }
        public ICommand OpenBaoCaoKQKDCommand { get; }
        public ICommand OpenBaoCaoTopCommand { get; }
        public ICommand OpenAiCommand { get; }
        public ICommand OpenUserAdminCommand { get; }
        public ICommand OpenAuditLogCommand { get; }
        public ICommand BuyFeaturedProductCommand { get; }
        public ICommand GlobalSearchCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand HomeCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ExitCommand { get; }

        public string GlobalSearchKeyword
        {
            get => globalSearchKeyword;
            set => SetProperty(ref globalSearchKeyword, value);
        }

        public DataView GlobalSearchResults
        {
            get => globalSearchResults;
            private set => SetProperty(ref globalSearchResults, value);
        }

        public bool HasSearchResults
        {
            get => hasSearchResults;
            private set => SetProperty(ref hasSearchResults, value);
        }

        private void ExecuteGlobalSearch()
        {
            if (string.IsNullOrWhiteSpace(GlobalSearchKeyword))
            {
                ClearGlobalSearch();
                return;
            }

            try
            {
                DataTable table = globalSearchService.Search(GlobalSearchKeyword);
                GlobalSearchResults = table.DefaultView;
                HasSearchResults = true;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Global search failed.", ex);
                dialogService.ShowError("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        private void ClearGlobalSearch()
        {
            GlobalSearchKeyword = "";
            GlobalSearchResults = null;
            HasSearchResults = false;
        }

        private RelayCommand CreateAuthorizedCommand(string permission, Action action)
        {
            return new RelayCommand(_ => RunAuthorized(permission, action), _ => HasPermission(permission));
        }

        private bool HasPermission(string permission)
        {
            return currentUser != null && currentUser.HasPermission(permission);
        }

        private void RunAuthorized(string permission, Action action)
        {
            if (!HasPermission(permission))
            {
                dialogService.ShowWarning("Tài khoản của bạn không có quyền sử dụng chức năng này.", "Không đủ quyền");
                return;
            }

            action();
        }

        private void Show(string title, object view, string navKey)
        {
            try
            {
                CurrentView = view;
                CurrentTitle = title;
                IsNotHome = true;
                ActiveNavKey = navKey;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Cannot show content view.", ex);
                dialogService.ShowError("Không thể mở chức năng này.\n" + ex.Message);
                GoHome();
            }
        }

        private void GoHome()
        {
            CurrentView = homeView;
            CurrentTitle = "Bảng điều khiển";
            IsNotHome = false;
            ActiveNavKey = "Home";
        }

        private void BuyFeaturedProduct(string productId)
        {
            if (!HasPermission(PermissionNames.SalesInvoice))
            {
                dialogService.ShowWarning("Tài khoản của bạn không có quyền bán hàng.", "Không đủ quyền");
                return;
            }

            if (string.IsNullOrWhiteSpace(productId))
            {
                dialogService.ShowWarning("Không xác định được sản phẩm cần mua.");
                return;
            }

            Show("Hóa đơn bán hàng", new HoaDonBanWindow(GoHome, productId.Trim()), "HoaDonBan");
        }

        private static DanhMucConfig TwoField(string title, string table, string key, string keyHeader, string name, string nameHeader)
        {
            return new DanhMucConfig(title, table, new List<FieldConfig>
            {
                new FieldConfig(key, keyHeader, FieldKind.Text, true, true),
                new FieldConfig(name, nameHeader, FieldKind.Text, true, false)
            });
        }

        private void Logout()
        {
            if (!dialogService.Confirm("Bạn có muốn đăng xuất khỏi hệ thống không?", "Đăng xuất")) return;

            if (Application.Current is App app)
            {
                app.SignOut(Application.Current.MainWindow);
            }
        }

        private void Exit()
        {
            if (dialogService.Confirm("Bạn có muốn thoát không?", "Thông báo"))
            {
                Application.Current.Shutdown();
            }
        }
    }
}
