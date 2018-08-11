// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Shapes.Interfaces
{
    /// <summary>
    /// Defines text shape contract.
    /// </summary>
    public interface ITextShape : IBaseShape
    {
        /// <summary>
        /// Gets or sets top-left corner point.
        /// </summary>
        IPointShape TopLeft { get; set; }

        /// <summary>
        /// Gets or sets bottom-right corner point.
        /// </summary>
        IPointShape BottomRight { get; set; }

        /// <summary>
        /// Gets or sets text string.
        /// </summary>
        string Text { get; set; }
    }
}
