// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Shapes.Interfaces
{
    /// <summary>
    /// Defines quadratic bezier shape contract.
    /// </summary>
    public interface IQuadraticBezier
    {
        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        PointShape Point1 { get; set; }

        /// <summary>
        /// Gets or sets control point.
        /// </summary>
        PointShape Point2 { get; set; }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        PointShape Point3 { get; set; }
    }
}
