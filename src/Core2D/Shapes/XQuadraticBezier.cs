// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes.Interfaces;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Quadratic bezier shape.
    /// </summary>
    public class XQuadraticBezier : BaseShape, IQuadraticBezier
    {
        private XPoint _point1;
        private XPoint _point2;
        private XPoint _point3;

        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        public XPoint Point1
        {
            get { return _point1; }
            set { Update(ref _point1, value); }
        }

        /// <summary>
        /// Gets or sets control point.
        /// </summary>
        public XPoint Point2
        {
            get { return _point2; }
            set { Update(ref _point2, value); }
        }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        public XPoint Point3
        {
            get { return _point3; }
            set { Update(ref _point3, value); }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var state = base.BeginTransform(dc, renderer);

            var record = this.Data.Record ?? r;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy, db, record);
            }

            if (renderer.State.SelectedShape != null)
            {
                if (this == renderer.State.SelectedShape)
                {
                    _point1.Draw(dc, renderer, dx, dy, db, record);
                    _point2.Draw(dc, renderer, dx, dy, db, record);
                    _point3.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_point1 == renderer.State.SelectedShape)
                {
                    _point1.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_point2 == renderer.State.SelectedShape)
                {
                    _point2.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_point3 == renderer.State.SelectedShape)
                {
                    _point3.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _point1.Draw(dc, renderer, dx, dy, db, record);
                    _point2.Draw(dc, renderer, dx, dy, db, record);
                    _point3.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            base.EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public override void Move(double dx, double dy)
        {
            if (!Point1.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point1.Move(dx, dy);
            }

            if (!Point2.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point2.Move(dx, dy);
            }

            if (!Point3.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point3.Move(dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISet<BaseShape> selected)
        {
            base.Select(selected);
            Point1.Select(selected);
            Point2.Select(selected);
            Point3.Select(selected);
        }

        /// <inheritdoc/>
        public override void Deselect(ISet<BaseShape> selected)
        {
            base.Deselect(selected);
            Point1.Deselect(selected);
            Point2.Deselect(selected);
            Point3.Deselect(selected);
        }

        /// <inheritdoc/>
        public override IEnumerable<XPoint> GetPoints()
        {
            yield return Point1;
            yield return Point2;
            yield return Point3;
        }

        /// <summary>
        /// Creates a new <see cref="XQuadraticBezier"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="XQuadraticBezier.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XQuadraticBezier.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="XQuadraticBezier.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XQuadraticBezier.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="XQuadraticBezier.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="XQuadraticBezier.Point3"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XQuadraticBezier"/> class.</returns>
        public static XQuadraticBezier Create(double x1, double y1, double x2, double y2, double x3, double y3, ShapeStyle style, BaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new XQuadraticBezier()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Point1 = XPoint.Create(x1, y1, point),
                Point2 = XPoint.Create(x2, y2, point),
                Point3 = XPoint.Create(x3, y3, point)
            };
        }

        /// <summary>
        /// Creates a new <see cref="XQuadraticBezier"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="XQuadraticBezier.Point1"/>, <see cref="XQuadraticBezier.Point2"/> and <see cref="XQuadraticBezier.Point3"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="XQuadraticBezier.Point1"/>, <see cref="XQuadraticBezier.Point2"/> and <see cref="XQuadraticBezier.Point3"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XQuadraticBezier"/> class.</returns>
        public static XQuadraticBezier Create(double x, double y, ShapeStyle style, BaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return Create(x, y, x, y, x, y, style, point, isStroked, isFilled, name);
        }

        /// <summary>
        /// Creates a new <see cref="XQuadraticBezier"/> instance.
        /// </summary>
        /// <param name="point1">The <see cref="XQuadraticBezier.Point1"/> point.</param>
        /// <param name="point2">The <see cref="XQuadraticBezier.Point2"/> point.</param>
        /// <param name="point3">The <see cref="XQuadraticBezier.Point3"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XQuadraticBezier"/> class.</returns>
        public static XQuadraticBezier Create(XPoint point1, XPoint point2, XPoint point3, ShapeStyle style, BaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new XQuadraticBezier()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Point1 = point1,
                Point2 = point2,
                Point3 = point3
            };
        }
    }
}
