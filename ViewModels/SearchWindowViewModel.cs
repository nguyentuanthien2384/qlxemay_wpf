using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Input;
using QLXeMay.Infrastructure;
using QLXeMay.Models;
using QLXeMay.Services;

namespace QLXeMay.ViewModels
{
    internal sealed class SearchWindowViewModel : ViewModelBase
    {
        private readonly SearchMode mode;
        private readonly ISearchService searchService;
        private readonly IExcelExportService excelExportService;
        private readonly IDialogService dialogService;
        private readonly Func<IReadOnlyDictionary<string, string>> readCriteria;
        private readonly Action clearCriteria;
        private readonly Action closeAction;
        private readonly RelayCommand exportCommand;
        private DataView resultView;
        private DataTable currentResult;

        public SearchWindowViewModel(
            SearchMode mode,
            ISearchService searchService,
            IExcelExportService excelExportService,
            IDialogService dialogService,
            Func<IReadOnlyDictionary<string, string>> readCriteria,
            Action clearCriteria,
            Action closeAction)
        {
            this.mode = mode;
            this.searchService = searchService;
            this.excelExportService = excelExportService;
            this.dialogService = dialogService;
            this.readCriteria = readCriteria;
            this.clearCriteria = clearCriteria;
            this.closeAction = closeAction;

            SearchCommand = new RelayCommand(_ => Search());
            ResetCommand = new RelayCommand(_ => Reset());
            exportCommand = new RelayCommand(_ => Export(), _ => currentResult != null && currentResult.Rows.Count > 0);
            ExportCommand = exportCommand;
            CloseCommand = new RelayCommand(_ => closeAction());
        }

        public string WindowTitle
        {
            get
            {
                if (mode == SearchMode.Hang) return "TÌM KIẾM HÀNG HÓA";
                if (mode == SearchMode.KhachHang) return "TÌM KIẾM KHÁCH HÀNG";
                if (mode == SearchMode.HoaDonNhap) return "TÌM KIẾM HÓA ĐƠN NHẬP";
                return "TÌM KIẾM ĐƠN ĐẶT HÀNG";
            }
        }

        public DataView ResultView
        {
            get => resultView;
            private set => SetProperty(ref resultView, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand CloseCommand { get; }

        private void Search()
        {
            currentResult = searchService.Search(mode, readCriteria());
            ResultView = currentResult.DefaultView;
            exportCommand.RaiseCanExecuteChanged();
            dialogService.ShowInformation($"Tìm thấy {currentResult.Rows.Count} dòng dữ liệu.", "Kết quả");
        }

        private void Reset()
        {
            clearCriteria();
            currentResult = null;
            ResultView = null;
            exportCommand.RaiseCanExecuteChanged();
        }

        private void Export()
        {
            if (currentResult == null || currentResult.Rows.Count == 0)
            {
                dialogService.ShowInformation("Không có dữ liệu để xuất Excel.");
                return;
            }

            excelExportService.Export(
                currentResult,
                WindowTitle,
                "TimKiem",
                BuildExportHeaderFields(),
                BuildExportFooterFields());
        }

        private IReadOnlyList<ExcelExportField> BuildExportHeaderFields()
        {
            List<ExcelExportField> fields = new List<ExcelExportField>
            {
                new ExcelExportField("Chức năng", WindowTitle)
            };

            IReadOnlyDictionary<string, string> criteria = readCriteria();
            foreach (KeyValuePair<string, string> item in criteria)
            {
                if (string.IsNullOrWhiteSpace(item.Value)) continue;
                fields.Add(new ExcelExportField(item.Key, item.Value));
            }

            return fields;
        }

        private IReadOnlyList<ExcelExportField> BuildExportFooterFields()
        {
            return new List<ExcelExportField>
            {
                new ExcelExportField("Số dòng kết quả", currentResult?.Rows.Count.ToString() ?? "0")
            };
        }
    }
}
