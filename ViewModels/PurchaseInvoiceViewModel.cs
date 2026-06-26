using System;
using System.Globalization;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Input;
using QLXeMay.Class;
using QLXeMay.Domain;
using QLXeMay.Infrastructure;
using QLXeMay.Models;
using QLXeMay.Services;

namespace QLXeMay.ViewModels
{
    internal sealed class PurchaseInvoiceViewModel : ViewModelBase
    {
        private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");
        private readonly IPurchaseInvoiceService invoiceService;
        private readonly IExcelExportService excelExportService;
        private readonly IDialogService dialogService;
        private readonly Action closeAction;
        private readonly RelayCommand exportCommand;
        private string invoiceNo;
        private DateTime? invoiceDate = DateTime.Today;
        private string selectedEmployeeId;
        private string employeeName;
        private string selectedSupplierId;
        private string supplierName;
        private string supplierAddress;
        private string supplierPhone;
        private string selectedProductId;
        private string productName;
        private string quantity;
        private string unitPrice;
        private string discount = "0";
        private string lineTotal = "0";
        private string total = "0";
        private string amountInWords;
        private bool isInvoiceActive;
        private DataView detailsView;
        private DataRowView selectedDetail;
        private string selectedSearchInvoiceNo;

        public PurchaseInvoiceViewModel(
            IPurchaseInvoiceService invoiceService,
            IExcelExportService excelExportService,
            IDialogService dialogService,
            Action closeAction)
        {
            this.invoiceService = invoiceService;
            this.excelExportService = excelExportService;
            this.dialogService = dialogService;
            this.closeAction = closeAction;

            NewInvoiceCommand = new RelayCommand(_ => NewInvoice());
            SaveLineCommand = new RelayCommand(_ => SaveLine());
            ResetCommand = new RelayCommand(_ => Reset());
            CancelInvoiceCommand = new RelayCommand(_ => CancelInvoice());
            SearchInvoiceCommand = new RelayCommand(_ => SearchInvoice());
            DeleteLineCommand = new RelayCommand(_ => DeleteSelectedLine());
            exportCommand = new RelayCommand(_ => Export(), _ => DetailsView != null && DetailsView.Count > 0);
            ExportCommand = exportCommand;
            CloseCommand = new RelayCommand(_ => closeAction());

            RefreshLookups();
            NewInvoice();
        }

        public DataView Employees { get; private set; }
        public DataView Suppliers { get; private set; }
        public DataView Products { get; private set; }
        public DataView InvoiceNumbers { get; private set; }

        public string InvoiceNo { get => invoiceNo; set => SetProperty(ref invoiceNo, value); }
        public DateTime? InvoiceDate { get => invoiceDate; set => SetProperty(ref invoiceDate, value); }

        public string SelectedEmployeeId
        {
            get => selectedEmployeeId;
            set
            {
                if (SetProperty(ref selectedEmployeeId, value))
                    EmployeeName = invoiceService.GetEmployeeName(value);
            }
        }

        public string EmployeeName { get => employeeName; private set => SetProperty(ref employeeName, value); }

        public string SelectedSupplierId
        {
            get => selectedSupplierId;
            set
            {
                if (!SetProperty(ref selectedSupplierId, value)) return;
                PartyInfo info = invoiceService.GetSupplierInfo(value);
                SupplierName = info.Name;
                SupplierAddress = info.Address;
                SupplierPhone = info.Phone;
            }
        }

        public string SupplierName { get => supplierName; private set => SetProperty(ref supplierName, value); }
        public string SupplierAddress { get => supplierAddress; private set => SetProperty(ref supplierAddress, value); }
        public string SupplierPhone { get => supplierPhone; private set => SetProperty(ref supplierPhone, value); }

        public string SelectedProductId
        {
            get => selectedProductId;
            set
            {
                if (SetProperty(ref selectedProductId, value))
                    ProductName = invoiceService.GetProductName(value);
            }
        }

        public string ProductName { get => productName; private set => SetProperty(ref productName, value); }
        public string Quantity
        {
            get => quantity;
            set
            {
                if (SetProperty(ref quantity, value))
                    RecalculateLineTotalPreview();
            }
        }

        public string UnitPrice
        {
            get => unitPrice;
            set
            {
                if (SetProperty(ref unitPrice, value))
                    RecalculateLineTotalPreview();
            }
        }

