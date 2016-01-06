// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using Portable.Xaml;

namespace Dependencies
{
    public static class CoreXamlReader
    {
        internal static readonly CoreXamlSchemaContext context = new CoreXamlSchemaContext();

        private static object Load(XamlXmlReader reader)
        {
            var writerSettings = new XamlObjectWriterSettings();
            using (var writer = new XamlObjectWriter(context, writerSettings))
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
    }
}
