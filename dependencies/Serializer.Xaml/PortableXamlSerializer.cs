// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;

namespace Serializer.Xaml
{
    /// <summary>
    /// Portable Xaml serializer.
    /// </summary>
    public sealed class PortableXamlSerializer : ITextSerializer
    {
        /// <inheritdoc/>
        string ITextSerializer.Serialize<T>(T value)
        {
            return CoreXamlSerializer.Serialize(value);
        }

        /// <inheritdoc/>
        T ITextSerializer.Deserialize<T>(string text)
        {
            return CoreXamlSerializer.Deserialize<T>(text);
        }
    }
}
