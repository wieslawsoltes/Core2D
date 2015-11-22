// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
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
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as ArgbColor;
            if (color != null)
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
            return null;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as SolidColorBrush;
            if (brush != null)
            {
                return ArgbColor.Create(
                    brush.Color.A,
                    brush.Color.R,
                    brush.Color.G,
                    brush.Color.B);
            }
            return null;
        }
    }
}
