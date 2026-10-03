using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SteamAccountSwitcher.Launcher.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (Invert) b = !b;
                return b ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
            {
                var b = v == Visibility.Visible;
                return Invert ? !b : b;
            }
            return false;
        }
    }

    public class StringNotNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var notEmpty = !string.IsNullOrWhiteSpace(value as string);
            if (Invert) notEmpty = !notEmpty;
            return notEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class StringToImageSourceConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string pathOrUrl && !string.IsNullOrWhiteSpace(pathOrUrl))
            {
                try
                {
                    if (File.Exists(pathOrUrl))
                    {
                        var bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.UriSource = new Uri(pathOrUrl, UriKind.Absolute);
                        bi.EndInit();
                        bi.Freeze();
                        return bi;
                    }
                    else if (pathOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        var bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.UriSource = new Uri(pathOrUrl, UriKind.Absolute);
                        bi.EndInit();
                        return bi;
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class HexToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hex && !string.IsNullOrWhiteSpace(hex))
            {
                try
                {
                    var converter = new BrushConverter();
                    var brush = (Brush?)converter.ConvertFromString(hex);
                    if (brush != null)
                    {
                        brush.Freeze();
                        return brush;
                    }
                }
                catch { }
            }
            return new SolidColorBrush(Color.FromRgb(102, 192, 244));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
