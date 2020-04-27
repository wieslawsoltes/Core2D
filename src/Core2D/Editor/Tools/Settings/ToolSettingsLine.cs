using System;
using System.Collections.Generic;

namespace Core2D.Editor.Tools.Settings
{
    /// <summary>
    /// Line tool settings.
    /// </summary>
    public class ToolSettingsLine : ObservableObject, ISettings
    {
        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
