using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.Style;

namespace Core2D.Converters
{
    public class FontStyleFlagsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontStyleFlags flags && parameter is FontStyleFlags flag && targetType == typeof(bool))
            {
                //return flags.HasFlag(flag) ? flags & ~flag : flags | ~flag;
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
