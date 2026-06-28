using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using QLXeMay.Infrastructure;
using QLXeMay.Models;
using QLXeMay.Services;

namespace QLXeMay.ViewModels
{
    internal sealed class ReportWindowViewModel : ViewModelBase
    {
        private readonly ReportMode mode;
        private readonly IReportService reportService;
        private readonly IExcelExportService excelExportService;
        private readonly IDialogService dialogService;
        private readonly Action closeAction;
        private readonly RelayCommand exportCommand;
        private DateTime? dateFrom;
        private DateTime? dateTo;
        private ReportGroupOption selectedGroup;
        private DataView mainView;
        private DataView summaryView;
        private DataTable currentMainTable;
        private string summaryText;

        public ReportWindowViewModel(
            ReportMode mode,
            IReportService reportService,
            IExcelExportService excelExportService,
            IDialogService dialogService,
            Action closeAction)
        {
            this.mode = mode;
            this.reportService = reportService;
            this.excelExportService = excelExportService;
            this.dialogService = dialogService;
            this.closeAction = closeAction;

            DateFrom = DateTime.Today.AddMonths(-1);
            DateTo = DateTime.Today;
            GroupOptions = new ObservableCollection<ReportGroupOption>
            {
                new ReportGroupOption("Sản phẩm", "sanpham"),
                new ReportGroupOption("Thể loại", "theloai"),
                new ReportGroupOption("Hãng sản xuất", "hangsx")
            };
            SelectedGroup = GroupOptions[0];

            ShowCommand = new RelayCommand(_ => LoadReport());
            exportCommand = new RelayCommand(_ => Export(), _ => currentMainTable != null && currentMainTable.Rows.Count > 0);
            ExportCommand = exportCommand;
            CloseCommand = new RelayCommand(_ => closeAction());

            LoadReport();
        }

        public string WindowTitle
        {
            get
            {
                if (mode == ReportMode.BanHang) return "BÁO CÁO BÁN HÀNG";
                if (mode == ReportMode.NhapHang) return "BÁO CÁO NHẬP HÀNG";
                if (mode == ReportMode.KetQuaKinhDoanh) return "BÁO CÁO KẾT QUẢ KINH DOANH";
                return "BÁO CÁO TOP SẢN PHẨM";
            }
        }

        public Visibility DateFilterVisibility => mode == ReportMode.TopSanPham ? Visibility.Collapsed : Visibility.Visible;
        public Visibility GroupFilterVisibility => mode == ReportMode.TopSanPham ? Visibility.Visible : Visibility.Collapsed;

        public DateTime? DateFrom
        {
            get => dateFrom;
            set => SetProperty(ref dateFrom, value);
        }

        public DateTime? DateTo
        {
            get => dateTo;
            set => SetProperty(ref dateTo, value);
        }

        public ObservableCollection<ReportGroupOption> GroupOptions { get; }

        public ReportGroupOption SelectedGroup
        {
            get => selectedGroup;
            set => SetProperty(ref selectedGroup, value);
        }

        public DataView MainView
        {
            get => mainView;
            private set => SetProperty(ref mainView, value);
        }

        public DataView SummaryView
        {
            get => summaryView;
            private set => SetProperty(ref summaryView, value);
        }

        public string SummaryText
        {
            get => summaryText;
            private set => SetProperty(ref summaryText, value);
        }

        public ICommand ShowCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand CloseCommand { get; }

        private void LoadReport()
        {
            if (DateFrom.HasValue && DateTo.HasValue && DateFrom.Value.Date > DateTo.Value.Date)
            {
                dialogService.ShowWarning("Từ ngày không được lớn hơn đến ngày.");
                return;
            }

            DateTime from = DateFrom ?? DateTime.Today.AddMonths(-1);
            DateTime to = DateTo ?? DateTime.Today;
            string groupKey = SelectedGroup == null ? "sanpham" : SelectedGroup.Key;
            ReportResult result = reportService.BuildReport(mode, from, to, groupKey);

            currentMainTable = result.MainTable;
            MainView = result.MainTable.DefaultView;
            SummaryView = result.SummaryTable == null ? null : result.SummaryTable.DefaultView;
            SummaryText = result.SummaryText;
            exportCommand.RaiseCanExecuteChanged();
        }

        private void Export()
        {
            if (currentMainTable == null || currentMainTable.Rows.Count == 0)
            {
                dialogService.ShowInformation("Không có dữ liệu để xuất Excel.");
                return;
            }

            excelExportService.Export(
                currentMainTable,
                WindowTitle,
                "BaoCao",
                BuildExportHeaderFields(),
                BuildExportFooterFields());
        }

        private IReadOnlyList<ExcelExportField> BuildExportHeaderFields()
        {
            List<ExcelExportField> result = new List<ExcelExportField>
            {
                new ExcelExportField("Loại báo cáo", WindowTitle)
            };

            if (mode == ReportMode.TopSanPham)
            {
                result.Add(new ExcelExportField("Nhóm thống kê", SelectedGroup?.Name ?? "Sản phẩm"));
            }
            else
            {
                result.Add(new ExcelExportField("Từ ngày", (DateFrom ?? DateTime.Today.AddMonths(-1)).ToString("dd/MM/yyyy")));
                result.Add(new ExcelExportField("Đến ngày", (DateTo ?? DateTime.Today).ToString("dd/MM/yyyy")));
            }

            return result;
        }

        private IReadOnlyList<ExcelExportField> BuildExportFooterFields()
        {
            List<ExcelExportField> result = new List<ExcelExportField>
            {
                new ExcelExportField("Tổng số dòng", currentMainTable?.Rows.Count.ToString() ?? "0")
            };

            if (!string.IsNullOrWhiteSpace(SummaryText))
            {
                result.Add(new ExcelExportField("Tóm tắt", SummaryText));
            }

            return result;
        }
    }
}
