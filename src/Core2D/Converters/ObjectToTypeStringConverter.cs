using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Core2D.Converters
{
    public class ObjectToTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value != AvaloniaProperty.UnsetValue)
            {
                return value.GetType().Name;
            }
            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
