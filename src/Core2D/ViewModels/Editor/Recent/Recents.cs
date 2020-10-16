
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Editor.Recent
{
    public class Recents : ObservableObject
    {
        private ImmutableArray<RecentFile> _files = ImmutableArray.Create<RecentFile>();
        private RecentFile _current = default;

        public ImmutableArray<RecentFile> Files
        {
            get => _files;
            set => RaiseAndSetIfChanged(ref _files, value);
        }

        public RecentFile Current
        {
            get => _current;
            set => RaiseAndSetIfChanged(ref _current, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public static Recents Create(ImmutableArray<RecentFile> files, RecentFile current)
        {
            return new Recents()
            {
                Files = files,
                Current = current
            };
        }
    }
}
