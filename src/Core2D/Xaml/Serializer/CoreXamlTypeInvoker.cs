// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml.Schema;

namespace Core2D.Xaml.Serializer
{
    internal class CoreXamlTypeInvoker : XamlTypeInvoker
    {
        public CoreXamlType XamlType { get; private set; }

        public CoreXamlTypeInvoker(CoreXamlType xamlType)
            : base(xamlType)
        {
            XamlType = xamlType;
        }
    }
}
