// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
        Italic = 2,

        /// <summary>
        /// Underlined text.
        /// </summary>
        Underline = 4,

        /// <summary>
        /// Text with a line through the middle.
        /// </summary>
        Strikeout = 8
    }
}
