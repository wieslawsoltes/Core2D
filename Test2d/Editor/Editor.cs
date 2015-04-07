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
        public ICommand NewCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand ToolNoneCommand { get; set; }
        public ICommand ToolLineCommand { get; set; }
        public ICommand ToolRectangleCommand { get; set; }
        public ICommand ToolEllipseCommand { get; set; }
        public ICommand ToolArcCommand { get; set; }
        public ICommand ToolBezierCommand { get; set; }
        public ICommand ToolQBezierCommand { get; set; }
        public ICommand ToolTextCommand { get; set; }

        public ICommand DefaultIsFilledCommand { get; set; }
        public ICommand SnapToGridCommand { get; set; }
        public ICommand DrawPointsCommand { get; set; }

        public ICommand AddLayerCommand { get; set; }
        public ICommand RemoveLayerCommand { get; set; }

        public ICommand AddStyleCommand { get; set; }
        public ICommand RemoveStyleCommand { get; set; }

        public ICommand RemoveShapeCommand { get; set; }

        public ICommand GroupSelectedCommand { get; set; }
        public ICommand GroupCurrentLayerCommand { get; set; }

        public ICommand LayersWindowCommand { get; set; }
        public ICommand StylesWindowCommand { get; set; }
        public ICommand ShapesWindowCommand { get; set; }
        public ICommand ContainerWindowCommand { get; set; }

        private Container _container;
        private IRenderer _renderer;
        private BaseShape _shape;
        private Tool _currentTool;
        private State _currentState;
        private bool _defaultIsFilled;
        private bool _snapToGrid;
        private double _snapX;
        private double _snapY;
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
                SnapToGrid = false,
                SnapX = 15.0,
                SnapY = 15.0,
                DefaultIsFilled = false,
                CurrentTool = Tool.Line,
                CurrentState = State.None,
                EnableObserver = true
            };

            editor.Container = container;
            editor.Renderer = renderer;

            if (editor.EnableObserver)
            {
                editor.Observer = new Observer(editor);
            }

            return editor;
        }

        public double Snap(double value, double snap)
        {
            double r = value % snap;
            return r >= snap / 2.0 ? value + snap - r : value - r;
        }

        public bool IsLeftAvailable()
        {
            return Container.CurrentLayer != null
                && Container.CurrentLayer.IsVisible
                && Container.CurrentStyle != null;
        }

        public bool IsRightAvailable()
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

        public void Left(double x, double y)
        {
            double sx = SnapToGrid ? Snap(x, SnapX) : x;
            double sy = SnapToGrid ? Snap(y, SnapY) : y;
            switch (CurrentTool)
            {
                case Tool.None:
                    {
                        NoneLeft(sx, sy);
                    }
                    break;
                case Tool.Line:
                    {
                        LineLeft(sx, sy);
                    }
                    break;
                case Tool.Rectangle:
                    {
                        RectangleLeft(sx, sy);
                    }
                    break;
                case Tool.Ellipse:
                    {
                        EllipseLeft(sx, sy);
                    }
                    break;
                case Tool.Arc:
                    {
                        ArcLeft(sx, sy);
                    }
                    break;
                case Tool.Bezier:
                    {
                        BezierLeft(sx, sy);
                    }
                    break;
                case Tool.QBezier:
                    {
                        QBezierLeft(sx, sy);
                    }
                    break;
                case Tool.Text:
                    {
                        TextLeft(sx, sy);
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
                case Tool.None:
                    {
                        NoneRight(sx, sy);
                    }
                    break;
                case Tool.Line:
                    {
                        LineRight(sx, sy);
                    }
                    break;
                case Tool.Rectangle:
                    {
                        RectangleRight(sx, sy);
                    }
                    break;
                case Tool.Ellipse:
                    {
                        EllipseRight(sx, sy);
                    }
                    break;
                case Tool.Arc:
                    {
                        ArcRight(sx, sy);
                    }
                    break;
                case Tool.Bezier:
                    {
                        BezierRight(sx, sy);
                    }
                    break;
                case Tool.QBezier:
                    {
                        QBezierRight(sx, sy);
                    }
                    break;
                case Tool.Text:
                    {
                        TextRight(sx, sy);
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
                case Tool.None:
                    {
                        NoneMove(sx, sy);
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

        private void NoneLeft(double sx, double sy)
        {
        }

        private void LineLeft(double sx, double sy)
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

        private void RectangleLeft(double sx, double sy)
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

        private void EllipseLeft(double sx, double sy)
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

        private void ArcLeft(double sx, double sy)
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

        private void BezierLeft(double sx, double sy)
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

        private void QBezierLeft(double sx, double sy)
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

        private void TextLeft(double sx, double sy)
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

        private void NoneRight(double sx, double sy)
        {
        }

        private void LineRight(double sx, double sy)
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

        private void RectangleRight(double sx, double sy)
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

        private void EllipseRight(double sx, double sy)
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

        private void ArcRight(double sx, double sy)
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

        private void BezierRight(double sx, double sy)
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

        private void QBezierRight(double sx, double sy)
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

        private void TextRight(double sx, double sy)
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

        private void NoneMove(double sx, double sy)
        {
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
