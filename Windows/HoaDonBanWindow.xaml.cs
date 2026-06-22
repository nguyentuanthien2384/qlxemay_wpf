using System;
using System.Windows.Controls;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class HoaDonBanWindow : UserControl
    {
        public HoaDonBanWindow(Action goBack)
        {
            InitializeComponent();
            DataContext = new SalesInvoiceViewModel(
                new SalesInvoiceService(),
                new ExcelExportService(),
                new DialogService(),
                goBack ?? (() => { }));
        }
    }
}
