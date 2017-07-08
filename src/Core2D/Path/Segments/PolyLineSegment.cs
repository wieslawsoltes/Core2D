// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Poly line path segment.
    /// </summary>
    public class PolyLineSegment : PathPolySegment, ICopyable
    {
        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="PolyLineSegment"/> instance.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="PolyLineSegment"/> class.</returns>
        public static PolyLineSegment Create(ImmutableArray<PointShape> points, bool isStroked, bool isSmoothJoin)
        {
            return new PolyLineSegment()
            {
                Points = points,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <inheritdoc/>
        public override string ToString() => (Points != null) && (Points.Length >= 1) ? "L" + ToString(Points) : "";
    }
}
