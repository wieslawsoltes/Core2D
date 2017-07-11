// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Shape;

namespace Core2D.Shapes
{
    /// <summary>
    /// Group shape.
    /// </summary>
    public class GroupShape : ConnectableShape, ICopyable
    {
        private ImmutableArray<Property> _shapesProperties;
        private ImmutableArray<BaseShape> _shapes;

        /// <summary>
        /// Gets all properties from <see cref="Shapes"/> collection.
        /// </summary>
        public ImmutableArray<Property> ShapesProperties => GetShapeProperties();

        /// <summary>
        /// Gets or sets shapes collection.
        /// </summary>
        public ImmutableArray<BaseShape> Shapes
        {
            get => _shapes;
            set
            {
                if (Update(ref _shapes, value))
                {
                    _shapesProperties = default(ImmutableArray<Property>);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupShape"/> class.
        /// </summary>
        public GroupShape()
            : base()
        {
            _shapes = ImmutableArray.Create<BaseShape>();
        }

        private ImmutableArray<Property> GetShapeProperties()
        {
            if (_shapesProperties == null)
            {
                if (_shapes != null)
                {
                    var builder = ImmutableArray.CreateBuilder<Property>();

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

            var record = this.Data?.Record ?? r;

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
        public override void Move(ISet<BaseShape> selected, double dx, double dy)
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
        public override void Select(ISet<BaseShape> selected)
        {
            base.Select(selected);
        }

        /// <inheritdoc/>
        public override void Deselect(ISet<BaseShape> selected)
        {
            base.Deselect(selected);
        }

        /// <inheritdoc/>
        public override IEnumerable<PointShape> GetPoints()
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
        public static GroupShape Create(string name)
        {
            return new GroupShape()
            {
                Name = name
            };
        }

        /// <summary>
        /// Creates a new <see cref="GroupShape"/> instance.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="source">The source shapes collection.</param>
        /// <returns>The new instance of the <see cref="GroupShape"/> class.</returns>
        public static GroupShape Group(string name, IEnumerable<BaseShape> shapes, IList<BaseShape> source = null)
        {
            var group = GroupShape.Create(name);

            if (shapes != null)
            {
                foreach (var shape in shapes)
                {
                    if (shape is PointShape)
                    {
                        group.AddConnectorAsNone(shape as PointShape);
                    }
                    else
                    {
                        group.AddShape(shape);
                    }

                    if (source != null)
                    {
                        source.Remove(shape);
                    }
                }
            }

            if (source != null)
            {
                source.Add(group);
            }

            return group;
        }

        /// <summary>
        /// Ungroup shape.
        /// </summary>
        /// <param name="shape">The shape instance.</param>
        /// <param name="source">The source shapes collection.</param>
        /// <param name="isShapeFromGroup">The flag indicating whether shape originates from group.</param>
        public static void Ungroup(BaseShape shape, IList<BaseShape> source, bool isShapeFromGroup = false)
        {
            if (shape != null && source != null)
            {
                if (shape is GroupShape)
                {
                    var group = shape as GroupShape;
                    Ungroup(group.Shapes, source, isShapeFromGroup: true);
                    Ungroup(group.Connectors, source, isShapeFromGroup: true);

                    // Remove group from source collection.
                    source.Remove(group);
                }
                else if (isShapeFromGroup)
                {
                    if (shape is PointShape)
                    {
                        // Remove connector related state flags.
                        shape.State.Flags &=
                            ~(ShapeStateFlags.Connector
                            | ShapeStateFlags.None
                            | ShapeStateFlags.Input
                            | ShapeStateFlags.Output);
                    }

                    // Add shape standalone flag.
                    shape.State.Flags |= ShapeStateFlags.Standalone;

                    // Add shape to source collection.
                    source.Add(shape);
                }
            }
        }

        /// <summary>
        /// Ungroup shapes.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="source">The source shapes collection.</param>
        /// <param name="isShapeFromGroup">The flag indicating whether shapes originate from group.</param>
        public static void Ungroup(IEnumerable<BaseShape> shapes, IList<BaseShape> source, bool isShapeFromGroup = false)
        {
            if (shapes != null && source != null)
            {
                foreach (var shape in shapes)
                {
                    Ungroup(shape, source, isShapeFromGroup);
                }
            }
        }

        /// <summary>
        /// Check whether the <see cref="Shapes"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeShapes() => _shapes.IsEmpty == false;
    }
}
