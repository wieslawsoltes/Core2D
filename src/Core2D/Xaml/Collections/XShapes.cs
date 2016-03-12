// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;
using Portable.Xaml.Markup;
using System.Collections.Immutable;

namespace Core2D.Xaml.Collections
{
    /// <summary>
    /// Observable <see cref="BaseShape"/> collection.
    /// </summary>
    [ContentProperty(nameof(Children))]
    [RuntimeNameProperty(nameof(Name))]
    public sealed class XShapes : ObservableResource
    {
        /// <summary>
        /// Gets or sets container name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets children collection.
        /// </summary>
        public ImmutableArray<BaseShape> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XShapes"/> class.
        /// </summary>
        public XShapes()
        {
            Children = ImmutableArray.Create<BaseShape>();
        }
    }
}
