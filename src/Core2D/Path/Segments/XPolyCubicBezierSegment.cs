// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Attributes;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Poly cubic bezier path segment.
    /// </summary>
    public class XPolyCubicBezierSegment : XPathSegment
    {
        private IList<XPoint> _points;

        /// <summary>
        /// Gets or sets points array.
        /// </summary>
        [Content]
        public IList<XPoint> Points
        {
            get { return _points; }
            set { Update(ref _points, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPolyCubicBezierSegment"/> class.
        /// </summary>
        public XPolyCubicBezierSegment()
            : base()
        {
            Points = new List<XPoint>();
        }

        /// <inheritdoc/>
        public override IEnumerable<XPoint> GetPoints()
        {
            return Points;
        }

        /// <summary>
        /// Creates a new <see cref="XPolyCubicBezierSegment"/> instance.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="XPolyCubicBezierSegment"/> class.</returns>
        public static XPolyCubicBezierSegment Create(IList<XPoint> points, bool isStroked, bool isSmoothJoin)
        {
            return new XPolyCubicBezierSegment()
            {
                Points = points,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return (Points != null) && (Points.Count >= 1) ? "C" + ToString(Points) : "";
        }
    }
}
