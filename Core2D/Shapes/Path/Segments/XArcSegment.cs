// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Core2D
{
    /// <summary>
    /// Arc path segment.
    /// </summary>
    public class XArcSegment : XPathSegment
    {
        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        public XPoint Point { get; set; }

        /// <summary>
        /// Gets or sets arc size.
        /// </summary>
        public XPathSize Size { get; set; }

        /// <summary>
        /// Gets or sets arc rotation angle.
        /// </summary>
        public double RotationAngle { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether arc is large.
        /// </summary>
        public bool IsLargeArc { get; set; }

        /// <summary>
        /// Gets or sets sweep direction.
        /// </summary>
        public XSweepDirection SweepDirection { get; set; }

        /// <inheritdoc/>
        public override IEnumerable<XPoint> GetPoints()
        {
            yield return Point;
        }

        /// <summary>
        /// Creates a new <see cref="XArcSegment"/> instance.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="size">The arc size.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <param name="isLargeArc">The is large flag.</param>
        /// <param name="sweepDirection">The sweep direction flag.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="XArcSegment"/> class.</returns>
        public static XArcSegment Create(XPoint point, XPathSize size, double rotationAngle, bool isLargeArc, XSweepDirection sweepDirection, bool isStroked, bool isSmoothJoin)
        {
            return new XArcSegment()
            {
                Point = point,
                Size = size,
                RotationAngle = rotationAngle,
                IsLargeArc = isLargeArc,
                SweepDirection = sweepDirection,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
