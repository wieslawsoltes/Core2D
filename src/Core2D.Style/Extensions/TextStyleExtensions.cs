// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Style
{
    /// <summary>
    /// Text style extensions.
    /// </summary>
    public static class TextStyleExtensions
    {
        /// <summary>
        /// Clones text style.
        /// </summary>
        /// <param name="textStyle">The text style to clone.</param>
        /// <returns>The new instance of the <see cref="TextStyle"/> class.</returns>
        public static ITextStyle Clone(this ITextStyle textStyle)
        {
            return new TextStyle()
            {
                Name = textStyle.Name,
                FontName = textStyle.FontName,
                FontFile = textStyle.FontFile,
                FontSize = textStyle.FontSize,
                FontStyle = textStyle.FontStyle.Clone(),
                TextHAlignment = textStyle.TextHAlignment,
                TextVAlignment = textStyle.TextVAlignment
            };
        }
    }
}
