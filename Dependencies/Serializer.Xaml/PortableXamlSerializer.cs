// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// Portable Xaml serializer.
    /// </summary>
    public class PortableXamlSerializer : ITextSerializer
    {
        /// <inheritdoc/>
        public string Serialize<T>(T value)
        {
            return CoreXamlSerializer.Serialize(value);
        }

        /// <inheritdoc/>
        public T Deserialize<T>(string text)
        {
            return CoreXamlSerializer.Deserialize<T>(text);
        }
    }
}
