using System;
using System.Collections.Generic;

namespace Core2D.Editor.Tools.Path.Settings
{
    /// <summary>
    /// Quadratic bezier path tool settings.
    /// </summary>
    public class PathToolSettingsQuadraticBezier : ObservableObject, ISettings
    {
        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
