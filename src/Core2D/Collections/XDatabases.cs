// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Attributes;
using Core2D.Data.Database;
using System.Collections.Immutable;

namespace Core2D.Collections
{
    /// <summary>
    /// Observable <see cref="XDatabase"/> collection.
    /// </summary>
    public class XDatabases : ObservableResource
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
        public ImmutableArray<XDatabase> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XDatabase"/> class.
        /// </summary>
        public XDatabases()
        {
            Children = ImmutableArray.Create<XDatabase>();
        }
    }
}
