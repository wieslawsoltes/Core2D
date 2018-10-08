// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Core2D
{
    /// <summary>
    /// Defines copyable contract.
    /// </summary>
    public interface ICopyable
    {
        /// <summary>
        /// Copies the object.
        /// </summary>
        /// <param name="shared">The shared objects dictionary.</param>
        /// <returns>The copy of the object.</returns>
        object Copy(IDictionary<object, object> shared);
    }
}
