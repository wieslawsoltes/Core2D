// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using System.Xml;
using Portable.Xaml;

namespace Core2D.Serializer.Xaml
{
    internal static class CoreXamlWriter
    {
        internal static readonly XamlSchemaContext context = new CoreXamlSchemaContext();

        internal static readonly XamlObjectReaderSettings settings = new XamlObjectReaderSettings();

        private static void Save(XamlXmlWriter writer, object instance)
        {
            using (var reader = new XamlObjectReader(instance, context, settings))
            {
                XamlServices.Transform(reader, writer);
            }
        }

        public static void Save(Stream stream, object instance)
        {
            using (var writer = new XamlXmlWriter(stream, context))
            {
                Save(writer, instance);
            }
        }

        public static void Save(TextWriter textWriter, object instance)
        {
            using (var writer = new XamlXmlWriter(textWriter, context))
            {
                Save(writer, instance);
            }
        }

        public static void Save(XmlWriter xmlWriter, object instance)
        {
            using (var writer = new XamlXmlWriter(xmlWriter, context))
            {
                Save(writer, instance);
            }
        }
    }
}
