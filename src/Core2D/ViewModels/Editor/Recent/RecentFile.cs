using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Editor.Recent
{
    [DataContract(IsReference = true)]
    public class RecentFile : ObservableObject
    {
        private string _path;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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
