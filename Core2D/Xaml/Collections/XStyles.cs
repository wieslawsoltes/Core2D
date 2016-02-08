// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Style;
using Portable.Xaml.Markup;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core2D.Xaml.Collections
{
    /// <summary>
    /// Observable <see cref="ShapeStyle"/> collection.
    /// </summary>
    [ContentProperty(nameof(Children))]
    [RuntimeNameProperty(nameof(Name))]
    public sealed class XStyles : ObservableResource
    {
        /// <summary>
        /// Gets or sets container name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets children collection.
        /// </summary>
        public ICollection<ShapeStyle> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XStyles"/> class.
        /// </summary>
        public XStyles()
        {
            Children = new Collection<ShapeStyle>();
        }
    }
}
