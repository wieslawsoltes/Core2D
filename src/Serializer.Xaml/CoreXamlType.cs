// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
#if NETSTANDARD
using System.ComponentModel;
#else
using Portable.Xaml.ComponentModel;
#endif
using Portable.Xaml;
using Portable.Xaml.Schema;

namespace Serializer.Xaml
{
    internal class CoreXamlType : XamlType
    {
        public CoreXamlType(Type underlyingType, XamlSchemaContext schemaContext)
            : base(underlyingType, schemaContext)
        {
        }

        protected override ICustomAttributeProvider LookupCustomAttributeProvider()
        {
            return new CoreAttributeProvider(this.UnderlyingType);
        }

        protected override XamlValueConverter<TypeConverter> LookupTypeConverter()
        {
            var result = CoreTypeConverterProvider.Find(this.UnderlyingType);
            if (result != null)
            {
                return new XamlValueConverter<TypeConverter>(result, this);
            }
            else
            {
                return base.LookupTypeConverter();
            }
        }
    }
}
