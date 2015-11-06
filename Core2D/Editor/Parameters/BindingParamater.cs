// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// The <see cref="Binding"/> command parameter object.
    /// </summary>
    public class BindingParameter
    {
        /// <summary>
        /// Gets or sets the <see cref="Binding"/> object owner.
        /// </summary>
        public object Owner { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Binding"/> object.
        /// </summary>
        public Binding Binding { get; set; }
    }
}
