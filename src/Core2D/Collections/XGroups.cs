// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using Core2D.Attributes;
using Core2D.Shapes;

namespace Core2D.Collections
{
    /// <summary>
    /// Observable <see cref="XGroup"/> collection.
    /// </summary>
    public class XGroups : ObservableObject
    {
        /// <summary>
        /// Gets or sets children collection.
        /// </summary>
        [Content]
        public ImmutableArray<XGroup> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XGroups"/> class.
        /// </summary>
        public XGroups()
        {
            Children = ImmutableArray.Create<XGroup>();
        }

        /// <summary>
        /// Check whether the <see cref="Children"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeChildren() => Children.IsEmpty == false;
    }
}
