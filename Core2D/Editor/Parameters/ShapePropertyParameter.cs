// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// The <see cref="Core2D.ShapeProperty"/> command parameter object.
    /// </summary>
    public class ShapePropertyParameter
    {
        /// <summary>
        /// Gets or sets the <see cref="Core2D.ShapeProperty"/> object owner.
        /// </summary>
        public object Owner { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Core2D.ShapeProperty"/> object.
        /// </summary>
        public ShapeProperty Property { get; set; }
    }
}
