using System;

namespace Core2D.Style
{
    /// <summary>
    /// Specifies line fixed length flags.
    /// </summary>
    [Flags]
    public enum LineFixedLengthFlags
    {
        /// <summary>
        /// Fixed length is disabled.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Line start point state trigger.
        /// </summary>
        Start = 1,

        /// <summary>
        /// Line end point state trigger.
        /// </summary>
        End = 2,

        /// <summary>
        /// Line must be vertical.
        /// </summary>
        Vertical = 4,

        /// <summary>
        /// Line must be horizontal.
        /// </summary>
        Horizontal = 8,

        /// <summary>
        /// All possible line start and end position are allowed.
        /// </summary>
        All = 16
    }
}
