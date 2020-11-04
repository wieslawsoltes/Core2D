using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Scripting
{
    [DataContract(IsReference = true)]
    public class Script : ObservableObject
    {
        private string _code;

        [IgnoreDataMember]
        public string Code
        {
            get => _code;
            set => RaiseAndSetIfChanged(ref _code, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
