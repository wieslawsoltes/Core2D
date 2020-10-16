
using System;
using System.Collections.Generic;

namespace Core2D.Editor.Recent
{
    public class RecentFile : ObservableObject
    {
        private string _path;

        public string Path
        {
            get => _path;
            set => RaiseAndSetIfChanged(ref _path, value);
        }

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
