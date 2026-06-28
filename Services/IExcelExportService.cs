using System.Data;
using System.Collections.Generic;

namespace QLXeMay.Services
{
    internal interface IExcelExportService
    {
        void Export(
            DataTable table,
            string title,
            string sheetName,
            IReadOnlyList<ExcelExportField> beforeTable = null,
            IReadOnlyList<ExcelExportField> afterTable = null);
    }

    public sealed class ExcelExportField
    {
        public ExcelExportField(string label, string value)
        {
            Label = label ?? string.Empty;
            Value = value ?? string.Empty;
        }

        public string Label { get; }
        public string Value { get; }
    }
}
