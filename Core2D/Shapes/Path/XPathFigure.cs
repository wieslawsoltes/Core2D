// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// Path figure.
    /// </summary>
    public class XPathFigure
    {
        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        public XPoint StartPoint { get; set; }

        /// <summary>
        /// Gets or sets segments collection.
        /// </summary>
        public IList<XPathSegment> Segments { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether path is filled.
        /// </summary>
        public bool IsFilled { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether path is closed.
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        /// Get all points in the figure.
        /// </summary>
        /// <returns>All points in the figure.</returns>
        public IEnumerable<XPoint> GetPoints()
        {
            yield return StartPoint;
            
            foreach (var point in Segments.SelectMany(s => s.GetPoints()))
            {
                yield return point;
            }
        }

        /// <summary>
        /// Creates a new <see cref="XPathFigure"/> instance.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="segments">The segments collection.</param>
        /// <param name="isFilled">The flag indicating whether path is filled.</param>
        /// <param name="isClosed">The flag indicating whether path is closed.</param>
        /// <returns>The new instance of the <see cref="XPathFigure"/> class.</returns>
        public static XPathFigure Create(XPoint startPoint, IList<XPathSegment> segments, bool isFilled, bool isClosed)
        {
            return new XPathFigure()
            {
                StartPoint = startPoint,
                Segments = segments,
                IsFilled = isFilled,
                IsClosed = isClosed
            };
        }
    }
}
