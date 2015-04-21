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
        private Tool _currentTool;
        private State _currentState;
        private bool _defaultIsFilled;
        private bool _snapToGrid;
        private double _snapX;
        private double _snapY;
        private ShapeStyle _selectionStyle;
        private bool _enableObserver;
        private Observer _observer;

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
                EnableObserver = true
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

        public static bool HitTest(Container container, XRectangle rectangle)
        {
            var rect = ShapeBounds.CreateRect(
                rectangle.TopLeft, 
                rectangle.BottomRight, 
                0.0, 0.0);
            
            var result = ShapeBounds.HitTest(container, rect, 6.0);
            if (result != null)
            {
                if (result.Count > 0)
                {
                    // TODO:
                    
                    return true;
                }
            }
            
            return false;
        }
        
        public static bool HitTest(Container container, double x, double y)
        {
            var result = ShapeBounds.HitTest(container, new Point2(x, y), 6.0);
            if (result != null)
            {
                container.CurrentShape = result;
                
                // TODO:
                
                return true;
            }
            
            container.CurrentShape = null;
            return false;
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
            double sx = SnapToGrid ? Snap(x, SnapX) : x;
            double sy = SnapToGrid ? Snap(y, SnapY) : y;
            switch (CurrentTool)
            {
                case Tool.None:
                    break;
                case Tool.Selection:
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
                        if (HitTest(_container, sx, sy))
                            break;

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
                    {
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
                        
                        HitTest(_container, rectangle);
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
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.Three;
                    }
                    break;
                case State.Three:
                    {
                        var bezier = _shape as XBezier;
                        bezier.Point2.X = sx;
                        bezier.Point2.Y = sy;
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
                        _container.WorkingLayer.Invalidate();
                        CurrentState = State.Two;
                    }
                    break;
                case State.Two:
                    {
                        var qbezier = _shape as XQBezier;
                        qbezier.Point2.X = sx;
                        qbezier.Point2.Y = sy;
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
                        _container.WorkingLayer.Shapes.Remove(_shape);
                        _container.CurrentLayer.Shapes.Add(_shape);
                        _container.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void LineRightDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                    }
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
                    {
                    }
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
                    {
                    }
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
                    {
                    }
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
                    {
                    }
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
                    {
                    }
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
                    {
                    }
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
                    {
                    }
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
        
        private void LineMove(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                    }
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
                    {
                    }
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
                    {
                    }
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
                    {
                    }
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
                    {
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
                    {
                    }
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
                    {
                    }
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

        public void GroupSelected()
        {
            throw new NotImplementedException();
        }

        public void GroupCurrentLayer()
        {
            var group = XGroup.Create("g");
            var layer = Container.CurrentLayer;
            foreach (var shape in layer.Shapes.ToList())
            {
                group.Shapes.Add(shape);
                layer.Shapes.Remove(shape);
            }

            layer.Shapes.Add(group);
            layer.Invalidate();
        }
    }
}
