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
    /// <summary>
    /// 
    /// </summary>
    public class Editor : ObservableObject
    {
        private Project _project;
        private IRenderer _renderer;
        private BaseShape _shape;
        private State _currentState = State.None;
        private Tool _currentTool = Tool.Selection;
        private bool _isContextMenu;
        private bool _enableObserver = true;
        private Observer _observer;
        private History<Project> _history;
        private double _startX;
        private double _startY;
        private double _historyX;
        private double _historyY;
        private bool _enableArcHelper = true;
        private ArcHelper _arcHelper;

        /// <summary>
        /// Gets or sets current project.
        /// </summary>
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

        /// <summary>
        /// Gets or sets current renderer.
        /// </summary>
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

        /// <summary>
        /// Gets or sets current editor state.
        /// </summary>
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

        /// <summary>
        /// Gets or sets current editor tool.
        /// </summary>
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

        /// <summary>
        /// Gets or sets if context menu is about to open.
        /// </summary>
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
        
        /// <summary>
        /// Gets or sets if project collections and objects observer is enabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets current project collections and objects observer.
        /// </summary>
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

        /// <summary>
        /// Gets or sets undo/redo history handler.
        /// </summary>
        public History<Project> History
        {
            get { return _history; }
            set
            {
                if (value != _history)
                {
                    _history = value;
                    Notify("History");
                }
            }
        }

        /// <summary>
        /// Get image path using common system open file dialog.
        /// </summary>
        public Func<string> GetImagePath { get; set; }

        /// <summary>
        /// Creates a new Editor instance.
        /// </summary>
        /// <param name="project">The project to edit.</param>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="history">The undo/redo history handler.</param>
        /// <returns></returns>
        public static Editor Create(Project project, IRenderer renderer, History<Project> history)
        {
            var editor = new Editor()
            {
                CurrentState = State.None,
                CurrentTool = Tool.Selection,
                EnableObserver = true,
                History = history,
            };

            editor.Project = project;
            editor.Renderer = renderer;
            
            if (editor.EnableObserver)
            {
                editor.Observer = new Observer(editor);
            }

            return editor;
        }

        /// <summary>
        /// Snaps value by specified snap parameter.
        /// </summary>
        /// <param name="value">The value to snap.</param>
        /// <param name="snap">The snap amount.</param>
        /// <returns>The snapped value.</returns>
        public static double Snap(double value, double snap)
        {
            double r = value % snap;
            return r >= snap / 2.0 ? value + snap - r : value - r;
        }

        /// <summary>
        /// Sets image Path property to relative Uri using the specified base Uri.
        /// </summary>
        /// <param name="baseUri">The base absolute Uri.</param>
        /// <param name="images">The collection if XImage shapes.</param>
        public void ToRelativeUri(Uri baseUri, IEnumerable<XImage> images)
        {
            foreach (var image in images)
            {
                var relative = baseUri.MakeRelativeUri(image.Path);
                image.Path = relative;
            }
        }

        /// <summary>
        /// Sets image Path property to absolute Uri using the specified base Uri.
        /// </summary>
        /// <param name="baseUri">The base absolute Uri.</param>
        /// <param name="images">The collection if XImage shapes.</param>
        public void ToAbsoluteUri(Uri baseUri, IEnumerable<XImage> images)
        {
            foreach (var image in images)
            {
                var absolute = new Uri(baseUri, image.Path);
                image.Path = absolute;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <returns></returns>
        public static IEnumerable<XPoint> GetAllPoints(IEnumerable<BaseShape> shapes)
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

                    if (!arc.Point3.State.HasFlag(ShapeState.Connector))
                    {
                        yield return arc.Point3;
                    }

                    if (!arc.Point4.State.HasFlag(ShapeState.Connector))
                    {
                        yield return arc.Point4;
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

                    foreach (var point in GetAllPoints(group.Shapes))
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <returns></returns>
        public static IEnumerable<BaseShape> GetAllShapes(IEnumerable<BaseShape> shapes)
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
                    foreach (var s in GetAllShapes((shape as XGroup).Shapes))
                    {
                        yield return s;
                    }

                    yield return shape;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="project"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllShapes<T>(Project project)
        {
            var shapes = project.Documents
                .SelectMany(d => d.Containers)
                .SelectMany(c => c.Layers)
                .SelectMany(l => l.Shapes);

            return GetAllShapes(shapes)
                .Where(s => s is T)
                .Cast<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
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

        /// <summary>
        /// Removes the Project.CurrentGroupLibrary object from the Project.GroupLibraries collection.
        /// </summary>
        public void RemoveCurrentGroupLibrary()
        {
            var gl = Project.CurrentGroupLibrary;
            if (gl != null)
            {
                _history.Snapshot(_project);
                Project.GroupLibraries.Remove(gl);
                Project.CurrentGroupLibrary = Project.GroupLibraries.FirstOrDefault();
            }
        }

        /// <summary>
        /// Removes the Project.CurrentGroupLibrary.CurrentGroup object from the Project.CurrentGroupLibrary.Groups collection.
        /// </summary>
        public void RemoveCurrentGroup()
        {
            var group = Project.CurrentGroupLibrary.CurrentGroup;
            if (group != null)
            {
                _history.Snapshot(_project);
                Project.CurrentGroupLibrary.Groups.Remove(group);
                Project.CurrentGroupLibrary.CurrentGroup = Project.CurrentGroupLibrary.Groups.FirstOrDefault();
            }
        }

        /// <summary>
        /// Removes the Container.CurrentLayer object from the Container.Layers collection.
        /// </summary>
        public void RemoveCurrentLayer()
        {
            var layer = Project.CurrentContainer.CurrentLayer;
            if (layer != null)
            {
                _history.Snapshot(_project);
                Project.CurrentContainer.Layers.Remove(layer);
                Project.CurrentContainer.CurrentLayer = Project.CurrentContainer.Layers.FirstOrDefault();
                //Project.CurrentContainer.Invalidate();
            }
        }

        /// <summary>
        /// Removes the Container.CurrentShape object from the Container.CurrentLayer.Shapes collection.
        /// </summary>
        public void RemoveCurrentShape()
        {
            var shape = Project.CurrentContainer.CurrentShape;
            if (shape != null)
            {
                _history.Snapshot(_project);
                Project.CurrentContainer.CurrentLayer.Shapes.Remove(shape);
                Project.CurrentContainer.CurrentShape = Project.CurrentContainer.CurrentLayer.Shapes.FirstOrDefault();
                //Project.CurrentContainer.Invalidate();
            }
        }

        /// <summary>
        /// Removed the Project.CurrentStyleGroup object from the Project.StyleGroups collection.
        /// </summary>
        public void RemoveCurrentStyleGroup()
        {
            var sg = Project.CurrentStyleGroup;
            if (sg != null)
            {
                _history.Snapshot(_project);
                Project.StyleGroups.Remove(sg);
                Project.CurrentStyleGroup = Project.StyleGroups.FirstOrDefault();
            }
        }

        /// <summary>
        /// Removes the Project.CurrentStyleGroup.CurrentStyle object from the Project.CurrentStyleGroup.Styles collection.
        /// </summary>
        public void RemoveCurrentStyle()
        {
            var style = Project.CurrentStyleGroup.CurrentStyle;
            if (style != null)
            {
                _history.Snapshot(_project);
                Project.CurrentStyleGroup.Styles.Remove(style);
                Project.CurrentStyleGroup.CurrentStyle = Project.CurrentStyleGroup.Styles.FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        public void Load(Project project)
        {
            Renderer.ClearCache();
            
            Project = project;
            //Project.CurrentContainer.Invalidate();

            if (EnableObserver)
            {
                Observer = new Observer(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void GroupSelected()
        {
            var layer = Project.CurrentContainer.CurrentLayer;
            if (_renderer.SelectedShapes != null)
            {
                _history.Snapshot(_project);
  
                var g = XGroup.Group("g", _renderer.SelectedShapes);

                foreach (var shape in _renderer.SelectedShapes)
                {
                    layer.Shapes.Remove(shape);
                }

                layer.Shapes.Add(g);
                Select(Project.CurrentContainer, g);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void GroupCurrentLayer()
        {
            var layer = Project.CurrentContainer.CurrentLayer;
            if (layer.Shapes.Count > 0)
            {
                _history.Snapshot(_project);
                
                var g = XGroup.Group("g", layer.Shapes);

                foreach (var shape in layer.Shapes.ToList())
                {
                    layer.Shapes.Remove(shape);
                }

                layer.Shapes.Add(g);
                Select(Project.CurrentContainer, g);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveSelection(double x, double y)
        {
            double sx = _project.Options.SnapToGrid ? Snap(x, _project.Options.SnapX) : x;
            double sy = _project.Options.SnapToGrid ? Snap(y, _project.Options.SnapY) : y;

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
                        GetAllPoints(Enumerable.Repeat(_renderer.SelectedShape, 1)).Distinct(),
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
                    GetAllPoints(_renderer.SelectedShapes.Where(s => !s.State.HasFlag(ShapeState.Locked))).Distinct(),
                    dx, dy);
            }
        }

        /// <summary>
        /// Removes container object from owner document Containers collection.
        /// </summary>
        /// <param name="container">The container object to remove from document Containers collection.</param>
        public void Delete(Container container)
        {
            var document = _project.Documents.FirstOrDefault(d => d.Containers.Contains(container));
            if (document != null)
            {
                _history.Snapshot(_project);
                document.Containers.Remove(container);
                _project.CurrentDocument = document;
                _project.CurrentContainer = document.Containers.FirstOrDefault();
            }
        }
        
        /// <summary>
        /// Removes document object from project Documents collection.
        /// </summary>
        /// <param name="document">The document object to remove from project Documents collection.</param>
        public void Delete(Document document)
        {
            _history.Snapshot(_project);
            _project.Documents.Remove(document);
            _project.CurrentDocument = _project.Documents.FirstOrDefault();
            if (_project.CurrentDocument != null)
            {
                _project.CurrentContainer = _project.CurrentDocument.Containers.FirstOrDefault();
            }
            else
            {
                _project.CurrentContainer = default(Container);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void DeleteSelected()
        {
            if (_renderer.SelectedShape != null)
            {
                _history.Snapshot(_project);

                Project.CurrentContainer.CurrentLayer.Shapes.Remove(_renderer.SelectedShape);
                Project.CurrentContainer.CurrentLayer.Invalidate();

                _renderer.SelectedShape = default(BaseShape);
            }

            if (_renderer.SelectedShapes != null && _renderer.SelectedShapes.Count > 0)
            {
                _history.Snapshot(_project);
                
                var layer = Project.CurrentContainer.CurrentLayer;

                foreach (var shape in _renderer.SelectedShapes)
                {
                    layer.Shapes.Remove(shape);
                }

                _renderer.SelectedShapes = default(ICollection<BaseShape>);

                layer.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="shape"></param>
        public void Select(Container container, BaseShape shape)
        {
            container.CurrentShape = shape;
            _renderer.SelectedShape = shape;
            _renderer.SelectedShapes = default(ICollection<BaseShape>);
            container.CurrentLayer.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="shapes"></param>
        public void Select(Container container, ICollection<BaseShape> shapes)
        {
            container.CurrentShape = default(BaseShape);
            _renderer.SelectedShape = default(BaseShape);
            _renderer.SelectedShapes = shapes;
            container.CurrentLayer.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public void Deselect(Container container)
        {
            container.CurrentShape = default(BaseShape);
            _renderer.SelectedShape = default(BaseShape);
            _renderer.SelectedShapes = default(ICollection<BaseShape>);
            container.CurrentLayer.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public bool TryToSelectShapes(Container container, XRectangle rectangle)
        {
            var rect = Rect2.Create(rectangle.TopLeft, rectangle.BottomRight);

            var result = ShapeBounds.HitTest(container, rect, _project.Options.HitTreshold);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool TryToSelectShape(Container container, double x, double y)
        {
            var result = ShapeBounds.HitTest(container, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null)
            {
                Select(container, result);
                return true;
            }

            Deselect(container);

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectStart(XLine line, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                line.Start = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectEnd(XLine line, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                line.End = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectTopLeft(XRectangle rectangle, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                rectangle.TopLeft = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectBottomRight(XRectangle rectangle, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                rectangle.BottomRight = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ellipse"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectTopLeft(XEllipse ellipse, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                ellipse.TopLeft = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ellipse"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectBottomRight(XEllipse ellipse, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                ellipse.BottomRight = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint1(XArc arc, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point1 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint2(XArc arc, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point2 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint3(XArc arc, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point3 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint4(XArc arc, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point4 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint1(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point1 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint2(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point2 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint3(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point3 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint4(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point4 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qbezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint1(XQBezier qbezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point1 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qbezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint2(XQBezier qbezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point2 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qbezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint3(XQBezier qbezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point3 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectTopLeft(XText text, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                text.TopLeft = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectBottomRight(XText text, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                text.BottomRight = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectTopLeft(XImage image, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                image.TopLeft = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectBottomRight(XImage image, double x, double y)
        {
            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                image.BottomRight = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLeftDownAvailable()
        {
            return Project.CurrentContainer != null
                && Project.CurrentContainer.CurrentLayer != null
                && Project.CurrentContainer.CurrentLayer.IsVisible
                && Project.CurrentStyleGroup != null
                && Project.CurrentStyleGroup.CurrentStyle != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLeftUpAvailable()
        {
            return Project.CurrentContainer != null
                && Project.CurrentContainer.CurrentLayer != null
                && Project.CurrentContainer.CurrentLayer.IsVisible
                && Project.CurrentStyleGroup != null
                && Project.CurrentStyleGroup.CurrentStyle != null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsRightDownAvailable()
        {
            return Project.CurrentContainer != null
                && Project.CurrentContainer.CurrentLayer != null
                && Project.CurrentContainer.CurrentLayer.IsVisible
                && Project.CurrentStyleGroup != null
                && Project.CurrentStyleGroup.CurrentStyle != null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsRightUpAvailable()
        {
            return Project.CurrentContainer != null
                && Project.CurrentContainer.CurrentLayer != null
                && Project.CurrentContainer.CurrentLayer.IsVisible
                && Project.CurrentStyleGroup != null
                && Project.CurrentStyleGroup.CurrentStyle != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsMoveAvailable()
        {
            return Project.CurrentContainer != null
                && Project.CurrentContainer.CurrentLayer != null
                && Project.CurrentContainer.CurrentLayer.IsVisible
                && Project.CurrentStyleGroup != null
                && Project.CurrentStyleGroup.CurrentStyle != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
                            var result = ShapeBounds.HitTest(Project.CurrentContainer, new Vector2(sx, sy), _project.Options.HitTreshold);
                            if (result != null)
                            {
                                _startX = _project.Options.SnapToGrid ? Snap(sx, _project.Options.SnapX) : sx;
                                _startY = _project.Options.SnapToGrid ? Snap(sy, _project.Options.SnapY) : sy;
                                _historyX = _startX;
                                _historyY = _startY;   
                                _history.Hold(_project);
                                IsContextMenu = false;
                                CurrentState = State.One;
                                break;
                            }
                        }

                        if (TryToSelectShape(Project.CurrentContainer, sx, sy))
                        {
                            _startX = _project.Options.SnapToGrid ? Snap(sx, _project.Options.SnapX) : sx;
                            _startY = _project.Options.SnapToGrid ? Snap(sy, _project.Options.SnapY) : sy;
                            _historyX = _startX;
                            _historyY = _startY;   
                            _history.Hold(_project);
                            IsContextMenu = false;
                            CurrentState = State.One;
                            break;
                        }

                        _shape = XRectangle.Create(
                            sx, sy,
                            _project.Options.SelectionStyle,
                            null,
                            true);
                        Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            double x = _project.Options.SnapToGrid ? Snap(sx, _project.Options.SnapX) : sx;
                            double y = _project.Options.SnapToGrid ? Snap(sy, _project.Options.SnapY) : sy;
                            if (_historyX != x || _historyY != y)
                            {
                                _history.Commit();
                            }
                            else
                            {
                                _history.Release();
                            }
                            CurrentState = State.None;
                            break;
                        }

                        var rectangle = _shape as XRectangle;
                        if (rectangle != null)
                        {
                            rectangle.BottomRight.X = sx;
                            rectangle.BottomRight.Y = sy;
                            Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            Project.CurrentContainer.WorkingLayer.Invalidate();
                            CurrentState = State.None;

                            TryToSelectShapes(Project.CurrentContainer, rectangle);
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
                        _history.Snapshot(_project);
                        Project.CurrentContainer.CurrentLayer.Shapes.Add(_shape);
                        //Project.CurrentContainer.Invalidate();
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
                        if (_project.Options.TryToConnect)
                        {
                            TryToConnectStart(_shape as XLine, sx, sy);
                        }
                        Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectEnd(_shape as XLine, sx, sy);
                            }
                            Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            _history.Snapshot(_project);
                            Project.CurrentContainer.CurrentLayer.Shapes.Add(_shape);
                            //Project.CurrentContainer.Invalidate();
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
                            _project.Options.DefaultIsFilled);
                        if (_project.Options.TryToConnect)
                        {
                            TryToConnectTopLeft(_shape as XRectangle, sx, sy);
                        }
                        Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectBottomRight(_shape as XRectangle, sx, sy);
                            }
                            Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            _history.Snapshot(_project);
                            Project.CurrentContainer.CurrentLayer.Shapes.Add(_shape);
                            //Project.CurrentContainer.Invalidate();
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
                            _project.Options.DefaultIsFilled);
                        if (_project.Options.TryToConnect)
                        {
                            TryToConnectTopLeft(_shape as XEllipse, sx, sy);
                        }
                        Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectBottomRight(_shape as XEllipse, sx, sy);
                            }
                            Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            _history.Snapshot(_project);
                            Project.CurrentContainer.CurrentLayer.Shapes.Add(_shape);
                            //Project.CurrentContainer.Invalidate();
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
                            _project.Options.DefaultIsFilled);
                        if (_project.Options.TryToConnect)
                        {
                            TryToConnectPoint1(_shape as XArc, sx, sy);
                        }
                        if (_enableArcHelper)
                        {
                            _arcHelper = new ArcHelper(Project);
                            _arcHelper.ToStateOne();
                            _arcHelper.Move(_shape as XArc);
                        }
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            arc.Point3.X = sx;
                            arc.Point3.Y = sy;
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectPoint2(_shape as XArc, sx, sy);
                            }
                            if (_enableArcHelper)
                            {
                                _arcHelper.ToStateTwo();
                                _arcHelper.Move(_shape as XArc);
                            }
                            Project.CurrentContainer.WorkingLayer.Invalidate();
                            CurrentState = State.Two;
                        }
                    }
                    break;
                case State.Two:
                    {
                        var arc = _shape as XArc;
                        if (arc != null)
                        {
                            arc.Point3.X = sx;
                            arc.Point3.Y = sy;
                            arc.Point4.X = sx;
                            arc.Point4.Y = sy;
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectPoint3(_shape as XArc, sx, sy);
                            }
                            if (_enableArcHelper)
                            {
                                _arcHelper.ToStateThree();
                                _arcHelper.Move(_shape as XArc);
                            }
                            Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                            Project.CurrentContainer.WorkingLayer.Invalidate();
                            CurrentState = State.Three;
                        }
                    }
                    break;
                case State.Three:
                    {
                        var arc = _shape as XArc;
                        if (arc != null)
                        {
                            arc.Point4.X = sx;
                            arc.Point4.Y = sy;
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectPoint4(_shape as XArc, sx, sy);
                            }
                            if (_enableArcHelper)
                            {
                                _arcHelper.Remove();
                                _arcHelper.Finalize(_shape as XArc);
                            }
                            Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            _history.Snapshot(_project);
                            Project.CurrentContainer.CurrentLayer.Shapes.Add(_shape);
                            //Project.CurrentContainer.Invalidate();
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
                            _project.Options.DefaultIsFilled);
                        if (_project.Options.TryToConnect)
                        {
                            TryToConnectPoint1(_shape as XBezier, sx, sy);
                        }
                        Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectPoint4(_shape as XBezier, sx, sy);
                            }
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectPoint3(_shape as XBezier, sx, sy);
                            }
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectPoint2(_shape as XBezier, sx, sy);
                            }
                            Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            _history.Snapshot(_project);
                            Project.CurrentContainer.CurrentLayer.Shapes.Add(_shape);
                            //Project.CurrentContainer.Invalidate();
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
                            _project.Options.DefaultIsFilled);
                        if (_project.Options.TryToConnect)
                        {
                            TryToConnectPoint1(_shape as XQBezier, sx, sy);
                        }
                        Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectPoint3(_shape as XQBezier, sx, sy);
                            }
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectPoint2(_shape as XQBezier, sx, sy);
                            }
                            Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            _history.Snapshot(_project);
                            Project.CurrentContainer.CurrentLayer.Shapes.Add(_shape);
                            //Project.CurrentContainer.Invalidate();
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
                            _project.Options.DefaultIsFilled);
                        if (_project.Options.TryToConnect)
                        {
                            TryToConnectTopLeft(_shape as XText, sx, sy);
                        }
                        Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectBottomRight(_shape as XText, sx, sy);
                            }
                            Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            _history.Snapshot(_project);
                            Project.CurrentContainer.CurrentLayer.Shapes.Add(_shape);
                            //Project.CurrentContainer.Invalidate();
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
                            _project.Options.DefaultIsFilled);
                        if (_project.Options.TryToConnect)
                        {
                            TryToConnectTopLeft(_shape as XImage, sx, sy);
                        }
                        Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            if (_project.Options.TryToConnect)
                            {
                                TryToConnectBottomRight(_shape as XImage, sx, sy);
                            }
                            Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            _history.Snapshot(_project);
                            Project.CurrentContainer.CurrentLayer.Shapes.Add(_shape);
                            //Project.CurrentContainer.Invalidate();
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
                        IsContextMenu = TryToSelectShape(Project.CurrentContainer, sx, sy) ? true : false;
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
                        Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                        Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                        Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                case State.Two:
                case State.Three:
                    {
                        if (_enableArcHelper)
                        {
                            _arcHelper.Remove();
                        }
                        Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                        Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                        Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                        Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                        Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            if (_enableArcHelper)
                            {
                                _arcHelper.Move(_shape as XArc);
                            }
                            Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                    }
                    break;
                case State.Two:
                    {
                        var arc = _shape as XArc;
                        if (arc != null)
                        {
                            arc.Point3.X = sx;
                            arc.Point3.Y = sy;
                            if (_enableArcHelper)
                            {
                                _arcHelper.Move(_shape as XArc);
                            }
                            Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                    }
                    break;
                case State.Three:
                    {
                        var arc = _shape as XArc;
                        if (arc != null)
                        {
                            arc.Point4.X = sx;
                            arc.Point4.Y = sy;
                            if (_enableArcHelper)
                            {
                                _arcHelper.Move(_shape as XArc);
                            }
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Invalidate();
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
                            Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void LeftDown(double x, double y)
        {
            double sx = _project.Options.SnapToGrid ? Snap(x, _project.Options.SnapX) : x;
            double sy = _project.Options.SnapToGrid ? Snap(y, _project.Options.SnapY) : y;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void LeftUp(double x, double y)
        {
            double sx = _project.Options.SnapToGrid ? Snap(x, _project.Options.SnapX) : x;
            double sy = _project.Options.SnapToGrid ? Snap(y, _project.Options.SnapY) : y;
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
   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void RightDown(double x, double y)
        {
            if (CurrentState == State.None)
            {
                SelectionRightDown(x, y);
                return;
            }

            double sx = _project.Options.SnapToGrid ? Snap(x, _project.Options.SnapX) : x;
            double sy = _project.Options.SnapToGrid ? Snap(y, _project.Options.SnapY) : y;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void RightUp(double x, double y)
        {
            double sx = _project.Options.SnapToGrid ? Snap(x, _project.Options.SnapX) : x;
            double sy = _project.Options.SnapToGrid ? Snap(y, _project.Options.SnapY) : y;
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(double x, double y)
        {
            double sx = _project.Options.SnapToGrid ? Snap(x, _project.Options.SnapX) : x;
            double sy = _project.Options.SnapToGrid ? Snap(y, _project.Options.SnapY) : y;
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
