using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using QLXeMay.Class;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal sealed class CategoryService : ICategoryService
    {
        public DataTable Load(DanhMucConfig config)
        {
            return Function.GetDataToTable("SELECT * FROM " + TableSql(config));
        }

        public bool Exists(DanhMucConfig config, string keyValue)
        {
            return Function.CheckKey(
                $"SELECT 1 FROM {TableSql(config)} WHERE {ColumnSql(config.KeyField)}=@key",
                Function.Param("@key", keyValue));
        }

        public void Insert(DanhMucConfig config, IReadOnlyDictionary<string, object> values)
        {
            string columns = string.Join(", ", config.Fields.Select(ColumnSql));
            List<string> parameterNames = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            for (int i = 0; i < config.Fields.Count; i++)
            {
                FieldConfig field = config.Fields[i];
                string parameterName = "@p" + i;
                parameterNames.Add(parameterName);
                parameters.Add(Function.Param(parameterName, values[field.ColumnName]));
            }

            Function.ExecuteSql(
                $"INSERT INTO {TableSql(config)}({columns}) VALUES({string.Join(", ", parameterNames)})",
                parameters.ToArray());
        }

        public void Update(DanhMucConfig config, IReadOnlyDictionary<string, object> values, string keyValue)
        {
            StringBuilder set = new StringBuilder();
            List<SqlParameter> parameters = new List<SqlParameter>();
            int index = 0;
            foreach (FieldConfig field in config.Fields.Where(f => !f.IsKey))
            {
                if (set.Length > 0) set.Append(", ");
                string parameterName = "@p" + index++;
                set.Append(ColumnSql(field)).Append("=").Append(parameterName);
                parameters.Add(Function.Param(parameterName, values[field.ColumnName]));
            }

            parameters.Add(Function.Param("@key", keyValue));
            Function.ExecuteSql(
                $"UPDATE {TableSql(config)} SET {set} WHERE {ColumnSql(config.KeyField)}=@key",
                parameters.ToArray());
        }

        public void Delete(DanhMucConfig config, string keyValue)
        {
            Function.ExecuteSql(
                $"DELETE FROM {TableSql(config)} WHERE {ColumnSql(config.KeyField)}=@key",
                Function.Param("@key", keyValue));
        }

        private static string TableSql(DanhMucConfig config)
        {
            return Function.QuoteIdentifier(config.TableName);
        }

        private static string ColumnSql(FieldConfig field)
        {
            return Function.QuoteIdentifier(field.ColumnName);
        }
    }
}
