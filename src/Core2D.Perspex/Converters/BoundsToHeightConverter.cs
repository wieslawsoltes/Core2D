// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using Perspex;
using Perspex.Markup;

namespace Core2D.Perspex.Converters
{
    /// <summary>
    /// Converts a binding value object from <see cref="Rect"/> to <see cref="Rect.Height"/>, convert back is not supported.
    /// </summary>
    public sealed class BoundsToHeightConverter : IValueConverter
    {
        /// <summary>
        /// Gets an instance of a <see cref="BoundsToHeightConverter"/>.
        /// </summary>
        public static readonly BoundsToHeightConverter Instance = new BoundsToHeightConverter();

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
            if (value == null || value == PerspexProperty.UnsetValue || value.GetType() != typeof(Rect))
            {
                return PerspexProperty.UnsetValue;
            }

            return (double)((Rect)value).Height;
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
            return PerspexProperty.UnsetValue;
        }
    }
}
