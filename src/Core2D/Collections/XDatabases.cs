// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using Core2D.Attributes;
using Core2D.Data.Database;

namespace Core2D.Collections
{
    /// <summary>
    /// Observable <see cref="XDatabase"/> collection.
    /// </summary>
    public class XDatabases : ObservableObject
    {
        /// <summary>
        /// Gets or sets children collection.
        /// </summary>
        [Content]
        public ImmutableArray<XDatabase> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XDatabase"/> class.
        /// </summary>
        public XDatabases()
        {
            Children = ImmutableArray.Create<XDatabase>();
        }

        /// <summary>
        /// Check whether the <see cref="Children"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeChildren() => Children.IsEmpty == false;
    }
}
