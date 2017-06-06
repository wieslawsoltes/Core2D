// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml;
using System.IO;
using System.Xml;

namespace Core2D.Serializer.Xaml
{
    internal static class CoreXamlReader
    {
        internal static readonly XamlSchemaContext context = new CoreXamlSchemaContext();

        private static object Load(XamlXmlReader reader)
        {
            using (var writer = new XamlObjectWriter(context, new XamlObjectWriterSettings()))
            {
                XamlServices.Transform(reader, writer);
                return writer.Result;
            }
        }

        public static object Load(Stream stream)
        {
            using (var reader = new XamlXmlReader(stream, context))
            {
                return Load(reader);
            }
        }

        public static object Load(string path)
        {
            using (var reader = new XamlXmlReader(path, context))
            {
                return Load(reader);
            }
        }

        public static object Load(TextReader textReader)
        {
            using (var reader = new XamlXmlReader(textReader, context))
            {
                return Load(reader);
            }
        }

        public static object Load(XmlReader xmlReader)
        {
            using (var reader = new XamlXmlReader(xmlReader, context))
            {
                return Load(reader);
            }
        }
    }
}
