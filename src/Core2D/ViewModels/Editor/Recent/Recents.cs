using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Editor.Recent
{
    public partial class Recents : ViewModelBase
    {
        [AutoNotify] private ImmutableArray<RecentFile> _files = ImmutableArray.Create<RecentFile>();
        [AutoNotify] private RecentFile _current = default;

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
