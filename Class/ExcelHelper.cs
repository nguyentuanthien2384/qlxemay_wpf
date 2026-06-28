using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using QLXeMay.Infrastructure;
using QLXeMay.Services;

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
            ExportDataTableToExcel(table, title, sheetName, null, null);
        }

        public static void ExportDataTableToExcel(
            DataTable table,
            string title,
            string sheetName,
            IReadOnlyList<ExcelExportField> beforeTable,
            IReadOnlyList<ExcelExportField> afterTable)
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

                int currentRow = 3;
                currentRow = WriteInfoSection(exSheet, beforeTable, colCount, currentRow);
                int headerRow = currentRow;
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    exSheet.Cells[headerRow, c + 1].Value = GridHeaderFormatter.Format(table.Columns[c].ColumnName);
                    exSheet.Cells[headerRow, c + 1].Font.Bold = true;
                    exSheet.Cells[headerRow, c + 1].Borders.LineStyle = 1;
                    exSheet.Cells[headerRow, c + 1].HorizontalAlignment = -4108;
                    exSheet.Cells[headerRow, c + 1].Interior.Color = 15132390;
                }

                bool[] numericColumns = BuildNumericColumnMap(table);
                for (int r = 0; r < table.Rows.Count; r++)
                {
                    for (int c = 0; c < table.Columns.Count; c++)
                    {
                        object value = table.Rows[r][c];
                        int rowIndex = headerRow + 1 + r;
                        object cellValue = NormalizeCellValue(value);
                        exSheet.Cells[rowIndex, c + 1].Value = cellValue;
                        exSheet.Cells[rowIndex, c + 1].Borders.LineStyle = 1;
                        if (numericColumns[c])
                        {
                            exSheet.Cells[rowIndex, c + 1].NumberFormat = "#,##0";
                            exSheet.Cells[rowIndex, c + 1].HorizontalAlignment = -4152;
                        }
                    }
                }

                int footerStartRow = headerRow + table.Rows.Count + 2;
                int rowAfterFooter = WriteInfoSection(exSheet, afterTable, colCount, footerStartRow);
                int exportedAtRow = rowAfterFooter > footerStartRow ? rowAfterFooter + 1 : footerStartRow;

                exSheet.Range[exSheet.Cells[headerRow, 1], exSheet.Cells[headerRow + table.Rows.Count, colCount]].VerticalAlignment = -4108;
                exSheet.Range[exSheet.Cells[headerRow, 1], exSheet.Cells[headerRow + table.Rows.Count, colCount]].WrapText = true;
                exSheet.Columns.AutoFit();
                exSheet.Cells[exportedAtRow, 1].Value = "Thời gian xuất: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                exSheet.Rows[headerRow + 1].Select();
                exApp.ActiveWindow.FreezePanes = true;
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

        private static int WriteInfoSection(dynamic exSheet, IReadOnlyList<ExcelExportField> fields, int colCount, int startRow)
        {
            if (fields == null || fields.Count == 0) return startRow;

            int row = startRow;
            for (int i = 0; i < fields.Count; i++)
            {
                ExcelExportField item = fields[i];
                exSheet.Cells[row, 1].Value = item.Label;
                exSheet.Cells[row, 1].Font.Bold = true;
                exSheet.Cells[row, 1].Borders.LineStyle = 1;

                object rangeObject = exSheet.Range[exSheet.Cells[row, 2], exSheet.Cells[row, colCount]];
                dynamic valueRange = rangeObject;
                valueRange.MergeCells = true;
                valueRange.Value = item.Value;
                valueRange.Borders.LineStyle = 1;
                valueRange.WrapText = true;
                ReleaseComObject(rangeObject);

                row++;
            }

            return row + 1;
        }

        private static bool[] BuildNumericColumnMap(DataTable table)
        {
            bool[] result = new bool[table.Columns.Count];
            for (int c = 0; c < table.Columns.Count; c++)
            {
                DataColumn column = table.Columns[c];
                if (IsNumericType(column.DataType))
                {
                    result[c] = true;
                    continue;
                }

                int seen = 0;
                int numeric = 0;
                for (int r = 0; r < table.Rows.Count; r++)
                {
                    object raw = table.Rows[r][c];
                    if (raw == null || raw == DBNull.Value) continue;
                    string text = raw.ToString();
                    if (string.IsNullOrWhiteSpace(text)) continue;
                    seen++;
                    if (TryParseNumber(text, out _)) numeric++;
                }

                result[c] = seen > 0 && numeric == seen;
            }

            return result;
        }

        private static object NormalizeCellValue(object value)
        {
            if (value == null || value == DBNull.Value) return string.Empty;
            if (value is DateTime dt) return dt;
            if (value is decimal || value is double || value is float ||
                value is byte || value is short || value is int || value is long ||
                value is sbyte || value is ushort || value is uint || value is ulong)
            {
                return value;
            }

            string text = value.ToString();
            if (TryParseNumber(text, out double parsed)) return parsed;
            return text;
        }

        private static bool IsNumericType(Type type)
        {
            return type == typeof(decimal)
                || type == typeof(double)
                || type == typeof(float)
                || type == typeof(byte)
                || type == typeof(short)
                || type == typeof(int)
                || type == typeof(long)
                || type == typeof(sbyte)
                || type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(ulong);
        }

        private static bool TryParseNumber(string text, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;

            string cleaned = text.Trim().ToLowerInvariant();
            cleaned = cleaned.Replace("vnđ", "").Replace("vnd", "").Replace("%", "").Replace(" ", "");

            if (double.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return true;

            if (double.TryParse(cleaned, NumberStyles.Any, CultureInfo.GetCultureInfo("vi-VN"), out value))
                return true;

            if (cleaned.Contains(".") && cleaned.Contains(","))
            {
                cleaned = cleaned.Replace(".", "").Replace(",", ".");
            }
            else if (cleaned.Contains(","))
            {
                cleaned = cleaned.Replace(",", ".");
            }

            return double.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
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
