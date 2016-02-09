// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines data stream serializer contract.
    /// </summary>
    public interface IStreamSerializer
    {
        /// <summary>
        /// Serialize the object value to destination stream.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="destination">The destination stream.</param>
        /// <param name="value">The object instance.</param>
        void Serialize<T>(Stream destination, T value);

        /// <summary>
        /// Deserialize the source stream to object.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="source">The source stream.</param>
        /// <param name="value">The existing object instance (which may be null).</param>
        /// <returns>The new instance of object of type <typeparamref name="T"/>.</returns>
        T Deserialize<T>(Stream source, T value);
    }
}
