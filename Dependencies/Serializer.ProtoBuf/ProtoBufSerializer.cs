// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using ProtoBuf.Meta;
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// ProtoBuf serializer.
    /// </summary>
    public class ProtoBufSerializer : IStreamSerializer
    {
        /// <summary>
        /// The ProtoBuf type model.
        /// </summary>
        public static TypeModel Model;

        /// <summary>
        /// Initializes static data.
        /// </summary>
        static ProtoBufSerializer()
        {
            Model = ProtoBufModel.Create().Compile();
        }

        /// <inheritdoc/>
        public void Serialize<T>(Stream destination, T value)
        {
            Model.Serialize(destination, value);
        }

        /// <inheritdoc/>
        public T Deserialize<T>(Stream source, T value)
        {
            return (T)Model.Deserialize(source, value, typeof(T));
        }
    }
}
