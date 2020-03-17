// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

namespace Core2D.UI.Avalonia.Converters
{
    /// <summary>
    /// Converts multi-binding inputs to a final value.
    /// </summary>
    public class EditorToTitleMultiConverter : IMultiValueConverter
    {
        /// <summary>
        /// Default title result.
        /// </summary>
        public static readonly string s_defaultTitle = "Core2D";

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
            if (values != null && values.Count() == 2 && values.All(x => x != AvaloniaProperty.UnsetValue))
            {
                if (values[0] == null || values[0].GetType() != typeof(string))
                {
                    return s_defaultTitle;
                }

                if (values[1] == null || values[1].GetType() != typeof(bool))
                {
                    return s_defaultTitle;
                }

                string name = (string)values[0];
                bool isDirty = (bool)values[1];
                return string.Concat(name, isDirty ? "*" : "", " - ", s_defaultTitle);
            }

            return s_defaultTitle;
        }
    }
}
