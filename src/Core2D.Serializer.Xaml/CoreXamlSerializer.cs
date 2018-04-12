// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Core2D.Serializer.Xaml
{
    /// <summary>
    /// Xaml serializer.
    /// </summary>
    internal static class CoreXamlSerializer
    {
        internal static readonly XmlWriterSettings settings = new XmlWriterSettings()
        {
            OmitXmlDeclaration = true,
            Encoding = Encoding.UTF8,
            Indent = true,
            IndentChars = "    ",
            NewLineChars = Environment.NewLine,
            NewLineHandling = NewLineHandling.None,
            NewLineOnAttributes = false,
            NamespaceHandling = NamespaceHandling.OmitDuplicates
        };

        /// <summary>
        /// Serialize the object value to xaml string.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <returns>The new instance of object of type <see cref="string"/>.</returns>
        public static string Serialize<T>(T value)
        {
            using (var textWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    CoreXamlWriter.Save(xmlWriter, value);
                    return textWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Deserialize the xaml string to object.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="xaml">The xaml string.</param>
        /// <returns>The new instance of object of type <typeparamref name="T"/>.</returns>
        public static T Deserialize<T>(string xaml)
        {
            using (var textReader = new StringReader(xaml))
            {
                return (T)CoreXamlReader.Load(textReader);
            }
        }
    }
}
