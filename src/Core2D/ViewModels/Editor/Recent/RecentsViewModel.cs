#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.ViewModels.Editor.Recent
{
    public partial class RecentsViewModel : ViewModelBase
    {
        [AutoNotify] private ImmutableArray<RecentFileViewModel> _files = ImmutableArray.Create<RecentFileViewModel>();
        [AutoNotify] private RecentFileViewModel _current;

        public RecentsViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }

        public static RecentsViewModel Create(IServiceProvider serviceProvider, ImmutableArray<RecentFileViewModel> files, RecentFileViewModel current)
        {
            return new RecentsViewModel(serviceProvider)
            {
                Files = files,
                Current = current
            };
        }
    }
}
