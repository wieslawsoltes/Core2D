// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// Xaml serializer.
    /// </summary>
    public class CoreXamlSerializer : ISerializer
    {
        /// <inheritdoc/>
        public string Serialize<T>(T value)
        {
            throw new NotImplementedException(nameof(Serialize));
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
