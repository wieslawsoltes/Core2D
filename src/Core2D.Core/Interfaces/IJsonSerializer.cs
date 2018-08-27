// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines json string serializer contract.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serialize the object value to json string.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <returns>The new instance of object of type <see cref="string"/>.</returns>
        string Serialize<T>(T value);

        /// <summary>
        /// Deserialize the json string to object.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="json">The json string.</param>
        /// <returns>The new instance of object of type <typeparamref name="T"/>.</returns>
        T Deserialize<T>(string json);
    }
}
