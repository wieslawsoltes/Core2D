// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
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
        private ImmutableArray<ShapeProperty> _properties;
        private ImmutableArray<Layer> _layers;
        private Container _template;
        private Layer _currentLayer;
        private Layer _workingLayer;
        private Layer _helperLayer;
        private BaseShape _currentShape;
        private bool _isTemplate;

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
        public ImmutableArray<ShapeProperty> Properties
        {
            get { return _properties; }
            set { Update(ref _properties, value); }
        }

        /// <summary>
        /// Gets or sets property Value using Name as key for Properties array values. If property with the specified key does not exist it is created.
        /// </summary>
        /// <param name="name">The property name value.</param>
        /// <returns>The property Value.</returns>
        public object this[string name]
        {
            get
            {
                var result = _properties.FirstOrDefault(p => p.Name == name);
                if (result != null)
                {
                    return result.Value;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    var result = _properties.FirstOrDefault(p => p.Name == name);
                    if (result != null)
                    {
                        result.Value = value;
                    }
                    else
                    {
                        var property = ShapeProperty.Create(name, value);
                        Properties = Properties.Add(property);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<Layer> Layers
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
        public bool IsTemplate
        {
            get { return _isTemplate; }
            set { Update(ref _isTemplate, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            foreach (var layer in Layers)
            {
                layer.Shapes = ImmutableArray.Create<BaseShape>();
            }
            WorkingLayer.Shapes = ImmutableArray.Create<BaseShape>();
            HelperLayer.Shapes = ImmutableArray.Create<BaseShape>();
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
        public static Container Create(string name = "Container", double width = 840, double height = 600)
        {
            var c = new Container()
            {
                Name = name,
                Width = width,
                Height = height,
                Background = ArgbColor.Create(0x00, 0xFF, 0xFF, 0xFF),
                Properties = ImmutableArray.Create<ShapeProperty>(),
                Layers = ImmutableArray.Create<Layer>()
            };

            var builder = c.Layers.ToBuilder();
            builder.Add(Layer.Create("Layer1", c));
            builder.Add(Layer.Create("Layer2", c));
            builder.Add(Layer.Create("Layer3", c));
            builder.Add(Layer.Create("Layer4", c));
            c.Layers = builder.ToImmutable();

            c.CurrentLayer = c.Layers.FirstOrDefault();
            c.WorkingLayer = Layer.Create("Working", c);
            c.HelperLayer = Layer.Create("Helper", c);

            return c;
        }
    }
}
