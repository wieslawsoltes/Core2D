using System;
using System.Collections.Generic;
using Core2D.Path;
using Core2D.Scripting;

namespace Core2D.Scripting
{
    /// <summary>
    /// Script.
    /// </summary>
    public class Script : ObservableObject
    {
        private string _code;

        /// <summary>
        /// Gets or sets script code.
        /// </summary>
        public string Code
        {
            get => _code;
            set => Update(ref _code, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the <see cref="Code"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCode() => _code != default;
    }
}
