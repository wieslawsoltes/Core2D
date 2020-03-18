using System;
using System.ComponentModel;
using System.Reflection;
using Portable.Xaml;
using Portable.Xaml.Schema;

namespace Core2D.Serializer.Xaml
{
    internal class CoreXamlType : XamlType
    {
        public CoreXamlType(Type underlyingType, XamlSchemaContext schemaContext)
            : base(underlyingType, schemaContext)
        {
        }

        protected override ICustomAttributeProvider LookupCustomAttributeProvider()
        {
            return new CoreAttributeProvider(UnderlyingType);
        }

        protected override XamlValueConverter<TypeConverter> LookupTypeConverter()
        {
            var result = CoreTypeConverterProvider.Find(UnderlyingType);
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
