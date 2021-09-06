#nullable enable
using System;
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
