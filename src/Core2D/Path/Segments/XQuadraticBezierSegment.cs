// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Quadratic bezier path segment.
    /// </summary>
    public class XQuadraticBezierSegment : XPathSegment
    {
        private XPoint _point1;
        private XPoint _point2;

        /// <summary>
        /// Gets or sets control point.
        /// </summary>
        public XPoint Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        public XPoint Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        /// <inheritdoc/>
        public override IEnumerable<XPoint> GetPoints()
        {
            yield return Point1;
            yield return Point2;
        }

        /// <summary>
        /// Creates a new <see cref="XQuadraticBezierSegment"/> instance.
        /// </summary>
        /// <param name="point1">The control point.</param>
        /// <param name="point2">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="XQuadraticBezierSegment"/> class.</returns>
        public static XQuadraticBezierSegment Create(XPoint point1, XPoint point2, bool isStroked, bool isSmoothJoin)
        {
            return new XQuadraticBezierSegment()
            {
                Point1 = point1,
                Point2 = point2,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <inheritdoc/>
        public override string ToString() => string.Format("Q{1}{0}{2}", " ", Point1, Point2);

        /// <summary>
        /// Check whether the <see cref="Point1"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializePoint1() => _point1 != null;

        /// <summary>
        /// Check whether the <see cref="Point2"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializePoint2() => _point2 != null;
    }
}
