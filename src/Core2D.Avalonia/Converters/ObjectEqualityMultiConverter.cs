// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Core2D.Avalonia.Converters
{
    /// <summary>
    /// Converts multi-binding inputs to a final value.
    /// </summary>
    public class ObjectEqualityMultiConverter : IMultiValueConverter
    {
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
            if (values != null && values.Count == 2 && values[0] != AvaloniaProperty.UnsetValue && values[1] != AvaloniaProperty.UnsetValue)
            {
                return values[0].Equals(values[1]);
            }
            return AvaloniaProperty.UnsetValue; 
        }
    }
}
