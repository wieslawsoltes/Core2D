// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes.Interfaces;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Arc shape.
    /// </summary>
    public class ArcShape : BaseShape, IArc, ICopyable
    {
        private PointShape _point1;
        private PointShape _point2;
        private PointShape _point3;
        private PointShape _point4;

        /// <summary>
        /// Gets or sets top-left corner point.
        /// </summary>
        public PointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        /// <summary>
        /// Gets or sets bottom-right corner point.
        /// </summary>
        public PointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        /// <summary>
        /// Gets or sets point used to calculate arc start angle.
        /// </summary>
        public PointShape Point3
        {
            get => _point3;
            set => Update(ref _point3, value);
        }

        /// <summary>
        /// Gets or sets point used to calculate arc end angle.
        /// </summary>
        public PointShape Point4
        {
            get => _point4;
            set => Update(ref _point4, value);
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
                    _point4.Draw(dc, renderer, dx, dy, db, record);
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
                else if (_point4 == renderer.State.SelectedShape)
                {
                    _point4.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _point1.Draw(dc, renderer, dx, dy, db, record);
                    _point2.Draw(dc, renderer, dx, dy, db, record);
                    _point3.Draw(dc, renderer, dx, dy, db, record);
                    _point4.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            base.EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public override void Move(ISet<BaseShape> selected, double dx, double dy)
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

            if (!Point4.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point4.Move(selected, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISet<BaseShape> selected)
        {
            base.Select(selected);
            Point1.Select(selected);
            Point2.Select(selected);
            Point3.Select(selected);
            Point4.Select(selected);
        }

        /// <inheritdoc/>
        public override void Deselect(ISet<BaseShape> selected)
        {
            base.Deselect(selected);
            Point1.Deselect(selected);
            Point2.Deselect(selected);
            Point3.Deselect(selected);
            Point4.Deselect(selected);
        }

        /// <inheritdoc/>
        public override IEnumerable<PointShape> GetPoints()
        {
            yield return Point1;
            yield return Point2;
            yield return Point3;
            yield return Point4;
        }

        /// <inheritdoc/>
        public virtual object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="ArcShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ArcShape.Point1"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ArcShape.Point1"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="ArcShape.Point2"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ArcShape.Point2"/> point.</param>
        /// <param name="x3">The X coordinate of <see cref="ArcShape.Point3"/> point.</param>
        /// <param name="y3">The Y coordinate of <see cref="ArcShape.Point3"/> point.</param>
        /// <param name="x4">The X coordinate of <see cref="ArcShape.Point4"/> point.</param>
        /// <param name="y4">The Y coordinate of <see cref="ArcShape.Point4"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ArcShape"/> class.</returns>
        public static ArcShape Create(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ShapeStyle style, BaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new ArcShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Point1 = PointShape.Create(x1, y1, point),
                Point2 = PointShape.Create(x2, y2, point),
                Point3 = PointShape.Create(x3, y3, point),
                Point4 = PointShape.Create(x4, y4, point)
            };
        }

        /// <summary>
        /// Creates a new <see cref="ArcShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ArcShape.Point1"/>, <see cref="ArcShape.Point2"/>, <see cref="ArcShape.Point3"/> and <see cref="ArcShape.Point4"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="ArcShape.Point1"/>, <see cref="ArcShape.Point2"/>, <see cref="ArcShape.Point3"/> and <see cref="ArcShape.Point4"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ArcShape"/> class.</returns>
        public static ArcShape Create(double x, double y, ShapeStyle style, BaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return Create(x, y, x, y, x, y, x, y, style, point, isStroked, isFilled, name);
        }

        /// <summary>
        /// Creates a new <see cref="ArcShape"/> instance.
        /// </summary>
        /// <param name="point1">The <see cref="ArcShape.Point1"/> point.</param>
        /// <param name="point2">The <see cref="ArcShape.Point2"/> point.</param>
        /// <param name="point3">The <see cref="ArcShape.Point3"/> point.</param>
        /// <param name="point4">The <see cref="ArcShape.Point4"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="ArcShape"/> class.</returns>
        public static ArcShape Create(PointShape point1, PointShape point2, PointShape point3, PointShape point4, ShapeStyle style, BaseShape point, bool isStroked = true, bool isFilled = false, string name = "")
        {
            return new ArcShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Point1 = point1,
                Point2 = point2,
                Point3 = point3,
                Point4 = point4
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

        /// <summary>
        /// Check whether the <see cref="Point4"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint4() => _point4 != null;
    }
}
