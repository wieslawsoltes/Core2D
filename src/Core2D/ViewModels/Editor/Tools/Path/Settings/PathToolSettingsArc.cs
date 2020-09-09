using System;
using System.Collections.Generic;

namespace Core2D.Editor.Tools.Path.Settings
{
    /// <summary>
    /// Arc path tool settings.
    /// </summary>
    public class PathToolSettingsArc : ObservableObject, ISettings
    {
        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }
    }
}
