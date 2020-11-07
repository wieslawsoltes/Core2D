using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace Core2D.Containers
{
    [DataContract(IsReference = true)]
    public class Library<T> : Library
    {
        private ImmutableArray<T> _items;
        private T _selected;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ImmutableArray<T> Items
        {
            get => _items;
            set => RaiseAndSetIfChanged(ref _items, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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
                if (item is ViewModelBase viewModelBase)
                {
                    isDirty |= viewModelBase.IsDirty();
                }
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var item in Items)
            {
                if (item is ViewModelBase viewModelBase)
                {
                    viewModelBase.Invalidate();
                }
            }
        }
    }
}
