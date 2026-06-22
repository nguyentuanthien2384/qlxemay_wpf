using System.Data;
using QLXeMay.Class;

namespace QLXeMay.Services
{
    internal sealed class ExcelExportService : IExcelExportService
    {
        public void Export(DataTable table, string title, string sheetName)
        {
            ExcelHelper.ExportDataTableToExcel(table, title, sheetName);
        }
    }
}
