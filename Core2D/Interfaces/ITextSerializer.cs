// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Defines text string serializer contract.
    /// </summary>
    public interface ITextSerializer
    {
        /// <summary>
        /// Serialize the object value to text string.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <returns>The new instance of object of type <see cref="string"/>.</returns>
        string Serialize<T>(T value);

        /// <summary>
        /// Deserialize the text string to object.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="text">The text string.</param>
        /// <returns>The new instance of object of type <typeparamref name="T"/>.</returns>
        T Deserialize<T>(string text);
    }
}