        public string Discount
        {
            get => discount;
            set
            {
                if (SetProperty(ref discount, value))
                    RecalculateLineTotalPreview();
            }
        }
        public string LineTotal { get => lineTotal; private set => SetProperty(ref lineTotal, value); }
        public string Total { get => total; private set => SetProperty(ref total, value); }
        public string AmountInWords { get => amountInWords; private set => SetProperty(ref amountInWords, value); }
        public bool IsInvoiceActive { get => isInvoiceActive; private set => SetProperty(ref isInvoiceActive, value); }
        public DataView DetailsView { get => detailsView; private set { SetProperty(ref detailsView, value); exportCommand.RaiseCanExecuteChanged(); } }
        public DataRowView SelectedDetail { get => selectedDetail; set => SetProperty(ref selectedDetail, value); }
        public string SelectedSearchInvoiceNo { get => selectedSearchInvoiceNo; set => SetProperty(ref selectedSearchInvoiceNo, value); }

        public ICommand NewInvoiceCommand { get; }
        public ICommand SaveLineCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand CancelInvoiceCommand { get; }
        public ICommand SearchInvoiceCommand { get; }
        public ICommand DeleteLineCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand CloseCommand { get; }

        public void FormatUnitPriceForDisplay()
        {
            if (!TryParseFlexibleNumber(UnitPrice, out double parsedUnitPrice))
                return;

            UnitPrice = parsedUnitPrice.ToString("N0", ViCulture);
        }

        private void NewInvoice()
        {
            InvoiceNo = Function.CreateKey("HDN");
            InvoiceDate = DateTime.Today;
            IsInvoiceActive = true;
            ClearDetailInputs();
            ApplyTotal(0);
            DetailsView = null;
        }

        private void SaveLine()
        {
            if (!Validate(out int parsedQuantity, out double parsedUnitPrice, out double parsedDiscount)) return;

            try
            {
                PurchaseInvoiceOperationResult result = invoiceService.SaveLine(new PurchaseInvoiceRequest
                {
                    InvoiceNo = InvoiceNo.Trim(),
                    EmployeeId = SelectedEmployeeId,
                    SupplierId = SelectedSupplierId,
                    ProductId = SelectedProductId,
                    InvoiceDate = InvoiceDate ?? DateTime.Today,
                    Quantity = parsedQuantity,
                    UnitPrice = parsedUnitPrice,
                    Discount = parsedDiscount
                });

                LineTotal = result.LineTotal.ToString("N0");
                ApplyTotal(result.Total);
                DetailsView = result.Details.DefaultView;
                RefreshLookups();
            }
            catch (Exception ex)
            {
                dialogService.ShowError(ex.Message);
            }
        }

        private void DeleteSelectedLine()
        {
            if (SelectedDetail == null)
            {
                dialogService.ShowInformation("Chọn dòng hàng cần xóa.");
                return;
            }

            if (!dialogService.Confirm("Bạn có muốn xóa dòng hàng này không?")) return;

            try
            {
                string productId = SelectedDetail["mahang"].ToString().Trim();
                int quantity = Function.ToInt(SelectedDetail["soluong"].ToString());
                PurchaseInvoiceOperationResult result = invoiceService.DeleteLine(InvoiceNo, productId, quantity);
                ApplyTotal(result.Total);
                DetailsView = result.Details.DefaultView;
                SelectedDetail = null;
            }
            catch (Exception ex)
            {
                dialogService.ShowError(ex.Message);
            }
        }

        private void CancelInvoice()
        {
            if (string.IsNullOrWhiteSpace(InvoiceNo)) return;
            if (!dialogService.Confirm("Bạn có muốn hủy toàn bộ hóa đơn không?")) return;

            try
            {
                invoiceService.CancelInvoice(InvoiceNo.Trim());
                Reset();
                RefreshLookups();
            }
            catch (Exception ex)
            {
                dialogService.ShowError(ex.Message);
            }
        }

        private void SearchInvoice()
        {
            if (string.IsNullOrWhiteSpace(SelectedSearchInvoiceNo))
            {
                dialogService.ShowInformation("Chọn mã hóa đơn.");
                return;
            }

            PurchaseInvoiceSnapshot invoice = invoiceService.LoadInvoice(SelectedSearchInvoiceNo);
            if (invoice == null) return;

            InvoiceNo = invoice.InvoiceNo;
            InvoiceDate = invoice.InvoiceDate;
            SelectedEmployeeId = invoice.EmployeeId;
            SelectedSupplierId = invoice.SupplierId;
            ApplyTotal(invoice.Total);
            DetailsView = invoice.Details.DefaultView;
            IsInvoiceActive = true;
        }

