// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Group shape.
    /// </summary>
    public class GroupShape : ConnectableShape, IGroupShape
    {
        private ImmutableArray<IProperty> _shapesProperties;
        private ImmutableArray<IBaseShape> _shapes;

        /// <inheritdoc/>
        public override Type TargetType => typeof(IGroupShape);

        /// <inheritdoc/>
        public ImmutableArray<IProperty> ShapesProperties => GetShapeProperties();

        /// <inheritdoc/>
        public ImmutableArray<IBaseShape> Shapes
        {
            get => _shapes;
            set
            {
                if (Update(ref _shapes, value))
                {
                    _shapesProperties = default;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupShape"/> class.
        /// </summary>
        public GroupShape()
            : base()
        {
            _shapes = ImmutableArray.Create<IBaseShape>();
        }

        private ImmutableArray<IProperty> GetShapeProperties()
        {
            if (_shapesProperties == null)
            {
                if (_shapes != null)
                {
                    var builder = ImmutableArray.CreateBuilder<IProperty>();

                    foreach (var shape in _shapes)
                    {
                        foreach (var property in shape.Data.Properties)
                        {
                            builder.Add(property);
                        }
                    }

                    foreach (var connector in base.Connectors)
                    {
                        foreach (var property in connector.Data.Properties)
                        {
                            builder.Add(property);
                        }
                    }

                    _shapesProperties = builder.ToImmutable();
                }
            }
            return _shapesProperties;
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var state = base.BeginTransform(dc, renderer);

            var record = Data?.Record ?? r;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                foreach (var shape in Shapes)
                {
                    shape.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            base.Draw(dc, renderer, dx, dy, db, record);

            base.EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public override void Move(ISet<IBaseShape> selected, double dx, double dy)
        {
            foreach (var shape in Shapes)
            {
                if (!shape.State.Flags.HasFlag(ShapeStateFlags.Connector))
                {
                    shape.Move(selected, dx, dy);
                }
            }

            base.Move(selected, dx, dy);
        }

        /// <inheritdoc/>
        public override void Select(ISet<IBaseShape> selected)
        {
            base.Select(selected);
        }

        /// <inheritdoc/>
        public override void Deselect(ISet<IBaseShape> selected)
        {
            base.Deselect(selected);
        }

        /// <inheritdoc/>
        public override IEnumerable<IPointShape> GetPoints()
        {
            return Enumerable.Concat(Shapes.SelectMany(s => s.GetPoints()), base.GetPoints());
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="GroupShape"/> instance.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <returns>The new instance of the <see cref="GroupShape"/> class.</returns>
        public static IGroupShape Create(string name)
        {
            return new GroupShape()
            {
                Name = name
            };
        }

        /// <summary>
        /// Check whether the <see cref="Shapes"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeShapes() => _shapes.IsEmpty == false;
    }
}
