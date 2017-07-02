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
    public class XGroup : BaseShape, ICopyable
    {
        private ImmutableArray<XProperty> _shapesProperties;
        private ImmutableArray<BaseShape> _shapes;
        private ImmutableArray<XPoint> _connectors;

        /// <summary>
        /// Gets all properties from <see cref="Shapes"/> collection.
        /// </summary>
        public ImmutableArray<XProperty> ShapesProperties => GetShapeProperties();

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
                    _shapesProperties = default(ImmutableArray<XProperty>);
                }
            }
        }

        /// <summary>
        /// Gets or sets connectors collection.
        /// </summary>
        public ImmutableArray<XPoint> Connectors
        {
            get => _connectors;
            set
            {
                if (Update(ref _connectors, value))
                {
                    _shapesProperties = default(ImmutableArray<XProperty>);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XGroup"/> class.
        /// </summary>
        public XGroup()
            : base()
        {
            _shapes = ImmutableArray.Create<BaseShape>();
            _connectors = ImmutableArray.Create<XPoint>();
        }

        private ImmutableArray<XProperty> GetShapeProperties()
        {
            if (_shapesProperties == null)
            {
                if (_shapes != null)
                {
                    var builder = ImmutableArray.CreateBuilder<XProperty>();

                    foreach (var shape in _shapes)
                    {
                        foreach (var property in shape.Data.Properties)
                        {
                            builder.Add(property);
                        }
                    }

                    foreach (var connector in _connectors)
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

            var record = this.Data.Record ?? r;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                foreach (var shape in Shapes)
                {
                    shape.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            if (renderer.State.SelectedShape != null)
            {
                if (this == renderer.State.SelectedShape)
                {
                    foreach (var connector in Connectors)
                    {
                        connector.Draw(dc, renderer, dx, dy, db, record);
                    }
                }
                else
                {
                    foreach (var connector in Connectors)
                    {
                        if (connector == renderer.State.SelectedShape)
                        {
                            connector.Draw(dc, renderer, dx, dy, db, record);
                        }
                    }
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    foreach (var connector in Connectors)
                    {
                        connector.Draw(dc, renderer, dx, dy, db, record);
                    }
                }
            }

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

            foreach (var connector in Connectors)
            {
                connector.Move(selected, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISet<BaseShape> selected)
        {
            base.Select(selected);

            foreach (var connector in Connectors)
            {
                connector.Select(selected);
            }
        }

        /// <inheritdoc/>
        public override void Deselect(ISet<BaseShape> selected)
        {
            base.Deselect(selected);

            foreach (var connector in Connectors)
            {
                connector.Deselect(selected);
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<XPoint> GetPoints()
        {
            return Enumerable.Concat(Shapes.SelectMany(s => s.GetPoints()), Connectors);
        }

        /// <inheritdoc/>
        public virtual object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds <see cref="BaseShape"/> to <see cref="Shapes"/> collection.
        /// </summary>
        /// <param name="shape">The shape object.</param>
        public void AddShape(BaseShape shape)
        {
            shape.Owner = this;
            shape.State.Flags &= ~ShapeStateFlags.Standalone;
            Shapes = Shapes.Add(shape);
        }

        /// <summary>
        /// Adds <see cref="XPoint"/> to <see cref="Connectors"/> collection with <see cref="ShapeStateFlags.None"/> flag set.
        /// </summary>
        /// <param name="point">The point object.</param>
        public void AddConnectorAsNone(XPoint point)
        {
            point.Owner = this;
            point.State.Flags |= ShapeStateFlags.Connector | ShapeStateFlags.None;
            point.State.Flags &= ~ShapeStateFlags.Standalone;
            Connectors = Connectors.Add(point);
        }

        /// <summary>
        /// Adds <see cref="XPoint"/> to <see cref="Connectors"/> collection with <see cref="ShapeStateFlags.Input"/> flag set.
        /// </summary>
        /// <param name="point">The point object.</param>
        public void AddConnectorAsInput(XPoint point)
        {
            point.Owner = this;
            point.State.Flags |= ShapeStateFlags.Connector | ShapeStateFlags.Input;
            point.State.Flags &= ~ShapeStateFlags.Standalone;
            Connectors = Connectors.Add(point);
        }

        /// <summary>
        /// Adds <see cref="XPoint"/> to <see cref="Connectors"/> collection with <see cref="ShapeStateFlags.Output"/> flag set.
        /// </summary>
        /// <param name="point">The point object.</param>
        public void AddConnectorAsOutput(XPoint point)
        {
            point.Owner = this;
            point.State.Flags |= ShapeStateFlags.Connector | ShapeStateFlags.Output;
            point.State.Flags &= ~ShapeStateFlags.Standalone;
            Connectors = Connectors.Add(point);
        }

        /// <summary>
        /// Creates a new <see cref="XGroup"/> instance.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <returns>The new instance of the <see cref="XGroup"/> class.</returns>
        public static XGroup Create(string name)
        {
            return new XGroup()
            {
                Name = name
            };
        }

        /// <summary>
        /// Creates a new <see cref="XGroup"/> instance.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="source">The source shapes collection.</param>
        /// <returns>The new instance of the <see cref="XGroup"/> class.</returns>
        public static XGroup Group(string name, IEnumerable<BaseShape> shapes, IList<BaseShape> source = null)
        {
            var group = XGroup.Create(name);

            if (shapes != null)
            {
                foreach (var shape in shapes)
                {
                    if (shape is XPoint)
                    {
                        group.AddConnectorAsNone(shape as XPoint);
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
                if (shape is XGroup)
                {
                    var group = shape as XGroup;
                    Ungroup(group.Shapes, source, isShapeFromGroup: true);
                    Ungroup(group.Connectors, source, isShapeFromGroup: true);

                    // Remove group from source collection.
                    source.Remove(group);
                }
                else if (isShapeFromGroup)
                {
                    if (shape is XPoint)
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

        /// <summary>
        /// Check whether the <see cref="Connectors"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeConnectors() => _connectors.IsEmpty == false;
    }
}
