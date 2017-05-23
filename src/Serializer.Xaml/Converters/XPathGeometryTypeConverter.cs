// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
#if NETSTANDARD1_3
using System.ComponentModel;
#else
using Portable.Xaml.ComponentModel;
#endif
using Core2D.Path;
using Core2D.Path.Parser;

namespace Serializer.Xaml.Converters
{
    /// <summary>
    /// Defines <see cref="XPathGeometry"/> type converter.
    /// </summary>
    internal class XPathGeometryTypeConverter : TypeConverter
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
            return XPathGeometryParser.Parse((string)value);
        }

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var geometry = value as XPathGeometry;
            if (geometry != null)
            {
                return geometry.ToString();
            }
            throw new NotSupportedException();
        }
    }
}
