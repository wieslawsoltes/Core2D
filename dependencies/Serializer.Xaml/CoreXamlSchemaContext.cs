// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml;
using Portable.Xaml.ComponentModel;
using System;
using System.Collections.Generic;

namespace Serializer.Xaml
{
    internal class CoreXamlSchemaContext : XamlSchemaContext
    {
        public const string CoreNamespace = "https://github.com/Core2D";

        private readonly Dictionary<Type, XamlType> _typeCache = new Dictionary<Type, XamlType>();

        protected override ICustomAttributeProvider GetCustomAttributeProvider(Type type)
        {
            return new CoreAttributeProvider(type);
        }

        public override XamlType GetXamlType(Type type)
        {
            XamlType xamlType;

            if (!_typeCache.TryGetValue(type, out xamlType))
            {
                xamlType = new CoreXamlType(type, this);
                _typeCache.Add(type, xamlType);
            }

            return xamlType;
        }
    }
}
