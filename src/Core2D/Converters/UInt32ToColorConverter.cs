using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Core2D.Converters;

public class UInt32ToColorConverter : IValueConverter
{
    public static UInt32ToColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is uint c && (targetType == typeof(Color?) || targetType == typeof(Color)))
        {
            try
            {
                return Color.FromUInt32(c);
            }
            catch (Exception)
            {
                return AvaloniaProperty.UnsetValue;
            }
        }
        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color c2 && targetType == typeof(uint))
        {
            try
            {
                return c2.ToUint32();
            }
            catch (Exception)
            {
                return AvaloniaProperty.UnsetValue;
            }
        }
        return AvaloniaProperty.UnsetValue;
    }
}
