// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Core2D
{
    /// <summary>
    /// Named items library.
    /// </summary>
    public class Library<T> : ObservableObject
    {
        private string _name;
        private ImmutableArray<T> _items;
        private T _selected;

        /// <summary>
        /// Gets or sets library name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets a items collection.
        /// </summary>
        public ImmutableArray<T> Items
        {
            get { return _items; }
            set { Update(ref _items, value); }
        }

        /// <summary>
        /// Gets or sets currently selected item from <see cref="Items"/> collection.
        /// </summary>
        public T Selected
        {
            get { return _selected; }
            set { Update(ref _selected, value); }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Library{T}"/> class.
        /// </summary>
        /// <param name="name">The library name.</param>
        /// <returns>The new instance of the <see cref="Library{T}"/> class.</returns>
        public static Library<T> Create(string name)
        {
            return new Library<T>()
            {
                Name = name,
                Items = ImmutableArray.Create<T>(),
                Selected = default(T)
            };
        }
    }
}
