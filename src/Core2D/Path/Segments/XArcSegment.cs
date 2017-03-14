// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Arc path segment.
    /// </summary>
    public class XArcSegment : XPathSegment
    {
        private XPoint _point;
        private XPathSize _size;
        private double _rotationAngle;
        private bool _isLargeArc;
        private XSweepDirection _sweepDirection;

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        public XPoint Point
        {
            get => _point;
            set => Update(ref _point, value);
        }

        /// <summary>
        /// Gets or sets arc size.
        /// </summary>
        public XPathSize Size
        {
            get => _size;
            set => Update(ref _size, value);
        }

        /// <summary>
        /// Gets or sets arc rotation angle.
        /// </summary>
        public double RotationAngle
        {
            get => _rotationAngle;
            set => Update(ref _rotationAngle, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether arc is large.
        /// </summary>
        public bool IsLargeArc
        {
            get => _isLargeArc;
            set => Update(ref _isLargeArc, value);
        }

        /// <summary>
        /// Gets or sets sweep direction.
        /// </summary>
        public XSweepDirection SweepDirection
        {
            get => _sweepDirection;
            set => Update(ref _sweepDirection, value);
        }

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

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(
                     "A{1}{0}{2}{0}{3}{0}{4}{0}{5}",
                     " ",
                     Size,
                     RotationAngle,
                     IsLargeArc ? "1" : "0",
                     SweepDirection == XSweepDirection.Clockwise ? "1" : "0",
                     Point);
        }
    }
}
