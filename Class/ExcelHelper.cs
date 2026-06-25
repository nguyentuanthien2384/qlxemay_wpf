using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using QLXeMay.Infrastructure;

namespace QLXeMay.Class
{
    public static class ExcelHelper
    {
        public static void ExportDataGridToExcel(DataGrid grid, string title, string sheetName)
        {
            DataTable table = null;
            if (grid.ItemsSource is DataView view) table = view.ToTable();
            else if (grid.ItemsSource is DataTable dt) table = dt;

            if (table == null || table.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất Excel.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ExportDataTableToExcel(table, title, sheetName);
        }

        public static void ExportDataTableToExcel(DataTable table, string title, string sheetName)
        {
            object excelAppObject = null;
            object workbookObject = null;
            object worksheetObject = null;
            object titleRangeObject = null;
            bool shownToUser = false;

            try
            {
                Type excelType = Type.GetTypeFromProgID("Excel.Application");
                if (excelType == null)
                {
                    MessageBox.Show("Không tìm thấy Microsoft Excel trên máy.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                excelAppObject = Activator.CreateInstance(excelType);
                dynamic exApp = excelAppObject;
                exApp.Visible = false;

                workbookObject = exApp.Workbooks.Add();
                dynamic exBook = workbookObject;
                worksheetObject = exBook.Worksheets[1];
                dynamic exSheet = worksheetObject;
                exSheet.Name = string.IsNullOrWhiteSpace(sheetName) ? "BaoCao" : sheetName;

                int colCount = Math.Max(table.Columns.Count, 1);
                titleRangeObject = exSheet.Range[exSheet.Cells[1, 1], exSheet.Cells[1, colCount]];
                dynamic titleRange = titleRangeObject;
                titleRange.MergeCells = true;
                titleRange.Value = title;
                titleRange.Font.Bold = true;
                titleRange.Font.Size = 16;
                titleRange.HorizontalAlignment = -4108;

                int headerRow = 3;
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    exSheet.Cells[headerRow, c + 1].Value = GridHeaderFormatter.Format(table.Columns[c].ColumnName);
                    exSheet.Cells[headerRow, c + 1].Font.Bold = true;
                    exSheet.Cells[headerRow, c + 1].Borders.LineStyle = 1;
                    exSheet.Cells[headerRow, c + 1].HorizontalAlignment = -4108;
                }

                for (int r = 0; r < table.Rows.Count; r++)
                {
                    for (int c = 0; c < table.Columns.Count; c++)
                    {
                        object value = table.Rows[r][c];
                        exSheet.Cells[headerRow + 1 + r, c + 1].Value = value == DBNull.Value ? "" : value.ToString();
                        exSheet.Cells[headerRow + 1 + r, c + 1].Borders.LineStyle = 1;
                    }
                }

                exSheet.Columns.AutoFit();
                exSheet.Cells[headerRow + table.Rows.Count + 2, 1].Value = "Thời gian xuất: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                exApp.Visible = true;
                shownToUser = true;
                MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                TryQuitExcel(excelAppObject, shownToUser);
                AppLogger.Error("Excel export failed.", ex);
                MessageBox.Show("Lỗi xuất Excel: " + ex.Message + "\n\nCần cài Microsoft Excel để dùng chức năng này.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ReleaseComObject(titleRangeObject);
                ReleaseComObject(worksheetObject);
                ReleaseComObject(workbookObject);
                ReleaseComObject(excelAppObject);
            }
        }

        private static void TryQuitExcel(object excelAppObject, bool shownToUser)
        {
            if (excelAppObject == null || shownToUser) return;

            try
            {
                dynamic exApp = excelAppObject;
                exApp.Quit();
            }
            catch
            {
                // Best effort cleanup only.
            }
        }

        private static void ReleaseComObject(object comObject)
        {
            if (comObject == null) return;

            try
            {
                if (Marshal.IsComObject(comObject))
                {
                    Marshal.FinalReleaseComObject(comObject);
                }
            }
            catch
            {
                // Avoid masking the export result with cleanup errors.
            }
        }
    }
}
