// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Arc path segment.
    /// </summary>
    public class ArcSegment : PathSegment, IArcSegment
    {
        private IPointShape _point;
        private IPathSize _size;
        private double _rotationAngle;
        private bool _isLargeArc;
        private SweepDirection _sweepDirection;

        /// <inheritdoc/>
        public IPointShape Point
        {
            get => _point;
            set => Update(ref _point, value);
        }

        /// <inheritdoc/>
        public IPathSize Size
        {
            get => _size;
            set => Update(ref _size, value);
        }

        /// <inheritdoc/>
        public double RotationAngle
        {
            get => _rotationAngle;
            set => Update(ref _rotationAngle, value);
        }

        /// <inheritdoc/>
        public bool IsLargeArc
        {
            get => _isLargeArc;
            set => Update(ref _isLargeArc, value);
        }

        /// <inheritdoc/>
        public SweepDirection SweepDirection
        {
            get => _sweepDirection;
            set => Update(ref _sweepDirection, value);
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
        /// Creates a new <see cref="ArcSegment"/> instance.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="size">The arc size.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <param name="isLargeArc">The is large flag.</param>
        /// <param name="sweepDirection">The sweep direction flag.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="ArcSegment"/> class.</returns>
        public static ArcSegment Create(IPointShape point, IPathSize size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked, bool isSmoothJoin)
        {
            return new ArcSegment()
            {
                Point = point,
                Size = size,
                RotationAngle = rotationAngle,
                IsLargeArc = isLargeArc,
                SweepDirection = sweepDirection,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(
                     "A{1}{0}{2}{0}{3}{0}{4}{0}{5}",
                     " ",
                     Size,
                     RotationAngle,
                     IsLargeArc ? "1" : "0",
                     SweepDirection == SweepDirection.Clockwise ? "1" : "0",
                     Point);
        }

        /// <summary>
        /// Check whether the <see cref="Point"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint() => _point != null;

        /// <summary>
        /// Check whether the <see cref="Size"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSize() => _size != null;

        /// <summary>
        /// Check whether the <see cref="RotationAngle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeRotationAngle() => _rotationAngle != default;

        /// <summary>
        /// Check whether the <see cref="IsLargeArc"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsLargeArc() => _isLargeArc != default;

        /// <summary>
        /// Check whether the <see cref="SweepDirection"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSweepDirection() => _sweepDirection != default;
    }
}
