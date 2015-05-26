// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Container : ObservableObject
    {
        private string _name;
        private double _width;
        private double _height;
        private ArgbColor _background;
        private IList<ShapeProperty> _properties;
        private IList<Layer> _layers;
        private Container _template;
        private Layer _currentLayer;
        private Layer _workingLayer;
        private Layer _helperLayer;
        private BaseShape _currentShape;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Width
        {
            get { return _width; }
            set { Update(ref _width, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Height
        {
            get { return _height; }
            set { Update(ref _height, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ArgbColor Background
        {
            get { return _background; }
            set { Update(ref _background, value); }
        }

        /// <summary>
        /// Gets or sets a colletion ShapeProperty that will be used during drawing.
        /// </summary>
        public IList<ShapeProperty> Properties
        {
            get { return _properties; }
            set { Update(ref _properties, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<Layer> Layers
        {
            get { return _layers; }
            set { Update(ref _layers, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Container Template
        {
            get { return _template; }
            set { Update(ref _template, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Layer CurrentLayer
        {
            get { return _currentLayer; }
            set { Update(ref _currentLayer, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Layer WorkingLayer
        {
            get { return _workingLayer; }
            set { Update(ref _workingLayer, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public Layer HelperLayer
        {
            get { return _helperLayer; }
            set { Update(ref _helperLayer, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BaseShape CurrentShape
        {
            get { return _currentShape; }
            set { Update(ref _currentShape, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            foreach (var layer in Layers)
            {
                layer.Shapes.Clear();
            }
            WorkingLayer.Shapes.Clear();
            HelperLayer.Shapes.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Invalidate()
        {
            if (Template != null)
            {
                Template.Invalidate();
            }
            
            if (Layers != null)
            {
                foreach (var layer in Layers)
                {
                    layer.Invalidate();
                }
            }
            
            if (WorkingLayer != null)
            {
                WorkingLayer.Invalidate();
            }
            
            if (HelperLayer != null)
            {
                HelperLayer.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Container Create(string name = "Container", double width = 810, double height = 600)
        {
            var c = new Container()
            {
                Name = name,
                Width = width,
                Height = height,
                Background = ArgbColor.Create(0x00, 0xFF, 0xFF, 0xFF),
                Properties = new ObservableCollection<ShapeProperty>(),
                Layers = new ObservableCollection<Layer>()
            };

            c.Layers.Add(Layer.Create("Layer1", c));
            c.Layers.Add(Layer.Create("Layer2", c));
            c.Layers.Add(Layer.Create("Layer3", c));
            c.Layers.Add(Layer.Create("Layer4", c));

            c.CurrentLayer = c.Layers.FirstOrDefault();

            c.WorkingLayer = Layer.Create("Working", c);
            c.HelperLayer = Layer.Create("Helper", c);

            return c;
        }
    }
}
