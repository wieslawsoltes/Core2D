// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;

namespace Serializer.Xaml
{
    /// <summary>
    /// Portable Xaml serializer.
    /// </summary>
    public sealed class PortableXamlSerializer : IXamlSerializer
    {
        /// <inheritdoc/>
        string IXamlSerializer.Serialize<T>(T value)
        {
            return CoreXamlSerializer.Serialize(value);
        }

        /// <inheritdoc/>
        T IXamlSerializer.Deserialize<T>(string xaml)
        {
            return CoreXamlSerializer.Deserialize<T>(xaml);
        }
    }
}
