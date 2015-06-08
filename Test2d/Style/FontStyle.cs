// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// Specifies style information applied to text.
    /// </summary>
    [Flags]
    public enum FontStyle
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
