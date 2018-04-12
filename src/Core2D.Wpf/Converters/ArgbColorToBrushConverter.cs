// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Style;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Core2D.Wpf.Converters
{
    /// <summary>
    /// Provides a way to apply custom logic to a binding.
    /// </summary>
    public class ArgbColorToBrushConverter : IValueConverter
    {
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
            if (value is ArgbColor color && value != DependencyProperty.UnsetValue)
            {
                var brush = new SolidColorBrush(
                    Color.FromArgb(
                        color.A,
                        color.R,
                        color.G,
                        color.B));
                brush.Freeze();
                return brush;
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
            if (value is SolidColorBrush brush && value != DependencyProperty.UnsetValue)
            {
                return ArgbColor.Create(
                    brush.Color.A,
                    brush.Color.R,
                    brush.Color.G,
                    brush.Color.B);
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
