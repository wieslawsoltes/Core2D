// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Portable.Xaml;
using Portable.Xaml.Schema;

namespace Core2D.Xaml.Serializer
{
    internal class CoreXamlType : XamlType
    {
        public CoreXamlType(Type underlyingType, XamlSchemaContext schemaContext)
            : base(underlyingType, schemaContext)
        {
        }

        protected override XamlTypeInvoker LookupInvoker()
        {
            return new CoreXamlTypeInvoker(this);
        }
    }
}
