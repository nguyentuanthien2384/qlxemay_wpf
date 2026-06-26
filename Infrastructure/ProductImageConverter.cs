using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace QLXeMay.Infrastructure
{
    /// <summary>
    /// Resolves a product image for catalog cards. Priority:
    /// 1) a real image file path stored on the product (absolute or relative to the app folder),
    /// 2) a brand image bundled with the app (inferred from the product name/brand),
    /// 3) a neutral default motorbike image.
    /// </summary>
    internal sealed class ProductImageConverter : IValueConverter
    {
        private const string ResourceBase = "pack://application:,,,/Images/Products/";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value?.ToString()?.Trim() ?? string.Empty;

            string filePath = ResolveExistingFile(text);
            if (filePath != null)
            {
                BitmapImage fromFile = LoadFromFile(filePath);
                if (fromFile != null) return fromFile;
            }

            return LoadFromResource(MapBrandToFile(text));
        }

        private static string MapBrandToFile(string text)
        {
            string lower = text.ToLowerInvariant();
            if (lower.Contains("honda")) return "honda.png";
            if (lower.Contains("yamaha")) return "yamaha.png";
            if (lower.Contains("suzuki")) return "suzuki.png";
            if (lower.Contains("vinfast")) return "vinfast.png";
            if (lower.Contains("piaggio") || lower.Contains("vespa")) return "piaggio.png";
            if (lower.Contains("kawasaki")) return "kawasaki.png";
            if (lower.Contains("sym")) return "sym.png";
            return "default.png";
        }

        private static string ResolveExistingFile(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path)) return null;
                if (Path.IsPathRooted(path) && File.Exists(path)) return path;

                string relative = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                if (File.Exists(relative)) return relative;
            }
            catch
            {
                // ignore and fall back to bundled images
            }

            return null;
        }

        private static BitmapImage LoadFromFile(string fullPath)
        {
            try
            {
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(fullPath, UriKind.Absolute);
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        private static BitmapImage LoadFromResource(string fileName)
        {
            try
            {
                BitmapImage bmp = new BitmapImage(new Uri(ResourceBase + fileName, UriKind.Absolute));
                bmp.Freeze();
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
