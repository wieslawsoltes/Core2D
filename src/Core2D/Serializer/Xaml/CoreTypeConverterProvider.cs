using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Serializer.Xaml.Converters;
using Core2D.Style;

namespace Core2D.Serializer.Xaml
{
    internal static class CoreTypeConverterProvider
    {
        private static Dictionary<Type, Type> s_registered = new Dictionary<Type, Type>
        {
            { typeof(ArgbColor), typeof(ArgbColorTypeConverter) },
            { typeof(FontStyle), typeof(FontStyleTypeConverter) },
            { typeof(ShapeState), typeof(ShapeStateTypeConverter) },
            { typeof(MatrixObject), typeof(MatrixObjectTypeConverter) }
        };

        public static Type Find(Type type)
        {
            s_registered.TryGetValue(type, out var result);
            return result;
        }
    }
}
