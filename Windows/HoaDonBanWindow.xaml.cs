using System.Windows;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class HoaDonBanWindow : Window
    {
        public HoaDonBanWindow()
        {
            InitializeComponent();
            DataContext = new SalesInvoiceViewModel(
                new SalesInvoiceService(),
                new ExcelExportService(),
                new DialogService(),
                Close);
        }
    }
}
