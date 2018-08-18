// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Path;
using Core2D.Renderer;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Path shape.
    /// </summary>
    public class PathShape : BaseShape, IPathShape
    {
        private IPathGeometry _geometry;

        /// <inheritdoc/>
        public override Type TargetType => typeof(IPathShape);

        /// <inheritdoc/>
        public IPathGeometry Geometry
        {
            get => _geometry;
            set => Update(ref _geometry, value);
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
                    var points = GetPoints();
                    foreach (var point in points)
                    {
                        point.Draw(dc, renderer, dx, dy, db, record);
                    }
                }
                else
                {
                    var points = GetPoints();
                    foreach (var point in points)
                    {
                        if (point == renderer.State.SelectedShape)
                        {
                            point.Draw(dc, renderer, dx, dy, db, record);
                        }
                    }
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    var points = GetPoints();
                    foreach (var point in points)
                    {
                        point.Draw(dc, renderer, dx, dy, db, record);
                    }
                }
            }

            base.EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public override void Move(ISet<IBaseShape> selected, double dx, double dy)
        {
            var points = GetPoints();
            foreach (var point in points)
            {
                point.Move(selected, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISet<IBaseShape> selected)
        {
            base.Select(selected);

            var points = GetPoints();
            foreach (var point in points)
            {
                point.Select(selected);
            }
        }

        /// <inheritdoc/>
        public override void Deselect(ISet<IBaseShape> selected)
        {
            base.Deselect(selected);

            var points = GetPoints();
            foreach (var point in points)
            {
                point.Deselect(selected);
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<IPointShape> GetPoints()
        {
            return Geometry.Figures.SelectMany(f => f.GetPoints());
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="PathShape"/> instance.
        /// </summary>
        /// <param name="style">The shape style.</param>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="PathShape"/> class.</returns>
        public static IPathShape Create(IShapeStyle style, IPathGeometry geometry, bool isStroked = true, bool isFilled = true)
        {
            return new PathShape()
            {
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Geometry = geometry
            };
        }

        /// <summary>
        /// Creates a new <see cref="PathShape"/> instance.
        /// </summary>
        /// <param name="name">The shape name.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="PathShape"/> class.</returns>
        public static IPathShape Create(string name, IShapeStyle style, IPathGeometry geometry, bool isStroked = true, bool isFilled = true)
        {
            return new PathShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Geometry = geometry
            };
        }

        /// <summary>
        /// Check whether the <see cref="Geometry"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeGeometry() => _geometry != null;
    }
}
