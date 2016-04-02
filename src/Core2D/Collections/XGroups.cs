// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Attributes;
using Core2D.Shapes;
using System.Collections.Immutable;

namespace Core2D.Collections
{
    /// <summary>
    /// Observable <see cref="XGroup"/> collection.
    /// </summary>
    public sealed class XGroups : ObservableResource
    {
        /// <summary>
        /// Gets or sets resource name.
        /// </summary>
        [Name]
        public string Name { get; set; }

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
    }
}
