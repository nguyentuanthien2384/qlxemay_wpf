using System.Data;
using System.Collections.Generic;
using QLXeMay.Class;

namespace QLXeMay.Services
{
    internal sealed class ExcelExportService : IExcelExportService
    {
        public void Export(
            DataTable table,
            string title,
            string sheetName,
            IReadOnlyList<ExcelExportField> beforeTable = null,
            IReadOnlyList<ExcelExportField> afterTable = null)
        {
            ExcelHelper.ExportDataTableToExcel(table, title, sheetName, beforeTable, afterTable);
        }
    }
}
