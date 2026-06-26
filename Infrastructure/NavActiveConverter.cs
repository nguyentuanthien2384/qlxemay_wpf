using System;
using System.Globalization;
using System.Windows.Data;

namespace QLXeMay.Infrastructure
{
    internal sealed class NavActiveConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return false;
            string buttonKey = values[0]?.ToString() ?? string.Empty;
            string activeKey = values[1]?.ToString() ?? string.Empty;
            return !string.IsNullOrWhiteSpace(buttonKey) && string.Equals(buttonKey, activeKey, StringComparison.OrdinalIgnoreCase);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
