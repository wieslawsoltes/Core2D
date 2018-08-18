// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Style
{
    /// <summary>
    /// Define shape style contract.
    /// </summary>
    public interface IShapeStyle : IBaseStyle
    {
        /// <summary>
        /// Gets or sets line style.
        /// </summary>
        ILineStyle LineStyle { get; set; }

        /// <summary>
        /// Gets or sets start arrow style.
        /// </summary>
        IArrowStyle StartArrowStyle { get; set; }

        /// <summary>
        /// Gets or sets end arrow style.
        /// </summary>
        IArrowStyle EndArrowStyle { get; set; }

        /// <summary>
        /// Gets or sets text style.
        /// </summary>
        ITextStyle TextStyle { get; set; }
    }
}
