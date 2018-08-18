// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// Geometry context.
    /// </summary>
    public abstract class GeometryContext
    {
        /// <summary>
        /// Begins path figure.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="isFilled">The flag indicating whether figure is filled.</param>
        /// <param name="isClosed">The flag indicating whether figure is closed.</param>
        public abstract void BeginFigure(IPointShape startPoint, bool isFilled = true, bool isClosed = true);

        /// <summary>
        /// Sets the current closed state of the figure. 
        /// </summary>
        /// <param name="isClosed">The flag indicating whether figure is closed.</param>
        public abstract void SetClosedState(bool isClosed);

        /// <summary>
        /// Adds line segment.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public abstract void LineTo(IPointShape point, bool isStroked = true, bool isSmoothJoin = true);

        /// <summary>
        /// Adds arc segment.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="size">The arc size.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <param name="isLargeArc">The is large flag.</param>
        /// <param name="sweepDirection">The sweep direction flag.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public abstract void ArcTo(IPointShape point, IPathSize size, double rotationAngle, bool isLargeArc = false, SweepDirection sweepDirection = SweepDirection.Clockwise, bool isStroked = true, bool isSmoothJoin = true);

        /// <summary>
        /// Adds cubic bezier segment.
        /// </summary>
        /// <param name="point1">The first control point.</param>
        /// <param name="point2">The second control point.</param>
        /// <param name="point3">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public abstract void CubicBezierTo(IPointShape point1, IPointShape point2, IPointShape point3, bool isStroked = true, bool isSmoothJoin = true);

        /// <summary>
        /// Adds quadratic bezier segment.
        /// </summary>
        /// <param name="point1">The control point.</param>
        /// <param name="point2">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public abstract void QuadraticBezierTo(IPointShape point1, IPointShape point2, bool isStroked = true, bool isSmoothJoin = true);

        /// <summary>
        /// Adds poly line segment.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public abstract void PolyLineTo(ImmutableArray<IPointShape> points, bool isStroked = true, bool isSmoothJoin = true);

        /// <summary>
        /// Adds poly cubic bezier segment.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public abstract void PolyCubicBezierTo(ImmutableArray<IPointShape> points, bool isStroked = true, bool isSmoothJoin = true);

        /// <summary>
        /// Adds poly quadratic bezier segment.
        /// </summary>
        /// <param name="points">The points array.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        public abstract void PolyQuadraticBezierTo(ImmutableArray<IPointShape> points, bool isStroked = true, bool isSmoothJoin = true);
    }
}
