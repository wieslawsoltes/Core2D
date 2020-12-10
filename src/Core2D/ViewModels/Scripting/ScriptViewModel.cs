using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Scripting
{
    public partial class ScriptViewModel : ViewModelBase
    {
        [AutoNotify] private string _code;

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
