using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Core2D.Converters
{
    public class ObjectToObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
