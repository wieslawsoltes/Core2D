// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Defines arc path segment contract.
    /// </summary>
    public interface IArcSegment : IPathSegment
    {
        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        IPointShape Point { get; set; }

        /// <summary>
        /// Gets or sets arc size.
        /// </summary>
        IPathSize Size { get; set; }

        /// <summary>
        /// Gets or sets arc rotation angle.
        /// </summary>
        double RotationAngle { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether arc is large.
        /// </summary>
        bool IsLargeArc { get; set; }

        /// <summary>
        /// Gets or sets sweep direction.
        /// </summary>
        SweepDirection SweepDirection { get; set; }
    }
}
