// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path;
using Core2D.Shape;
using Core2D.Style;
using Serializer.Xaml.Converters;
using System;
using System.Collections.Generic;

namespace Serializer.Xaml
{
    internal static class CoreTypeConverterProvider
    {
        private static Dictionary<Type, Type> _registered = new Dictionary<Type, Type>
        {
            { typeof(ArgbColor), typeof(ArgbColorTypeConverter) },
            { typeof(FontStyle), typeof(FontStyleTypeConverter) },
            { typeof(ShapeState), typeof(ShapeStateTypeConverter) },
            { typeof(XPathGeometry), typeof(XPathGeometryTypeConverter) }
        };

        public static Type Find(Type type)
        {
            Type result;
            _registered.TryGetValue(type, out result);
            return result;
        }
    }
}
