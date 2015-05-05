// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Test2d
{
    public class XGroup : BaseShape
    {
        private IList<KeyValuePair<string, ShapeProperty>> _database;
        private IList<BaseShape> _shapes;
        private IList<XPoint> _connectors;

        public IList<KeyValuePair<string, ShapeProperty>> Database
        {
            get { return _database; }
            set
            {
                if (value != _database)
                {
                    _database = value;
                    Notify("Database");
                }
            }
        }

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
   
        public override void Draw(object dc, IRenderer renderer, double dx, double dy)
        {
            if (State.HasFlag(ShapeState.Visible))
            {
                foreach (var shape in Shapes)
                {
                    shape.Draw(dc, renderer, dx, dy);
                }
 
                if (renderer.SelectedShape != null)
                {
                    if (this == renderer.SelectedShape)
                    {
                        foreach (var connector in Connectors)
                        {
                            connector.Draw(dc, renderer, dx, dy);
                        }
                        
                        //foreach (var connector in Connectors)
                        //{
                        //    if (connector.Shape != null)
                        //    {
                        //        connector.Shape.Draw(dc, renderer, connector.X + dx, connector.Y + dy);
                        //    }
                        //}
                    }
                }
                
                if (renderer.SelectedShapes != null)
                {
                    if (renderer.SelectedShapes.Contains(this))
                    {
                        foreach (var connector in Connectors)
                        {
                            connector.Draw(dc, renderer, dx, dy);
                        }
                        
                        //foreach (var connector in Connectors)
                        //{
                        //    if (connector.Shape != null)
                        //    {
                        //        connector.Shape.Draw(dc, renderer, connector.X + dx, connector.Y + dy);
                        //    }
                        //}
                    }
                }
            }
        }

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

        public void AddProperty(ShapeProperty property)
        {
            _database.Add(
                new KeyValuePair<string, ShapeProperty>(
                    property.Name,
                    property));
        }

        public void AddProperty(string key, ShapeProperty property)
        {
            _database.Add(
                new KeyValuePair<string, ShapeProperty>(
                    key,
                    property));
        }

        public void AddShape(BaseShape shape)
        {
            shape.Owner = this;
            shape.State &= ~ShapeState.Standalone;
            Shapes.Add(shape);
        }

        public void AddConnectorAsNone(XPoint point)
        {
            point.Owner = this;
            point.State |= ShapeState.Connector | ShapeState.None;
            point.State &= ~ShapeState.Standalone;
            Connectors.Add(point);
        }

        public void AddConnectorAsInput(XPoint point)
        {
            point.Owner = this;
            point.State |= ShapeState.Connector | ShapeState.Input;
            point.State &= ~ShapeState.Standalone;
            Connectors.Add(point);
        }

        public void AddConnectorAsOutput(XPoint point)
        {
            point.Owner = this;
            point.State |= ShapeState.Connector | ShapeState.Output;
            point.State &= ~ShapeState.Standalone;
            Connectors.Add(point);
        }

        public static XGroup Create(string name)
        {
            return new XGroup()
            {
                Name = name,
                Database = new ObservableCollection<KeyValuePair<string, ShapeProperty>>(),
                Shapes = new ObservableCollection<BaseShape>(),
                Connectors = new ObservableCollection<XPoint>()
            };
        }

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
