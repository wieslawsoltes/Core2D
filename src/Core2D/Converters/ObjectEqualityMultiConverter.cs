using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Core2D.Converters
{
    public class ObjectEqualityMultiConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Count == 2 && values[0] != AvaloniaProperty.UnsetValue && values[1] != AvaloniaProperty.UnsetValue)
            {
                return values[0]?.Equals(values[1]);
            }
            return AvaloniaProperty.UnsetValue;
        }
    }
}
