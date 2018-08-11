// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes.Interfaces;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Line shape.
    /// </summary>
    public class LineShape : BaseShape, ILineShape
    {
        private IPointShape _start;
        private IPointShape _end;

        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        public IPointShape Start
        {
            get => _start;
            set => Update(ref _start, value);
        }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        public IPointShape End
        {
            get => _end;
            set => Update(ref _end, value);
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
                    _start.Draw(dc, renderer, dx, dy, db, record);
                    _end.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_start == renderer.State.SelectedShape)
                {
                    _start.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_end == renderer.State.SelectedShape)
                {
                    _end.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _start.Draw(dc, renderer, dx, dy, db, record);
                    _end.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            base.EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public override void Move(ISet<IBaseShape> selected, double dx, double dy)
        {
            if (!Start.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Start.Move(selected, dx, dy);
            }

            if (!End.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                End.Move(selected, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISet<IBaseShape> selected)
        {
            base.Select(selected);
            Start.Select(selected);
            End.Select(selected);
        }

        /// <inheritdoc/>
        public override void Deselect(ISet<IBaseShape> selected)
        {
            base.Deselect(selected);
            Start.Deselect(selected);
            End.Deselect(selected);
        }

        /// <inheritdoc/>
        public override IEnumerable<IPointShape> GetPoints()
        {
            yield return Start;
            yield return End;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="LineShape"/> instance.
        /// </summary>
        /// <param name="start">The <see cref="LineShape.Start"/> point.</param>
        /// <param name="end">The <see cref="LineShape.End"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="LineShape"/> class.</returns>
        public static LineShape Create(PointShape start, PointShape end, ShapeStyle style, BaseShape point, bool isStroked = true, string name = "")
        {
            return new LineShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = false,
                Start = start,
                End = end
            };
        }

        /// <summary>
        /// Creates a new <see cref="LineShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="LineShape.Start"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="LineShape.Start"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="LineShape.End"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="LineShape.End"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="LineShape"/> class.</returns>
        public static LineShape Create(double x1, double y1, double x2, double y2, ShapeStyle style, BaseShape point, bool isStroked = true, string name = "")
        {
            return new LineShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = false,
                Start = PointShape.Create(x1, y1, point),
                End = PointShape.Create(x2, y2, point)
            };
        }

        /// <summary>
        /// Creates a new <see cref="LineShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="LineShape.Start"/> and <see cref="LineShape.End"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="LineShape.Start"/> and <see cref="LineShape.End"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="LineShape"/> class.</returns>
        public static LineShape Create(double x, double y, ShapeStyle style, BaseShape point, bool isStroked = true, string name = "")
        {
            return Create(x, y, x, y, style, point, isStroked, name);
        }

        /// <summary>
        /// Check whether the <see cref="Start"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeStart() => _start != null;

        /// <summary>
        /// Check whether the <see cref="End"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeEnd() => _end != null;
    }
}
