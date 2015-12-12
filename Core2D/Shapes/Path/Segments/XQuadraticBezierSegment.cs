// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Quadratic bezier path segment.
    /// </summary>
    public class XQuadraticBezierSegment : XPathSegment
    {
        /// <summary>
        /// Gets or sets control point.
        /// </summary>
        public XPoint Point1 { get; set; }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        public XPoint Point2 { get; set; }

        /// <summary>
        /// Creates a new <see cref="XQuadraticBezierSegment"/> instance.
        /// </summary>
        /// <param name="point1">The control point.</param>
        /// <param name="point2">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="XQuadraticBezierSegment"/> class.</returns>
        public static XQuadraticBezierSegment Create(
            XPoint point1,
            XPoint point2,
            bool isStroked,
            bool isSmoothJoin)
        {
            return new XQuadraticBezierSegment()
            {
                Point1 = point1,
                Point2 = point2,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
