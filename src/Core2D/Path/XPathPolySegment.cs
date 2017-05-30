// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
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
        private ImmutableArray<XPoint> _points;

        /// <summary>
        /// Gets or sets points array.
        /// </summary>
        [Content]
        public ImmutableArray<XPoint> Points
        {
            get => _points;
            set => Update(ref _points, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPathPolySegment"/> class.
        /// </summary>
        public XPathPolySegment() : base() => Points = ImmutableArray.Create<XPoint>();

        /// <inheritdoc/>
        public override IEnumerable<XPoint> GetPoints() => Points;

        /// <summary>
        /// Creates a string representation of points collection.
        /// </summary>
        /// <param name="points">The points collection.</param>
        /// <returns>A string representation of points collection.</returns>
        public string ToString(ImmutableArray<XPoint> points)
        {
            if (points.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < points.Length; i++)
            {
                sb.Append(points[i]);
                if (i != points.Length - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Check whether the <see cref="Points"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializePoints() => _points.IsEmpty == false;
    }
}
