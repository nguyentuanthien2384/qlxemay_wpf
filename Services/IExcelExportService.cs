using System.Data;

namespace QLXeMay.Services
{
    internal interface IExcelExportService
    {
        void Export(DataTable table, string title, string sheetName);
    }
}
