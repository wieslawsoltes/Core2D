// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Attributes;

namespace Core2D.Containers
{
    /// <summary>
    /// Named items library.
    /// </summary>
    public class Library<T> : ObservableObject, ILibrary<T>
    {
        private ImmutableArray<T> _items;
        private T _selected;

        /// <inheritdoc/>
        [Content]
        public ImmutableArray<T> Items
        {
            get => _items;
            set => Update(ref _items, value);
        }

        /// <inheritdoc/>
        public T Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Library{T}"/> class.
        /// </summary>
        public Library()
            : base()
        {
            _items = ImmutableArray.Create<T>();
            _selected = default;
        }

        /// <inheritdoc/>
        public void SetSelected(T item) => Selected = item;

        /// <inheritdoc/>
        public virtual object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Library{T}"/> class.
        /// </summary>
        /// <param name="name">The library name.</param>
        /// <returns>The new instance of the <see cref="Library{T}"/> class.</returns>
        public static ILibrary<T> Create(string name) => new Library<T>() { Name = name };

        /// <summary>
        /// Creates a new instance of the <see cref="Library{T}"/> class.
        /// </summary>
        /// <param name="name">The library name.</param>
        /// <param name="items">The items collection.</param>
        /// <returns>The new instance of the <see cref="Library{T}"/> class.</returns>
        public static ILibrary<T> Create(string name, IEnumerable<T> items)
        {
            return new Library<T>()
            {
                Name = name,
                Items = ImmutableArray.CreateRange<T>(items),
                Selected = items.FirstOrDefault()
            };
        }

        /// <summary>
        /// Check whether the <see cref="Items"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeItems() => _items.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="Selected"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSelected() => _selected != null;
    }
}
