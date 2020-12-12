using System;
using System.Collections.Immutable;

namespace Core2D.ViewModels.Containers
{
    public partial class LibraryViewModel<T> : ViewModelBase
    {
        [AutoNotify] private ImmutableArray<T> _items;
        [AutoNotify] private T _selected;

        public LibraryViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void SetSelected(T item) => Selected = item;

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
