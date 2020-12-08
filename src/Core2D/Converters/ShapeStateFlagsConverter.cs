using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.Renderer;

namespace Core2D.Converters
{
    public class ShapeStateFlagsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ShapeStateFlags flags && parameter is ShapeStateFlags flag)
            {
                return flags.HasFlag(flag);
            }
            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
