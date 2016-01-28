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
        private static readonly TypeModel _model;

        static ProtoBufSerializer()
        {
            _model = ProtoBufModel.Create().Compile();
        }

        /// <inheritdoc/>
        public void Serialize<T>(Stream destination, T value)
        {
            _model.Serialize(destination, value);
        }

        /// <inheritdoc/>
        public T Deserialize<T>(Stream source, T value)
        {
            return (T)_model.Deserialize(source, value, typeof(T));
        }
    }
}
