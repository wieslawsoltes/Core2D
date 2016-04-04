// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using System.IO;

namespace Serializer.ProtoBuf
{
    /// <summary>
    /// ProtoBuf serializer.
    /// </summary>
    public sealed class ProtoBufStreamSerializer : IStreamSerializer
    {
        /// <summary>
        /// The compiled ProtoBuf serializer.
        /// </summary>
        private static readonly ProtoBufSerializer Serializer = new ProtoBufSerializer();

        /// <inheritdoc/>
        void IStreamSerializer.Serialize<T>(Stream destination, T value)
        {
            Serializer.Serialize(destination, value);
        }

        /// <inheritdoc/>
        T IStreamSerializer.Deserialize<T>(Stream source, T value)
        {
            return (T)Serializer.Deserialize(source, value, typeof(T));
        }
    }
}
