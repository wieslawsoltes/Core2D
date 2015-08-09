// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XGroup : BaseShape
    {
        private ImmutableArray<ShapeProperty> _shapesProperties;
        private ImmutableArray<BaseShape> _shapes;
        private ImmutableArray<XPoint> _connectors;

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<ShapeProperty> ShapesProperties
        {
            get 
            {
                if (_shapesProperties == null)
                {
                    if (_shapes != null)
                    {
                        var builder = ImmutableArray.CreateBuilder<ShapeProperty>();

                        foreach (var shape in _shapes)
                        {
                            foreach (var property in shape.Properties)
                            {
                                builder.Add(property);
                            }
                        }

                        foreach (var connector in _connectors)
                        {
                            foreach (var property in connector.Properties)
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
        /// 
        /// </summary>
        public ImmutableArray<BaseShape> Shapes
        {
            get { return _shapes; }
            set { Update(ref _shapes, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<XPoint> Connectors
        {
            get { return _connectors; }
            set { Update(ref _connectors, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public override void Bind(Record r)
        {
            var record = r ?? this.Record;

            foreach (var shape in Shapes)
            {
                shape.Bind(record);
            }

            foreach (var connector in Connectors)
            {
                connector.Bind(record);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="renderer"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var record = r ?? this.Record;

            if (State.HasFlag(ShapeState.Visible))
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public override void Move(double dx, double dy)
        {
            foreach (var shape in Shapes)
            {
                if (!shape.State.HasFlag(ShapeState.Connector))
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
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public void AddShape(BaseShape shape)
        {
            shape.Owner = this;
            shape.State &= ~ShapeState.Standalone;
            Shapes = Shapes.Add(shape);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void AddConnectorAsNone(XPoint point)
        {
            point.Owner = this;
            point.State |= ShapeState.Connector | ShapeState.None;
            point.State &= ~ShapeState.Standalone;
            Connectors = Connectors.Add(point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void AddConnectorAsInput(XPoint point)
        {
            point.Owner = this;
            point.State |= ShapeState.Connector | ShapeState.Input;
            point.State &= ~ShapeState.Standalone;
            Connectors = Connectors.Add(point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void AddConnectorAsOutput(XPoint point)
        {
            point.Owner = this;
            point.State |= ShapeState.Connector | ShapeState.Output;
            point.State &= ~ShapeState.Standalone;
            Connectors = Connectors.Add(point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XGroup Create(string name)
        {
            return new XGroup()
            {
                Name = name,
                Style = default(ShapeStyle),
                Bindings = ImmutableArray.Create<ShapeBinding>(),
                Properties = ImmutableArray.Create<ShapeProperty>(),
                Shapes = ImmutableArray.Create<BaseShape>(),
                Connectors = ImmutableArray.Create<XPoint>()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="shapes"></param>
        /// <returns></returns>
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
