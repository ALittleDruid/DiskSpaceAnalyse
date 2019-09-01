using System;
using System.Globalization;
using System.Windows.Data;

namespace DiskSpaceAnalyse
{
    public static class DiskSpaceUtility
    {
        private readonly static string[] KMG = new string[] { "B", "KB", "MB", "GB" };

        public static string RootPath { get; set; }
        public static bool Analyse { get; set; }

        public static string GetFriendlySize(float size)
        {
            int a = 0;
            while (a < KMG.Length && size >= 1024)
            {
                size /= 1024.0f;
                ++a;
            }
            if (a == KMG.Length)
            {
                --a;
            }
            return $"{size:0.##}{KMG[a]}";
        }

        public static string GetDiskDetail(long total, long free)
        {
            string result = $"{GetFriendlySize(total)} total, {GetFriendlySize(total - free)} used, {(float)(free) / total:P} free";
            return result;
        }
    }

    public class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long a)
            {
                return DiskSpaceUtility.GetFriendlySize(a);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
