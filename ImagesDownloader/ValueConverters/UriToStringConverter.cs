using System.Windows.Data;
using System.Globalization;

namespace ImagesDownloader.ValueConverters
{
    class UriToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value?.ToString() ?? string.Empty;

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? original = (string?)value;
            if (string.IsNullOrEmpty(original)) return null;
            return new Uri(original);
        }
    }
}
