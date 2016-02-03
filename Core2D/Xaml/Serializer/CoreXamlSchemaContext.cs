// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Portable.Xaml;

namespace Core2D
{
    internal class CoreXamlSchemaContext : XamlSchemaContext
    {
        public override XamlType GetXamlType(Type type)
        {
            return new CoreXamlType(type, this);
        }

        protected override XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
        {
            var type = base.GetXamlType(xamlNamespace, name, typeArguments);
            return new CoreXamlType(type.UnderlyingType, this);
        }
    }
}
