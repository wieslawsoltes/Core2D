// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Test2d;

namespace Test.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class ArgbColorToBrushConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
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
