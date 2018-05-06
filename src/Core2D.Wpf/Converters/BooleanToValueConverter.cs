// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Core2D.Wpf.Converters
{
    /// <summary>
    /// Provides a way to apply custom logic to a binding.
    /// </summary>
    public abstract class BooleanToValueConverter<T> : IValueConverter
    {
        /// <summary>
        /// Gets or sets False value substitution.
        /// </summary>
        public T FalseValue { get; set; }

        /// <summary>
        /// Gets or sets True value substitution.
        /// </summary>
        public T TrueValue { get; set; }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return (bool)value ? TrueValue : FalseValue;
            }
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return value != null ? value.Equals(TrueValue) : false;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
