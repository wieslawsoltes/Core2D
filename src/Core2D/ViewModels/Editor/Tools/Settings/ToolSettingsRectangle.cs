using System;
using System.Collections.Generic;

namespace Core2D.Editor.Tools.Settings
{
    /// <summary>
    /// Rectangle tool settings.
    /// </summary>
    public class ToolSettingsRectangle : ObservableObject, ISettings
    {
        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
