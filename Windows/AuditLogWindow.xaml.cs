using System;
using System.Windows.Controls;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class AuditLogWindow : UserControl
    {
        public AuditLogWindow(Action goBack)
        {
            InitializeComponent();
            DataContext = new AuditLogWindowViewModel(new AuditLogService(), new DialogService(), goBack ?? (() => { }));
        }
    }
}
