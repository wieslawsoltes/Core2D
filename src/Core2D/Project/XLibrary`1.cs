// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Attributes;

namespace Core2D.Project
{
    /// <summary>
    /// Named items library.
    /// </summary>
    public class XLibrary<T> : ObservableObject, ILibrary
    {
        private string _name;
        private ImmutableArray<T> _items;
        private T _selected;

        /// <summary>
        /// Gets or sets library name.
        /// </summary>
        [Name]
        public string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }

        /// <summary>
        /// Gets or sets a items collection.
        /// </summary>
        [Content]
        public ImmutableArray<T> Items
        {
            get => _items;
            set => Update(ref _items, value);
        }

        /// <summary>
        /// Gets or sets currently selected item from <see cref="Items"/> collection.
        /// </summary>
        public T Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XLibrary{T}"/> class.
        /// </summary>
        public XLibrary()
            : base()
        {
            _items = ImmutableArray.Create<T>();
            _selected = default(T);
        }

        /// <summary>
        /// Set selected.
        /// </summary>
        /// <param name="item">The item instance.</param>
        public void SetSelected(T item) => Selected = item;

        /// <summary>
        /// Creates a new instance of the <see cref="XLibrary{T}"/> class.
        /// </summary>
        /// <param name="name">The library name.</param>
        /// <returns>The new instance of the <see cref="XLibrary{T}"/> class.</returns>
        public static XLibrary<T> Create(string name) => new XLibrary<T>() { Name = name };

        /// <summary>
        /// Creates a new instance of the <see cref="XLibrary{T}"/> class.
        /// </summary>
        /// <param name="name">The library name.</param>
        /// <param name="items">The items collection.</param>
        /// <returns>The new instance of the <see cref="XLibrary{T}"/> class.</returns>
        public static XLibrary<T> Create(string name, IEnumerable<T> items)
        {
            return new XLibrary<T>()
            {
                Name = name,
                Items = ImmutableArray.CreateRange<T>(items),
                Selected = items.FirstOrDefault()
            };
        }
    }
}
