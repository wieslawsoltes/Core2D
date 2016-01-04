// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using OmniXaml;

namespace Dependencies
{
    internal class DefaultXamlLoader : IXamlLoader
    {
        private readonly XamlXmlLoader xamlXmlLoader;

        public DefaultXamlLoader(IWiringContext wiringContext)
        {
            IXamlParserFactory parserFactory = new DefaultParserFactory(wiringContext);
            xamlXmlLoader = new XamlXmlLoader(parserFactory);
        }

        public object Load(Stream stream)
        {
            return xamlXmlLoader.Load(stream);
        }

        public object Load(Stream stream, object instance)
        {
            return xamlXmlLoader.Load(stream, instance);
        }
    }
}
