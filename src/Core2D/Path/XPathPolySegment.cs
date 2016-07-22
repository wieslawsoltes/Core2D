// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Text;
using Core2D.Attributes;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// <see cref="XPathFigure"/> poly segment base class.
    /// </summary>
    public abstract class XPathPolySegment : XPathSegment
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
        /// Initializes a new instance of the <see cref="XPathPolySegment"/> class.
        /// </summary>
        public XPathPolySegment()
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
        /// Creates a string representation of points collection.
        /// </summary>
        /// <param name="points">The points collection.</param>
        /// <returns>A string representation of points collection.</returns>
        public string ToString(IList<XPoint> points)
        {
            if (points?.Count == 0)
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
