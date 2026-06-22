using System;
using System.Collections.Generic;
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
        private readonly UserSession currentUser;
        private readonly HomeView homeView;

        private object currentView;
        private bool isNotHome;
        private string currentTitle = "Bảng điều khiển";

        private string revenueText = "—";
        private string profitText = "—";
        private string orderCountText = "—";
        private string stockText = "—";
        private string customerText = "—";
        private System.Collections.Generic.IReadOnlyList<RevenueBar> monthlyRevenue = new System.Collections.Generic.List<RevenueBar>();

        public MainWindowViewModel(IWindowService windowService, IDialogService dialogService)
        {
            this.windowService = windowService;
            this.dialogService = dialogService;
            currentUser = AppSession.CurrentUser;
            homeView = new HomeView();
            LoadDashboard();

            OpenCongViecCommand = CreateAuthorizedCommand(PermissionNames.ManageEmployees, () => Show("Danh mục công việc", new DanhMucWindow(new DanhMucConfig("DANH MỤC CÔNG VIỆC", "tblcongviec", new List<FieldConfig>
            {
                new FieldConfig("macv", "Mã công việc", FieldKind.Text, true, true),
                new FieldConfig("tencv", "Tên công việc", FieldKind.Text, true, false),
                new FieldConfig("luongthang", "Lương tháng", FieldKind.Number, true, false)
            }), GoHome)));

            OpenNhanVienCommand = CreateAuthorizedCommand(PermissionNames.ManageEmployees, () => Show("Nhân viên", new DanhMucWindow(new DanhMucConfig("DANH MỤC NHÂN VIÊN", "tblnhanvien", new List<FieldConfig>
            {
                new FieldConfig("manv", "Mã nhân viên", FieldKind.Text, true, true),
                new FieldConfig("tennv", "Tên nhân viên", FieldKind.Text, true, false),
                new FieldConfig("gioitinh", "Giới tính", FieldKind.Text, true, false),
                new FieldConfig("ngaysinh", "Ngày sinh", FieldKind.Date, true, false),
                new FieldConfig("sdt", "Điện thoại", FieldKind.Text, true, false),
                new FieldConfig("diachi", "Địa chỉ", FieldKind.Text, true, false),
                new FieldConfig("macv", "Công việc", FieldKind.Combo, true, false,
                    "SELECT macv, macv + ' - ' + tencv AS hienthi FROM tblcongviec", "macv", "hienthi")
            }), GoHome)));

            OpenKhachHangCommand = CreateAuthorizedCommand(PermissionNames.ManageCustomers, () => Show("Khách hàng", new DanhMucWindow(new DanhMucConfig("DANH MỤC KHÁCH HÀNG", "tblkhachhang", new List<FieldConfig>
            {
                new FieldConfig("makhach", "Mã khách", FieldKind.Text, true, true),
                new FieldConfig("tenkhach", "Tên khách", FieldKind.Text, true, false),
                new FieldConfig("diachi", "Địa chỉ", FieldKind.Text, true, false),
                new FieldConfig("sdt", "Điện thoại", FieldKind.Text, true, false)
            }), GoHome)));

            OpenNhaCungCapCommand = CreateAuthorizedCommand(PermissionNames.ManageSuppliers, () => Show("Nhà cung cấp", new DanhMucWindow(new DanhMucConfig("DANH MỤC NHÀ CUNG CẤP", "tblnhacungcap", new List<FieldConfig>
            {
                new FieldConfig("mancc", "Mã NCC", FieldKind.Text, true, true),
                new FieldConfig("tenncc", "Tên NCC", FieldKind.Text, true, false),
                new FieldConfig("diachi", "Địa chỉ", FieldKind.Text, true, false),
                new FieldConfig("sdt", "Điện thoại", FieldKind.Text, true, false)
            }), GoHome)));

            OpenHangHoaCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Xe máy", new DanhMucWindow(new DanhMucConfig("DANH MỤC HÀNG HÓA", "tbldmhang", new List<FieldConfig>
            {
                new FieldConfig("mahang", "Mã hàng", FieldKind.Text, true, true),
                new FieldConfig("tenhang", "Tên hàng", FieldKind.Text, true, false),
                new FieldConfig("maloai", "Thể loại", FieldKind.Combo, true, false, "SELECT maloai, maloai + ' - ' + tenloai AS ht FROM tbltheloai", "maloai", "ht"),
                new FieldConfig("mahangsx", "Hãng SX", FieldKind.Combo, true, false, "SELECT mahangsx, mahangsx + ' - ' + tenhangsx AS ht FROM tblhangsx", "mahangsx", "ht"),
                new FieldConfig("mamau", "Màu", FieldKind.Combo, true, false, "SELECT mamau, mamau + ' - ' + tenmau AS ht FROM tblmausac", "mamau", "ht"),
                new FieldConfig("namsx", "Năm SX", FieldKind.Number, true, false),
                new FieldConfig("maphanh", "Phanh", FieldKind.Combo, true, false, "SELECT maphanh, maphanh + ' - ' + tenphanh AS ht FROM tblphanhxe", "maphanh", "ht"),
                new FieldConfig("madongco", "Động cơ", FieldKind.Combo, true, false, "SELECT madongco, madongco + ' - ' + tendongco AS ht FROM tbldongco", "madongco", "ht"),
                new FieldConfig("manuocsx", "Nước SX", FieldKind.Combo, true, false, "SELECT manuocsx, manuocsx + ' - ' + tennuocsx AS ht FROM tblnuocsx", "manuocsx", "ht"),
                new FieldConfig("matt", "Tình trạng", FieldKind.Combo, true, false, "SELECT matt, matt + ' - ' + tentt AS ht FROM tbltinhtrang", "matt", "ht"),
                new FieldConfig("dungtichbinhxang", "Dung tích bình xăng", FieldKind.Number, true, false),
                new FieldConfig("anh", "Đường dẫn ảnh", FieldKind.Text, false, false),
                new FieldConfig("thoigianbaohanh", "Bảo hành", FieldKind.Number, true, false),
                new FieldConfig("soluong", "Số lượng", FieldKind.Number, true, false),
                new FieldConfig("dongianhap", "Đơn giá nhập", FieldKind.Number, true, false),
                new FieldConfig("dongiaban", "Đơn giá bán", FieldKind.Number, true, false)
            }), GoHome)));

            OpenTheLoaiCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Thể loại", new DanhMucWindow(TwoField("DANH MỤC THỂ LOẠI", "tbltheloai", "maloai", "Mã loại", "tenloai", "Tên loại"), GoHome)));
            OpenMauSacCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Màu sắc", new DanhMucWindow(TwoField("DANH MỤC MÀU SẮC", "tblmausac", "mamau", "Mã màu", "tenmau", "Tên màu"), GoHome)));
            OpenHangSXCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Hãng sản xuất", new DanhMucWindow(TwoField("DANH MỤC HÃNG SẢN XUẤT", "tblhangsx", "mahangsx", "Mã hãng", "tenhangsx", "Tên hãng"), GoHome)));
            OpenNuocSXCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Nước sản xuất", new DanhMucWindow(TwoField("DANH MỤC NƯỚC SẢN XUẤT", "tblnuocsx", "manuocsx", "Mã nước", "tennuocsx", "Tên nước"), GoHome)));
            OpenPhanhCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Phanh xe", new DanhMucWindow(TwoField("DANH MỤC PHANH XE", "tblphanhxe", "maphanh", "Mã phanh", "tenphanh", "Tên phanh"), GoHome)));
            OpenDongCoCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Động cơ", new DanhMucWindow(TwoField("DANH MỤC ĐỘNG CƠ", "tbldongco", "madongco", "Mã động cơ", "tendongco", "Tên động cơ"), GoHome)));
            OpenTinhTrangCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Show("Tình trạng", new DanhMucWindow(TwoField("DANH MỤC TÌNH TRẠNG", "tbltinhtrang", "matt", "Mã tình trạng", "tentt", "Tên tình trạng"), GoHome)));

            OpenHoaDonNhapCommand = CreateAuthorizedCommand(PermissionNames.PurchaseInvoice, () => Show("Hóa đơn nhập hàng", new HoaDonNhapWindow(GoHome)));
            OpenHoaDonBanCommand = CreateAuthorizedCommand(PermissionNames.SalesInvoice, () => Show("Hóa đơn bán hàng", new HoaDonBanWindow(GoHome)));
            OpenTimHangCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Show("Tìm kiếm", new SearchWindow(SearchMode.Hang, GoHome)));
            OpenTimKhachHangCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Show("Tìm kiếm khách hàng", new SearchWindow(SearchMode.KhachHang, GoHome)));
            OpenTimHDNCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Show("Tìm hóa đơn nhập", new SearchWindow(SearchMode.HoaDonNhap, GoHome)));
            OpenTimDDHCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Show("Tìm đơn đặt hàng", new SearchWindow(SearchMode.DonDatHang, GoHome)));
            OpenBaoCaoBanCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Show("Báo cáo bán hàng", new ReportWindow(ReportMode.BanHang, GoHome)));
            OpenBaoCaoNhapCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Show("Báo cáo nhập hàng", new ReportWindow(ReportMode.NhapHang, GoHome)));
            OpenBaoCaoKQKDCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Show("Doanh thu", new ReportWindow(ReportMode.KetQuaKinhDoanh, GoHome)));
            OpenBaoCaoTopCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Show("Top sản phẩm", new ReportWindow(ReportMode.TopSanPham, GoHome)));
            OpenAiCommand = CreateAuthorizedCommand(PermissionNames.AiAssistant, () => Show("Trợ lý AI", new AIAssistantWindow()));
            OpenUserAdminCommand = CreateAuthorizedCommand(PermissionNames.UserAdmin, () => Show("Quản trị tài khoản", new UserAdminWindow(GoHome)));
            OpenAuditLogCommand = CreateAuthorizedCommand(PermissionNames.AuditLog, () => Show("Nhật ký hệ thống", new AuditLogWindow(GoHome)));
            HomeCommand = new RelayCommand(_ => GoHome());
            BackCommand = new RelayCommand(_ => GoHome());
            LogoutCommand = new RelayCommand(_ => Logout());
            ExitCommand = new RelayCommand(_ => Exit());

            GoHome();
        }

        public string CurrentUserText => currentUser == null ? "Chưa đăng nhập" : currentUser.DisplayText;
        public string AccountMenuHeader => currentUser == null ? "Tài khoản" : "Tài khoản: " + currentUser.UserName;
        public string RoleText => currentUser == null ? "" : currentUser.RoleDisplayName;

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
        public ICommand HomeCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ExitCommand { get; }

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

        private void Show(string title, object view)
        {
            try
            {
                CurrentView = view;
                CurrentTitle = title;
                IsNotHome = true;
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
