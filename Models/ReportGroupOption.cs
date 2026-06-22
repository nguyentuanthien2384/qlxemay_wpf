namespace QLXeMay.Models
{
    internal sealed class ReportGroupOption
    {
        public ReportGroupOption(string name, string key)
        {
            Name = name;
            Key = key;
        }

        public string Name { get; }
        public string Key { get; }
    }
}
