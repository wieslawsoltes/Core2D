// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D
{
    /// <summary>
    /// Object representing grouped shapes.
    /// </summary>
    public class XGroup : BaseShape
    {
        private ImmutableArray<Property> _shapesProperties;
        private ImmutableArray<BaseShape> _shapes;
        private ImmutableArray<XPoint> _connectors;

        /// <summary>
        /// Gets all properties from <see cref="Shapes"/> collection.
        /// </summary>
        public ImmutableArray<Property> ShapesProperties
        {
            get
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
        }

        /// <summary>
        /// Gets or sets shapes collection.
        /// </summary>
        public ImmutableArray<BaseShape> Shapes
        {
            get { return _shapes; }
            set { Update(ref _shapes, value); }
        }

        /// <summary>
        /// Gets or sets connectors collection.
        /// </summary>
        public ImmutableArray<XPoint> Connectors
        {
            get { return _connectors; }
            set { Update(ref _connectors, value); }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Renderer renderer, double dx, double dy, ImmutableArray<Property> db, Record r)
        {
            var record = r ?? this.Data.Record;

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
        }

        /// <inheritdoc/>
        public override void Move(double dx, double dy)
        {
            foreach (var shape in Shapes)
            {
                if (!shape.State.Flags.HasFlag(ShapeStateFlags.Connector))
                {
                    shape.Move(dx, dy);
                }
            }

            foreach (var connector in Connectors)
            {
                connector.Move(dx, dy);
            }
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
                Name = name,
                Style = default(ShapeStyle),
                Data = new Data()
                {
                    Properties = ImmutableArray.Create<Property>()
                },
                Shapes = ImmutableArray.Create<BaseShape>(),
                Connectors = ImmutableArray.Create<XPoint>()
            };
        }

        /// <summary>
        /// Creates a new <see cref="XGroup"/> instance.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <param name="shapes">The shapes collection.</param>
        /// <returns>The new instance of the <see cref="XGroup"/> class.</returns>
        public static XGroup Group(string name, IEnumerable<BaseShape> shapes)
        {
            var g = XGroup.Create(name);
            if (shapes == null)
                return g;

            foreach (var shape in shapes)
            {
                if (shape is XPoint)
                {
                    g.AddConnectorAsNone(shape as XPoint);
                }
                else
                {
                    g.AddShape(shape);
                }
            }

            return g;
        }
    }
}
