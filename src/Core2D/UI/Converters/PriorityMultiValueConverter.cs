using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Core2D.UI.Converters
{
    /// <summary>
    /// Converts multi-binding inputs to a final value.
    /// </summary>
    public class PriorityMultiValueConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts multi-binding inputs to a final value.
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Count == 2)
            {
                var first = values[0];
                var second = values[1];
                return IsValid(first) ? first : second;
            }
            bool IsValid(object value)
            {
                return value != null && value != AvaloniaProperty.UnsetValue && !(value is BindingNotification);
            }
            return AvaloniaProperty.UnsetValue;
        }
    }
}
