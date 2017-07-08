// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Style;
using Core2D.Serializer.Xaml.Converters;
using System;
using System.Collections.Generic;

namespace Core2D.Serializer.Xaml
{
    internal static class CoreTypeConverterProvider
    {
        private static Dictionary<Type, Type> _registered = new Dictionary<Type, Type>
        {
            { typeof(ArgbColor), typeof(ArgbColorTypeConverter) },
            { typeof(FontStyle), typeof(FontStyleTypeConverter) },
            { typeof(ShapeState), typeof(ShapeStateTypeConverter) },
            { typeof(PathGeometry), typeof(PathGeometryTypeConverter) },
            { typeof(MatrixObject), typeof(MatrixObjectTypeConverter) }
        };

        public static Type Find(Type type)
        {
            _registered.TryGetValue(type, out Type result);
            return result;
        }
    }
}
