using System.Collections.Generic;
using System.Linq;

namespace QLXeMay.Models
{
    public enum FieldKind
    {
        Text,
        Number,
        Date,
        Combo
    }

    public sealed class FieldConfig
    {
        public FieldConfig(string columnName, string header, FieldKind kind, bool required, bool isKey,
            string comboSql = null, string valueMember = null, string displayMember = null)
        {
            ColumnName = columnName;
            Header = header;
            Kind = kind;
            Required = required;
            IsKey = isKey;
            ComboSql = comboSql;
            ValueMember = valueMember;
            DisplayMember = displayMember;
        }

        public string ColumnName { get; }
        public string Header { get; }
        public FieldKind Kind { get; }
        public bool Required { get; }
        public bool IsKey { get; }
        public string ComboSql { get; }
        public string ValueMember { get; }
        public string DisplayMember { get; }
    }

    public sealed class DanhMucConfig
    {
        public DanhMucConfig(string title, string tableName, List<FieldConfig> fields)
        {
            Title = title;
            TableName = tableName;
            Fields = fields;
        }

        public string Title { get; }
        public string TableName { get; }
        public List<FieldConfig> Fields { get; }
        public FieldConfig KeyField => Fields.First(f => f.IsKey);
    }
}
