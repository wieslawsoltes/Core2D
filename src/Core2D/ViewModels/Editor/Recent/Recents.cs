using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace Core2D.Editor.Recent
{
    [DataContract(IsReference = true)]
    public class Recents : ObservableObject
    {
        private ImmutableArray<RecentFile> _files = ImmutableArray.Create<RecentFile>();
        private RecentFile _current = default;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ImmutableArray<RecentFile> Files
        {
            get => _files;
            set => RaiseAndSetIfChanged(ref _files, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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
