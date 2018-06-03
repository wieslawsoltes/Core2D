// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Core2D.Avalonia.Converters
{
    /// <summary>
    /// Converts a binding value object from <see cref="object"/> to <see cref="bool"/> True if value is equal to parameter otherwise return False.
    /// </summary>
    public class ObjectEqualsConverter : IValueConverter
    {
        /// <summary>
        /// Convert value to boolean, true if matches parameter.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return value.Equals(parameter);
            }
            return AvaloniaProperty.UnsetValue;
        }

        /// <summary>
        /// Convert boolean to parameter value, returning parameter if true.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return (bool)value ? parameter : AvaloniaProperty.UnsetValue;
            }
            return AvaloniaProperty.UnsetValue;
        }
    }
}
