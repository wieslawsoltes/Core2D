#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.ViewModels.Editor.Recent
{
    public partial class RecentsViewModel : ViewModelBase
    {
        [AutoNotify] private ImmutableArray<RecentFileViewModel> _files = ImmutableArray.Create<RecentFileViewModel>();
        [AutoNotify] private RecentFileViewModel? _current;

        public RecentsViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var files = _files.Copy(shared).ToImmutable();
            var currentIndex = _current is null ? -1 : _files.IndexOf(_current);
            var current = currentIndex == -1 ? null : files[currentIndex];

            return new RecentsViewModel(ServiceProvider)
            {
                Name = Name,
                Files = files,
                Current = current
            };
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
