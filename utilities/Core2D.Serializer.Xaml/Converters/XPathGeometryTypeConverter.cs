// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
#if NETSTANDARD1_3 || NETCOREAPP1_0
using System.ComponentModel;
#else
using Portable.Xaml.ComponentModel;
#endif
using Core2D.Path;
using Core2D.Path.Parser;

namespace Core2D.Serializer.Xaml.Converters
{
    /// <summary>
    /// Defines <see cref="PathGeometry"/> type converter.
    /// </summary>
    internal class PathGeometryTypeConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false; // destinationType == typeof(string);
        }

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return PathGeometryParser.Parse((string)value);
        }

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value is PathGeometry geometry ? geometry.ToString() : throw new NotSupportedException();
        }
    }
}
