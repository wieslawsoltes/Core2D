using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Containers
{
    public class Library<T> : Library
    {
        private ImmutableArray<T> _items;
        private T _selected;

        public ImmutableArray<T> Items
        {
            get => _items;
            set => RaiseAndSetIfChanged(ref _items, value);
        }

        public T Selected
        {
            get => _selected;
            set => RaiseAndSetIfChanged(ref _selected, value);
        }

        public void SetSelected(T item) => Selected = item;

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

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

        public virtual bool ShouldSerializeItems() => true;

        public virtual bool ShouldSerializeSelected() => _selected != null;
    }
}
