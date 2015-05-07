// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
{
    public class Container : ObservableObject
    {
        private string _name;
        private double _width;
        private double _height;
        private IList<KeyValuePair<string, ShapeProperty>> _database;
        private IList<Layer> _layers;
        private Container _template;
        private Layer _currentLayer;
        private Layer _templateLayer;
        private Layer _workingLayer;
        private BaseShape _currentShape;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    Notify("Name");
                }
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                if (value != _width)
                {
                    _width = value;
                    Notify("Width");
                }
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (value != _height)
                {
                    _height = value;
                    Notify("Height");
                }
            }
        }

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

        public IList<Layer> Layers
        {
            get { return _layers; }
            set
            {
                if (value != _layers)
                {
                    _layers = value;
                    Notify("Layers");
                }
            }
        }

        public Container Template
        {
            get { return _template; }
            set
            {
                if (value != _template)
                {
                    _template = value;
                    Notify("Template");
                }
            }
        }

        public Layer CurrentLayer
        {
            get { return _currentLayer; }
            set
            {
                if (value != _currentLayer)
                {
                    _currentLayer = value;
                    Notify("CurrentLayer");
                }
            }
        }

        public Layer TemplateLayer
        {
            get { return _templateLayer; }
            set
            {
                if (value != _templateLayer)
                {
                    _templateLayer = value;
                    Notify("TemplateLayer");
                }
            }
        }

        public Layer WorkingLayer
        {
            get { return _workingLayer; }
            set
            {
                if (value != _workingLayer)
                {
                    _workingLayer = value;
                    Notify("WorkingLayer");
                }
            }
        }

        public BaseShape CurrentShape
        {
            get { return _currentShape; }
            set
            {
                if (value != _currentShape)
                {
                    _currentShape = value;
                    Notify("CurrentShape");
                }
            }
        }

        public void Clear()
        {
            foreach (var layer in Layers)
            {
                layer.Shapes.Clear();
            }
            WorkingLayer.Shapes.Clear();
        }

        public void Invalidate()
        {
            TemplateLayer.Invalidate();
            foreach (var layer in Layers)
            {
                layer.Invalidate();
            }
            WorkingLayer.Invalidate();
        }

        public static Container Create(string name = "Container", double width = 810, double height = 600)
        {
            var c = new Container()
            {
                Name = name,
                Width = width,
                Height = height,
                Layers = new ObservableCollection<Layer>()
            };

            c.Layers.Add(Layer.Create("Layer1"));
            c.Layers.Add(Layer.Create("Layer2"));
            c.Layers.Add(Layer.Create("Layer3"));
            c.Layers.Add(Layer.Create("Layer4"));

            c.CurrentLayer = c.Layers.FirstOrDefault();

            c.TemplateLayer = Layer.Create("Template");
            c.WorkingLayer = Layer.Create("Working");

            return c;
        }

        public static void CreateGrid(Container c, ShapeStyle gs, double width = 810, double height = 600)
        {
            var settings = LineGrid.Settings.Create(0, 0, width, height, 30, 30);
            var g = LineGrid.Create(gs, settings);
            c.TemplateLayer.Shapes.Add(g);
        }
    }
}
