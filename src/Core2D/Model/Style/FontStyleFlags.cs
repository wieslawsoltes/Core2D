using System;

namespace Core2D.Style
{
    /// <summary>
    /// Specifies style flags information applied to text.
    /// </summary>
    [Flags]
    public enum FontStyleFlags
    {
        /// <summary>
        /// Normal text.
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Bold text.
        /// </summary>
        Bold = 1,

        /// <summary>
        /// Italic text.
        /// </summary>
        Italic = 2
    }
}
