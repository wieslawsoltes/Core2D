// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;
using Perspex.Markup;

namespace Core2D.Perspex.Converters
{
    /// <summary>
    /// Converts a binding value object from <see cref="object"/> to <see cref="bool"/> True if value is equal to parameter otherwise return False.
    /// </summary>
    public class ObjectEqualsConverter : IValueConverter
    {
        /// <summary>
        /// Gets an instance of a <see cref="ObjectEqualsConverter"/>.
        /// </summary>
        public static readonly ObjectEqualsConverter Instance = new ObjectEqualsConverter();

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
            return value.Equals(parameter);
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
            return (bool)value ? parameter : PerspexProperty.UnsetValue;
        }
    }
}
