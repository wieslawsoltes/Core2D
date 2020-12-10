using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.ViewModels.Editor.Recent
{
    public partial class RecentsViewModel : ViewModelBase
    {
        [AutoNotify] private ImmutableArray<RecentFileViewModel> _files = ImmutableArray.Create<RecentFileViewModel>();
        [AutoNotify] private RecentFileViewModel _current = default;

        public static RecentsViewModel Create(ImmutableArray<RecentFileViewModel> files, RecentFileViewModel current)
        {
            return new RecentsViewModel()
            {
                Files = files,
                Current = current
            };
        }
    }
}
