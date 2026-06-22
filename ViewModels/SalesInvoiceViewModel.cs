using System;
using System.Data;
using System.Windows.Input;
using QLXeMay.Class;
using QLXeMay.Infrastructure;
using QLXeMay.Models;
using QLXeMay.Services;

namespace QLXeMay.ViewModels
{
    internal sealed class SalesInvoiceViewModel : ViewModelBase
    {
        private readonly ISalesInvoiceService invoiceService;
        private readonly IExcelExportService excelExportService;
        private readonly IDialogService dialogService;
        private readonly Action closeAction;
        private readonly RelayCommand exportCommand;
        private string invoiceNo;
        private DateTime? invoiceDate = DateTime.Today;
        private string selectedEmployeeId;
        private string employeeName;
        private string selectedCustomerId;
        private string customerName;
        private string customerAddress;
        private string customerPhone;
        private string selectedProductId;
        private string productName;
        private string quantity;
        private string discount = "0";
        private string lineTotal = "0";
        private string total = "0";
        private string tax = "0";
        private string deposit = "0";
        private string amountInWords;
        private bool isInvoiceActive;
        private DataView detailsView;
        private DataRowView selectedDetail;
        private string selectedSearchInvoiceNo;

        public SalesInvoiceViewModel(
            ISalesInvoiceService invoiceService,
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
            IsInvoiceActive = false;
        }

        public DataView Employees { get; private set; }
        public DataView Customers { get; private set; }
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

        public string SelectedCustomerId
        {
            get => selectedCustomerId;
            set
            {
                if (!SetProperty(ref selectedCustomerId, value)) return;
                PartyInfo info = invoiceService.GetCustomerInfo(value);
                CustomerName = info.Name;
                CustomerAddress = info.Address;
                CustomerPhone = info.Phone;
            }
        }

        public string CustomerName { get => customerName; private set => SetProperty(ref customerName, value); }
        public string CustomerAddress { get => customerAddress; private set => SetProperty(ref customerAddress, value); }
        public string CustomerPhone { get => customerPhone; private set => SetProperty(ref customerPhone, value); }

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
        public string Quantity { get => quantity; set => SetProperty(ref quantity, value); }
        public string Discount { get => discount; set => SetProperty(ref discount, value); }
        public string LineTotal { get => lineTotal; private set => SetProperty(ref lineTotal, value); }
        public string Total { get => total; private set => SetProperty(ref total, value); }
        public string Tax { get => tax; private set => SetProperty(ref tax, value); }
        public string Deposit { get => deposit; private set => SetProperty(ref deposit, value); }
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

        private void NewInvoice()
        {
            InvoiceNo = Function.CreateKey("DDH");
            InvoiceDate = DateTime.Today;
            IsInvoiceActive = true;
            ClearDetailInputs();
            ApplyTotals(0, 0, 0);
            DetailsView = null;
        }

        private void SaveLine()
        {
            if (!Validate(out int parsedQuantity, out double parsedDiscount)) return;

            try
            {
                SalesInvoiceOperationResult result = invoiceService.SaveLine(new SalesInvoiceRequest
                {
                    InvoiceNo = InvoiceNo.Trim(),
                    EmployeeId = SelectedEmployeeId,
                    CustomerId = SelectedCustomerId,
                    ProductId = SelectedProductId,
                    InvoiceDate = InvoiceDate ?? DateTime.Today,
                    Quantity = parsedQuantity,
                    Discount = parsedDiscount
                });

                LineTotal = result.LineTotal.ToString("N0");
                ApplyTotals(result.Total, result.Tax, result.Deposit);
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
                SalesInvoiceOperationResult result = invoiceService.DeleteLine(InvoiceNo, productId, quantity);
                ApplyTotals(result.Total, result.Tax, result.Deposit);
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

            SalesInvoiceSnapshot invoice = invoiceService.LoadInvoice(SelectedSearchInvoiceNo);
            if (invoice == null) return;

            InvoiceNo = invoice.InvoiceNo;
            InvoiceDate = invoice.InvoiceDate;
            SelectedEmployeeId = invoice.EmployeeId;
            SelectedCustomerId = invoice.CustomerId;
            ApplyTotals(invoice.Total, invoice.Tax, invoice.Deposit);
            DetailsView = invoice.Details.DefaultView;
            IsInvoiceActive = true;
        }

        private void Reset()
        {
            InvoiceNo = "";
            SelectedEmployeeId = null;
            SelectedCustomerId = null;
            EmployeeName = "";
            CustomerName = "";
            CustomerAddress = "";
            CustomerPhone = "";
            ClearDetailInputs();
            ApplyTotals(0, 0, 0);
            DetailsView = null;
            IsInvoiceActive = false;
        }

        private void Export()
        {
            if (DetailsView == null || DetailsView.Count == 0)
            {
                dialogService.ShowInformation("Không có dữ liệu để xuất Excel.");
                return;
            }

            excelExportService.Export(DetailsView.ToTable(), "HÓA ĐƠN BÁN HÀNG - " + InvoiceNo, "HoaDonBan");
        }

        private bool Validate(out int parsedQuantity, out double parsedDiscount)
        {
            parsedQuantity = 0;
            parsedDiscount = 0;
            if (string.IsNullOrWhiteSpace(InvoiceNo)) { dialogService.ShowWarning("Bạn phải tạo hóa đơn trước."); return false; }
            if (string.IsNullOrWhiteSpace(SelectedEmployeeId)) { dialogService.ShowWarning("Chọn mã nhân viên."); return false; }
            if (string.IsNullOrWhiteSpace(SelectedCustomerId)) { dialogService.ShowWarning("Chọn mã khách hàng."); return false; }
            if (string.IsNullOrWhiteSpace(SelectedProductId)) { dialogService.ShowWarning("Chọn mã hàng."); return false; }
            if (!Function.IsSoNguyen(Quantity) || Function.ToInt(Quantity) <= 0) { dialogService.ShowWarning("Số lượng phải là số nguyên dương."); return false; }
            if (!Function.IsSoThuc(Discount)) Discount = "0";
            parsedQuantity = Function.ToInt(Quantity);
            parsedDiscount = Function.ToDouble(Discount);
            return true;
        }

        private void ClearDetailInputs()
        {
            SelectedProductId = null;
            ProductName = "";
            Quantity = "";
            Discount = "0";
            LineTotal = "0";
        }

        private void ApplyTotals(double totalValue, double taxValue, double depositValue)
        {
            Total = totalValue.ToString("N0");
            Tax = taxValue.ToString("N0");
            Deposit = depositValue.ToString("N0");
            AmountInWords = Function.ChuyenSoSangChu(((long)totalValue).ToString());
        }

        private void RefreshLookups()
        {
            Employees = invoiceService.LoadEmployees().DefaultView;
            Customers = invoiceService.LoadCustomers().DefaultView;
            Products = invoiceService.LoadProducts().DefaultView;
            InvoiceNumbers = invoiceService.LoadInvoiceNumbers().DefaultView;
            OnPropertyChanged(nameof(Employees));
            OnPropertyChanged(nameof(Customers));
            OnPropertyChanged(nameof(Products));
            OnPropertyChanged(nameof(InvoiceNumbers));
        }
    }
}
