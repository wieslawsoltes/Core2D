// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// The <see cref="Core2D.Value"/> command parameter object.
    /// </summary>
    public class ValueParameter
    {
        /// <summary>
        /// Gets or sets the <see cref="Core2D.Value"/> object owner.
        /// </summary>
        public object Owner { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Core2D.Value"/> object.
        /// </summary>
        public Value Value { get; set; }
    }
}
