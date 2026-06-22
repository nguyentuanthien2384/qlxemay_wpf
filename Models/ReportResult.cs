using System.Data;

namespace QLXeMay.Models
{
    internal sealed class ReportResult
    {
        public ReportResult(DataTable mainTable, DataTable summaryTable, string summaryText)
        {
            MainTable = mainTable ?? new DataTable();
            SummaryTable = summaryTable;
            SummaryText = summaryText ?? string.Empty;
        }

        public DataTable MainTable { get; }
        public DataTable SummaryTable { get; }
        public string SummaryText { get; }
    }
}
