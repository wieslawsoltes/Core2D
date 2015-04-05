// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class Container : ObservableObject
    {
        private double _width;
        private double _height;
        private IList<ShapeStyle> _styles;
        private ShapeStyle _currentStyle;
        private BaseShape _pointShape;
        private IList<Layer> _layers;
        private Layer _currentLayer;
        private Layer _workingLayer;
        private BaseShape _currentShape;

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

        public IList<ShapeStyle> Styles
        {
            get { return _styles; }
            set
            {
                if (value != _styles)
                {
                    _styles = value;
                    Notify("Styles");
                }
            }
        }

        public ShapeStyle CurrentStyle
        {
            get { return _currentStyle; }
            set
            {
                if (value != _currentStyle)
                {
                    _currentStyle = value;
                    Notify("CurrentStyle");
                }
            }
        }

        public BaseShape PointShape
        {
            get { return _pointShape; }
            set
            {
                if (value != _pointShape)
                {
                    _pointShape = value;
                    Notify("PointShape");
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
            foreach (var layer in Layers)
            {
                layer.Invalidate();
            }
            WorkingLayer.Invalidate();
        }

        public static Container Create(double width = 800, double height = 600)
        {
            var c = new Container()
            {
                Width = width,
                Height = height,
                Layers = new ObservableCollection<Layer>(),
                Styles = new ObservableCollection<ShapeStyle>()
            };

            c.Layers.Add(Layer.Create("Layer1"));
            c.Layers.Add(Layer.Create("Layer2"));
            c.Layers.Add(Layer.Create("Layer3"));
            c.Layers.Add(Layer.Create("Layer4"));

            c.CurrentLayer = c.Layers.FirstOrDefault();

            c.WorkingLayer = Layer.Create("Working");

            c.Styles.Add(ShapeStyle.Create("Black", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
            c.Styles.Add(ShapeStyle.Create("Yellow", 255, 255, 255, 0, 255, 255, 255, 0, 2.0));
            c.Styles.Add(ShapeStyle.Create("Red", 255, 255, 0, 0, 255, 255, 0, 0, 2.0));
            c.Styles.Add(ShapeStyle.Create("Green", 255, 0, 255, 0, 255, 0, 255, 0, 2.0));
            c.Styles.Add(ShapeStyle.Create("Blue", 255, 0, 0, 255, 255, 0, 0, 255, 2.0));

            c.CurrentStyle = c.Styles.FirstOrDefault();

            CrossPointShape(
                c, 
                ShapeStyle.Create("PointShape", 255, 255, 0, 0, 255, 255, 0, 0, 2.0));

            return c;
        }

        public static void EllipsePointShape(Container c, ShapeStyle pss)
        {
            c.PointShape = XEllipse.Create(-4, -4, 4, 4, pss, null, false);
        }

        public static void FilledEllipsePointShape(Container c, ShapeStyle pss)
        {
            c.PointShape = XEllipse.Create(-3, -3, 3, 3, pss, null, true);
        }

        public static void RectanglePointShape(Container c, ShapeStyle pss)
        {
            c.PointShape = XRectangle.Create(-4, -4, 4, 4, pss, null, false);
        }

        public static void FilledRectanglePointShape(Container c, ShapeStyle pss)
        {
            c.PointShape = XRectangle.Create(-3, -3, 3, 3, pss, null, true);
        }

        public static void CrossPointShape(Container c, ShapeStyle pss)
        {
            var g = XGroup.Create("PointShape");
            g.Shapes.Add(XLine.Create(-4, 0, 4, 0, pss, null));
            g.Shapes.Add(XLine.Create(0, -4, 0, 4, pss, null));
            c.PointShape = g;
        }
    }
}
