// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        private ILog _log;
        private Project _project;
        private IRenderer[] _renderers;
        private Tool _currentTool;
        private bool _isContextMenu;
        private bool _enableObserver;
        private Observer _observer;
        private History _history;
        private BaseShape _hover;

        /// <summary>
        /// Gets or sets current log.
        /// </summary>
        public ILog Log
        {
            get { return _log; }
            set { Update(ref _log, value); }
        }

        /// <summary>
        /// Gets or sets current project.
        /// </summary>
        public Project Project
        {
            get { return _project; }
            set { Update(ref _project, value); }
        }

        /// <summary>
        /// Gets or sets current renderers.
        /// </summary>
        public IRenderer[] Renderers
        {
            get { return _renderers; }
            set { Update(ref _renderers, value); }
        }

        /// <summary>
        /// Gets or sets current editor tool.
        /// </summary>
        public Tool CurrentTool
        {
            get { return _currentTool; }
            set { Update(ref _currentTool, value); }
        }

        /// <summary>
        /// Gets or sets if context menu is about to open.
        /// </summary>
        public bool IsContextMenu
        {
            get { return _isContextMenu; }
            set { Update(ref _isContextMenu, value); }
        }
        
        /// <summary>
        /// Gets or sets if project collections and objects observer is enabled.
        /// </summary>
        public bool EnableObserver
        {
            get { return _enableObserver; }
            set { Update(ref _enableObserver, value); }
        }

        /// <summary>
        /// Gets or sets current project collections and objects observer.
        /// </summary>
        public Observer Observer
        {
            get { return _observer; }
            set { Update(ref _observer, value); }
        }

        /// <summary>
        /// Gets or sets undo/redo history handler.
        /// </summary>
        public History History
        {
            get { return _history; }
            set { Update(ref _history, value); }
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
        public GroupHelper GroupHelper { get; set; }

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
        /// 
        /// </summary>
        public PathHelper PathHelper { get; set; }

        /// <summary>
        /// Creates a new Editor instance.
        /// </summary>
        /// <param name="project">The project to edit.</param>
        /// <param name="renderers">The shape renderers.</param>
        /// <param name="serializer">The object serializer.</param>
        /// <param name="compressor">The binary data compressor.</param>
        /// <returns></returns>
        public static Editor Create(
            Project project, 
            IRenderer[] renderers,
            ISerializer serializer,
            ICompressor compressor)
        {
            var editor = new Editor()
            {
                CurrentTool = Tool.Selection,
                EnableObserver = true
            };

            editor.Project = project;
            editor.Renderers = renderers;

            editor.History = new History();

            if (editor.EnableObserver)
            {
                editor.Observer = new Observer(editor);
            }

            editor.SelectionHelper = new SelectionHelper(editor);
            editor.GroupHelper = new GroupHelper(editor);
            editor.PointHelper = new PointHelper(editor);
            editor.LineHelper = new LineHelper(editor);
            editor.RectangleHelper = new RectangleHelper(editor);
            editor.EllipseHelper = new EllipseHelper(editor);
            editor.ArcHelper = new ArcHelper(editor);
            editor.BezierHelper = new BezierHelper(editor);
            editor.QBezierHelper = new QBezierHelper(editor);
            editor.TextHelper = new TextHelper(editor);
            editor.ImageHelper = new ImageHelper(editor);
            editor.PathHelper = new PathHelper(editor);

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
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<XPoint> GetAllPoints(IEnumerable<BaseShape> shapes, ShapeState exclude)
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

                    if (!point.State.HasFlag(exclude))
                    {
                        yield return shape as XPoint;
                    }
                }
                else if (shape is XLine)
                {
                    var line = shape as XLine;

                    if (!line.Start.State.HasFlag(exclude))
                    {
                        yield return line.Start;
                    }

                    if (!line.End.State.HasFlag(exclude))
                    {
                        yield return line.End;
                    }
                }
                else if (shape is XRectangle)
                {
                    var rectangle = shape as XRectangle;

                    if (!rectangle.TopLeft.State.HasFlag(exclude))
                    {
                        yield return rectangle.TopLeft; 
                    }

                    if (!rectangle.BottomRight.State.HasFlag(exclude))
                    {
                        yield return rectangle.BottomRight; 
                    }
                }
                else if (shape is XEllipse)
                {
                    var ellipse = shape as XEllipse;

                    if (!ellipse.TopLeft.State.HasFlag(exclude))
                    {
                        yield return ellipse.TopLeft; 
                    }

                    if (!ellipse.BottomRight.State.HasFlag(exclude))
                    {
                        yield return ellipse.BottomRight; 
                    }
                }
                else if (shape is XArc)
                {
                    var arc = shape as XArc;

                    if (!arc.Point1.State.HasFlag(exclude))
                    {
                        yield return arc.Point1; 
                    }

                    if (!arc.Point2.State.HasFlag(exclude))
                    {
                        yield return arc.Point2; 
                    }

                    if (!arc.Point3.State.HasFlag(exclude))
                    {
                        yield return arc.Point3;
                    }

                    if (!arc.Point4.State.HasFlag(exclude))
                    {
                        yield return arc.Point4;
                    }
                }
                else if (shape is XBezier)
                {
                    var bezier = shape as XBezier;

                    if (!bezier.Point1.State.HasFlag(exclude))
                    {
                        yield return bezier.Point1; 
                    }

                    if (!bezier.Point2.State.HasFlag(exclude))
                    {
                        yield return bezier.Point2; 
                    }

                    if (!bezier.Point3.State.HasFlag(exclude))
                    {
                        yield return bezier.Point3; 
                    }

                    if (!bezier.Point4.State.HasFlag(exclude))
                    {
                        yield return bezier.Point4; 
                    }
                }
                else if (shape is XQBezier)
                {
                    var qbezier = shape as XQBezier;

                    if (!qbezier.Point1.State.HasFlag(exclude))
                    {
                        yield return qbezier.Point1; 
                    }

                    if (!qbezier.Point2.State.HasFlag(exclude))
                    {
                        yield return qbezier.Point2; 
                    }

                    if (!qbezier.Point3.State.HasFlag(exclude))
                    {
                        yield return qbezier.Point3; 
                    }
                }
                else if (shape is XText)
                {
                    var text = shape as XText;

                    if (!text.TopLeft.State.HasFlag(exclude))
                    {
                        yield return text.TopLeft; 
                    }

                    if (!text.BottomRight.State.HasFlag(exclude))
                    {
                        yield return text.BottomRight; 
                    }
                }
                else if (shape is XImage)
                {
                    var image = shape as XImage;

                    if (!image.TopLeft.State.HasFlag(exclude))
                    {
                        yield return image.TopLeft;
                    }

                    if (!image.BottomRight.State.HasFlag(exclude))
                    {
                        yield return image.BottomRight;
                    }
                }
                else if (shape is XGroup)
                {
                    var group = shape as XGroup;

                    foreach (var point in GetAllPoints(group.Shapes, exclude))
                    {
                        if (!point.State.HasFlag(exclude))
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
        public static void MovePointsBy(IEnumerable<XPoint> points, double dx, double dy)
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
        /// <param name="shapes"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public static void MoveShapesBy(IEnumerable<BaseShape> shapes, double dx, double dy)
        {
            foreach (var shape in shapes)
            {
                if (!shape.State.HasFlag(ShapeState.Locked))
                {
                    shape.Move(dx, dy);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public void AddWithHistory(BaseShape shape)
        {
            var layer = _project.CurrentContainer.CurrentLayer;
            var previous = layer.Shapes;
            var next = layer.Shapes.Add(shape);
            _history.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        public void AddWithHistory(IEnumerable<BaseShape> shapes)
        {
            var layer = _project.CurrentContainer.CurrentLayer;
            var previous = layer.Shapes;
            var next = layer.Shapes.AddRange(shapes);
            _history.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveCurrentTemplate()
        {
            var template = _project.CurrentTemplate;
            if (template != null)
            {
                var previous = _project.Templates;
                var next =  _project.Templates.Remove(_project.CurrentTemplate);
                _history.Snapshot(previous, next, (p) => _project.Templates = p);
                _project.Templates = next;

                _project.CurrentTemplate = _project.Templates.FirstOrDefault();
            }
        }

        /// <summary>
        /// Removes the Project.CurrentGroupLibrary object from the Project.GroupLibraries collection.
        /// </summary>
        public void RemoveCurrentGroupLibrary()
        {
            var gl = _project.CurrentGroupLibrary;
            if (gl != null)
            {
                var previous = _project.GroupLibraries;
                var next = _project.GroupLibraries.Remove(gl);
                _history.Snapshot(previous, next, (p) => _project.GroupLibraries = p);
                _project.GroupLibraries = next;

                _project.CurrentGroupLibrary = _project.GroupLibraries.FirstOrDefault();
            }
        }

        /// <summary>
        /// Removes the Project.CurrentGroupLibrary.CurrentGroup object from the Project.CurrentGroupLibrary.Groups collection.
        /// </summary>
        public void RemoveCurrentGroup()
        {
            var group = _project.CurrentGroupLibrary.CurrentGroup;
            if (group != null)
            {
                var gl = _project.CurrentGroupLibrary;
                var previous = gl.Groups;
                var next = gl.Groups.Remove(group);
                _history.Snapshot(previous, next, (p) => gl.Groups = p);
                gl.Groups = next;

                _project.CurrentGroupLibrary.CurrentGroup = _project.CurrentGroupLibrary.Groups.FirstOrDefault();
            }
        }

        /// <summary>
        /// Removes the Container.CurrentLayer object from the Container.Layers collection.
        /// </summary>
        public void RemoveCurrentLayer()
        {
            var layer = _project.CurrentContainer.CurrentLayer;
            if (layer != null)
            {
                var container = _project.CurrentContainer;
                var previous = container.Layers;
                var next = container.Layers.Remove(layer);
                _history.Snapshot(previous, next, (p) => container.Layers = p);
                container.Layers = next;

                _project.CurrentContainer.CurrentLayer = _project.CurrentContainer.Layers.FirstOrDefault();
            }
        }

        /// <summary>
        /// Removes the Container.CurrentShape object from the Container.CurrentLayer.Shapes collection.
        /// </summary>
        public void RemoveCurrentShape()
        {
            var shape = _project.CurrentContainer.CurrentShape;
            if (shape != null)
            {
                var layer = _project.CurrentContainer.CurrentLayer;
                var previous = layer.Shapes;
                var next = layer.Shapes.Remove(shape);
                _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                _project.CurrentContainer.CurrentShape = _project.CurrentContainer.CurrentLayer.Shapes.FirstOrDefault();
            }
        }

        /// <summary>
        /// Removed the Project.CurrentStyleLibrary object from the Project.StyleLibraries collection.
        /// </summary>
        public void RemoveCurrentStyleLibrary()
        {
            var sg = _project.CurrentStyleLibrary;
            if (sg != null)
            {
                var previous = _project.StyleLibraries;
                var next = _project.StyleLibraries.Remove(sg);
                _history.Snapshot(previous, next, (p) => _project.StyleLibraries = p);
                _project.StyleLibraries = next;

                _project.CurrentStyleLibrary = _project.StyleLibraries.FirstOrDefault();
            }
        }

        /// <summary>
        /// Removes the Project.CurrentStyleLibrary.CurrentStyle object from the Project.CurrentStyleLibrary.Styles collection.
        /// </summary>
        public void RemoveCurrentStyle()
        {
            var style = _project.CurrentStyleLibrary.CurrentStyle;
            if (style != null)
            {
                var sg = _project.CurrentStyleLibrary;
                var previous = sg.Styles;
                var next = sg.Styles.Remove(style);
                _history.Snapshot(previous, next, (p) => sg.Styles = p);
                sg.Styles = next;

                _project.CurrentStyleLibrary.CurrentStyle = _project.CurrentStyleLibrary.Styles.FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        public void Load(Project project)
        {
            Deselect();

            foreach (var renderer in _renderers)
            {
                renderer.ClearCache();
            }

            Project = project;

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
            var layer = _project.CurrentContainer.CurrentLayer;
            if (_renderers[0].State.SelectedShapes != null)
            {
                // TODO: Group method changes SelectedShapes State properties.
                var g = XGroup.Group("g", _renderers[0].State.SelectedShapes);

                var builder = layer.Shapes.ToBuilder();
                foreach (var shape in _renderers[0].State.SelectedShapes)
                {
                    builder.Remove(shape);
                }
                builder.Add(g);

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                Select(_project.CurrentContainer, g);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void GroupCurrentLayer()
        {
            var layer = _project.CurrentContainer.CurrentLayer;
            if (layer.Shapes.Length > 0)
            {
                // TODO: Group method changes SelectedShapes State properties.
                var g = XGroup.Group("g", layer.Shapes);

                var builder = layer.Shapes.ToBuilder();
                foreach (var shape in layer.Shapes.ToList())
                {
                    builder.Remove(shape);
                }
                builder.Add(g);
                layer.Shapes = builder.ToImmutable();

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                Select(_project.CurrentContainer, g);
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
                var previous = document.Containers;
                var next = document.Containers.Remove(container);
                _history.Snapshot(previous, next, (p) => document.Containers = p);
                document.Containers = next;

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
            var previous = _project.Documents;
            var next = _project.Documents.Remove(document);
            _history.Snapshot(previous, next, (p) => _project.Documents = p);
            _project.Documents = next;

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
            if (_renderers[0].State.SelectedShape != null)
            {
                var layer = _project.CurrentContainer.CurrentLayer;
                var previous = layer.Shapes;
                var next = layer.Shapes.Remove(_renderers[0].State.SelectedShape); ;
                _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
 
                _project.CurrentContainer.CurrentLayer.Invalidate();
                _renderers[0].State.SelectedShape = default(BaseShape);
            }

            if (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0)
            {
                var layer = _project.CurrentContainer.CurrentLayer;

                var builder = layer.Shapes.ToBuilder();
                foreach (var shape in _renderers[0].State.SelectedShapes)
                {
                    builder.Remove(shape);
                }

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
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
            _renderers[0].State.SelectedShape = shape;
            _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
            container.CurrentLayer.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="shapes"></param>
        public void Select(Container container, ImmutableHashSet<BaseShape> shapes)
        {
            container.CurrentShape = default(BaseShape);
            _renderers[0].State.SelectedShape = default(BaseShape);
            _renderers[0].State.SelectedShapes = shapes;
            container.CurrentLayer.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Deselect()
        {
            _renderers[0].State.SelectedShape = default(BaseShape);
            _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public void Deselect(Container container)
        {
            if (_renderers[0].State.SelectedShape != null
                || _renderers[0].State.SelectedShapes != null)
            {
                _renderers[0].State.SelectedShape = default(BaseShape);
                _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);

                container.CurrentShape = default(BaseShape);
                container.CurrentLayer.Invalidate();
            }
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
        /// <param name="shape"></param>
        public void Hover(BaseShape shape)
        {
            Select(_project.CurrentContainer, shape);
            _hover = shape;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dehover()
        {
            _hover = default(BaseShape);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public void Dehover(Container container)
        {
            _hover = default(BaseShape);
            Deselect(container);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public bool TryToHoverShape(double x, double y)
        {
            if (_renderers[0].State.SelectedShapes == null
                && !(_renderers[0].State.SelectedShape != null && _hover != _renderers[0].State.SelectedShape))
            {
                var result = ShapeBounds.HitTest(
                    _project.CurrentContainer,
                    new Vector2(x, y),
                    _project.Options.HitTreshold);
                if (result != null)
                {
                    Select(_project.CurrentContainer, result);
                    _hover = result;

                    return true;
                }
                else
                {
                    if (_renderers[0].State.SelectedShape != null 
                        && _renderers[0].State.SelectedShape == _hover)
                    {
                        _hover = default(BaseShape);
                        Deselect(_project.CurrentContainer);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="point"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public bool TryToSplitLine(double x, double y, XPoint point, bool select = false)
        {
            var result = ShapeBounds.HitTest(
                _project.CurrentContainer,
                new Vector2(x, y),
                _project.Options.HitTreshold);

            if (result is XLine)
            {
                var line = result as XLine;

                if (!_project.Options.SnapToGrid)
                {
                    var a = new Vector2(line.Start.X, line.Start.Y);
                    var b = new Vector2(line.End.X, line.End.Y);
                    var nearest = MathHelpers.NearestPointOnLine(a, b, new Vector2(x, y));
                    point.X = nearest.X;
                    point.Y = nearest.Y;
                }

                var split = XLine.Create(
                    x, y,
                    line.Style,
                    _project.Options.PointShape);

                double ds = point.Distance(line.Start);
                double de = point.Distance(line.End);

                if (ds < de)
                {
                    split.Start = line.Start;
                    split.End = point;

                    line.Start = point;
                }
                else
                {
                    split.Start = point;
                    split.End = line.End;

                    line.End = point;
                }

                AddWithHistory(split);

                if (select)
                {
                    Select(_project.CurrentContainer, point);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns></returns>
        public bool TryToSplitLine(XLine line, XPoint p0, XPoint p1)
        {
            // points must be aligned horizontally or vertically
            if (p0.X != p1.X && p0.Y != p1.Y)
                return false;

            // line must be horizontal or vertical
            if (line.Start.X != line.End.X && line.Start.Y != line.End.Y)
                return false;

            XLine split;
            if (line.Start.X > line.End.X || line.Start.Y > line.End.Y)
            {
                split = XLine.Create(
                    p0,
                    line.End,
                    line.Style,
                    _project.Options.PointShape);
                line.End = p1;
            }
            else
            {
                split = XLine.Create(
                    p1,
                    line.End,
                    line.Style,
                    _project.Options.PointShape);
                line.End = p0;
            }

            AddWithHistory(split);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool TryToConnect(XGroup group)
        {
            var layer = _project.CurrentContainer.CurrentLayer;
            if (group.Connectors.Length > 0)
            {
                var wires = Editor.GetAllShapes<XLine>(layer.Shapes);
                var dict = new Dictionary<XLine, IList<XPoint>>();

                // find possible group to line connections
                foreach (var connector in group.Connectors)
                {
                    var p = new Vector2(connector.X, connector.Y);
                    var t = _project.Options.HitTreshold;
                    XLine result = null;
                    foreach (var line in wires)
                    {
                        if (ShapeBounds.HitTest(line, p, t))
                        {
                            result = line;
                            break;
                        }
                    }

                    if (result != null)
                    {
                        if (dict.ContainsKey(result))
                        {
                            dict[result].Add(connector);
                        }
                        else
                        {
                            dict.Add(result, new List<XPoint>());
                            dict[result].Add(connector);
                        }
                    }
                }

                bool success = false;

                // try to split lines using group connectors
                foreach (var kv in dict)
                {
                    IList<XPoint> points = kv.Value;
                    if (points.Count == 2)
                    {
                        success = TryToSplitLine(kv.Key, points[0], points[1]);
                    }
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLeftDownAvailable()
        {
            return _project.CurrentContainer != null
                && _project.CurrentContainer.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project.CurrentStyleLibrary != null
                && _project.CurrentStyleLibrary.CurrentStyle != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLeftUpAvailable()
        {
            return _project.CurrentContainer != null
                && _project.CurrentContainer.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project.CurrentStyleLibrary != null
                && _project.CurrentStyleLibrary.CurrentStyle != null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsRightDownAvailable()
        {
            return _project.CurrentContainer != null
                && _project.CurrentContainer.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project.CurrentStyleLibrary != null
                && _project.CurrentStyleLibrary.CurrentStyle != null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsRightUpAvailable()
        {
            return _project.CurrentContainer != null
                && _project.CurrentContainer.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project.CurrentStyleLibrary != null
                && _project.CurrentStyleLibrary.CurrentStyle != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsMoveAvailable()
        {
            return _project.CurrentContainer != null
                && _project.CurrentContainer.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project.CurrentStyleLibrary != null
                && _project.CurrentStyleLibrary.CurrentStyle != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsSelectionAvailable()
        {
            return _renderers[0].State.SelectedShape != null
                || _renderers[0].State.SelectedShapes != null;
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
                case Tool.Group:
                    {
                        GroupHelper.LeftDown(x, y);
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
                case Tool.Path:
                    {
                        PathHelper.LeftDown(x, y);
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
                case Tool.Group:
                    {
                        GroupHelper.LeftUp(x, y);
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
                case Tool.Path:
                    {
                        PathHelper.LeftUp(x, y);
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
                case Tool.Group:
                    {
                        GroupHelper.RightDown(x, y);
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
                case Tool.Path:
                    {
                        PathHelper.RightDown(x, y);
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
                case Tool.Group:
                    {
                        GroupHelper.RightUp(x, y);
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
                case Tool.Path:
                    {
                        PathHelper.RightUp(x, y);
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
                case Tool.Group:
                    {
                        GroupHelper.Move(x, y);
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
                case Tool.Path:
                    {
                        PathHelper.Move(x, y);
                    }
                    break;
            }
        }
    }
}
