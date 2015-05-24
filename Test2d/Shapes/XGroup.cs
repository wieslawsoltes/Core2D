// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XGroup : BaseShape
    {
        private IList<ShapeProperty> _database;
        private IList<BaseShape> _shapes;
        private IList<XPoint> _connectors;

        /// <summary>
        /// 
        /// </summary>
        public IList<ShapeProperty> Database
        {
            get 
            {
                if (_database == null)
                {
                    if (_shapes != null)
                    {
                        _database = new ObservableCollection<ShapeProperty>();
                        foreach (var shape in _shapes)
                        {
                            foreach (var property in shape.Properties)
                            {
                                _database.Add(property);
                            }
                        }
                    }
                }
                return _database;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<BaseShape> Shapes
        {
            get { return _shapes; }
            set
            {
                if (value != _shapes)
                {
                    _shapes = value;
                    Notify("Shapes");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<XPoint> Connectors
        {
            get { return _connectors; }
            set
            {
                if (value != _connectors)
                {
                    _connectors = value;
                    Notify("Connectors");
                }
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
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, IList<ShapeProperty> db)
        {
            if (State.HasFlag(ShapeState.Visible))
            {
                foreach (var shape in Shapes)
                {
                    shape.Draw(dc, renderer, dx, dy, db);
                }
 
                if (renderer.SelectedShape != null)
                {
                    if (this == renderer.SelectedShape)
                    {
                        foreach (var connector in Connectors)
                        {
                            connector.Draw(dc, renderer, dx, dy, db);
                        }
                    }
                }
                
                if (renderer.SelectedShapes != null)
                {
                    if (renderer.SelectedShapes.Contains(this))
                    {
                        foreach (var connector in Connectors)
                        {
                            connector.Draw(dc, renderer, dx, dy, db);
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
            foreach (var shape in Shapes)
            {
                shape.Move(dx, dy);
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
            Shapes.Add(shape);
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
            Connectors.Add(point);
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
            Connectors.Add(point);
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
            Connectors.Add(point);
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
                Properties = new ObservableCollection<ShapeProperty>(),
                Shapes = new ObservableCollection<BaseShape>(),
                Connectors = new ObservableCollection<XPoint>()
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
