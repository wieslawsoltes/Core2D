using System;
using System.Collections.Generic;

namespace Core2D.Editor.Tools.Path.Settings
{
    /// <summary>
    /// Line path tool settings.
    /// </summary>
    public class PathToolSettingsLine : ObservableObject, ISettings
    {
        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
