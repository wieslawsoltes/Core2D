// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Shapes.Interfaces
{
    /// <summary>
    /// Defines rectangle shape contract.
    /// </summary>
    public interface IRectangleShape : ITextShape
    {
        /// <summary>
        /// Gets or sets flag indicating whether shape is grid.
        /// </summary>
        bool IsGrid { get; set; }

        /// <summary>
        /// Gets or sets grid X coordinate offset.
        /// </summary>
        double OffsetX { get; set; }

        /// <summary>
        /// Gets or sets grid Y coordinate offset.
        /// </summary>
        double OffsetY { get; set; }

        /// <summary>
        /// Gets or sets grid cell width.
        /// </summary>
        double CellWidth { get; set; }

        /// <summary>
        /// Gets or sets grid cell height.
        /// </summary>
        double CellHeight { get; set; }
    }
}
