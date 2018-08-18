// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Style
{
    /// <summary>
    /// Font style extensions.
    /// </summary>
    public static class FontStyleExtensions
    {
        /// <summary>
        /// Clones font style.
        /// </summary>
        /// <param name="fontStyle">The font style to clone.</param>
        /// <returns>The new instance of the <see cref="FontStyle"/> class.</returns>
        public static IFontStyle Clone(this IFontStyle fontStyle)
        {
            return new FontStyle()
            {
                Flags = fontStyle.Flags
            };
        }
    }
}
