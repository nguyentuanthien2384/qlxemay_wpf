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

        public MainWindowViewModel(IWindowService windowService, IDialogService dialogService)
        {
            this.windowService = windowService;
            this.dialogService = dialogService;
            currentUser = AppSession.CurrentUser;

            OpenCongViecCommand = CreateAuthorizedCommand(PermissionNames.ManageEmployees, () => Open(new DanhMucWindow(new DanhMucConfig("DANH MỤC CÔNG VIỆC", "tblcongviec", new List<FieldConfig>
            {
                new FieldConfig("macv", "Mã công việc", FieldKind.Text, true, true),
                new FieldConfig("tencv", "Tên công việc", FieldKind.Text, true, false),
                new FieldConfig("luongthang", "Lương tháng", FieldKind.Number, true, false)
            }))));

            OpenNhanVienCommand = CreateAuthorizedCommand(PermissionNames.ManageEmployees, () => Open(new DanhMucWindow(new DanhMucConfig("DANH MỤC NHÂN VIÊN", "tblnhanvien", new List<FieldConfig>
            {
                new FieldConfig("manv", "Mã nhân viên", FieldKind.Text, true, true),
                new FieldConfig("tennv", "Tên nhân viên", FieldKind.Text, true, false),
                new FieldConfig("gioitinh", "Giới tính", FieldKind.Text, true, false),
                new FieldConfig("ngaysinh", "Ngày sinh", FieldKind.Date, true, false),
                new FieldConfig("sdt", "Điện thoại", FieldKind.Text, true, false),
                new FieldConfig("diachi", "Địa chỉ", FieldKind.Text, true, false),
                new FieldConfig("macv", "Công việc", FieldKind.Combo, true, false,
                    "SELECT macv, macv + ' - ' + tencv AS hienthi FROM tblcongviec", "macv", "hienthi")
            }))));

            OpenKhachHangCommand = CreateAuthorizedCommand(PermissionNames.ManageCustomers, () => Open(new DanhMucWindow(new DanhMucConfig("DANH MỤC KHÁCH HÀNG", "tblkhachhang", new List<FieldConfig>
            {
                new FieldConfig("makhach", "Mã khách", FieldKind.Text, true, true),
                new FieldConfig("tenkhach", "Tên khách", FieldKind.Text, true, false),
                new FieldConfig("diachi", "Địa chỉ", FieldKind.Text, true, false),
                new FieldConfig("sdt", "Điện thoại", FieldKind.Text, true, false)
            }))));

            OpenNhaCungCapCommand = CreateAuthorizedCommand(PermissionNames.ManageSuppliers, () => Open(new DanhMucWindow(new DanhMucConfig("DANH MỤC NHÀ CUNG CẤP", "tblnhacungcap", new List<FieldConfig>
            {
                new FieldConfig("mancc", "Mã NCC", FieldKind.Text, true, true),
                new FieldConfig("tenncc", "Tên NCC", FieldKind.Text, true, false),
                new FieldConfig("diachi", "Địa chỉ", FieldKind.Text, true, false),
                new FieldConfig("sdt", "Điện thoại", FieldKind.Text, true, false)
            }))));

            OpenHangHoaCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => Open(new DanhMucWindow(new DanhMucConfig("DANH MỤC HÀNG HÓA", "tbldmhang", new List<FieldConfig>
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
            }))));

            OpenTheLoaiCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => OpenCategory(TwoField("DANH MỤC THỂ LOẠI", "tbltheloai", "maloai", "Mã loại", "tenloai", "Tên loại")));
            OpenMauSacCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => OpenCategory(TwoField("DANH MỤC MÀU SẮC", "tblmausac", "mamau", "Mã màu", "tenmau", "Tên màu")));
            OpenHangSXCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => OpenCategory(TwoField("DANH MỤC HÃNG SẢN XUẤT", "tblhangsx", "mahangsx", "Mã hãng", "tenhangsx", "Tên hãng")));
            OpenNuocSXCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => OpenCategory(TwoField("DANH MỤC NƯỚC SẢN XUẤT", "tblnuocsx", "manuocsx", "Mã nước", "tennuocsx", "Tên nước")));
            OpenPhanhCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => OpenCategory(TwoField("DANH MỤC PHANH XE", "tblphanhxe", "maphanh", "Mã phanh", "tenphanh", "Tên phanh")));
            OpenDongCoCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => OpenCategory(TwoField("DANH MỤC ĐỘNG CƠ", "tbldongco", "madongco", "Mã động cơ", "tendongco", "Tên động cơ")));
            OpenTinhTrangCommand = CreateAuthorizedCommand(PermissionNames.ManageProducts, () => OpenCategory(TwoField("DANH MỤC TÌNH TRẠNG", "tbltinhtrang", "matt", "Mã tình trạng", "tentt", "Tên tình trạng")));

            OpenHoaDonNhapCommand = CreateAuthorizedCommand(PermissionNames.PurchaseInvoice, () => Open(new HoaDonNhapWindow()));
            OpenHoaDonBanCommand = CreateAuthorizedCommand(PermissionNames.SalesInvoice, () => Open(new HoaDonBanWindow()));
            OpenTimHangCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Open(new SearchWindow(SearchMode.Hang)));
            OpenTimKhachHangCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Open(new SearchWindow(SearchMode.KhachHang)));
            OpenTimHDNCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Open(new SearchWindow(SearchMode.HoaDonNhap)));
            OpenTimDDHCommand = CreateAuthorizedCommand(PermissionNames.Search, () => Open(new SearchWindow(SearchMode.DonDatHang)));
            OpenBaoCaoBanCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Open(new ReportWindow(ReportMode.BanHang)));
            OpenBaoCaoNhapCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Open(new ReportWindow(ReportMode.NhapHang)));
            OpenBaoCaoKQKDCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Open(new ReportWindow(ReportMode.KetQuaKinhDoanh)));
            OpenBaoCaoTopCommand = CreateAuthorizedCommand(PermissionNames.Reports, () => Open(new ReportWindow(ReportMode.TopSanPham)));
            OpenAiCommand = CreateAuthorizedCommand(PermissionNames.AiAssistant, () => Open(new AIAssistantWindow()));
            OpenUserAdminCommand = CreateAuthorizedCommand(PermissionNames.UserAdmin, () => Open(new UserAdminWindow()));
            LogoutCommand = new RelayCommand(_ => Logout());
            ExitCommand = new RelayCommand(_ => Exit());
        }

        public string CurrentUserText => currentUser == null ? "Chưa đăng nhập" : currentUser.DisplayText;
        public string AccountMenuHeader => currentUser == null ? "Tài khoản" : "Tài khoản: " + currentUser.UserName;

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

        private void Open(Window window)
        {
            try
            {
                windowService.ShowDialog(window);
            }
            catch (Exception ex)
            {
                AppLogger.Error("Cannot open child window from main menu.", ex);
                dialogService.ShowError("Không thể mở chức năng này.\n" + ex.Message);
            }
        }

        private void OpenCategory(DanhMucConfig config)
        {
            Open(new DanhMucWindow(config));
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
