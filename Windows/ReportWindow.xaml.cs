using System.Windows;
using QLXeMay.Models;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class ReportWindow : Window
    {
        public ReportWindow(ReportMode mode)
        {
            InitializeComponent();
            DataContext = new ReportWindowViewModel(
                mode,
                new ReportService(),
                new ExcelExportService(),
                new DialogService(),
                Close);
        }
    }
}
