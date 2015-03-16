using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public class PortableEditor
    {
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

        public Tool CurrentTool { get; set; }
        public State CurrentState { get; set; }
        public bool DefaultIsFilled { get; set; }

        private readonly IContainer _container;
        private XShape _temp;

        public PortableEditor(IContainer container)
        {
            _container = container;
            
            _temp = null;

            DefaultIsFilled = false;
            CurrentTool = Tool.Line;
            CurrentState = State.None;
        }
        
        public void Left(double x, double y)
        {
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
                                    _temp = XLine.Create(x, y, _container.CurrentStyle);
                                    _container.WorkingLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.One;
                                }
                                break;
                            case State.One:	
                                {
                                    var line = _temp as XLine;
                                    line.End.X = x;
                                    line.End.Y = y;
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.CurrentLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    _container.CurrentLayer.Invalidate();
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
                                    _temp = XRectangle.Create(x, y, _container.CurrentStyle, DefaultIsFilled);
                                    _container.WorkingLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.One;
                                }
                                break;
                            case State.One:	
                                {
                                    var rectangle = _temp as XRectangle;
                                    rectangle.BottomRight.X = x;
                                    rectangle.BottomRight.Y = y;
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.CurrentLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    _container.CurrentLayer.Invalidate();
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
                                    _temp = XEllipse.Create(x, y, _container.CurrentStyle, DefaultIsFilled);
                                    _container.WorkingLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.One;
                                }
                                break;
                            case State.One:	
                                {
                                    var ellipse = _temp as XEllipse;
                                    ellipse.BottomRight.X = x;
                                    ellipse.BottomRight.Y = y;
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.CurrentLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    _container.CurrentLayer.Invalidate();
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
                                    _temp = XBezier.Create(x, y, _container.CurrentStyle);
                                    _container.WorkingLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.One;
                                }
                                break;
                            case State.One:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
                                    bezier.Point3.X = x;
                                    bezier.Point3.Y = y;
                                    bezier.Point4.X = x;
                                    bezier.Point4.Y = y;
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.Two;
                                }
                                break;
                            case State.Two:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
                                    bezier.Point3.X = x;
                                    bezier.Point3.Y = y;
                                    _container.WorkingLayer.Invalidate();
                                    CurrentState = State.Three;
                                }
                                break;
                            case State.Three:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
                                    _container.WorkingLayer.Shapes.Remove(_temp);
                                    _container.CurrentLayer.Shapes.Add(_temp);
                                    _container.WorkingLayer.Invalidate();
                                    _container.CurrentLayer.Invalidate();
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
                                    line.End.X = x;
                                    line.End.Y = y;
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
                                    rectangle.BottomRight.X = x;
                                    rectangle.BottomRight.Y = y;
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
                                    ellipse.BottomRight.X = x;
                                    ellipse.BottomRight.Y = y;
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
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
                                    bezier.Point3.X = x;
                                    bezier.Point3.Y = y;
                                    bezier.Point4.X = x;
                                    bezier.Point4.Y = y;
                                    _container.WorkingLayer.Invalidate();
                                }
                                break;
                            case State.Two:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
                                    bezier.Point3.X = x;
                                    bezier.Point3.Y = y;
                                    _container.WorkingLayer.Invalidate();
                                }
                                break;
                            case State.Three:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
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
