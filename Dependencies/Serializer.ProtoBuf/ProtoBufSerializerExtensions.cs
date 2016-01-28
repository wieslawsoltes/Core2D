// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// ProtoBuf serializer extensions methods.
    /// </summary>
    public static class ProtoBufSerializerExtensions
    {
        /// <summary>
        /// Serialize the object value to destination file path.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="serializer">The protobuf serializer.</param>
        /// <param name="path">The file path.</param>
        /// <param name="value">The object instance.</param>
        public static void Serialize<T>(this IStreamSerializer serializer, string path, T value)
        {
            using (var destination = File.Create(path))
            {
                serializer.Serialize(destination, value);
            }
        }

        /// <summary>
        ///  Deserialize the file from path to object.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="serializer">The protobuf serializer.</param>
        /// <param name="path">The file path.</param>
        /// <param name="value">The existing object instance (which may be null).</param>
        /// <returns>The new instance of object of type <typeparamref name="T"/>.</returns>
        public static T Deserialize<T>(this IStreamSerializer serializer, string path, T value)
        {
            using (var destination = File.OpenRead(path))
            {
                return serializer.Deserialize(destination, value);
            }
        }
    }
}
