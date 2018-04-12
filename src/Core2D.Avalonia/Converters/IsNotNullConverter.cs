// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Markup;
using System;
using System.Globalization;

namespace Core2D.Avalonia.Converters
{
    /// <summary>
    /// Converts a binding value object from <see cref="object"/> to <see cref="bool"/> True if value is not equal to null and <see cref="AvaloniaProperty.UnsetValue"/> otherwise return False.
    /// </summary>
    public class IsNotNullConverter : IValueConverter
    {
        /// <summary>
        /// Gets an instance of a <see cref="IsNotNullConverter"/>.
        /// </summary>
        public static readonly IsNotNullConverter Instance = new IsNotNullConverter();

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != AvaloniaProperty.UnsetValue && value != null;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
