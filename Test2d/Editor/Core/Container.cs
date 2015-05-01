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
        private double _width;
        private double _height;
        private IList<ShapeStyleGroup> _styleGroups;
        private ShapeStyleGroup _currentStyleGroup;
        private BaseShape _pointShape;
        private IList<Layer> _layers;
        private Layer _currentLayer;
        private Layer _templateLayer;
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

        public IList<ShapeStyleGroup> StyleGroups
        {
            get { return _styleGroups; }
            set
            {
                if (value != _styleGroups)
                {
                    _styleGroups = value;
                    Notify("StyleGroups");
                }
            }
        }

        public ShapeStyleGroup CurrentStyleGroup
        {
            get { return _currentStyleGroup; }
            set
            {
                if (value != _currentStyleGroup)
                {
                    _currentStyleGroup = value;
                    Notify("CurrentStyleGroup");
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

        public static Container Create(double width = 810, double height = 600, bool grid = true)
        {
            var c = new Container()
            {
                Width = width,
                Height = height,
                Layers = new ObservableCollection<Layer>(),
                StyleGroups = new ObservableCollection<ShapeStyleGroup>()
            };

            c.Layers.Add(Layer.Create("Layer1"));
            c.Layers.Add(Layer.Create("Layer2"));
            c.Layers.Add(Layer.Create("Layer3"));
            c.Layers.Add(Layer.Create("Layer4"));

            c.CurrentLayer = c.Layers.FirstOrDefault();

            c.TemplateLayer = Layer.Create("Template");
            c.WorkingLayer = Layer.Create("Working");

            // default styles group
            var sgd = ShapeStyleGroup.Create("Default");
            sgd.Styles.Add(ShapeStyle.Create("Black", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
            sgd.Styles.Add(ShapeStyle.Create("Yellow", 255, 255, 255, 0, 255, 255, 255, 0, 2.0));
            sgd.Styles.Add(ShapeStyle.Create("Red", 255, 255, 0, 0, 255, 255, 0, 0, 2.0));
            sgd.Styles.Add(ShapeStyle.Create("Green", 255, 0, 255, 0, 255, 0, 255, 0, 2.0));
            sgd.Styles.Add(ShapeStyle.Create("Blue", 255, 0, 0, 255, 255, 0, 0, 255, 2.0));
            sgd.CurrentStyle = sgd.Styles.FirstOrDefault();

            c.StyleGroups.Add(sgd);
            c.CurrentStyleGroup = c.StyleGroups.FirstOrDefault();

            // template styles group
            var sgt = ShapeStyleGroup.Create("Template");
            var pss = ShapeStyle.Create("PointShape", 255, 255, 0, 0, 255, 255, 0, 0, 2.0);
            var gs = ShapeStyle.Create("Grid", 255, 172, 172, 172, 255, 172, 172, 172, 1.0);
            sgt.Styles.Add(pss);
            sgt.Styles.Add(gs);
            c.StyleGroups.Add(sgt);
            sgt.CurrentStyle = sgt.Styles.FirstOrDefault();

            CrossPointShape(c, pss);

            if (grid)
            {
                var s = LineGrid.Settings.Create(0, 0, width, height, 30, 30);
                var g = LineGrid.Create(gs, s);
                c.TemplateLayer.Shapes.Add(g);
            }

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
