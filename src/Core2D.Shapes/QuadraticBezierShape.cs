// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Quadratic bezier shape.
    /// </summary>
    public class QuadraticBezierShape : BaseShape, IQuadraticBezierShape
    {
        private IPointShape _point1;
        private IPointShape _point2;
        private IPointShape _point3;

        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        public IPointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        /// <summary>
        /// Gets or sets control point.
        /// </summary>
        public IPointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        public IPointShape Point3
        {
            get => _point3;
            set => Update(ref _point3, value);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var state = base.BeginTransform(dc, renderer);

            var record = Data?.Record ?? r;

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
        public override void Move(ISet<IBaseShape> selected, double dx, double dy)
        {
            if (!Point1.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point1.Move(selected, dx, dy);
            }

            if (!Point2.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point2.Move(selected, dx, dy);
            }

            if (!Point3.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point3.Move(selected, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISet<IBaseShape> selected)
        {
            base.Select(selected);
            Point1.Select(selected);
            Point2.Select(selected);
            Point3.Select(selected);
        }

        /// <inheritdoc/>
        public override void Deselect(ISet<IBaseShape> selected)
        {
            base.Deselect(selected);
            Point1.Deselect(selected);
            Point2.Deselect(selected);
            Point3.Deselect(selected);
        }

        /// <inheritdoc/>
        public override IEnumerable<IPointShape> GetPoints()
        {
            yield return Point1;
            yield return Point2;
            yield return Point3;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="QuadraticBezierShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="IQuadraticBezierShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="IQuadraticBezierShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="IQuadraticBezierShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="IQuadraticBezierShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="IQuadraticBezierShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="IQuadraticBezierShape.Point3"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierShape"/> class.</returns>
        public static QuadraticBezierShape Create(double x1, double y1, double x2, double y2, double x3, double y3, ShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new QuadraticBezierShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Point1 = PointShape.Create(x1, y1, point),
                Point2 = PointShape.Create(x2, y2, point),
                Point3 = PointShape.Create(x3, y3, point)
            };
        }

        /// <summary>
        /// Creates a new <see cref="QuadraticBezierShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="IQuadraticBezierShape.Point1"/>, <see cref="IQuadraticBezierShape.Point2"/> and <see cref="IQuadraticBezierShape.Point3"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="IQuadraticBezierShape.Point1"/>, <see cref="IQuadraticBezierShape.Point2"/> and <see cref="IQuadraticBezierShape.Point3"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierShape"/> class.</returns>
        public static QuadraticBezierShape Create(double x, double y, ShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return Create(x, y, x, y, x, y, style, point, isStroked, isFilled, name);
        }

        /// <summary>
        /// Creates a new <see cref="QuadraticBezierShape"/> instance.
        /// </summary>
        /// <param name="point1">The <see cref="IQuadraticBezierShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="IQuadraticBezierShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="IQuadraticBezierShape.Point3"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="QuadraticBezierShape"/> class.</returns>
        public static QuadraticBezierShape Create(IPointShape point1, IPointShape point2, IPointShape point3, ShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new QuadraticBezierShape()
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

        /// <summary>
        /// Check whether the <see cref="Point1"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint1() => _point1 != null;

        /// <summary>
        /// Check whether the <see cref="Point2"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint2() => _point2 != null;

        /// <summary>
        /// Check whether the <see cref="Point3"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint3() => _point3 != null;
    }
}
