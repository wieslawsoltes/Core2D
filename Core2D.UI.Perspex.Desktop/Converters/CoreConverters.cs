// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;
using Perspex.Markup;
using Perspex.Media;

namespace Core2D.UI.Perspex.Desktop.Converters
{
    /// <summary>
    /// Core2D objects to Xaml objects value converters.
    /// </summary>
    public static class CoreConverters
    {
        /// <summary>
        /// Convert ArgbColor to Color.
        /// </summary>
        /// <param name="color">The ArgbColor object.</param>
        /// <returns>The converted Color object.</returns>
        public static Color ToColor(ArgbColor color)
        {
            return Color.FromArgb(
                (byte)color.A,
                (byte)color.R,
                (byte)color.G,
                (byte)color.B);
        }

        /// <summary>
        /// Convert ArgbColor to SolidColorBrush.
        /// </summary>
        /// <param name="color">The ArgbColor object.</param>
        /// <returns>The converted SolidColorBrush object.</returns>
        public static SolidColorBrush ToSolidBrush(ArgbColor color)
        {
            return new SolidColorBrush(ToColor(color));
        }

        /// <summary>
        /// Convert ArgbColor to SolidColorBrush.
        /// </summary>
        public static readonly IValueConverter ArgbColorToBrush =
            new FuncValueConverter<ArgbColor, SolidColorBrush>(x => ToSolidBrush(x));

        /// <summary>
        /// 
        /// </summary>
        public static readonly IValueConverter IntToInt =
            new FuncValueConverter<int, int>(x => x);

        /// <summary>
        /// Convert ArgbColor individual A, R, G and B properties to SolidColorBrush.
        /// </summary>
        public static readonly IMultiValueConverter ArgbColorsToBrush =
            new FuncMultiValueConverter<int, SolidColorBrush>(x =>
            {
                var values = x.ToList();
                if (values.Count() == 4)
                {
                    var color = Color.FromArgb(
                        (byte)values[0],
                        (byte)values[1],
                        (byte)values[2],
                        (byte)values[3]);
                    return new SolidColorBrush(color);
                }
                return null;
            });
    }
}
