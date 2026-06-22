using System.Windows;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class HoaDonNhapWindow : Window
    {
        public HoaDonNhapWindow()
        {
            InitializeComponent();
            DataContext = new PurchaseInvoiceViewModel(
                new PurchaseInvoiceService(),
                new ExcelExportService(),
                new DialogService(),
                Close);
        }
    }
}
