// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Line path segment.
    /// </summary>
    public class LineSegment : PathSegment, ICopyable
    {
        private IPointShape _point;

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        public IPointShape Point
        {
            get => _point;
            set => Update(ref _point, value);
        }

        /// <inheritdoc/>
        public override IEnumerable<IPointShape> GetPoints()
        {
            yield return Point;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="LineSegment"/> instance.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="LineSegment"/> class.</returns>
        public static LineSegment Create(IPointShape point, bool isStroked, bool isSmoothJoin)
        {
            return new LineSegment()
            {
                Point = point,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <inheritdoc/>
        public override string ToString() => "L" + Point.ToString();

        /// <summary>
        /// Check whether the <see cref="Point"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint() => _point != null;
    }
}
