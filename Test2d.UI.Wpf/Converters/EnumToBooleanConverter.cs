// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using System.Windows.Data;

namespace Test.Converters
{
    /// <summary>
    /// Enum to Boolean value converter.
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Convert enum [value] to boolean, true if matches [param].
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="param"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object param, CultureInfo culture)
        {
            return value.Equals(param);
        }

        /// <summary>
        /// Convert boolean to enum, returning [param] if true.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="param"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture)
        {
            return (bool)value ? param : Binding.DoNothing;
        }
    }
}
