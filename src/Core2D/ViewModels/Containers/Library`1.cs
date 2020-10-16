using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Containers
{
    /// <summary>
    /// Named items library.
    /// </summary>
    public class Library<T> : Library
    {
        private ImmutableArray<T> _items;
        private T _selected;

        /// <summary>
        /// Gets or sets a items collection.
        /// </summary>
        public ImmutableArray<T> Items
        {
            get => _items;
            set => RaiseAndSetIfChanged(ref _items, value);
        }

        /// <summary>
        /// Gets or sets currently selected item from <see cref="Items"/> collection.
        /// </summary>
        public T Selected
        {
            get => _selected;
            set => RaiseAndSetIfChanged(ref _selected, value);
        }

        /// <summary>
        /// Set selected.
        /// </summary>
        /// <param name="item">The item instance.</param>
        public void SetSelected(T item) => Selected = item;

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var item in Items)
            {
                if (item is ObservableObject observableObject)
                {
                    isDirty |= observableObject.IsDirty();
                }
            }

            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var item in Items)
            {
                if (item is ObservableObject observableObject)
                {
                    observableObject.Invalidate();
                }
            }
        }

        /// <summary>
        /// Check whether the <see cref="Items"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeItems() => true;

        /// <summary>
        /// Check whether the <see cref="Selected"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSelected() => _selected != null;
    }
}
