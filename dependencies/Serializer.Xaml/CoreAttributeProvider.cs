// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace Serializer.Xaml
{
    internal class CoreAttributeProvider : Portable.Xaml.ComponentModel.ICustomAttributeProvider
    {
        private readonly Type _type;

        public CoreAttributeProvider(Type type)
        {
            _type = type;
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Attribute result = null;

            if (attributeType == typeof(Portable.Xaml.Markup.ContentPropertyAttribute))
            {
                result = GetContentPropertyAttribute(inherit);
            }
            else if (attributeType == typeof(Portable.Xaml.Markup.RuntimeNamePropertyAttribute))
            {
                result = GetRuntimeNamePropertyAttribute(inherit);
            }

            if (result != null)
            {
                return new[] { result };
            }
            else
            {
                return new object[0];
            }
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        private Attribute GetContentPropertyAttribute(bool inherit)
        {
            var type = _type;

            while (type != null)
            {
                var properties = type.GetTypeInfo().DeclaredProperties.Where(x => x.GetCustomAttribute<ContentAttribute>() != null);
                string result = null;

                foreach (var property in properties)
                {
                    if (result != null)
                    {
                        throw new Exception($"Content property defined more than once on {type}.");
                    }
                    result = property.Name;
                }

                if (result != null)
                {
                    return new Portable.Xaml.Markup.ContentPropertyAttribute(result);
                }

                type = inherit ? type.GetTypeInfo().BaseType : null;
            }

            return null;
        }

        private Attribute GetRuntimeNamePropertyAttribute(bool inherit)
        {
            var type = _type;

            while (type != null)
            {
                var properties = type.GetTypeInfo().DeclaredProperties.Where(x => x.GetCustomAttribute<NameAttribute>() != null);
                string result = null;

                foreach (var property in properties)
                {
                    if (result != null)
                    {
                        throw new Exception($"Name property defined more than once on {type}.");
                    }
                    result = property.Name;
                }

                if (result != null)
                {
                    return new Portable.Xaml.Markup.RuntimeNamePropertyAttribute(result);
                }

                type = inherit ? type.GetTypeInfo().BaseType : null;
            }

            return null;
        }
    }
}
