// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes;
using Portable.Xaml.Markup;
using System.Collections.Immutable;

namespace Core2D.Collections
{
    /// <summary>
    /// Observable <see cref="XGroup"/> collection.
    /// </summary>
    [ContentProperty(nameof(Children))]
    [RuntimeNameProperty(nameof(Name))]
    public sealed class XGroups : ObservableResource
    {
        /// <summary>
        /// Gets or sets resource name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets children collection.
        /// </summary>
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
