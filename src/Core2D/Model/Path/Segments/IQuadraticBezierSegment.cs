// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Defines quadratic bezier path segment contract.
    /// </summary>
    public interface IQuadraticBezierSegment : IPathSegment
    {
        /// <summary>
        /// Gets or sets control point.
        /// </summary>
        IPointShape Point1 { get; set; }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        IPointShape Point2 { get; set; }
    }
}
