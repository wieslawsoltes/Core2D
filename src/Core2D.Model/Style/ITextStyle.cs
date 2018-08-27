// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Style
{
    /// <summary>
    /// Define text style contract.
    /// </summary>
    public interface ITextStyle : IObservableObject
    {
        /// <summary>
        /// Gets or sets font name.
        /// </summary>
        string FontName { get; set; }

        /// <summary>
        /// Gets or sets font file path.
        /// </summary>
        string FontFile { get; set; }

        /// <summary>
        /// Gets or sets font size.
        /// </summary>
        double FontSize { get; set; }

        /// <summary>
        /// Gets or sets font style.
        /// </summary>
        IFontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets text horizontal alignment.
        /// </summary>
        TextHAlignment TextHAlignment { get; set; }

        /// <summary>
        /// Gets or sets text vertical alignment.
        /// </summary>
        TextVAlignment TextVAlignment { get; set; }
    }
}
