using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using QLXeMay.Infrastructure;
using QLXeMay.Models;
using QLXeMay.Services;

namespace QLXeMay.ViewModels
{
    internal sealed class AuditLogWindowViewModel : ViewModelBase
    {
        private readonly IAuditLogService auditLogService;
        private readonly IDialogService dialogService;
        private readonly Action closeAction;
        private DateTime? fromDate = DateTime.Today.AddDays(-7);
        private DateTime? toDate = DateTime.Today;
        private string keyword;
        private int maxRows = 300;
        private string statusMessage;
        private AuditLogEntry selectedEntry;

        public AuditLogWindowViewModel(IAuditLogService auditLogService, IDialogService dialogService, Action closeAction)
        {
            this.auditLogService = auditLogService;
            this.dialogService = dialogService;
            this.closeAction = closeAction;
            Entries = new ObservableCollection<AuditLogEntry>();
            SearchCommand = new RelayCommand(_ => Load());
            TodayCommand = new RelayCommand(_ => ApplyToday());
            Last7DaysCommand = new RelayCommand(_ => ApplyLast7Days());
            ClearFilterCommand = new RelayCommand(_ => ClearFilter());
            CloseCommand = new RelayCommand(_ => closeAction());
            Load();
        }

        public ObservableCollection<AuditLogEntry> Entries { get; }

        public DateTime? FromDate
        {
            get => fromDate;
            set => SetProperty(ref fromDate, value);
        }

        public DateTime? ToDate
        {
            get => toDate;
            set => SetProperty(ref toDate, value);
        }

        public string Keyword
        {
            get => keyword;
            set => SetProperty(ref keyword, value);
        }

        public int MaxRows
        {
            get => maxRows;
            set => SetProperty(ref maxRows, value);
        }

        public string StatusMessage
        {
            get => statusMessage;
            private set => SetProperty(ref statusMessage, value);
        }

        public AuditLogEntry SelectedEntry
        {
            get => selectedEntry;
            set => SetProperty(ref selectedEntry, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand TodayCommand { get; }
        public ICommand Last7DaysCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand CloseCommand { get; }

        private void Load()
        {
            try
            {
                Entries.Clear();
                foreach (AuditLogEntry entry in auditLogService.Load(FromDate, ToDate, Keyword, MaxRows))
                {
                    Entries.Add(entry);
                }

                StatusMessage = "Đã tải " + Entries.Count + " dòng nhật ký.";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                dialogService.ShowError(ex.Message);
            }
        }

        private void ApplyToday()
        {
            FromDate = DateTime.Today;
            ToDate = DateTime.Today;
            Load();
        }

        private void ApplyLast7Days()
        {
            FromDate = DateTime.Today.AddDays(-7);
            ToDate = DateTime.Today;
            Load();
        }

        private void ClearFilter()
        {
            FromDate = null;
            ToDate = null;
            Keyword = string.Empty;
            MaxRows = 300;
            Load();
        }
    }
}
