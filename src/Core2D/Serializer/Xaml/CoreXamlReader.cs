using System.IO;
using System.Xml;
using Portable.Xaml;

namespace Core2D.Serializer.Xaml
{
    internal static class CoreXamlReader
    {
        internal static readonly XamlSchemaContext s_context = new CoreXamlSchemaContext();

        private static object Load(XamlXmlReader reader)
        {
            using (var writer = new XamlObjectWriter(s_context, new XamlObjectWriterSettings()))
            {
                XamlServices.Transform(reader, writer);
                return writer.Result;
            }
        }

        public static object Load(Stream stream)
        {
            using (var reader = new XamlXmlReader(stream, s_context))
            {
                return Load(reader);
            }
        }

        public static object Load(string path)
        {
            using (var reader = new XamlXmlReader(path, s_context))
            {
                return Load(reader);
            }
        }

        public static object Load(TextReader textReader)
        {
            using (var reader = new XamlXmlReader(textReader, s_context))
            {
                return Load(reader);
            }
        }

        public static object Load(XmlReader xmlReader)
        {
            using (var reader = new XamlXmlReader(xmlReader, s_context))
            {
                return Load(reader);
            }
        }
    }
}
