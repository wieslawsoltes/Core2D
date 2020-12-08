using System;
using System.Collections.Generic;

namespace Core2D.Editor.Recent
{
    public partial class RecentFile : ViewModelBase
    {
        [AutoNotify] private string _path;

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public static RecentFile Create(string name, string path)
        {
            return new RecentFile()
            {
                Name = name,
                Path = path
            };
        }
    }
}
