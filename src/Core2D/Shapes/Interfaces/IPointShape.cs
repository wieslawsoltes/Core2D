// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;

namespace Core2D.Shapes.Interfaces
{
    /// <summary>
    /// Defines point shape contract.
    /// </summary>
    public interface IPointShape : IShape
    {
        /// <summary>
        /// Gets or sets X coordinate of point.
        /// </summary>
        double X { get; set; }

        /// <summary>
        /// Gets or sets Y coordinate of point.
        /// </summary>
        double Y { get; set; }

        /// <summary>
        /// Gets or sets point alignment.
        /// </summary>
        PointAlignment Alignment { get; set; }

        /// <summary>
        /// Gets or sets point template shape.
        /// </summary>
        IShape Shape { get; set; }
    }
}
