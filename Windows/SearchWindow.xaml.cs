using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using QLXeMay.Infrastructure;
using QLXeMay.Class;
using QLXeMay.Models;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class SearchWindow : UserControl
    {
        private readonly SearchMode mode;
        private readonly Dictionary<string, Control> controls = new Dictionary<string, Control>();

        public SearchWindow(SearchMode mode, Action goBack)
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
                goBack ?? (() => { }));
            Loaded += (s, e) => BuildCriteria();
        }

        private void BuildCriteria()
        {
            CriteriaPanel.Children.Clear();
            controls.Clear();
            if (mode == SearchMode.Hang)
            {
                AddText("tenhang", "Tên xe");
                AddCombo("maloai", "Thể loại", "SELECT maloai, maloai + ' - ' + tenloai AS ht FROM tbltheloai", "maloai", "ht");
                AddCombo("mahangsx", "Hãng sản xuất", "SELECT mahangsx, mahangsx + ' - ' + tenhangsx AS ht FROM tblhangsx", "mahangsx", "ht");
                AddCombo("matt", "Tình trạng", "SELECT matt, matt + ' - ' + tentt AS ht FROM tbltinhtrang", "matt", "ht");
            }
            else if (mode == SearchMode.KhachHang)
            {
                AddText("makhach", "Mã khách hàng");
                AddText("tenkhach", "Tên khách hàng");
                AddText("diachi", "Địa chỉ");
                AddText("sdt", "Số điện thoại");
            }
            else if (mode == SearchMode.HoaDonNhap)
            {
                AddCombo("sohdn", "Mã hóa đơn nhập", "SELECT sohdn, sohdn FROM tblhoadonnhap", "sohdn", "sohdn");
                AddCombo("mancc", "Nhà cung cấp", "SELECT mancc, mancc + ' - ' + tenncc AS ht FROM tblnhacungcap", "mancc", "ht");
                AddCombo("manv", "Nhân viên", "SELECT manv, manv + ' - ' + tennv AS ht FROM tblnhanvien", "manv", "ht");
            }
            else
            {
                AddCombo("soddh", "Mã đơn bán", "SELECT soddh, soddh FROM tbldondathang", "soddh", "soddh");
                AddCombo("makhach", "Khách hàng", "SELECT makhach, makhach + ' - ' + tenkhach AS ht FROM tblkhachhang", "makhach", "ht");
                AddCombo("manv", "Nhân viên", "SELECT manv, manv + ' - ' + tennv AS ht FROM tblnhanvien", "manv", "ht");
            }
        }

        private void AddText(string name, string label)
        {
            StackPanel panel = CreateCriteriaField(label);
            TextBox textBox = new TextBox { Margin = new Thickness(0), Height = 38 };
            panel.Children.Add(textBox);
            CriteriaPanel.Children.Add(panel);
            controls[name] = textBox;
        }

        private void AddCombo(string name, string label, string sql, string value, string display)
        {
            StackPanel panel = CreateCriteriaField(label);
            ComboBox comboBox = new ComboBox { Margin = new Thickness(0), Height = 38 };
            panel.Children.Add(comboBox);
            CriteriaPanel.Children.Add(panel);
            controls[name] = comboBox;
            Function.FillCombo(sql, comboBox, value, display);
        }

        private static StackPanel CreateCriteriaField(string label)
        {
            StackPanel panel = new StackPanel { Width = 235, Margin = new Thickness(6, 0, 6, 8) };
            panel.Children.Add(new Label
            {
                Content = label,
                FontWeight = FontWeights.SemiBold,
                Padding = new Thickness(0),
                Margin = new Thickness(0, 0, 0, 6)
            });
            return panel;
        }

        private void ResultGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            GridHeaderFormatter.Apply(sender, e);
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
