using System;
using System.Collections.Generic;
using Core2D.Path;
using Core2D.Scripting;

namespace Core2D.Scripting
{
    public class Script : ObservableObject
    {
        private string _code;

        public string Code
        {
            get => _code;
            set => RaiseAndSetIfChanged(ref _code, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public virtual bool ShouldSerializeCode() => _code != default;
    }
}
