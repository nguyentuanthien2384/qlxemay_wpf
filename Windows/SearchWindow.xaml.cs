using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using QLXeMay.Class;
using QLXeMay.Models;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class SearchWindow : Window
    {
        private readonly SearchMode mode;
        private readonly Dictionary<string, Control> controls = new Dictionary<string, Control>();

        public SearchWindow(SearchMode mode)
        {
            InitializeComponent();
            this.mode = mode;
            DataContext = new SearchWindowViewModel(
                mode,
                new SearchService(),
                new ExcelExportService(),
                new DialogService(),
                ReadCriteria,
                ClearCriteria,
                Close);
            Loaded += (s, e) => BuildCriteria();
        }

        private void BuildCriteria()
        {
            CriteriaPanel.Children.Clear();
            controls.Clear();
            if (mode == SearchMode.Hang)
            {
                AddText("tenhang", "Tên hàng");
                AddCombo("maloai", "Thể loại", "SELECT maloai, maloai + ' - ' + tenloai AS ht FROM tbltheloai", "maloai", "ht");
                AddCombo("mahangsx", "Hãng SX", "SELECT mahangsx, mahangsx + ' - ' + tenhangsx AS ht FROM tblhangsx", "mahangsx", "ht");
                AddCombo("matt", "Tình trạng", "SELECT matt, matt + ' - ' + tentt AS ht FROM tbltinhtrang", "matt", "ht");
            }
            else if (mode == SearchMode.KhachHang)
            {
                AddText("makhach", "Mã khách");
                AddText("tenkhach", "Tên khách");
                AddText("diachi", "Địa chỉ");
                AddText("sdt", "Điện thoại");
            }
            else if (mode == SearchMode.HoaDonNhap)
            {
                AddCombo("sohdn", "Mã hóa đơn", "SELECT sohdn, sohdn FROM tblhoadonnhap", "sohdn", "sohdn");
                AddCombo("mancc", "Nhà cung cấp", "SELECT mancc, mancc FROM tblnhacungcap", "mancc", "mancc");
                AddCombo("manv", "Nhân viên", "SELECT manv, manv FROM tblnhanvien", "manv", "manv");
            }
            else
            {
                AddCombo("soddh", "Mã hóa đơn", "SELECT soddh, soddh FROM tbldondathang", "soddh", "soddh");
                AddCombo("makhach", "Khách hàng", "SELECT makhach, makhach FROM tblkhachhang", "makhach", "makhach");
                AddCombo("manv", "Nhân viên", "SELECT manv, manv FROM tblnhanvien", "manv", "manv");
            }
        }

        private void AddText(string name, string label)
        {
            StackPanel panel = new StackPanel { Width = 205, Margin = new Thickness(4) };
            panel.Children.Add(new Label { Content = label });
            TextBox textBox = new TextBox();
            panel.Children.Add(textBox);
            CriteriaPanel.Children.Add(panel);
            controls[name] = textBox;
        }

        private void AddCombo(string name, string label, string sql, string value, string display)
        {
            StackPanel panel = new StackPanel { Width = 205, Margin = new Thickness(4) };
            panel.Children.Add(new Label { Content = label });
            ComboBox comboBox = new ComboBox();
            panel.Children.Add(comboBox);
            CriteriaPanel.Children.Add(panel);
            controls[name] = comboBox;
            Function.FillCombo(sql, comboBox, value, display);
        }

        private IReadOnlyDictionary<string, string> ReadCriteria()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            foreach (KeyValuePair<string, Control> entry in controls)
            {
                if (entry.Value is TextBox textBox) values[entry.Key] = textBox.Text.Trim();
                else if (entry.Value is ComboBox comboBox) values[entry.Key] = Function.GetSelectedValue(comboBox);
            }

            return values;
        }

        private void ClearCriteria()
        {
            foreach (Control control in controls.Values)
            {
                if (control is TextBox textBox) textBox.Text = "";
                if (control is ComboBox comboBox) comboBox.SelectedIndex = -1;
            }
        }
    }
}
