// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Test2d
{
    public class Editor : ObservableObject
    {
        private Container _container;
        private IRenderer _renderer;
        private BaseShape _shape;
        private Tool _currentTool = Tool.Selection;
        private State _currentState = State.None;
        private bool _defaultIsFilled = false;
        private bool _snapToGrid = true;
        private double _snapX = 15.0;
        private double _snapY = 15.0;
        private ShapeStyle _selectionStyle;
        private bool _isContextMenu;
        private bool _enableObserver = true;
        private Observer _observer;
        private double _startX;
        private double _startY;
        private double _hitTreshold = 6.0;
        private bool _tryToConnect = true;
        
        public Container Container
        {
            get { return _container; }
            set
            {
                if (value != _container)
                {
                    _container = value;
                    Notify("Container");
                }
            }
        }

        public IRenderer Renderer
        {
            get { return _renderer; }
            set
            {
                if (value != _renderer)
                {
                    _renderer = value;
                    Notify("Renderer");
                }
            }
        }

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

        public State CurrentState
        {
            get { return _currentState; }
            set
            {
                if (value != _currentState)
                {
                    _currentState = value;
                    Notify("CurrentState");
                }
            }
        }

        public bool DefaultIsFilled
        {
            get { return _defaultIsFilled; }
            set
            {
                if (value != _defaultIsFilled)
                {
                    _defaultIsFilled = value;
                    Notify("DefaultIsFilled");
                }
            }
        }

        public bool SnapToGrid
        {
            get { return _snapToGrid; }
            set
            {
                if (value != _snapToGrid)
                {
                    _snapToGrid = value;
                    Notify("SnapToGrid");
                }
            }
        }

        public double SnapX
        {
            get { return _snapX; }
            set
            {
                if (value != _snapX)
                {
                    _snapX = value;
                    Notify("SnapX");
                }
            }
        }

        public double SnapY
        {
            get { return _snapY; }
            set
            {
                if (value != _snapY)
                {
                    _snapY = value;
                    Notify("SnapY");
                }
            }
        }

        public ShapeStyle SelectionStyle
        {
            get { return _selectionStyle; }
            set
            {
                if (value != _selectionStyle)
                {
                    _selectionStyle = value;
                    Notify("SelectionStyle");
                }
            }
        }

        public bool IsContextMenu
        {
            get { return _isContextMenu; }
            set
            {
                if (value != _isContextMenu)
                {
                    _isContextMenu = value;
                    Notify("IsContextMenu");
                }
            }
        }
        
        public bool EnableObserver
        {
            get { return _enableObserver; }
            set
            {
                if (value != _enableObserver)
                {
                    _enableObserver = value;
                    Notify("EnableObserver");
                }
            }
        }

        public Observer Observer
        {
            get { return _observer; }
            set
            {
                if (value != _observer)
                {
                    _observer = value;
                    Notify("Observer");
                }
            }
        }

        public double HitTreshold
        {
            get { return _hitTreshold; }
            set
            {
                if (value != _hitTreshold)
                {
                    _hitTreshold = value;
                    Notify("HitTreshold");
                }
            }
        }

        public bool TryToConnect
        {
            get { return _tryToConnect; }
            set
            {
                if (value != _tryToConnect)
                {
                    _tryToConnect = value;
                    Notify("TryToConnect");
                }
            }
        }

        public static Editor Create(Container container, IRenderer renderer)
        {
            var editor = new Editor()
            {
                SnapToGrid = true,
                SnapX = 15.0,
                SnapY = 15.0,
                DefaultIsFilled = false,
                CurrentTool = Tool.Selection,
                CurrentState = State.None,
                EnableObserver = true,
                HitTreshold = 6.0,
                TryToConnect = true
            };

            editor.SelectionStyle = 
                ShapeStyle.Create(
                    "Selection",
                    0x7F, 0x33, 0x33, 0xFF,
                    0x4F, 0x33, 0x33, 0xFF,
                    1.0, 
                    LineStyle.Create(
                        ArrowStyle.Create(), 
                        ArrowStyle.Create()));
       
            editor.Container = container;
            editor.Renderer = renderer;
            
            if (editor.EnableObserver)
            {
                editor.Observer = new Observer(editor);
            }

            return editor;
        }

        public static double Snap(double value, double snap)
        {
            double r = value % snap;
            return r >= snap / 2.0 ? value + snap - r : value - r;
        }

        public static IEnumerable<XPoint> GetPoints(IEnumerable<BaseShape> shapes)
        {
            if (shapes == null)
                yield break;
            foreach (var shape in shapes)
            {
                if (shape is XPoint)
                {
                    yield return shape as XPoint;
                }
                else if (shape is XLine)
                {
                    var line = shape as XLine;
                    yield return line.Start;
                    yield return line.End;
                }
                else if (shape is XRectangle)
                {
                    var rectangle = shape as XRectangle;
                    yield return rectangle.TopLeft;
                    yield return rectangle.BottomRight;
                }
                else if (shape is XEllipse)
                {
                    var ellipse = shape as XEllipse;
                    yield return ellipse.TopLeft;
                    yield return ellipse.BottomRight;
                }
                else if (shape is XArc)
                {
                    var arc = shape as XArc;
                    yield return arc.Point1;
                    yield return arc.Point2;
                }
                else if (shape is XBezier)
                {
                    var bezier = shape as XBezier;
                    yield return bezier.Point1;
                    yield return bezier.Point2;
                    yield return bezier.Point3;
                    yield return bezier.Point4;
                }
                else if (shape is XQBezier)
                {
                    var qbezier = shape as XQBezier;
                    yield return qbezier.Point1;
                    yield return qbezier.Point2;
                    yield return qbezier.Point3;
                }
                else if (shape is XText)
                {
                    var text = shape as XText;
                    yield return text.TopLeft;
                    yield return text.BottomRight;
                }
                else if (shape is XGroup)
                {
                    var group = shape as XGroup;
                    foreach (var point in GetPoints(group.Shapes))
                    {
                        yield return point;
                    }
                    foreach (var point in group.Connectors)
                    {
                        yield return point;
                    }
                }
            }
        }

        public static void Move(IEnumerable<XPoint> points, double dx, double dy)
        {
            foreach (var point in points)
            {
                point.Move(dx, dy);
            }
        }

        public bool TryToSelectShapes(Container container, XRectangle rectangle)
        {
            var rect = Rect2.Create(rectangle.TopLeft, rectangle.BottomRight);

            var result = ShapeBounds.HitTest(container, rect, _hitTreshold);
            if (result != null)
            {
                if (result.Count > 0)
                {
                    container.CurrentShape = null;
                    _renderer.SelectedShape = null;
                    _renderer.SelectedShapes = result;
                    container.CurrentLayer.Invalidate();
                    return true;
                }
            }

            container.CurrentShape = null;
            _renderer.SelectedShape = null;
            _renderer.SelectedShapes = null;
            container.CurrentLayer.Invalidate();
            return false;
        }
        
        public bool TryToSelectShape(Container container, double x, double y)
        {
            var result = ShapeBounds.HitTest(container, new Vector2(x, y), _hitTreshold);
            if (result != null)
            {
                container.CurrentShape = result;
                _renderer.SelectedShape = result;
                _renderer.SelectedShapes = null;
                container.CurrentLayer.Invalidate();
                return true;
            }
            
            container.CurrentShape = null;
            _renderer.SelectedShape = null;
            _renderer.SelectedShapes = null;
            container.CurrentLayer.Invalidate();
            return false;
        }

        public void TryToConnectStart(XLine line, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                line.Start = result as XPoint;
            }
        }

        public void TryToConnectEnd(XLine line, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                line.End = result as XPoint;
            }
        }

        public void TryToConnectTopLeft(XRectangle rectangle, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                rectangle.TopLeft = result as XPoint;
            }
        }

        public void TryToConnectBottomRight(XRectangle rectangle, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                rectangle.BottomRight = result as XPoint;
            }
        }

        public void TryToConnectTopLeft(XEllipse ellipse, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                ellipse.TopLeft = result as XPoint;
            }
        }

        public void TryToConnectBottomRight(XEllipse ellipse, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                ellipse.BottomRight = result as XPoint;
            }
        }

        public void TryToConnectPoint1(XArc arc, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point1 = result as XPoint;
            }
        }

        public void TryToConnectPoint2(XArc arc, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point2 = result as XPoint;
            }
        }

        public void TryToConnectPoint1(XBezier bezier, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point1 = result as XPoint;
            }
        }

        public void TryToConnectPoint2(XBezier bezier, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point2 = result as XPoint;
            }
        }

        public void TryToConnectPoint3(XBezier bezier, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point3 = result as XPoint;
            }
        }

        public void TryToConnectPoint4(XBezier bezier, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point4 = result as XPoint;
            }
        }

        public void TryToConnectPoint1(XQBezier qbezier, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point1 = result as XPoint;
            }
        }

        public void TryToConnectPoint2(XQBezier qbezier, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point2 = result as XPoint;
            }
        }

        public void TryToConnectPoint3(XQBezier qbezier, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point3 = result as XPoint;
            }
        }

        public void TryToConnectTopLeft(XText text, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                text.TopLeft = result as XPoint;
            }
        }

        public void TryToConnectBottomRight(XText text, double sx, double sy)
        {
            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
            if (result != null && result is XPoint)
            {
                text.BottomRight = result as XPoint;
            }
        }

        public bool IsLeftDownAvailable()
        {
            return Container.CurrentLayer != null
                && Container.CurrentLayer.IsVisible
                && Container.CurrentStyle != null;
        }

        public bool IsLeftUpAvailable()
        {
            return Container.CurrentLayer != null
                && Container.CurrentLayer.IsVisible
                && Container.CurrentStyle != null;
        }
        
        public bool IsRightDownAvailable()
        {
            return Container.CurrentLayer != null
                && Container.CurrentLayer.IsVisible
                && Container.CurrentStyle != null;
        }
        
        public bool IsRightUpAvailable()
        {
            return Container.CurrentLayer != null
                && Container.CurrentLayer.IsVisible
                && Container.CurrentStyle != null;
        }

        public bool IsMoveAvailable()
        {
            return Container.CurrentLayer != null
                && Container.CurrentLayer.IsVisible
                && Container.CurrentStyle != null;
        }

        public void LeftDown(double x, double y)
        {
            double sx = SnapToGrid ? Snap(x, SnapX) : x;
            double sy = SnapToGrid ? Snap(y, SnapY) : y;
            switch (CurrentTool)
            {
                case Tool.None:
                    break;
                case Tool.Selection:
                    {
                        SelectionLeftDown(x, y);
                    }
                    break;
                case Tool.Point:
                    {
                        PointLeftDown(sx, sy);
                    }
                    break;
                case Tool.Line:
                    {
                        LineLeftDown(sx, sy);
                    }
                    break;
                case Tool.Rectangle:
                    {
                        RectangleLeftDown(sx, sy);
                    }
                    break;
                case Tool.Ellipse:
                    {
                        EllipseLeftDown(sx, sy);
                    }
                    break;
                case Tool.Arc:
                    {
                        ArcLeftDown(sx, sy);
                    }
                    break;
                case Tool.Bezier:
                    {
                        BezierLeftDown(sx, sy);
                    }
                    break;
                case Tool.QBezier:
                    {
                        QBezierLeftDown(sx, sy);
                    }
                    break;
                case Tool.Text:
                    {
                        TextLeftDown(sx, sy);
                    }
                    break;
            }
        }

        public void LeftUp(double x, double y)
        {
            double sx = SnapToGrid ? Snap(x, SnapX) : x;
            double sy = SnapToGrid ? Snap(y, SnapY) : y;
            switch (CurrentTool)
            {
                case Tool.None:
                    break;
                case Tool.Selection:
                    {
                        SelectionLeftUp(x, y);
                    }
                    break;
                case Tool.Point:
                    break;
                case Tool.Line:
                    break;
                case Tool.Rectangle:
                    break;
                case Tool.Ellipse:
                    break;
                case Tool.Arc:
                    break;
                case Tool.Bezier:
                    break;
                case Tool.QBezier:
                    break;
                case Tool.Text:
                    break;
            }
        }
   
        public void RightDown(double x, double y)
        {
            if (CurrentState == State.None)
            {
                SelectionRightDown(x, y);
                return;
            }
            
            double sx = SnapToGrid ? Snap(x, SnapX) : x;
            double sy = SnapToGrid ? Snap(y, SnapY) : y;
            switch (CurrentTool)
            {
                case Tool.None:
                    break;
                case Tool.Selection:
                    break;
                case Tool.Point:
                    {
                        PointRightDown(sx, sy);
                    }
                    break;
                case Tool.Line:
                    {
                        LineRightDown(sx, sy);
                    }
                    break;
                case Tool.Rectangle:
                    {
                        RectangleRightDown(sx, sy);
                    }
                    break;
                case Tool.Ellipse:
                    {
                        EllipseRightDown(sx, sy);
                    }
                    break;
                case Tool.Arc:
                    {
                        ArcRightDown(sx, sy);
                    }
                    break;
                case Tool.Bezier:
                    {
                        BezierRightDown(sx, sy);
                    }
                    break;
                case Tool.QBezier:
                    {
                        QBezierRightDown(sx, sy);
                    }
                    break;
                case Tool.Text:
                    {
                        TextRightDown(sx, sy);
                    }
                    break;
            }
        }

        public void RightUp(double x, double y)
        {
            double sx = SnapToGrid ? Snap(x, SnapX) : x;
            double sy = SnapToGrid ? Snap(y, SnapY) : y;
            switch (CurrentTool)
            {
                case Tool.None:
                    break;
                case Tool.Selection:
                    break;
                case Tool.Point:
                    break;
                case Tool.Line:
                    break;
                case Tool.Rectangle:
                    break;
                case Tool.Ellipse:
                    break;
                case Tool.Arc:
                    break;
                case Tool.Bezier:
                    break;
                case Tool.QBezier:
                    break;
                case Tool.Text:
                    break;
            }
        }
        
        public void Move(double x, double y)
        {
            double sx = SnapToGrid ? Snap(x, SnapX) : x;
            double sy = SnapToGrid ? Snap(y, SnapY) : y;
            switch (CurrentTool)
            {
                case Tool.None:
                    break;
                case Tool.Selection:
                    {
                        SelectionMove(x, y);
                    }
                    break;
                case Tool.Point:
                    {
                        PointMove(sx, sy);
                    }
                    break;
                case Tool.Line:
                    {
                        LineMove(sx, sy);
                    }
                    break;
                case Tool.Rectangle:
                    {
                        RectangleMove(sx, sy);
                    }
                    break;
                case Tool.Ellipse:
                    {
                        EllipseMove(sx, sy);
                    }
                    break;
                case Tool.Arc:
                    {
                        ArcMove(sx, sy);
                    }
                    break;
                case Tool.Bezier:
                    {
                        BezierMove(sx, sy);
                    }
                    break;
                case Tool.QBezier:
                    {
                        QBezierMove(sx, sy);
                    }
                    break;
                case Tool.Text:
                    {
                        TextMove(sx, sy);
                    }
                    break;
            }
        }

        private void SelectionLeftDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        if (_renderer.SelectedShape == null 
                            && _renderer.SelectedShapes != null)
                        {
                            var result = ShapeBounds.HitTest(_container, new Vector2(sx, sy), _hitTreshold);
                            if (result != null)
                            {
                                _startX = SnapToGrid ? Snap(sx, SnapX) : sx;
                                _startY = SnapToGrid ? Snap(sy, SnapY) : sy;
                                IsContextMenu = false;
                                CurrentState = State.One;
                                break;
                            }
                        }
                        
                        if (TryToSelectShape(_container, sx, sy))
                        {
                            _startX = SnapToGrid ? Snap(sx, SnapX) : sx;
                            _startY = SnapToGrid ? Snap(sy, SnapY) : sy;
                            IsContextMenu = false;
                            CurrentState = State.One;
                            break;
                        }

                        _shape = XRectangle.Create(
                            sx, sy,
                            _selectionStyle,
                            null,
                            true);
                        _container.WorkingLayer.Shapes.Add(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var rectangle = _shape as XRectangle;
                        rectangle.BottomRight.X = sx;
                        rectangle.BottomRight.Y = sy;
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void SelectionLeftUp(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        if (_renderer.SelectedShape != null 
                            || _renderer.SelectedShapes != null)
                        {
                            CurrentState = State.None;
                            break;
                        }

                        var rectangle = _shape as XRectangle;
                        rectangle.BottomRight.X = sx;
                        rectangle.BottomRight.Y = sy;
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.None;
                        
                        TryToSelectShapes(_container, rectangle);
                    }
                    break;
            }
        }

        private void PointLeftDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        //var ellipse = XEllipse.Create(-3, -3, 3, 3, _container.CurrentStyle, null, true, "");
                        //var point = XPoint.Create(sx, sy, ellipse);
                        //_container.CurrentLayer.Shapes.Add(point);
                        _shape = XPoint.Create(sx, sy, _container.PointShape);
                        _container.CurrentLayer.Shapes.Add(_shape);
                        _container.Invalidate();
                    }
                    break;
            }
        }

        private void LineLeftDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        _shape = XLine.Create(
                            sx, sy,
                            _container.CurrentStyle,
                            _container.PointShape);
                        if (_tryToConnect)
                        {
                            TryToConnectStart(_shape as XLine, sx, sy);
                        }
                        _container.WorkingLayer.Shapes.Add(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var line = _shape as XLine;
                        line.End.X = sx;
                        line.End.Y = sy;
                        if (_tryToConnect)
                        {
                            TryToConnectEnd(_shape as XLine, sx, sy); 
                        }
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.CurrentLayer.Shapes.Add(_shape);
                        _container.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void RectangleLeftDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        _shape = XRectangle.Create(
                            sx, sy,
                            _container.CurrentStyle,
                            _container.PointShape,
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectTopLeft(_shape as XRectangle, sx, sy); 
                        }
                        _container.WorkingLayer.Shapes.Add(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var rectangle = _shape as XRectangle;
                        rectangle.BottomRight.X = sx;
                        rectangle.BottomRight.Y = sy;
                        if (_tryToConnect)
                        {
                            TryToConnectBottomRight(_shape as XRectangle, sx, sy); 
                        }
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.CurrentLayer.Shapes.Add(_shape);
                        _container.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void EllipseLeftDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        _shape = XEllipse.Create(
                            sx, sy,
                            _container.CurrentStyle,
                            _container.PointShape,
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectTopLeft(_shape as XEllipse, sx, sy);
                        }
                        _container.WorkingLayer.Shapes.Add(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var ellipse = _shape as XEllipse;
                        ellipse.BottomRight.X = sx;
                        ellipse.BottomRight.Y = sy;
                        if (_tryToConnect)
                        {
                            TryToConnectBottomRight(_shape as XEllipse, sx, sy);
                        }
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.CurrentLayer.Shapes.Add(_shape);
                        _container.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void ArcLeftDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        _shape = XArc.Create(
                            sx, sy,
                            _container.CurrentStyle,
                            _container.PointShape,
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectPoint1(_shape as XArc, sx, sy);
                        }
                        _container.WorkingLayer.Shapes.Add(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var arc = _shape as XArc;
                        arc.Point2.X = sx;
                        arc.Point2.Y = sy;
                        if (_tryToConnect)
                        {
                            TryToConnectPoint2(_shape as XArc, sx, sy);
                        }
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.CurrentLayer.Shapes.Add(_shape);
                        _container.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void BezierLeftDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        _shape = XBezier.Create(
                            sx, sy,
                            _container.CurrentStyle,
                            _container.PointShape,
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectPoint1(_shape as XBezier, sx, sy);
                        }
                        _container.WorkingLayer.Shapes.Add(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var bezier = _shape as XBezier;
                        bezier.Point2.X = sx;
                        bezier.Point2.Y = sy;
                        bezier.Point3.X = sx;
                        bezier.Point3.Y = sy;
                        bezier.Point4.X = sx;
                        bezier.Point4.Y = sy;
                        if (_tryToConnect)
                        {
                            TryToConnectPoint4(_shape as XBezier, sx, sy);
                        }
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.Two;
                    }
                    break;
                case State.Two:
                    {
                        var bezier = _shape as XBezier;
                        bezier.Point2.X = sx;
                        bezier.Point2.Y = sy;
                        bezier.Point3.X = sx;
                        bezier.Point3.Y = sy;
                        if (_tryToConnect)
                        {
                            TryToConnectPoint3(_shape as XBezier, sx, sy);
                        }
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.Three;
                    }
                    break;
                case State.Three:
                    {
                        var bezier = _shape as XBezier;
                        bezier.Point2.X = sx;
                        bezier.Point2.Y = sy;
                        if (_tryToConnect)
                        {
                            TryToConnectPoint2(_shape as XBezier, sx, sy);
                        }
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.CurrentLayer.Shapes.Add(_shape);
                        _container.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void QBezierLeftDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        _shape = XQBezier.Create(
                            sx, sy,
                            _container.CurrentStyle,
                            _container.PointShape,
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectPoint1(_shape as XQBezier, sx, sy);
                        }
                        _container.WorkingLayer.Shapes.Add(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var qbezier = _shape as XQBezier;
                        qbezier.Point2.X = sx;
                        qbezier.Point2.Y = sy;
                        qbezier.Point3.X = sx;
                        qbezier.Point3.Y = sy;
                        if (_tryToConnect)
                        {
                            TryToConnectPoint3(_shape as XQBezier, sx, sy);
                        }
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.Two;
                    }
                    break;
                case State.Two:
                    {
                        var qbezier = _shape as XQBezier;
                        qbezier.Point2.X = sx;
                        qbezier.Point2.Y = sy;
                        if (_tryToConnect)
                        {
                            TryToConnectPoint2(_shape as XQBezier, sx, sy);
                        }
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.CurrentLayer.Shapes.Add(_shape);
                        _container.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void TextLeftDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        _shape = XText.Create(
                            sx, sy,
                            _container.CurrentStyle,
                            _container.PointShape,
                            "Text",
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectTopLeft(_shape as XText, sx, sy);
                        }
                        _container.WorkingLayer.Shapes.Add(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var text = _shape as XText;
                        text.BottomRight.X = sx;
                        text.BottomRight.Y = sy;
                        if (_tryToConnect)
                        {
                            TryToConnectBottomRight(_shape as XText, sx, sy);
                        }
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.CurrentLayer.Shapes.Add(_shape);
                        _container.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void SelectionRightDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        IsContextMenu = TryToSelectShape(_container, sx, sy) ? true : false;
                    }
                    break;
            }
        }

        private void PointRightDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
            }
        }

        private void LineRightDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void RectangleRightDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void EllipseRightDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void ArcRightDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void BezierRightDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                case State.Two:
                case State.Three:
                    {
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void QBezierRightDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                case State.Two:
                    {
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void TextRightDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void SelectionMove(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        if (_renderer.SelectedShape != null 
                            || _renderer.SelectedShapes != null)
                        {
                            MoveSelection(sx, sy);
                            break;
                        }

                        var rectangle = _shape as XRectangle;
                        rectangle.BottomRight.X = sx;
                        rectangle.BottomRight.Y = sy;
                        _container.WorkingLayer.Invalidate();
                    }
                    break;
            }
        }

        private void PointMove(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
            }
        }

        private void LineMove(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        var line = _shape as XLine;
                        line.End.X = sx;
                        line.End.Y = sy;
                        _container.WorkingLayer.Invalidate();
                    }
                    break;
            }
        }

        private void RectangleMove(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        var rectangle = _shape as XRectangle;
                        rectangle.BottomRight.X = sx;
                        rectangle.BottomRight.Y = sy;
                        _container.WorkingLayer.Invalidate();
                    }
                    break;
            }
        }

        private void EllipseMove(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        var ellipse = _shape as XEllipse;
                        ellipse.BottomRight.X = sx;
                        ellipse.BottomRight.Y = sy;
                        _container.WorkingLayer.Invalidate();
                    }
                    break;
            }
        }

        private void ArcMove(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        var arc = _shape as XArc;
                        arc.Point2.X = sx;
                        arc.Point2.Y = sy;
                        _container.WorkingLayer.Invalidate();
                    }
                    break;
            }
        }

        private void BezierMove(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        var bezier = _shape as XBezier;
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
                        var bezier = _shape as XBezier;
                        bezier.Point2.X = sx;
                        bezier.Point2.Y = sy;
                        bezier.Point3.X = sx;
                        bezier.Point3.Y = sy;
                        _container.WorkingLayer.Invalidate();
                    }
                    break;
                case State.Three:
                    {
                        var bezier = _shape as XBezier;
                        bezier.Point2.X = sx;
                        bezier.Point2.Y = sy;
                        _container.WorkingLayer.Invalidate();
                    }
                    break;
            }
        }

        private void QBezierMove(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        var bezier = _shape as XQBezier;
                        bezier.Point2.X = sx;
                        bezier.Point2.Y = sy;
                        bezier.Point3.X = sx;
                        bezier.Point3.Y = sy;
                        _container.WorkingLayer.Invalidate();
                    }
                    break;
                case State.Two:
                    {
                        var bezier = _shape as XQBezier;
                        bezier.Point2.X = sx;
                        bezier.Point2.Y = sy;
                        _container.WorkingLayer.Invalidate();
                    }
                    break;
            }
        }

        private void TextMove(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        var text = _shape as XText;
                        text.BottomRight.X = sx;
                        text.BottomRight.Y = sy;
                        _container.WorkingLayer.Invalidate();
                    }
                    break;
            }
        }

        public Layer RemoveCurrentLayer()
        {
            var layer = Container.CurrentLayer;
            Container.Layers.Remove(layer);
            Container.CurrentLayer = Container.Layers.FirstOrDefault();
            Container.Invalidate();
            return layer;
        }

        public BaseShape RemoveCurrentShape()
        {
            var shape = Container.CurrentShape;
            Container.CurrentLayer.Shapes.Remove(shape);
            Container.CurrentShape = Container.CurrentLayer.Shapes.FirstOrDefault();
            Container.Invalidate();
            return shape;
        }

        public ShapeStyle RemoveCurrentStyle()
        {
            var style = Container.CurrentStyle;
            Container.Styles.Remove(style);
            Container.CurrentStyle = Container.Styles.FirstOrDefault();
            return style;
        }
        
        public void Load(Container container)
        {
            Renderer.ClearCache();

            Container = container;
            Container.Invalidate();

            if (EnableObserver)
            {
                Observer = new Observer(this);
            }
        }

        public void Group(IEnumerable<BaseShape> shapes, Layer layer, string name)
        {
            var group = XGroup.Create(name);

            foreach (var shape in shapes)
            {
                if (shape is XPoint)
                {
                    group.Connectors.Add(shape as XPoint);
                }
                else
                {
                    group.Shapes.Add(shape);
                }

                layer.Shapes.Remove(shape);
            }

            layer.Shapes.Add(group);
            layer.Invalidate();
        }

        public void GroupSelected()
        {
            if (_renderer.SelectedShapes != null)
            {
                Group(_renderer.SelectedShapes, Container.CurrentLayer, "g");
            }
        }

        public void GroupCurrentLayer()
        {
            var layer = Container.CurrentLayer;
            if (layer.Shapes.Count > 0)
            {
                Group(layer.Shapes.ToList(), layer, "g");
            }
        }

        public void MoveSelection(double sx, double sy)
        {
            double x = SnapToGrid ? Snap(sx, SnapX) : sx;
            double y = SnapToGrid ? Snap(sy, SnapY) : sy;

            double dx = x - _startX;
            double dy = y - _startY;

            _startX = x;
            _startY = y;

            if (_renderer.SelectedShape != null)
            {
                //_renderer.SelectedShape.Move(dx, dy);
                Move(
                    GetPoints(Enumerable.Repeat(_renderer.SelectedShape, 1)).Distinct(), 
                    dx, dy);
            }

            if (_renderer.SelectedShapes != null)
            {
                //foreach (var shape in _renderer.SelectedShapes)
                //{
                //    shape.Move(dx, dy);
                //}
                Move(
                    GetPoints(_renderer.SelectedShapes).Distinct(), 
                    dx, dy);
            }
        }

        public void DeleteSelected()
        {
            if (_renderer.SelectedShape != null)
            {
                _container.CurrentLayer.Shapes.Remove(_renderer.SelectedShape);
                _container.CurrentLayer.Invalidate();

                _renderer.SelectedShape = null;
            }

            if (_renderer.SelectedShapes != null)
            {
                var layer = _container.CurrentLayer;

                foreach (var shape in _renderer.SelectedShapes)
                {
                    layer.Shapes.Remove(shape);
                }

                _renderer.SelectedShapes = null;

                layer.Invalidate();
            }
        }
    }
}
