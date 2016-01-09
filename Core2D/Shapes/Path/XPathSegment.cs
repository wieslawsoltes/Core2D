// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Text;

namespace Core2D
{
    /// <summary>
    /// Base class for <see cref="XPathFigure"/> segments.
    /// </summary>
    public abstract class XPathSegment
    {
        /// <summary>
        /// Gets or sets flag indicating whether segment is stroked.
        /// </summary>
        public bool IsStroked { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether segment is smooth join.
        /// </summary>
        public bool IsSmoothJoin { get; set; }

        /// <summary>
        /// Get all points in the segment.
        /// </summary>
        /// <returns>All points in the segment.</returns>
        public abstract IEnumerable<XPoint> GetPoints();

        /// <summary>
        /// Creates a string representation of points collection.
        /// </summary>
        /// <param name="points">The points collection.</param>
        /// <returns>A string representation of points collection.</returns>
        public string ToString(IList<XPoint> points)
        {
            if (points.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < points.Count; i++)
            {
                sb.Append(points[i]);
                if (i != points.Count - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }
    }
}
