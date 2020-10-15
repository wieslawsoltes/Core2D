using System;

namespace Core2D.Input
{
    /// <summary>
    /// Specifies modifier flags.
    /// </summary>
    [Flags]
    public enum ModifierFlags
    {
        /// <summary>
        /// No modifiers.
        /// </summary>
        None = 0,

        /// <summary>
        /// Alt modifier.
        /// </summary>
        Alt = 1,

        /// <summary>
        /// Control modifier.
        /// </summary>
        Control = 2,

        /// <summary>
        /// Shift modifier.
        /// </summary>
        Shift = 4
    }
}
