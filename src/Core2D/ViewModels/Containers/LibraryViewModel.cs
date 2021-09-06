#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.ViewModels.Containers
{
    public partial class LibraryViewModel : ViewModelBase
    {
        [AutoNotify] private ImmutableArray<ViewModelBase> _items;
        [AutoNotify] private ViewModelBase? _selected;

        public LibraryViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var items = _items.Copy(shared).ToImmutable();
            var selectedIndex = _selected is null ? -1 : _items.IndexOf(_selected);
            var selected = selectedIndex == -1 ? null : items[selectedIndex];

            return new LibraryViewModel(ServiceProvider)
            {
                Name = Name,
                Items = items,
                Selected = selected
            };
        }

        public void SetSelected(ViewModelBase item) => Selected = item;

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var item in _items)
            {
                if (item is { } viewModelBase)
                {
                    isDirty |= viewModelBase.IsDirty();
                }
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var item in _items)
            {
                if (item is { } viewModelBase)
                {
                    viewModelBase.Invalidate();
                }
            }
        }
    }
}
