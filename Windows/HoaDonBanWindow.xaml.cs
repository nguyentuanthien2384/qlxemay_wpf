using System;
using System.Windows.Controls;
using QLXeMay.Infrastructure;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class HoaDonBanWindow : UserControl
    {
        public HoaDonBanWindow(Action goBack, string initialProductId = null)
        {
            InitializeComponent();
            DataContext = new SalesInvoiceViewModel(
                new SalesInvoiceService(),
                new ExcelExportService(),
                new DialogService(),
                goBack ?? (() => { }),
                initialProductId);
        }

        private void DetailsGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            GridHeaderFormatter.Apply(sender, e);
        }
    }
}
