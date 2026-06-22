using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Input;
using QLXeMay.Infrastructure;
using QLXeMay.Models;
using QLXeMay.Services;

namespace QLXeMay.ViewModels
{
    internal sealed class CategoryWindowViewModel : ViewModelBase
    {
        private readonly DanhMucConfig config;
        private readonly ICategoryService categoryService;
        private readonly IDialogService dialogService;
        private readonly Func<bool> validateData;
        private readonly Func<IReadOnlyDictionary<string, object>> readValues;
        private readonly Func<string> readKeyValue;
        private readonly Action clearFields;
        private readonly Action<bool> setKeyEnabled;
        private readonly Action<DataRowView> applySelectedRow;
        private readonly Action reloadLookups;
        private readonly Action closeAction;
        private DataView itemsView;
        private DataRowView selectedRow;

        public CategoryWindowViewModel(
            DanhMucConfig config,
            ICategoryService categoryService,
            IDialogService dialogService,
            Func<bool> validateData,
            Func<IReadOnlyDictionary<string, object>> readValues,
            Func<string> readKeyValue,
            Action clearFields,
            Action<bool> setKeyEnabled,
            Action<DataRowView> applySelectedRow,
            Action reloadLookups,
            Action closeAction)
        {
            this.config = config;
            this.categoryService = categoryService;
            this.dialogService = dialogService;
            this.validateData = validateData;
            this.readValues = readValues;
            this.readKeyValue = readKeyValue;
            this.clearFields = clearFields;
            this.setKeyEnabled = setKeyEnabled;
            this.applySelectedRow = applySelectedRow;
            this.reloadLookups = reloadLookups;
            this.closeAction = closeAction;

            AddCommand = new RelayCommand(_ => Add());
            SaveCommand = new RelayCommand(_ => Save());
            UpdateCommand = new RelayCommand(_ => Update());
            DeleteCommand = new RelayCommand(_ => Delete());
            ResetCommand = new RelayCommand(_ => Reset());
            RefreshCommand = new RelayCommand(_ => Refresh());
            CloseCommand = new RelayCommand(_ => closeAction());
        }

        public DataView ItemsView
        {
            get => itemsView;
            private set => SetProperty(ref itemsView, value);
        }

        public DataRowView SelectedRow
        {
            get => selectedRow;
            set
            {
                if (!SetProperty(ref selectedRow, value)) return;
                if (value == null) return;
                applySelectedRow(value);
                setKeyEnabled(false);
            }
        }

        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CloseCommand { get; }

        public void LoadData()
        {
            ItemsView = categoryService.Load(config).DefaultView;
        }

        private void Add()
        {
            SelectedRow = null;
            clearFields();
            setKeyEnabled(true);
        }

        private void Save()
        {
            if (!validateData()) return;
            string keyValue = readKeyValue();
            if (categoryService.Exists(config, keyValue))
            {
                dialogService.ShowWarning("Mã đã tồn tại, vui lòng nhập mã khác.");
                return;
            }

            try
            {
                categoryService.Insert(config, readValues());
                LoadData();
                SelectedRow = null;
                clearFields();
            }
            catch (Exception ex)
            {
                dialogService.ShowError("Không thể lưu dữ liệu.\n" + ex.Message);
            }
        }

        private void Update()
        {
            if (!validateData()) return;
            string keyValue = readKeyValue();
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                dialogService.ShowWarning("Bạn cần chọn dòng cần sửa.");
                return;
            }

            try
            {
                categoryService.Update(config, readValues(), keyValue);
                LoadData();
            }
            catch (Exception ex)
            {
                dialogService.ShowError("Không thể cập nhật dữ liệu.\n" + ex.Message);
            }
        }

        private void Delete()
        {
            string keyValue = readKeyValue();
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                dialogService.ShowWarning("Bạn cần chọn dòng cần xóa.");
                return;
            }

            if (!dialogService.Confirm("Bạn có muốn xóa bản ghi này không?")) return;

            try
            {
                categoryService.Delete(config, keyValue);
                LoadData();
                SelectedRow = null;
                clearFields();
                setKeyEnabled(true);
            }
            catch (Exception ex)
            {
                dialogService.ShowWarning("Dữ liệu đang được dùng hoặc không thể xóa.\n" + ex.Message);
            }
        }

        private void Reset()
        {
            SelectedRow = null;
            clearFields();
            setKeyEnabled(true);
        }

        private void Refresh()
        {
            reloadLookups();
            LoadData();
        }
    }
}
