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
        private Project _project;
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
            get { return Project.CurrentDocument.CurrentContainer; }
        }

        public Project Project
        {
            get { return _project; }
            set
            {
                if (value != _project)
                {
                    _project = value;
                    Notify("Project");
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

        public Func<string> GetImagePath { get; set; }

        public static Editor Create(Project project, IRenderer renderer)
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

            editor.Project = project;
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
            {
                yield break;
            }

            foreach (var shape in shapes)
            {
                if (shape is XPoint)
                {
                    var point = shape as XPoint;

                    if (!point.State.HasFlag(ShapeState.Connector))
                    {
                        yield return shape as XPoint;
                    }
                }
                else if (shape is XLine)
                {
                    var line = shape as XLine;

                    if (!line.Start.State.HasFlag(ShapeState.Connector))
                    {
                        yield return line.Start;
                    }

                    if (!line.End.State.HasFlag(ShapeState.Connector))
                    {
                        yield return line.End;
                    }
                }
                else if (shape is XRectangle)
                {
                    var rectangle = shape as XRectangle;

                    if (!rectangle.TopLeft.State.HasFlag(ShapeState.Connector))
                    {
                        yield return rectangle.TopLeft; 
                    }

                    if (!rectangle.BottomRight.State.HasFlag(ShapeState.Connector))
                    {
                        yield return rectangle.BottomRight; 
                    }
                }
                else if (shape is XEllipse)
                {
                    var ellipse = shape as XEllipse;

                    if (!ellipse.TopLeft.State.HasFlag(ShapeState.Connector))
                    {
                        yield return ellipse.TopLeft; 
                    }

                    if (!ellipse.BottomRight.State.HasFlag(ShapeState.Connector))
                    {
                        yield return ellipse.BottomRight; 
                    }
                }
                else if (shape is XArc)
                {
                    var arc = shape as XArc;

                    if (!arc.Point1.State.HasFlag(ShapeState.Connector))
                    {
                        yield return arc.Point1; 
                    }

                    if (!arc.Point2.State.HasFlag(ShapeState.Connector))
                    {
                        yield return arc.Point2; 
                    }
                }
                else if (shape is XBezier)
                {
                    var bezier = shape as XBezier;

                    if (!bezier.Point1.State.HasFlag(ShapeState.Connector))
                    {
                        yield return bezier.Point1; 
                    }

                    if (!bezier.Point2.State.HasFlag(ShapeState.Connector))
                    {
                        yield return bezier.Point2; 
                    }

                    if (!bezier.Point3.State.HasFlag(ShapeState.Connector))
                    {
                        yield return bezier.Point3; 
                    }

                    if (!bezier.Point4.State.HasFlag(ShapeState.Connector))
                    {
                        yield return bezier.Point4; 
                    }
                }
                else if (shape is XQBezier)
                {
                    var qbezier = shape as XQBezier;

                    if (!qbezier.Point1.State.HasFlag(ShapeState.Connector))
                    {
                        yield return qbezier.Point1; 
                    }

                    if (!qbezier.Point2.State.HasFlag(ShapeState.Connector))
                    {
                        yield return qbezier.Point2; 
                    }

                    if (!qbezier.Point3.State.HasFlag(ShapeState.Connector))
                    {
                        yield return qbezier.Point3; 
                    }
                }
                else if (shape is XText)
                {
                    var text = shape as XText;

                    if (!text.TopLeft.State.HasFlag(ShapeState.Connector))
                    {
                        yield return text.TopLeft; 
                    }

                    if (!text.BottomRight.State.HasFlag(ShapeState.Connector))
                    {
                        yield return text.BottomRight; 
                    }
                }
                else if (shape is XImage)
                {
                    var image = shape as XImage;

                    if (!image.TopLeft.State.HasFlag(ShapeState.Connector))
                    {
                        yield return image.TopLeft;
                    }

                    if (!image.BottomRight.State.HasFlag(ShapeState.Connector))
                    {
                        yield return image.BottomRight;
                    }
                }
                else if (shape is XGroup)
                {
                    var group = shape as XGroup;

                    foreach (var point in GetPoints(group.Shapes))
                    {
                        if (!point.State.HasFlag(ShapeState.Connector))
                        {
                            yield return point; 
                        }
                    }

                    foreach (var point in group.Connectors)
                    {
                        yield return point;
                    }
                }
            }
        }

        public static IEnumerable<BaseShape> GetShapes(IEnumerable<BaseShape> shapes)
        {
            if (shapes == null)
            {
                yield break;
            }

            foreach (var shape in shapes)
            {
                if (shape is XPoint)
                {
                    yield return shape;
                }
                else if (shape is XLine)
                {
                    yield return shape;
                }
                else if (shape is XRectangle)
                {
                    yield return shape;
                }
                else if (shape is XEllipse)
                {
                    yield return shape;
                }
                else if (shape is XArc)
                {
                    yield return shape;
                }
                else if (shape is XBezier)
                {
                    yield return shape;
                }
                else if (shape is XQBezier)
                {
                    yield return shape;
                }
                else if (shape is XText)
                {
                    yield return shape;
                }
                else if (shape is XImage)
                {
                    yield return shape;
                }
                else if (shape is XGroup)
                {
                    foreach (var s in GetShapes((shape as XGroup).Shapes))
                    {
                        yield return s;
                    }

                    yield return shape;
                }
            }
        }

        public static void Move(IEnumerable<XPoint> points, double dx, double dy)
        {
            foreach (var point in points)
            {
                if (!point.State.HasFlag(ShapeState.Locked))
                {
                    point.Move(dx, dy);
                }
            }
        }

        public void RemoveCurrentGroupLibrary()
        {
            var gl = Project.CurrentGroupLibrary;
            if (gl != null)
            {
                Project.GroupLibraries.Remove(gl);
                Project.CurrentGroupLibrary = Project.GroupLibraries.FirstOrDefault();
            }
        }

        public void RemoveCurrentGroup()
        {
            var group = Project.CurrentGroupLibrary.CurrentGroup;
            if (group != null)
            {
                Project.CurrentGroupLibrary.Groups.Remove(group);
                Project.CurrentGroupLibrary.CurrentGroup = Project.CurrentGroupLibrary.Groups.FirstOrDefault();
            }
        }

        public void RemoveCurrentLayer()
        {
            var layer = Container.CurrentLayer;
            if (layer != null)
            {
                Container.Layers.Remove(layer);
                Container.CurrentLayer = Container.Layers.FirstOrDefault();
                Container.Invalidate();
            }
        }

        public void RemoveCurrentShape()
        {
            var shape = Container.CurrentShape;
            if (shape != null)
            {
                Container.CurrentLayer.Shapes.Remove(shape);
                Container.CurrentShape = Container.CurrentLayer.Shapes.FirstOrDefault();
                Container.Invalidate();
            }
        }

        public void RemoveCurrentStyleGroup()
        {
            var sg = Project.CurrentStyleGroup;
            if (sg != null)
            {
                Project.StyleGroups.Remove(sg);
                Project.CurrentStyleGroup = Project.StyleGroups.FirstOrDefault();
            }
        }

        public void RemoveCurrentStyle()
        {
            var style = Project.CurrentStyleGroup.CurrentStyle;
            if (style != null)
            {
                Project.CurrentStyleGroup.Styles.Remove(style);
                Project.CurrentStyleGroup.CurrentStyle = Project.CurrentStyleGroup.Styles.FirstOrDefault();
            }
        }

        public void Load(Project project)
        {
            Renderer.ClearCache();

            Project = project;
            Container.Invalidate();

            if (EnableObserver)
            {
                Observer = new Observer(this);
            }
        }

        public void GroupSelected()
        {
            var layer = Container.CurrentLayer;
            if (_renderer.SelectedShapes != null)
            {
                var g = XGroup.Group("g", _renderer.SelectedShapes);

                foreach (var shape in _renderer.SelectedShapes)
                {
                    layer.Shapes.Remove(shape);
                }

                layer.Shapes.Add(g);
                Select(Container, g);
            }
        }

        public void GroupCurrentLayer()
        {
            var layer = Container.CurrentLayer;
            if (layer.Shapes.Count > 0)
            {
                var g = XGroup.Group("g", layer.Shapes);

                foreach (var shape in layer.Shapes.ToList())
                {
                    layer.Shapes.Remove(shape);
                }

                layer.Shapes.Add(g);
                Select(Container, g);
            }
        }

        public void MoveSelection(double x, double y)
        {
            double sx = SnapToGrid ? Snap(x, SnapX) : x;
            double sy = SnapToGrid ? Snap(y, SnapY) : y;

            double dx = sx - _startX;
            double dy = sy - _startY;

            _startX = sx;
            _startY = sy;

            if (_renderer.SelectedShape != null)
            {
                if (!_renderer.SelectedShape.State.HasFlag(ShapeState.Locked))
                {
                    //_renderer.SelectedShape.Move(dx, dy);
                    Move(
                        GetPoints(Enumerable.Repeat(_renderer.SelectedShape, 1)).Distinct(),
                        dx, dy);
                }
            }

            if (_renderer.SelectedShapes != null)
            {
                //foreach (var shape in _renderer.SelectedShapes)
                //{
                //    shape.Move(dx, dy);
                //}
                Move(
                    GetPoints(_renderer.SelectedShapes.Where(s => !s.State.HasFlag(ShapeState.Locked))).Distinct(),
                    dx, dy);
            }
        }

        public void DeleteSelected()
        {
            if (_renderer.SelectedShape != null)
            {
                Container.CurrentLayer.Shapes.Remove(_renderer.SelectedShape);
                Container.CurrentLayer.Invalidate();

                _renderer.SelectedShape = null;
            }

            if (_renderer.SelectedShapes != null)
            {
                var layer = Container.CurrentLayer;

                foreach (var shape in _renderer.SelectedShapes)
                {
                    layer.Shapes.Remove(shape);
                }

                _renderer.SelectedShapes = null;

                layer.Invalidate();
            }
        }

        public void Select(Container container, BaseShape shape)
        {
            container.CurrentShape = shape;
            _renderer.SelectedShape = shape;
            _renderer.SelectedShapes = null;
            container.CurrentLayer.Invalidate();
        }

        public void Select(Container container, ICollection<BaseShape> shapes)
        {
            container.CurrentShape = null;
            _renderer.SelectedShape = null;
            _renderer.SelectedShapes = shapes;
            container.CurrentLayer.Invalidate();
        }

        public void Deselect(Container container)
        {
            container.CurrentShape = null;
            _renderer.SelectedShape = null;
            _renderer.SelectedShapes = null;
            container.CurrentLayer.Invalidate();
        }

        public bool TryToSelectShapes(Container container, XRectangle rectangle)
        {
            var rect = Rect2.Create(rectangle.TopLeft, rectangle.BottomRight);

            var result = ShapeBounds.HitTest(container, rect, _hitTreshold);
            if (result != null)
            {
                if (result.Count > 0)
                {
                    if (result.Count == 1)
                    {
                        Select(container, result.FirstOrDefault());
                    }
                    else
                    {
                        Select(container, result);
                    }
                    return true;
                }
            }

            Deselect(container);

            return false;
        }

        public bool TryToSelectShape(Container container, double x, double y)
        {
            var result = ShapeBounds.HitTest(container, new Vector2(x, y), _hitTreshold);
            if (result != null)
            {
                Select(container, result);
                return true;
            }

            Deselect(container);

            return false;
        }

        public void TryToConnectStart(XLine line, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                line.Start = result as XPoint;
            }
        }

        public void TryToConnectEnd(XLine line, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                line.End = result as XPoint;
            }
        }

        public void TryToConnectTopLeft(XRectangle rectangle, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                rectangle.TopLeft = result as XPoint;
            }
        }

        public void TryToConnectBottomRight(XRectangle rectangle, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                rectangle.BottomRight = result as XPoint;
            }
        }

        public void TryToConnectTopLeft(XEllipse ellipse, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                ellipse.TopLeft = result as XPoint;
            }
        }

        public void TryToConnectBottomRight(XEllipse ellipse, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                ellipse.BottomRight = result as XPoint;
            }
        }

        public void TryToConnectPoint1(XArc arc, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point1 = result as XPoint;
            }
        }

        public void TryToConnectPoint2(XArc arc, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point2 = result as XPoint;
            }
        }

        public void TryToConnectPoint1(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point1 = result as XPoint;
            }
        }

        public void TryToConnectPoint2(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point2 = result as XPoint;
            }
        }

        public void TryToConnectPoint3(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point3 = result as XPoint;
            }
        }

        public void TryToConnectPoint4(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point4 = result as XPoint;
            }
        }

        public void TryToConnectPoint1(XQBezier qbezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point1 = result as XPoint;
            }
        }

        public void TryToConnectPoint2(XQBezier qbezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point2 = result as XPoint;
            }
        }

        public void TryToConnectPoint3(XQBezier qbezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point3 = result as XPoint;
            }
        }

        public void TryToConnectTopLeft(XText text, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                text.TopLeft = result as XPoint;
            }
        }

        public void TryToConnectBottomRight(XText text, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                text.BottomRight = result as XPoint;
            }
        }

        public void TryToConnectTopLeft(XImage image, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                image.TopLeft = result as XPoint;
            }
        }

        public void TryToConnectBottomRight(XImage image, double x, double y)
        {
            var result = ShapeBounds.HitTest(Container, new Vector2(x, y), _hitTreshold);
            if (result != null && result is XPoint)
            {
                image.BottomRight = result as XPoint;
            }
        }

        public bool IsLeftDownAvailable()
        {
            return Container.CurrentLayer != null
                && Container.CurrentLayer.IsVisible
                && Project.CurrentStyleGroup != null
                && Project.CurrentStyleGroup.CurrentStyle != null;
        }

        public bool IsLeftUpAvailable()
        {
            return Container.CurrentLayer != null
                && Container.CurrentLayer.IsVisible
                && Project.CurrentStyleGroup != null
                && Project.CurrentStyleGroup.CurrentStyle != null;
        }
        
        public bool IsRightDownAvailable()
        {
            return Container.CurrentLayer != null
                && Container.CurrentLayer.IsVisible
                && Project.CurrentStyleGroup != null
                && Project.CurrentStyleGroup.CurrentStyle != null;
        }
        
        public bool IsRightUpAvailable()
        {
            return Container.CurrentLayer != null
                && Container.CurrentLayer.IsVisible
                && Project.CurrentStyleGroup != null
                && Project.CurrentStyleGroup.CurrentStyle != null;
        }

        public bool IsMoveAvailable()
        {
            return Container.CurrentLayer != null
                && Container.CurrentLayer.IsVisible
                && Project.CurrentStyleGroup != null
                && Project.CurrentStyleGroup.CurrentStyle != null;
        }

        public bool IsSelectionAvailable()
        {
            return _renderer.SelectedShape != null
                || _renderer.SelectedShapes != null;
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
                            var result = ShapeBounds.HitTest(Container, new Vector2(sx, sy), _hitTreshold);
                            if (result != null)
                            {
                                _startX = SnapToGrid ? Snap(sx, SnapX) : sx;
                                _startY = SnapToGrid ? Snap(sy, SnapY) : sy;
                                IsContextMenu = false;
                                CurrentState = State.One;
                                break;
                            }
                        }

                        if (TryToSelectShape(Container, sx, sy))
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
                        Container.WorkingLayer.Shapes.Add(_shape);
                        Container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var rectangle = _shape as XRectangle;
                        if (rectangle != null)
                        {
                            rectangle.BottomRight.X = sx;
                            rectangle.BottomRight.Y = sy;
                            Container.WorkingLayer.Shapes.Remove(_shape);
                            Container.WorkingLayer.Invalidate();
                            CurrentState = State.None;
                        }
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
                        if (IsSelectionAvailable())
                        {
                            CurrentState = State.None;
                            break;
                        }

                        var rectangle = _shape as XRectangle;
                        if (rectangle != null)
                        {
                            rectangle.BottomRight.X = sx;
                            rectangle.BottomRight.Y = sy;
                            Container.WorkingLayer.Shapes.Remove(_shape);
                            Container.WorkingLayer.Invalidate();
                            CurrentState = State.None;

                            TryToSelectShapes(Container, rectangle);
                        }
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
                        _shape = XPoint.Create(sx, sy, Project.PointShape);
                        Container.CurrentLayer.Shapes.Add(_shape);
                        Container.Invalidate();
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
                            Project.CurrentStyleGroup.CurrentStyle,
                            Project.PointShape);
                        if (_tryToConnect)
                        {
                            TryToConnectStart(_shape as XLine, sx, sy);
                        }
                        Container.WorkingLayer.Shapes.Add(_shape);
                        Container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var line = _shape as XLine;
                        if (line != null)
                        {
                            line.End.X = sx;
                            line.End.Y = sy;
                            if (_tryToConnect)
                            {
                                TryToConnectEnd(_shape as XLine, sx, sy);
                            }
                            Container.WorkingLayer.Shapes.Remove(_shape);
                            Container.CurrentLayer.Shapes.Add(_shape);
                            Container.Invalidate();
                            CurrentState = State.None;
                        }
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
                            Project.CurrentStyleGroup.CurrentStyle,
                            Project.PointShape,
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectTopLeft(_shape as XRectangle, sx, sy);
                        }
                        Container.WorkingLayer.Shapes.Add(_shape);
                        Container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var rectangle = _shape as XRectangle;
                        if (rectangle != null)
                        {
                            rectangle.BottomRight.X = sx;
                            rectangle.BottomRight.Y = sy;
                            if (_tryToConnect)
                            {
                                TryToConnectBottomRight(_shape as XRectangle, sx, sy);
                            }
                            Container.WorkingLayer.Shapes.Remove(_shape);
                            Container.CurrentLayer.Shapes.Add(_shape);
                            Container.Invalidate();
                            CurrentState = State.None;
                        }
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
                            Project.CurrentStyleGroup.CurrentStyle,
                            Project.PointShape,
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectTopLeft(_shape as XEllipse, sx, sy);
                        }
                        Container.WorkingLayer.Shapes.Add(_shape);
                        Container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var ellipse = _shape as XEllipse;
                        if (ellipse != null)
                        {
                            ellipse.BottomRight.X = sx;
                            ellipse.BottomRight.Y = sy;
                            if (_tryToConnect)
                            {
                                TryToConnectBottomRight(_shape as XEllipse, sx, sy);
                            }
                            Container.WorkingLayer.Shapes.Remove(_shape);
                            Container.CurrentLayer.Shapes.Add(_shape);
                            Container.Invalidate();
                            CurrentState = State.None;
                        }
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
                            Project.CurrentStyleGroup.CurrentStyle,
                            Project.PointShape,
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectPoint1(_shape as XArc, sx, sy);
                        }
                        Container.WorkingLayer.Shapes.Add(_shape);
                        Container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var arc = _shape as XArc;
                        if (arc != null)
                        {
                            arc.Point2.X = sx;
                            arc.Point2.Y = sy;
                            if (_tryToConnect)
                            {
                                TryToConnectPoint2(_shape as XArc, sx, sy);
                            }
                            Container.WorkingLayer.Shapes.Remove(_shape);
                            Container.CurrentLayer.Shapes.Add(_shape);
                            Container.Invalidate();
                            CurrentState = State.None;
                        }
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
                            Project.CurrentStyleGroup.CurrentStyle,
                            Project.PointShape,
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectPoint1(_shape as XBezier, sx, sy);
                        }
                        Container.WorkingLayer.Shapes.Add(_shape);
                        Container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var bezier = _shape as XBezier;
                        if (bezier != null)
                        {
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
                            Container.WorkingLayer.Invalidate();
                            CurrentState = State.Two;
                        }
                    }
                    break;
                case State.Two:
                    {
                        var bezier = _shape as XBezier;
                        if (bezier != null)
                        {
                            bezier.Point2.X = sx;
                            bezier.Point2.Y = sy;
                            bezier.Point3.X = sx;
                            bezier.Point3.Y = sy;
                            if (_tryToConnect)
                            {
                                TryToConnectPoint3(_shape as XBezier, sx, sy);
                            }
                            Container.WorkingLayer.Invalidate();
                            CurrentState = State.Three;
                        }
                    }
                    break;
                case State.Three:
                    {
                        var bezier = _shape as XBezier;
                        if (bezier != null)
                        {
                            bezier.Point2.X = sx;
                            bezier.Point2.Y = sy;
                            if (_tryToConnect)
                            {
                                TryToConnectPoint2(_shape as XBezier, sx, sy);
                            }
                            Container.WorkingLayer.Shapes.Remove(_shape);
                            Container.CurrentLayer.Shapes.Add(_shape);
                            Container.Invalidate();
                            CurrentState = State.None;
                        }
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
                            Project.CurrentStyleGroup.CurrentStyle,
                            Project.PointShape,
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectPoint1(_shape as XQBezier, sx, sy);
                        }
                        Container.WorkingLayer.Shapes.Add(_shape);
                        Container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var qbezier = _shape as XQBezier;
                        if (qbezier != null)
                        {
                            qbezier.Point2.X = sx;
                            qbezier.Point2.Y = sy;
                            qbezier.Point3.X = sx;
                            qbezier.Point3.Y = sy;
                            if (_tryToConnect)
                            {
                                TryToConnectPoint3(_shape as XQBezier, sx, sy);
                            }
                            Container.WorkingLayer.Invalidate();
                            CurrentState = State.Two;
                        }
                    }
                    break;
                case State.Two:
                    {
                        var qbezier = _shape as XQBezier;
                        if (qbezier != null)
                        {
                            qbezier.Point2.X = sx;
                            qbezier.Point2.Y = sy;
                            if (_tryToConnect)
                            {
                                TryToConnectPoint2(_shape as XQBezier, sx, sy);
                            }
                            Container.WorkingLayer.Shapes.Remove(_shape);
                            Container.CurrentLayer.Shapes.Add(_shape);
                            Container.Invalidate();
                            CurrentState = State.None;
                        }
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
                            Project.CurrentStyleGroup.CurrentStyle,
                            Project.PointShape,
                            "Text",
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectTopLeft(_shape as XText, sx, sy);
                        }
                        Container.WorkingLayer.Shapes.Add(_shape);
                        Container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var text = _shape as XText;
                        if (text != null)
                        {
                            text.BottomRight.X = sx;
                            text.BottomRight.Y = sy;
                            if (_tryToConnect)
                            {
                                TryToConnectBottomRight(_shape as XText, sx, sy);
                            }
                            Container.WorkingLayer.Shapes.Remove(_shape);
                            Container.CurrentLayer.Shapes.Add(_shape);
                            Container.Invalidate();
                            CurrentState = State.None;
                        }
                    }
                    break;
            }
        }

        private void ImageLeftDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        var path = GetImagePath();
                        if (string.IsNullOrEmpty(path))
                            return;

                        var uri = new Uri(path);

                        _shape = XImage.Create(
                            sx, sy,
                            Project.CurrentStyleGroup.CurrentStyle,
                            Project.PointShape,
                            uri,
                            DefaultIsFilled);
                        if (_tryToConnect)
                        {
                            TryToConnectTopLeft(_shape as XImage, sx, sy);
                        }
                        Container.WorkingLayer.Shapes.Add(_shape);
                        Container.WorkingLayer.Invalidate();
                        CurrentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var image = _shape as XImage;
                        if (image != null)
                        {
                            image.BottomRight.X = sx;
                            image.BottomRight.Y = sy;
                            if (_tryToConnect)
                            {
                                TryToConnectBottomRight(_shape as XImage, sx, sy);
                            }
                            Container.WorkingLayer.Shapes.Remove(_shape);
                            Container.CurrentLayer.Shapes.Add(_shape);
                            Container.Invalidate();
                            CurrentState = State.None;
                        }
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
                        IsContextMenu = TryToSelectShape(Container, sx, sy) ? true : false;
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
                        Container.WorkingLayer.Shapes.Remove(_shape);
                        Container.WorkingLayer.Invalidate();
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
                        Container.WorkingLayer.Shapes.Remove(_shape);
                        Container.WorkingLayer.Invalidate();
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
                        Container.WorkingLayer.Shapes.Remove(_shape);
                        Container.WorkingLayer.Invalidate();
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
                        Container.WorkingLayer.Shapes.Remove(_shape);
                        Container.WorkingLayer.Invalidate();
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
                        Container.WorkingLayer.Shapes.Remove(_shape);
                        Container.WorkingLayer.Invalidate();
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
                        Container.WorkingLayer.Shapes.Remove(_shape);
                        Container.WorkingLayer.Invalidate();
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
                        Container.WorkingLayer.Shapes.Remove(_shape);
                        Container.WorkingLayer.Invalidate();
                        CurrentState = State.None;
                    }
                    break;
            }
        }

        private void ImageRightDown(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        Container.WorkingLayer.Shapes.Remove(_shape);
                        Container.WorkingLayer.Invalidate();
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
                        if (IsSelectionAvailable())
                        {
                            MoveSelection(sx, sy);
                            break;
                        }

                        var rectangle = _shape as XRectangle;
                        if (rectangle != null)
                        {
                            rectangle.BottomRight.X = sx;
                            rectangle.BottomRight.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
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
                        if (line != null)
                        {
                            line.End.X = sx;
                            line.End.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
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
                        if (rectangle != null)
                        {
                            rectangle.BottomRight.X = sx;
                            rectangle.BottomRight.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
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
                        if (ellipse != null)
                        {
                            ellipse.BottomRight.X = sx;
                            ellipse.BottomRight.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
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
                        if (arc != null)
                        {
                            arc.Point2.X = sx;
                            arc.Point2.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
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
                        if (bezier != null)
                        {
                            bezier.Point2.X = sx;
                            bezier.Point2.Y = sy;
                            bezier.Point3.X = sx;
                            bezier.Point3.Y = sy;
                            bezier.Point4.X = sx;
                            bezier.Point4.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
                    }
                    break;
                case State.Two:
                    {
                        var bezier = _shape as XBezier;
                        if (bezier != null)
                        {
                            bezier.Point2.X = sx;
                            bezier.Point2.Y = sy;
                            bezier.Point3.X = sx;
                            bezier.Point3.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
                    }
                    break;
                case State.Three:
                    {
                        var bezier = _shape as XBezier;
                        if (bezier != null)
                        {
                            bezier.Point2.X = sx;
                            bezier.Point2.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
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
                        var qbezier = _shape as XQBezier;
                        if (qbezier != null)
                        {
                            qbezier.Point2.X = sx;
                            qbezier.Point2.Y = sy;
                            qbezier.Point3.X = sx;
                            qbezier.Point3.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
                    }
                    break;
                case State.Two:
                    {
                        var qbezier = _shape as XQBezier;
                        if (qbezier != null)
                        {
                            qbezier.Point2.X = sx;
                            qbezier.Point2.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
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
                        if (text != null)
                        {
                            text.BottomRight.X = sx;
                            text.BottomRight.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
                    }
                    break;
            }
        }

        private void ImageMove(double sx, double sy)
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        var image = _shape as XImage;
                        if (image != null)
                        {
                            image.BottomRight.X = sx;
                            image.BottomRight.Y = sy;
                            Container.WorkingLayer.Invalidate();
                        }
                    }
                    break;
            }
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
                case Tool.Image:
                    {
                        ImageLeftDown(sx, sy);
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
                case Tool.Image:
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
                case Tool.Image:
                    {
                        ImageRightDown(sx, sy);
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
                case Tool.Image:
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
                case Tool.Image:
                    {
                        ImageMove(sx, sy);
                    }
                    break;
            }
        }
    }
}
