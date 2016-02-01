// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using Perspex;
using Perspex.Markup;

namespace Core2D.Perspex.Converters
{
    /// <summary>
    /// Rounds a double-precision floating-point binding value to a specified number of fractional digits.
    /// </summary>
    public sealed class RoundedDoubleToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Gets an instance of a <see cref="RoundedDoubleToDoubleConverter"/>.
        /// </summary>
        public static readonly RoundedDoubleToDoubleConverter Instance = new RoundedDoubleToDoubleConverter(1);

        /// <summary>
        /// The number of fractional digits in the converted value.
        /// </summary>
        private readonly int _digits;

        /// <summary>
        /// Creates a new instance of the <see cref="RoundedDoubleToDoubleConverter"/>.
        /// </summary>
        /// <param name="digits">The default number of fractional digits in the converted value.</param>
        public RoundedDoubleToDoubleConverter(int digits)
        {
            _digits = digits;
        }

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
            int digits = _digits;
            if (parameter != null && value.GetType() == typeof(string))
            {
                int.TryParse(parameter as string, out digits);
            }

            if (value != null && value.GetType() == typeof(double))
            {
                return Math.Round((double)value, digits);
            }

            return PerspexProperty.UnsetValue;
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
            int digits = _digits;
            if (parameter != null && value.GetType() == typeof(string))
            {
                int.TryParse(parameter as string, out digits);
            }

            if (value != null && value.GetType() == typeof(double))
            {
                return Math.Round((double)value, digits);
            }

            return PerspexProperty.UnsetValue;
        }
    }
}