        private void Reset()
        {
            NewInvoice();
        }

        private void Export()
        {
            if (DetailsView == null || DetailsView.Count == 0)
            {
                dialogService.ShowInformation("Không có dữ liệu để xuất Excel.");
                return;
            }

            excelExportService.Export(DetailsView.ToTable(), "HÓA ĐƠN NHẬP HÀNG - " + InvoiceNo, "HoaDonNhap");
        }

        private bool Validate(out int parsedQuantity, out double parsedUnitPrice, out double parsedDiscount)
        {
            parsedQuantity = 0;
            parsedUnitPrice = 0;
            parsedDiscount = 0;
            if (string.IsNullOrWhiteSpace(InvoiceNo)) { dialogService.ShowWarning("Bạn phải tạo hóa đơn trước."); return false; }
            if (string.IsNullOrWhiteSpace(SelectedEmployeeId)) { dialogService.ShowWarning("Chọn mã nhân viên."); return false; }
            if (string.IsNullOrWhiteSpace(SelectedSupplierId)) { dialogService.ShowWarning("Chọn mã nhà cung cấp."); return false; }
            if (string.IsNullOrWhiteSpace(SelectedProductId)) { dialogService.ShowWarning("Chọn mã hàng."); return false; }
            if (!int.TryParse((Quantity ?? string.Empty).Trim(), out parsedQuantity) || parsedQuantity <= 0)
            {
                dialogService.ShowWarning("Số lượng phải là số nguyên dương.");
                return false;
            }

            if (!TryParseFlexibleNumber(UnitPrice, out parsedUnitPrice) || parsedUnitPrice <= 0)
            {
                dialogService.ShowWarning("Đơn giá phải là số dương.");
                return false;
            }

            if (!TryParseFlexibleNumber(Discount, out parsedDiscount))
                parsedDiscount = 0;

            parsedDiscount = Math.Max(0, Math.Min(100, parsedDiscount));
            return true;
        }

        private void ClearDetailInputs()
        {
            SelectedProductId = null;
            ProductName = "";
            Quantity = "";
            UnitPrice = "";
            Discount = "0";
            LineTotal = "0";
        }

        private void RecalculateLineTotalPreview()
        {
            if (!int.TryParse((Quantity ?? string.Empty).Trim(), out int parsedQuantity) || parsedQuantity <= 0)
            {
                LineTotal = "0";
                return;
            }

            if (!TryParseFlexibleNumber(UnitPrice, out double parsedUnitPrice) || parsedUnitPrice <= 0)
            {
                LineTotal = "0";
                return;
            }

            if (!TryParseFlexibleNumber(Discount, out double parsedDiscount))
                parsedDiscount = 0;

            double line = InvoiceCalculator.CalculateLineTotal(parsedQuantity, parsedUnitPrice, parsedDiscount);
            LineTotal = line.ToString("N0", ViCulture);
        }

        private static bool TryParseFlexibleNumber(string input, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string cleaned = input.Trim().ToLowerInvariant();
            cleaned = cleaned.Replace("vnđ", "").Replace("vnd", "").Replace("%", "").Replace(" ", "");

            if (Regex.IsMatch(cleaned, @"^\d{1,3}(\.\d{3})+(,\d+)?$"))
            {
                cleaned = cleaned.Replace(".", "").Replace(",", ".");
            }
            else if (Regex.IsMatch(cleaned, @"^\d{1,3}(,\d{3})+(\.\d+)?$"))
            {
                cleaned = cleaned.Replace(",", "");
            }
            else
            {
                cleaned = cleaned.Replace(",", ".");
            }

            return double.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private void ApplyTotal(double totalValue)
        {
            Total = totalValue.ToString("N0");
            AmountInWords = Function.ChuyenSoSangChu(((long)totalValue).ToString());
        }

        private void RefreshLookups()
        {
            Employees = invoiceService.LoadEmployees().DefaultView;
            Suppliers = invoiceService.LoadSuppliers().DefaultView;
            Products = invoiceService.LoadProducts().DefaultView;
            InvoiceNumbers = invoiceService.LoadInvoiceNumbers().DefaultView;
            OnPropertyChanged(nameof(Employees));
            OnPropertyChanged(nameof(Suppliers));
            OnPropertyChanged(nameof(Products));
            OnPropertyChanged(nameof(InvoiceNumbers));
        }
    }
}
