// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Markup;

namespace Core2D.Avalonia.Converters
{
    /// <summary>
    /// Pass-through a binding value <see cref="object"/> as <see cref="object"/> to avoid <see cref="AvaloniaProperty.UnsetValue"/>.
    /// </summary>
    public class ObjectToObjectConverter : IValueConverter
    {
        /// <summary>
        /// Gets an instance of a <see cref="ObjectToObjectConverter"/>.
        /// </summary>
        public static readonly ObjectToObjectConverter Instance = new ObjectToObjectConverter();

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
            return value;
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
            return value;
        }
    }
}
