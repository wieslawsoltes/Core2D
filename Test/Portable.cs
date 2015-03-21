using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Test
{
    public class XColor
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static XColor Create(byte a, byte r, byte g, byte b)
        {
            return new XColor() { A = a, R = r, G = g, B = b };
        }
    }
    
    public class XStyle
    {
        public string Name { get; set; }
        public XColor Stroke { get; set; }
        public XColor Fill { get; set; }
        public double Thickness { get; set;	}

        public static XStyle Create(
            string name,
            byte sa, byte sr, byte sg, byte sb,
            byte fa, byte fr, byte fg, byte fb,
            double thickness)
        {
            return new XStyle()
            {
                Name = name,
                Stroke = XColor.Create(sa, sr, sg, sb),
                Fill = XColor.Create(fa, fr, fg, fb),
                Thickness = thickness
            };
        }
    }

    public class XPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static XPoint Create(double x, double y)
        {
            return new XPoint() { X = x, Y = y };
        }
    }
    
    public abstract class XShape
    {
        public abstract void Draw(object dc, IRenderer renderer);
    }

    public interface ILayer
    {
        string Name { get; set; }
        IList<XShape> Shapes { get; set; }
        Action Invalidate { get; set; }
    }

    public interface IContainer
    {
        IList<ILayer> Layers { get; set; }
        ILayer CurrentLayer { get; set; }
        ILayer WorkingLayer { get; set; }
        IList<XStyle> Styles { get; set; }
        XStyle CurrentStyle { get; set; }
        XShape CurrentShape { get; set; }
    }

    public class XLayer : ILayer
    {
        public string Name { get; set; }
        public IList<XShape> Shapes { get; set; }
        public Action Invalidate { get; set; }
    }

    public class XContainer : IContainer
    {
        public IList<ILayer> Layers { get; set; }
        public ILayer CurrentLayer { get; set; }
        public ILayer WorkingLayer { get; set; }
        public IList<XStyle> Styles { get; set; }
        public XStyle CurrentStyle { get; set; }
        public XShape CurrentShape { get; set; }
    }

    public interface IElement
    {
        void Invalidate();
    }
    
    public interface IRenderer
    {
        void Render(object dc, ILayer layer);
        void Draw(object dc, XLine line);
        void Draw(object dc, XRectangle rectangle);
        void Draw(object dc, XEllipse ellipse);
        void Draw(object dc, XBezier bezier);
    }

    public class XLine : XShape
    {
        public XStyle Style { get; set; }
        public XPoint Start { get; set; }
        public XPoint End { get; set; }

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XLine Create(
            double x1, double y1, 
            double x2, double y2, 
            XStyle style)
        {
            return new XLine()
            {
                Style = style,
                Start = XPoint.Create(x1, y1),
                End = XPoint.Create(x2, y2)
            };
        }
        
        public static XLine Create(
            double x, double y, 
            XStyle style)
        {
            return Create(x, y, x, y, style);
        }
    }

    public class XRectangle : XShape
    {
        public XStyle Style { get; set; }
        public XPoint TopLeft { get; set; }
        public XPoint BottomRight { get; set; }
        public bool IsFilled { get; set; }

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XRectangle Create(
            double x1, double y1, 
            double x2, double y2, 
            XStyle style, 
            bool isFilled = false)
        {
            return new XRectangle()
            {
                Style = style,
                TopLeft = XPoint.Create(x1, y1),
                BottomRight = XPoint.Create(x2, y2),
                IsFilled = isFilled
            };
        }
        
        public static XRectangle Create(
            double x, double y, 
            XStyle style,
            bool isFilled = false)
        {
            return Create(x, y, x, y, style, isFilled);
        }
    }
    
    public class XEllipse : XShape
    {
        public XStyle Style { get; set; }
        public XPoint TopLeft { get; set; }
        public XPoint BottomRight { get; set; }
        public bool IsFilled { get; set; }

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XEllipse Create(
            double x1, double y1, 
            double x2, double y2, 
            XStyle style, 
            bool isFilled = false)
        {
            return new XEllipse()
            {
                Style = style,
                TopLeft = XPoint.Create(x1, y1),
                BottomRight = XPoint.Create(x2, y2),
                IsFilled = isFilled
            };
        }
        
        public static XEllipse Create(
            double x, double y, 
            XStyle style, 
            bool isFilled = false)
        {
            return Create(x, y, x, y, style, isFilled);
        }
    }

    public class XBezier : XShape
    {
        public XStyle Style { get; set; }
        public XPoint Point1 { get; set; }
        public XPoint Point2 { get; set; }
        public XPoint Point3 { get; set; }
        public XPoint Point4 { get; set; }
        public bool IsFilled { get; set; }

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XBezier Create(
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            double x4, double y4,
            XStyle style,
            bool isFilled = false)
        {
            return new XBezier()
            {
                Style = style,
                Point1 = XPoint.Create(x1, y1),
                Point2 = XPoint.Create(x2, y2),
                Point3 = XPoint.Create(x3, y3),
                Point4 = XPoint.Create(x4, y4),
                IsFilled = isFilled
            };
        }

        public static XBezier Create(
            double x, double y,
            XStyle style,
            bool isFilled = false)
        {
            return Create(x, y, x, y, x, y, x, y, style, isFilled);
        }
    }

    public enum Tool
    {
        None,
        Line,
        Rectangle,
        Ellipse,
        Bezier
    }

    public enum State
    {
        None,
        One,
        Two,
        Three,
        Four
    }

    public class ContainerEditor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Tool _currentTool;
        public Tool CurrentTool 
        {
            get { return _currentTool; } 
            set
            {
                if (value != _currentTool)
                {
                    _currentTool = value;
                    Notify("CurrentTool");
                }
            }
        }

        public State CurrentState { get; set; }
        public bool DefaultIsFilled { get; set; }
        public bool SnapToGrid { get; set; }
        public double SnapX { get; set; }
        public double SnapY { get; set; }
        
        private readonly IContainer _container;
        private XShape _temp;

        public ContainerEditor(IContainer container)
        {
            _container = container;
        }
        
        public static double Snap(double value, double snap)
        {
            double r = value % snap;
            return r >= snap / 2.0 ? value + snap - r : value - r;
        }
        
        public void Left(double x, double y)
        {
            double sx = SnapToGrid ? Snap(x, SnapX) : x;
            double sy = SnapToGrid ? Snap(y, SnapY) : y;
            switch (CurrentTool)
            {
                    // None
                case Tool.None:
                    {
                    }
                    break;
                    // Line
                case Tool.Line:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                    _temp = XLine.Create(sx, sy, _container.CurrentStyle);
                                    _container.WorkingLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.One;
                                }
                                break;
                            case State.One:
                                {
                                    var line = _temp as XLine;
                                    line.End.X = sx;
                                    line.End.Y = sy;
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.CurrentLayer.Shapes.Add(_temp);
                                    //_container.CurrentLayer.Invalidate();
                                    foreach (var layer in _container.Layers)
                                        layer.Invalidate();
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.None;
                                }
                                break;
                        }
                    }
                    break;
                    // Rectangle
                case Tool.Rectangle:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                    _temp = XRectangle.Create(sx, sy, _container.CurrentStyle, DefaultIsFilled);
                                    _container.WorkingLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.One;
                                }
                                break;
                            case State.One:
                                {
                                    var rectangle = _temp as XRectangle;
                                    rectangle.BottomRight.X = sx;
                                    rectangle.BottomRight.Y = sy;
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.CurrentLayer.Shapes.Add(_temp);
                                    //_container.CurrentLayer.Invalidate();
                                    foreach (var layer in _container.Layers)
                                        layer.Invalidate();
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.None;
                                }
                                break;
                        }
                    }
                    break;
                    // Ellipse
                case Tool.Ellipse:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                    _temp = XEllipse.Create(sx, sy, _container.CurrentStyle, DefaultIsFilled);
                                    _container.WorkingLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.One;
                                }
                                break;
                            case State.One:
                                {
                                    var ellipse = _temp as XEllipse;
                                    ellipse.BottomRight.X = sx;
                                    ellipse.BottomRight.Y = sy;
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.CurrentLayer.Shapes.Add(_temp);
                                    //_container.CurrentLayer.Invalidate();
                                    foreach (var layer in _container.Layers)
                                        layer.Invalidate();
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.None;
                                }
                                break;
                        }
                    }
                    break;
                    // Bezier
                case Tool.Bezier:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                    _temp = XBezier.Create(sx, sy, _container.CurrentStyle);
                                    _container.WorkingLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.One;
                                }
                                break;
                            case State.One:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = sx;
                                    bezier.Point2.Y = sy;
                                    bezier.Point3.X = sx;
                                    bezier.Point3.Y = sy;
                                    bezier.Point4.X = sx;
                                    bezier.Point4.Y = sy;
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.Two;
                                }
                                break;
                            case State.Two:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = sx;
                                    bezier.Point2.Y = sy;
                                    bezier.Point3.X = sx;
                                    bezier.Point3.Y = sy;
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.Three;
                                }
                                break;
                            case State.Three:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = sx;
                                    bezier.Point2.Y = sy;
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.CurrentLayer.Shapes.Add(_temp);
                                    //_container.CurrentLayer.Invalidate();
                                    foreach (var layer in _container.Layers)
                                        layer.Invalidate();
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.None;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        
        public void Right(double x, double y)
        {
            double sx = SnapToGrid ? Snap(x, SnapX) : x;
            double sy = SnapToGrid ? Snap(y, SnapY) : y;
            switch (CurrentTool)
            {
                    // None
                case Tool.None:
                    {
                    }
                    break;
                    // Line
                case Tool.Line:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:
                                {
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.None;
                                }
                                break;
                        }
                    }
                    break;
                    // Rectangle
                case Tool.Rectangle:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:
                                {
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.None;
                                }
                                break;
                        }
                    }
                    break;
                    // Ellipse
                case Tool.Ellipse:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:
                                {
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.None;
                                }
                                break;
                        }
                    }
                    break;
                    // Bezier
                case Tool.Bezier:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:
                            case State.Two:
                            case State.Three:
                                {
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.None;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        
        public void Move(double x, double y)
        {
            double sx = SnapToGrid ? Snap(x, SnapX) : x;
            double sy = SnapToGrid ? Snap(y, SnapY) : y;
            switch (CurrentTool)
            {
                    // None
                case Tool.None:
                    {
                    }
                    break;
                    // Line
                case Tool.Line:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:
                                {
                                    var line = _temp as XLine;
                                    line.End.X = sx;
                                    line.End.Y = sy;
                                    _container.WorkingLayer.Invalidate();
                                }
                                break;
                        }
                    }
                    break;
                    // Rectangle
                case Tool.Rectangle:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:
                                {
                                    var rectangle = _temp as XRectangle;
                                    rectangle.BottomRight.X = sx;
                                    rectangle.BottomRight.Y = sy;
                                    _container.WorkingLayer.Invalidate();
                                }
                                break;
                        }
                    }
                    break;
                    // Ellipse
                case Tool.Ellipse:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:
                                {
                                    var ellipse = _temp as XEllipse;
                                    ellipse.BottomRight.X = sx;
                                    ellipse.BottomRight.Y = sy;
                                    _container.WorkingLayer.Invalidate();
                                }
                                break;
                        }
                    }
                    break;
                    // Bezier
                case Tool.Bezier:
                    {
                        switch (CurrentState)
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = sx;
                                    bezier.Point2.Y = sy;
                                    bezier.Point3.X = sx;
                                    bezier.Point3.Y = sy;
                                    bezier.Point4.X = sx;
                                    bezier.Point4.Y = sy;
                                    _container.WorkingLayer.Invalidate();
                                }
                                break;
                            case State.Two:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = sx;
                                    bezier.Point2.Y = sy;
                                    bezier.Point3.X = sx;
                                    bezier.Point3.Y = sy;
                                    _container.WorkingLayer.Invalidate();
                                }
                                break;
                            case State.Three:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = sx;
                                    bezier.Point2.Y = sy;
                                    _container.WorkingLayer.Invalidate();
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
