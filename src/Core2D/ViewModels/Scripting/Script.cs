using System;
using System.Collections.Generic;

namespace Core2D.Scripting
{
    public partial class Script : ViewModelBase
    {
        [AutoNotify] private string _code;

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
