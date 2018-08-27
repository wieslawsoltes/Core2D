// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Shapes
{
    /// <summary>
    /// Defines quadratic bezier shape contract.
    /// </summary>
    public interface IQuadraticBezierShape : IBaseShape
    {
        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        IPointShape Point1 { get; set; }

        /// <summary>
        /// Gets or sets control point.
        /// </summary>
        IPointShape Point2 { get; set; }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        IPointShape Point3 { get; set; }
    }
}
