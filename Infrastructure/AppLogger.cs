using System;
using System.IO;

namespace QLXeMay.Infrastructure
{
    public static class AppLogger
    {
        private static readonly object SyncRoot = new object();

        public static void Info(string message)
        {
            Write("INFO", message, null);
        }

        public static void Error(string message, Exception exception = null)
        {
            Write("ERROR", message, exception);
        }

        private static void Write(string level, string message, Exception exception)
        {
            try
            {
                lock (SyncRoot)
                {
                    string directory = GetLogDirectory();
                    Directory.CreateDirectory(directory);
                    string path = Path.Combine(directory, "app-" + DateTime.Now.ToString("yyyyMMdd") + ".log");

                    using (StreamWriter writer = File.AppendText(path))
                    {
                        writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}");
                        if (exception != null)
                        {
                            writer.WriteLine(exception);
                        }
                    }
                }
            }
            catch
            {
                // Logging must never break the desktop application.
            }
        }

        private static string GetLogDirectory()
        {
            string configuredPath = Environment.GetEnvironmentVariable("QLXEMAY_LOG_DIR");
            if (!string.IsNullOrWhiteSpace(configuredPath)) return configuredPath;

            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppData, "QLXeMay", "Logs");
        }
    }
}
