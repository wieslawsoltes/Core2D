using System;
using System.Collections.Generic;
using System.Reflection;
using Portable.Xaml;

namespace Core2D.Serializer.Xaml
{
    internal class CoreXamlSchemaContext : XamlSchemaContext
    {
        private readonly Dictionary<Type, XamlType> _typeCache = new Dictionary<Type, XamlType>();

        protected override ICustomAttributeProvider GetCustomAttributeProvider(Type type)
        {
            return new CoreAttributeProvider(type);
        }

        public override XamlType GetXamlType(Type type)
        {
            if (!_typeCache.TryGetValue(type, out var xamlType))
            {
                xamlType = new CoreXamlType(type, this);
                _typeCache.Add(type, xamlType);
            }

            return xamlType;
        }
    }
}
