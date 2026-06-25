using System;
using System.Windows.Controls;
using QLXeMay.Infrastructure;
using QLXeMay.Models;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class ReportWindow : UserControl
    {
        public ReportWindow(ReportMode mode, Action goBack)
        {
            InitializeComponent();
            DataContext = new ReportWindowViewModel(
                mode,
                new ReportService(),
                new ExcelExportService(),
                new DialogService(),
                goBack ?? (() => { }));
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            GridHeaderFormatter.Apply(sender, e);
        }
    }
}
