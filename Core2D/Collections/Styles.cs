// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Portable.Xaml.Markup;

namespace Core2D
{
    /// <summary>
    /// The <see cref="ShapeStyle"/> collection container.
    /// </summary>
    [ContentProperty("Children")]
    [RuntimeNameProperty("Name")]
    public class Styles : ObservableResource
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
        /// Initializes a new instance of the <see cref="Styles"/> class.
        /// </summary>
        public Styles()
        {
            Children = new Collection<ShapeStyle>();
        }
    }
}
