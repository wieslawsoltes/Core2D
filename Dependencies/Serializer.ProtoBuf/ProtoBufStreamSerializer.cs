// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using System.IO;

namespace Serializer.ProtoBuf
{
    /// <summary>
    /// ProtoBuf serializer.
    /// </summary>
    public class ProtoBufStreamSerializer : IStreamSerializer
    {
        /// <summary>
        /// The compiled ProtoBuf serializer.
        /// </summary>
        public static ProtoBufSerializer Serializer = new ProtoBufSerializer();

        /// <inheritdoc/>
        public void Serialize<T>(Stream destination, T value)
        {
            Serializer.Serialize(destination, value);
        }

        /// <inheritdoc/>
        public T Deserialize<T>(Stream source, T value)
        {
            return (T)Serializer.Deserialize(source, value, typeof(T));
        }
    }
}
