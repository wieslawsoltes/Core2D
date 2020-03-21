using System.IO;
using System.Xml;
using Portable.Xaml;

namespace Core2D.Serializer.Xaml
{
    internal static class CoreXamlWriter
    {
        internal static readonly XamlSchemaContext s_context = new CoreXamlSchemaContext();

        internal static readonly XamlObjectReaderSettings s_settings = new XamlObjectReaderSettings();

        private static void Save(XamlXmlWriter writer, object instance)
        {
            using var reader = new XamlObjectReader(instance, s_context, s_settings);
            XamlServices.Transform(reader, writer);
        }

        public static void Save(Stream stream, object instance)
        {
            using var writer = new XamlXmlWriter(stream, s_context);
            Save(writer, instance);
        }

        public static void Save(TextWriter textWriter, object instance)
        {
            using var writer = new XamlXmlWriter(textWriter, s_context);
            Save(writer, instance);
        }

        public static void Save(XmlWriter xmlWriter, object instance)
        {
            using var writer = new XamlXmlWriter(xmlWriter, s_context);
            Save(writer, instance);
        }
    }
}
