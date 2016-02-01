// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using Perspex.Markup;

namespace Core2D.Perspex.Converters
{
    /// <summary>
    /// Converts multi-binding inputs to a final value.
    /// </summary>
    public sealed class ObjectEqualityMultiConverter : IMultiValueConverter
    {
        /// <summary>
        /// Gets an instance of a <see cref="ObjectEqualityMultiConverter"/>.
        /// </summary>
        public static readonly ObjectEqualityMultiConverter Instance = new ObjectEqualityMultiConverter();

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
            return (values[0] == values[1]) ? true : false;
        }
    }
}
