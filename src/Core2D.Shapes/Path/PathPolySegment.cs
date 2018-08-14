// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Core2D.Attributes;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// <see cref="PathFigure"/> poly segment base class.
    /// </summary>
    public abstract class PathPolySegment : PathSegment, IPathPolySegment
    {
        private ImmutableArray<IPointShape> _points;

        /// <inheritdoc/>
        [Content]
        public ImmutableArray<IPointShape> Points
        {
            get => _points;
            set => Update(ref _points, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathPolySegment"/> class.
        /// </summary>
        public PathPolySegment() : base() => Points = ImmutableArray.Create<IPointShape>();

        /// <inheritdoc/>
        public override IEnumerable<IPointShape> GetPoints() => Points;

        /// <summary>
        /// Creates a string representation of points collection.
        /// </summary>
        /// <param name="points">The points collection.</param>
        /// <returns>A string representation of points collection.</returns>
        public string ToString(ImmutableArray<IPointShape> points)
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
        public virtual bool ShouldSerializePoints() => _points.IsEmpty == false;
    }
}
