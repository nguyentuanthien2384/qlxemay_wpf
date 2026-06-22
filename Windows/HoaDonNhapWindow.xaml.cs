using System;
using System.Windows.Controls;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class HoaDonNhapWindow : UserControl
    {
        public HoaDonNhapWindow(Action goBack)
        {
            InitializeComponent();
            DataContext = new PurchaseInvoiceViewModel(
                new PurchaseInvoiceService(),
                new ExcelExportService(),
                new DialogService(),
                goBack ?? (() => { }));
        }
    }
}
