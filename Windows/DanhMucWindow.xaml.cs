using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QLXeMay.Infrastructure;
using QLXeMay.Class;
using QLXeMay.Models;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class DanhMucWindow : UserControl
    {
        private readonly DanhMucConfig config;
        private readonly CategoryWindowViewModel viewModel;
        private readonly IDialogService dialogService;
        private readonly Dictionary<string, Control> controls = new Dictionary<string, Control>();

        public DanhMucWindow(DanhMucConfig config, Action goBack)
            : this(config, new CategoryService(), goBack)
        {
        }

        internal DanhMucWindow(DanhMucConfig config, ICategoryService categoryService, Action goBack)
        {
            InitializeComponent();
            this.config = config;
            dialogService = new DialogService();
            viewModel = new CategoryWindowViewModel(
                config,
                categoryService,
                dialogService,
                ValidateData,
                ReadValues,
                () => GetControlValue(config.KeyField),
                ClearFields,
                SetKeyEnabled,
                ApplySelectedRow,
                LoadComboBoxes,
                goBack ?? (() => { }));
            DataContext = viewModel;
            txtTitle.Text = config.Title;
            BuildFields();
            Loaded += (s, e) =>
            {
                LoadComboBoxes();
                viewModel.LoadData();
                SetKeyEnabled(true);
            };
        }

        private void BuildFields()
        {
            FieldsPanel.Children.Clear();
            controls.Clear();

            foreach (FieldConfig f in config.Fields)
            {
                Grid grid = new Grid { Margin = new Thickness(0, 0, 0, 10) };
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(155) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Label label = new Label
                {
                    Content = f.Header + (f.Required ? " *" : string.Empty),
                    FontWeight = FontWeights.SemiBold,
                    Padding = new Thickness(0),
                    Margin = new Thickness(0, 0, 12, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);

                Control input;
                if (f.Kind == FieldKind.Date)
                {
                    input = new DatePicker
                    {
                        SelectedDate = DateTime.Today,
                        SelectedDateFormat = DatePickerFormat.Short
                    };
                }
                else if (f.Kind == FieldKind.Combo)
                {
                    input = new ComboBox { IsEditable = false };
                }
                else
                {
                    input = new TextBox();
                }

                input.Margin = new Thickness(0);
                input.Height = 38;
                input.MinWidth = 210;
                input.HorizontalAlignment = HorizontalAlignment.Stretch;

                Grid.SetColumn(input, 1);
                grid.Children.Add(input);
                FieldsPanel.Children.Add(grid);
                controls[f.ColumnName] = input;
            }
        }


        private void CategoryGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            GridHeaderFormatter.Apply(sender, e);
            FieldConfig field = config.Fields.FirstOrDefault(f => string.Equals(f.ColumnName, e.PropertyName, StringComparison.OrdinalIgnoreCase));
            if (field != null)
            {
                e.Column.Header = field.Header;
            }
        }

        private void LoadComboBoxes()
        {
            foreach (FieldConfig f in config.Fields.Where(x => x.Kind == FieldKind.Combo))
            {
                ComboBox cbo = (ComboBox)controls[f.ColumnName];
                Function.FillCombo(f.ComboSql, cbo, f.ValueMember, f.DisplayMember);
            }
        }

        private void ApplySelectedRow(DataRowView row)
        {
            if (row == null) return;

            foreach (FieldConfig f in config.Fields)
            {
                object value = row.Row.Table.Columns.Contains(f.ColumnName) ? row[f.ColumnName] : null;
                SetControlValue(f, value);
            }
        }

        private void SetControlValue(FieldConfig f, object value)
        {
            Control control = controls[f.ColumnName];
            string text = value == null || value == DBNull.Value ? "" : value.ToString().Trim();

            if (control is TextBox tb)
            {
                tb.Text = text;
            }
            else if (control is DatePicker dp)
            {
                DateTime dt;
                dp.SelectedDate = DateTime.TryParse(text, out dt) ? dt : DateTime.Today;
            }
            else if (control is ComboBox cb)
            {
                cb.SelectedValue = text;
            }
        }

        private string GetControlValue(FieldConfig f)
        {
            Control control = controls[f.ColumnName];
            if (control is TextBox tb) return tb.Text.Trim();
            if (control is DatePicker dp) return dp.SelectedDate.HasValue ? dp.SelectedDate.Value.ToString("yyyy-MM-dd") : "";
            if (control is ComboBox cb) return Function.GetSelectedValue(cb);
            return "";
        }

        private object GetDbValue(FieldConfig f)
        {
            string value = GetControlValue(f);
            if (!f.Required && string.IsNullOrWhiteSpace(value)) return DBNull.Value;
            if (f.Kind == FieldKind.Number)
            {
                return Function.ToDouble(value, 0);
            }
            if (f.Kind == FieldKind.Date)
            {
                return value;
            }
            return value;
        }

        private bool ValidateData()
        {
            foreach (FieldConfig f in config.Fields)
            {
                string value = GetControlValue(f);
                if (f.Required && string.IsNullOrWhiteSpace(value))
                {
                    dialogService.ShowWarning("Bạn chưa nhập/chọn: " + f.Header);
                    return false;
                }
                if (f.Kind == FieldKind.Number && !string.IsNullOrWhiteSpace(value) && !Function.IsSoThuc(value))
                {
                    dialogService.ShowWarning(f.Header + " phải là số.");
                    return false;
                }
            }
            return true;
        }

        private void ClearFields()
        {
            foreach (FieldConfig f in config.Fields)
            {
                Control control = controls[f.ColumnName];
                if (control is TextBox tb) tb.Text = "";
                else if (control is DatePicker dp) dp.SelectedDate = DateTime.Today;
                else if (control is ComboBox cb) cb.SelectedIndex = -1;
            }
        }

        private void SetKeyEnabled(bool enabled)
        {
            Control key = controls[config.KeyField.ColumnName];
            key.IsEnabled = enabled;
        }

        private IReadOnlyDictionary<string, object> ReadValues()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (FieldConfig field in config.Fields)
            {
                values[field.ColumnName] = GetDbValue(field);
            }

            return values;
        }
    }
}
