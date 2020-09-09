using System.Collections.Generic;

namespace Core2D
{
    /// <summary>
    /// Defines copyable contract.
    /// </summary>
    public interface ICopyable
    {
        /// <summary>
        /// Copies the object.
        /// </summary>
        /// <param name="shared">The shared objects dictionary.</param>
        /// <returns>The copy of the object.</returns>
        object Copy(IDictionary<object, object>? shared);
    }
}
