// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using Core2D.Attributes;
using Core2D.Shape;

namespace Core2D.Collections
{
    /// <summary>
    /// Observable <see cref="BaseShape"/> collection.
    /// </summary>
    public class XShapes : ObservableObject
    {
        /// <summary>
        /// Gets or sets children collection.
        /// </summary>
        [Content]
        public ImmutableArray<BaseShape> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XShapes"/> class.
        /// </summary>
        public XShapes()
        {
            Children = ImmutableArray.Create<BaseShape>();
        }

        /// <summary>
        /// Check whether the <see cref="Children"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeChildren() => Children.IsEmpty == false;
    }
}
