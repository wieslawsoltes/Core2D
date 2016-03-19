// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data.Database;
using Portable.Xaml.Markup;
using System.Collections.Immutable;

namespace Core2D.Xaml.Collections
{
    /// <summary>
    /// Observable <see cref="XDatabase"/> collection.
    /// </summary>
    [ContentProperty(nameof(Children))]
    [RuntimeNameProperty(nameof(Name))]
    public sealed class XDatabases : ObservableResource
    {
        /// <summary>
        /// Gets or sets resource name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets children collection.
        /// </summary>
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
