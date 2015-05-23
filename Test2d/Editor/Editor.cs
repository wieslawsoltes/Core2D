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
        private Tool _currentTool;
        private bool _isContextMenu;
        private bool _enableObserver;
        private Observer _observer;
        private History<Project> _history;

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
        /// 
        /// </summary>
        public SelectionHelper SelectionHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PointHelper PointHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LineHelper LineHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RectangleHelper RectangleHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EllipseHelper EllipseHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ArcHelper ArcHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BezierHelper BezierHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public QBezierHelper QBezierHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TextHelper TextHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ImageHelper ImageHelper { get; set; }

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

            editor.SelectionHelper = new SelectionHelper(editor);
            editor.PointHelper = new PointHelper(editor);
            editor.LineHelper = new LineHelper(editor);
            editor.RectangleHelper = new RectangleHelper(editor);
            editor.EllipseHelper = new EllipseHelper(editor);
            editor.ArcHelper = new ArcHelper(editor);
            editor.BezierHelper = new BezierHelper(editor);
            editor.QBezierHelper = new QBezierHelper(editor);
            editor.TextHelper = new TextHelper(editor);
            editor.ImageHelper = new ImageHelper(editor);

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
        /// <param name="shapes"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllShapes<T>(IEnumerable<BaseShape> shapes)
        {
            return GetAllShapes(shapes)
                .Where(s => s is T)
                .Cast<T>();
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
        /// 
        /// </summary>
        public void RemoveCurrentTemplate()
        {
            var template = Project.CurrentTemplate;
            if (template != null)
            {
                _history.Snapshot(_project);
                Project.Templates.Remove(_project.CurrentTemplate);
                Project.CurrentTemplate = _project.Templates.FirstOrDefault();
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void LeftDown(double x, double y)
        {
            switch (CurrentTool)
            {
                case Tool.None:
                    break;
                case Tool.Selection:
                    {
                        SelectionHelper.LeftDown(x, y);
                    }
                    break;
                case Tool.Point:
                    {
                        PointHelper.LeftDown(x, y);
                    }
                    break;
                case Tool.Line:
                    {
                        LineHelper.LeftDown(x, y);
                    }
                    break;
                case Tool.Rectangle:
                    {
                        RectangleHelper.LeftDown(x, y);
                    }
                    break;
                case Tool.Ellipse:
                    {
                        EllipseHelper.LeftDown(x, y);
                    }
                    break;
                case Tool.Arc:
                    {
                        ArcHelper.LeftDown(x, y);
                    }
                    break;
                case Tool.Bezier:
                    {
                        BezierHelper.LeftDown(x, y);
                    }
                    break;
                case Tool.QBezier:
                    {
                        QBezierHelper.LeftDown(x, y);
                    }
                    break;
                case Tool.Text:
                    {
                        TextHelper.LeftDown(x, y);
                    }
                    break;
                case Tool.Image:
                    {
                        ImageHelper.LeftDown(x, y);
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
            switch (CurrentTool)
            {
                case Tool.None:
                    break;
                case Tool.Selection:
                    {
                        SelectionHelper.LeftUp(x, y);
                    }
                    break;
                case Tool.Point:
                    {
                        PointHelper.LeftUp(x, y);
                    }
                    break;
                case Tool.Line:
                    {
                        LineHelper.LeftUp(x, y);
                    }
                    break;
                case Tool.Rectangle:
                    {
                        RectangleHelper.LeftUp(x, y);
                    }
                    break;
                case Tool.Ellipse:
                    {
                        EllipseHelper.LeftUp(x, y);
                    }
                    break;
                case Tool.Arc:
                    {
                        ArcHelper.LeftUp(x, y);
                    }
                    break;
                case Tool.Bezier:
                    {
                        BezierHelper.LeftUp(x, y);
                    }
                    break;
                case Tool.QBezier:
                    {
                        QBezierHelper.LeftUp(x, y);
                    }
                    break;
                case Tool.Text:
                    {
                        TextHelper.LeftUp(x, y);
                    }
                    break;
                case Tool.Image:
                    {
                        ImageHelper.LeftUp(x, y);
                    }
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
            switch (CurrentTool)
            {
                case Tool.None:
                    break;
                case Tool.Selection:
                    {
                        SelectionHelper.RightDown(x, y);
                    }
                    break;
                case Tool.Point:
                    {
                        PointHelper.RightDown(x, y);
                    }
                    break;
                case Tool.Line:
                    {
                        LineHelper.RightDown(x, y);
                    }
                    break;
                case Tool.Rectangle:
                    {
                        RectangleHelper.RightDown(x, y);
                    }
                    break;
                case Tool.Ellipse:
                    {
                        EllipseHelper.RightDown(x, y);
                    }
                    break;
                case Tool.Arc:
                    {
                        ArcHelper.RightDown(x, y);
                    }
                    break;
                case Tool.Bezier:
                    {
                        BezierHelper.RightDown(x, y);
                    }
                    break;
                case Tool.QBezier:
                    {
                        QBezierHelper.RightDown(x, y);
                    }
                    break;
                case Tool.Text:
                    {
                        TextHelper.RightDown(x, y);
                    }
                    break;
                case Tool.Image:
                    {
                        ImageHelper.RightDown(x, y);
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
            switch (CurrentTool)
            {
                case Tool.None:
                    break;
                case Tool.Selection:
                    {
                        SelectionHelper.RightUp(x, y);
                    }
                    break;
                case Tool.Point:
                    {
                        PointHelper.RightUp(x, y);
                    }
                    break;
                case Tool.Line:
                    {
                        LineHelper.RightUp(x, y);
                    }
                    break;
                case Tool.Rectangle:
                    {
                        RectangleHelper.RightUp(x, y);
                    }
                    break;
                case Tool.Ellipse:
                    {
                        EllipseHelper.RightUp(x, y);
                    }
                    break;
                case Tool.Arc:
                    {
                        ArcHelper.RightUp(x, y);
                    }
                    break;
                case Tool.Bezier:
                    {
                        BezierHelper.RightUp(x, y);
                    }
                    break;
                case Tool.QBezier:
                    {
                        QBezierHelper.RightUp(x, y);
                    }
                    break;
                case Tool.Text:
                    {
                        TextHelper.RightUp(x, y);
                    }
                    break;
                case Tool.Image:
                    {
                        ImageHelper.RightUp(x, y);
                    }
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
            switch (CurrentTool)
            {
                case Tool.None:
                    break;
                case Tool.Selection:
                    {
                        SelectionHelper.Move(x, y);
                    }
                    break;
                case Tool.Point:
                    {
                        PointHelper.Move(x, y);
                    }
                    break;
                case Tool.Line:
                    {
                        LineHelper.Move(x, y);
                    }
                    break;
                case Tool.Rectangle:
                    {
                        RectangleHelper.Move(x, y);
                    }
                    break;
                case Tool.Ellipse:
                    {
                        EllipseHelper.Move(x, y);
                    }
                    break;
                case Tool.Arc:
                    {
                        ArcHelper.Move(x, y);
                    }
                    break;
                case Tool.Bezier:
                    {
                        BezierHelper.Move(x, y);
                    }
                    break;
                case Tool.QBezier:
                    {
                        QBezierHelper.Move(x, y);
                    }
                    break;
                case Tool.Text:
                    {
                        TextHelper.Move(x, y);
                    }
                    break;
                case Tool.Image:
                    {
                        ImageHelper.Move(x, y);
                    }
                    break;
            }
        }
    }
}
