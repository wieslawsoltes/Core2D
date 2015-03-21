using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
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
