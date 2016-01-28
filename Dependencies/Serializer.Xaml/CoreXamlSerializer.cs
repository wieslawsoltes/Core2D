// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Text;
using System.Xml;
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// Xaml serializer.
    /// </summary>
    public class CoreXamlSerializer : ITextSerializer
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

        /// <inheritdoc/>
        public string Serialize<T>(T value)
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

        /// <inheritdoc/>
        public T Deserialize<T>(string text)
        {
            using (var textReader = new StringReader(text))
            {
                return (T)CoreXamlReader.Load(textReader);
            }
        }
    }
}
