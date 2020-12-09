using System;
using System.Collections.Generic;

namespace Core2D.Editor.Recent
{
    public partial class RecentFileViewModel : ViewModelBase
    {
        [AutoNotify] private string _path;

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public static RecentFileViewModel Create(string name, string path)
        {
            return new RecentFileViewModel()
            {
                Name = name,
                Path = path
            };
        }
    }
}
