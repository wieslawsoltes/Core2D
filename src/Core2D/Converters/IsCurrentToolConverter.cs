using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.Model.Editor;

namespace Core2D.Converters
{
    public class IsCurrentToolConverter : IValueConverter
    {
        public static IsCurrentToolConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ITool tool && parameter is string title)
            {
                return tool.Title == title;
            }
            return AvaloniaProperty.UnsetValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
