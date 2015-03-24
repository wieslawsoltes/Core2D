using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XContainer : XObject, IContainer
    {
        private double _width;
        private double _height;
        private IList<XStyle> _styles;
        private XStyle _currentStyle;
        private XShape _pointShape;
        private IList<ILayer> _layers;
        private ILayer _currentLayer;
        private ILayer _workingLayer;
        private XShape _currentShape;

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

        public IList<XStyle> Styles
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

        public XStyle CurrentStyle
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

        public XShape PointShape
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

        public IList<ILayer> Layers
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

        public ILayer CurrentLayer
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

        public ILayer WorkingLayer
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

        public XShape CurrentShape
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

        public static IContainer Create(double width, double height)
        {
            var c = new XContainer()
            {
                Width = width,
                Height = height,
                Layers = new ObservableCollection<ILayer>(),
                Styles = new ObservableCollection<XStyle>()
            };

            c.Layers.Add(XLayer.Create("Layer1"));
            c.Layers.Add(XLayer.Create("Layer2"));
            c.Layers.Add(XLayer.Create("Layer3"));

            c.CurrentLayer = c.Layers.FirstOrDefault();

            c.WorkingLayer = XLayer.Create("Working");

            c.Styles.Add(XStyle.Create("Black", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
            c.Styles.Add(XStyle.Create("Yellow", 255, 255, 255, 0, 255, 255, 255, 0, 2.0));
            c.Styles.Add(XStyle.Create("Red", 255, 255, 0, 0, 255, 255, 0, 0, 2.0));
            c.Styles.Add(XStyle.Create("Green", 255, 0, 255, 0, 255, 0, 255, 0, 2.0));
            c.Styles.Add(XStyle.Create("Blue", 255, 0, 0, 255, 255, 0, 0, 255, 2.0));

            c.CurrentStyle = c.Styles.FirstOrDefault();

            var pss = XStyle.Create("PointShape", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            
            c.PointShape = XEllipse.Create(-3, -3, 3, 3, pss, null, true);
            //c.PointShape = XRectangle.Create(-3, -3, 3, 3, pss, null, true);
            //c.PointShape = XRectangle.Create(-4, -4, 4, 4, pss, null, false);
            
            //var g = XGroup.Create("PointShape");
            //g.Shapes.Add(XLine.Create(-4, 0, 4, 0, pss, null));
            //g.Shapes.Add(XLine.Create(0, -4, 0, 4, pss, null));
            //c.PointShape = g;

            return c;
        }
    }
}
