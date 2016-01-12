// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Portable.Xaml.Markup;

namespace Core2D
{
    /// <summary>
    /// Base class for objects with observable resources.
    /// </summary>
    [ContentProperty(nameof(Resources))]
    public abstract class ObservableResource : ObservableObject
    {
        private IList<ObservableObject> _resources;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableResource"/> class.
        /// </summary>
        public ObservableResource()
            : base()
        {
            Resources = new ObservableCollection<ObservableObject>();
        }

        /// <summary>
        /// Gets or sets shape resources.
        /// </summary>
        public IList<ObservableObject> Resources
        {
            get { return _resources; }
            set { Update(ref _resources, value); }
        }
    }
}
