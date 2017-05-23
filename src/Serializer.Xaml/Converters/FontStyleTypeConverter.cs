// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
#if NETSTANDARD1_3
using System.ComponentModel;
#else
using Portable.Xaml.ComponentModel;
#endif
using Core2D.Style;

namespace Serializer.Xaml.Converters
{
    /// <summary>
    /// Defines <see cref="FontStyle"/> type converter.
    /// </summary>
    internal class FontStyleTypeConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return FontStyle.Parse((string)value);
        }

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var style = value as FontStyle;
            if (style != null)
            {
                return style.ToString();
            }
            throw new NotSupportedException();
        }
    }
}
