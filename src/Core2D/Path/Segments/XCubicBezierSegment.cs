// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Cubic bezier path segment.
    /// </summary>
    public class XCubicBezierSegment : XPathSegment
    {
        private XPoint _point1;
        private XPoint _point2;
        private XPoint _point3;

        /// <summary>
        /// Gets or sets first control point.
        /// </summary>
        public XPoint Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        /// <summary>
        /// Gets or sets second control point.
        /// </summary>
        public XPoint Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        public XPoint Point3
        {
            get => _point3;
            set => Update(ref _point3, value);
        }

        /// <inheritdoc/>
        public override IEnumerable<XPoint> GetPoints()
        {
            yield return Point1;
            yield return Point2;
            yield return Point3;
        }

        /// <summary>
        /// Creates a new <see cref="XCubicBezierSegment"/> instance.
        /// </summary>
        /// <param name="point1">The first control point.</param>
        /// <param name="point2">The second control point.</param>
        /// <param name="point3">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="XCubicBezierSegment"/> class.</returns>
        public static XCubicBezierSegment Create(XPoint point1, XPoint point2, XPoint point3, bool isStroked, bool isSmoothJoin)
        {
            return new XCubicBezierSegment()
            {
                Point1 = point1,
                Point2 = point2,
                Point3 = point3,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("C{1}{0}{2}{0}{2}", " ", Point1, Point2, Point3);
        }
    }
}
