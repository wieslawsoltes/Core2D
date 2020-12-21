#nullable disable
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Core2D.Converters
{
    public class HexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && targetType == typeof(Color))
            {
                try
                {
                    if (Color.TryParse(s, out var color))
                    {
                        return color;
                    }
                }
                catch (Exception)
                {
                    return AvaloniaProperty.UnsetValue;
                }
            }
            if (value is uint n && targetType == typeof(Color))
            {
                try
                {
                    return Color.FromUInt32(n);
                }
                catch (Exception)
                {
                    return AvaloniaProperty.UnsetValue;
                }
            }
            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c1 && targetType == typeof(string))
            {
                try
                {
                    return $"#{c1.ToUint32():x8}";
                }
                catch (Exception)
                {
                    return AvaloniaProperty.UnsetValue;
                }
            }
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
}
