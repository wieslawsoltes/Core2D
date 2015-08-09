// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XReference : BaseShape
    {
        private ImmutableArray<ShapeProperty> _shapesProperties;
        private XPoint _origin;
        private BaseShape _shape;

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<ShapeProperty> ShapesProperties
        {
            get 
            {
                if (_shapesProperties == null)
                {
                    if (_shape != null)
                    {
                        var builder = ImmutableArray.CreateBuilder<ShapeProperty>();
                        if (_shape is XGroup)
                        {
                            var group = _shape as XGroup;

                            foreach (var shape in group.Shapes)
                            {
                                foreach (var property in shape.Properties)
                                {
                                    builder.Add(property);
                                }
                            }

                            foreach (var connector in group.Connectors)
                            {
                                foreach (var property in connector.Properties)
                                {
                                    builder.Add(property);
                                }
                            }
                        }
                        else
                        {
                            foreach (var property in _shape.Properties)
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
        public XPoint Origin
        {
            get { return _origin; }
            set { Update(ref _origin, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public BaseShape Shape
        {
            get { return _shape; }
            set { Update(ref _shape, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public override void Bind(Record r)
        {
            var record = r ?? this.Record;
            
            if (_origin != null)
            {
                _origin.Bind(record);
            }
            
            if (_shape != null)
            {
                _shape.Bind(record);
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

            if (_shape != null)
            {
                if (State.HasFlag(ShapeState.Visible))
                {
                    _shape.Draw(dc, renderer, _origin.X + dx, _origin.Y + dy, db, record);

                    if (renderer.State.SelectedShape != null)
                    {
                        if (this == renderer.State.SelectedShape)
                        {
                            _origin.Draw(dc, renderer, dx, dy, db, record);
                        }
                        else
                        {
                            if (_origin == renderer.State.SelectedShape)
                            {
                                _origin.Draw(dc, renderer, dx, dy, db, record);
                            }
                        }
                    }
                    
                    if (renderer.State.SelectedShapes != null)
                    {
                        if (renderer.State.SelectedShapes.Contains(this))
                        {
                            _origin.Draw(dc, renderer, dx, dy, db, record);
                        }
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
            if (_origin != null)
            {
                if (!_origin.State.HasFlag(ShapeState.Connector))
                {
                    _origin.Move(dx, dy);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="origin"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static XReference Create(string name, XPoint origin, BaseShape shape)
        {
            return new XReference() 
            { 
                Name = name,
                Style = default(ShapeStyle),
                Bindings = ImmutableArray.Create<ShapeBinding>(),
                Properties = ImmutableArray.Create<ShapeProperty>(),
                Origin = origin, 
                Shape = shape 
            };
        }
    }
}
