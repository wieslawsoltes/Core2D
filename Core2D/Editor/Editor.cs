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

namespace Core2D
{
    /// <summary>
    /// Project editor.
    /// </summary>
    public class Editor : ObservableObject
    {
        private ILog _log;
        private Project _project;
        private string _projectPath;
        private bool _isProjectDirty;
        private IRenderer[] _renderers;
        private Tool _currentTool;
        private PathTool _currentPathTool;
        private bool _enableObserver;
        private Observer _observer;
        private bool _enableHistory;
        private History _history;
        private Action _invalidate;
        private bool _cancelAvailable;
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
        /// Gets or sets current project path.
        /// </summary>
        public string ProjectPath
        {
            get { return _projectPath; }
            set { Update(ref _projectPath, value); }
        }

        /// <summary>
        /// Gets or sets flag indicating that current project was modified.
        /// </summary>
        public bool IsProjectDirty
        {
            get { return _isProjectDirty; }
            set { Update(ref _isProjectDirty, value); }
        }

        /// <summary>
        /// Gets or sets current renderer's.
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
        /// Gets or sets current editor path tool.
        /// </summary>
        public PathTool CurrentPathTool
        {
            get { return _currentPathTool; }
            set { Update(ref _currentPathTool, value); }
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
        /// Gets or sets if undo/redo history handler is enabled.
        /// </summary>
        public bool EnableHistory
        {
            get { return _enableHistory; }
            set { Update(ref _enableHistory, value); }
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
        /// Gets or sets invalidate action.
        /// </summary>
        public Action Invalidate
        {
            get { return _invalidate; }
            set { Update(ref _invalidate, value); }
        }

        /// <summary>
        /// Gets or sets flag indicating that current operation can be canceled.
        /// </summary>
        public bool CancelAvailable
        {
            get { return _cancelAvailable; }
            set { Update(ref _cancelAvailable, value); }
        }

        /// <summary>
        /// Get image key using common system open file dialog.
        /// </summary>
        public Func<Task<string>> GetImageKey { get; set; }

        /// <summary>
        /// Gets or sets editor tool helpers dictionary.
        /// </summary>
        public ImmutableDictionary<Tool, Helper> Helpers { get; set; }

        /// <summary>
        /// Loads project.
        /// </summary>
        /// <param name="project">The project to load.</param>
        /// <param name="path">The project path.</param>
        public void Load(Project project, string path = null)
        {
            Deselect();

            if (_renderers != null)
            {
                foreach (var renderer in _renderers)
                {
                    renderer.ClearCache(isZooming: false);
                    renderer.State.ImageCache = project;
                }
            }

            Project = project;
            ProjectPath = path;
            IsProjectDirty = false;

            if (_enableObserver)
            {
                Observer = new Observer(this);
            }
        }

        /// <summary>
        /// Unloads project.
        /// </summary>
        public void Unload()
        {
            if (_enableObserver && _project != null)
            {
                Observer.Remove(_project);
            }

            Deselect();

            if (_renderers != null)
            {
                foreach (var renderer in _renderers)
                {
                    renderer.ClearCache(isZooming: false);
                }
            }

            if (_project != null)
            {
                _project.PurgeUnusedImages(new HashSet<string>(Enumerable.Empty<string>()));
            }

            Project = null;
            ProjectPath = string.Empty;
            IsProjectDirty = false;
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
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<XPoint> GetAllPathPoints(XPath path)
        {
            if (path == null || path.Geometry == null)
                yield break;

            foreach (var figure in path.Geometry.Figures)
            {
                yield return figure.StartPoint;

                foreach (var segment in figure.Segments)
                {
                    if (segment is XArcSegment)
                    {
                        var arcSegment = segment as XArcSegment;
                        yield return arcSegment.Point;
                    }
                    else if (segment is XBezierSegment)
                    {
                        var bezierSegment = segment as XBezierSegment;
                        yield return bezierSegment.Point1;
                        yield return bezierSegment.Point2;
                        yield return bezierSegment.Point3;
                    }
                    else if (segment is XLineSegment)
                    {
                        var lineSegment = segment as XLineSegment;
                        yield return lineSegment.Point;
                    }
                    else if (segment is XPolyBezierSegment)
                    {
                        var polyBezierSegment = segment as XPolyBezierSegment;
                        foreach (var point in polyBezierSegment.Points)
                        {
                            yield return point;
                        }
                    }
                    else if (segment is XPolyLineSegment)
                    {
                        var polyLineSegment = segment as XPolyLineSegment;
                        foreach (var point in polyLineSegment.Points)
                        {
                            yield return point;
                        }
                    }
                    else if (segment is XPolyQuadraticBezierSegment)
                    {
                        var polyQuadraticSegment = segment as XPolyQuadraticBezierSegment;
                        foreach (var point in polyQuadraticSegment.Points)
                        {
                            yield return point;
                        }
                    }
                    else if (segment is XQuadraticBezierSegment)
                    {
                        var qbezierSegment = segment as XQuadraticBezierSegment;
                        yield return qbezierSegment.Point1;
                        yield return qbezierSegment.Point2;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<XPoint> GetAllPoints(IEnumerable<BaseShape> shapes, ShapeStateFlags exclude)
        {
            if (shapes == null)
                yield break;

            foreach (var shape in shapes)
            {
                if (shape is XPoint)
                {
                    var point = shape as XPoint;

                    if (!point.State.Flags.HasFlag(exclude))
                    {
                        yield return shape as XPoint;
                    }
                }
                else if (shape is XLine)
                {
                    var line = shape as XLine;

                    if (!line.Start.State.Flags.HasFlag(exclude))
                    {
                        yield return line.Start;
                    }

                    if (!line.End.State.Flags.HasFlag(exclude))
                    {
                        yield return line.End;
                    }
                }
                else if (shape is XRectangle)
                {
                    var rectangle = shape as XRectangle;

                    if (!rectangle.TopLeft.State.Flags.HasFlag(exclude))
                    {
                        yield return rectangle.TopLeft;
                    }

                    if (!rectangle.BottomRight.State.Flags.HasFlag(exclude))
                    {
                        yield return rectangle.BottomRight;
                    }
                }
                else if (shape is XEllipse)
                {
                    var ellipse = shape as XEllipse;

                    if (!ellipse.TopLeft.State.Flags.HasFlag(exclude))
                    {
                        yield return ellipse.TopLeft;
                    }

                    if (!ellipse.BottomRight.State.Flags.HasFlag(exclude))
                    {
                        yield return ellipse.BottomRight;
                    }
                }
                else if (shape is XArc)
                {
                    var arc = shape as XArc;

                    if (!arc.Point1.State.Flags.HasFlag(exclude))
                    {
                        yield return arc.Point1;
                    }

                    if (!arc.Point2.State.Flags.HasFlag(exclude))
                    {
                        yield return arc.Point2;
                    }

                    if (!arc.Point3.State.Flags.HasFlag(exclude))
                    {
                        yield return arc.Point3;
                    }

                    if (!arc.Point4.State.Flags.HasFlag(exclude))
                    {
                        yield return arc.Point4;
                    }
                }
                else if (shape is XBezier)
                {
                    var bezier = shape as XBezier;

                    if (!bezier.Point1.State.Flags.HasFlag(exclude))
                    {
                        yield return bezier.Point1;
                    }

                    if (!bezier.Point2.State.Flags.HasFlag(exclude))
                    {
                        yield return bezier.Point2;
                    }

                    if (!bezier.Point3.State.Flags.HasFlag(exclude))
                    {
                        yield return bezier.Point3;
                    }

                    if (!bezier.Point4.State.Flags.HasFlag(exclude))
                    {
                        yield return bezier.Point4;
                    }
                }
                else if (shape is XQBezier)
                {
                    var qbezier = shape as XQBezier;

                    if (!qbezier.Point1.State.Flags.HasFlag(exclude))
                    {
                        yield return qbezier.Point1;
                    }

                    if (!qbezier.Point2.State.Flags.HasFlag(exclude))
                    {
                        yield return qbezier.Point2;
                    }

                    if (!qbezier.Point3.State.Flags.HasFlag(exclude))
                    {
                        yield return qbezier.Point3;
                    }
                }
                else if (shape is XText)
                {
                    var text = shape as XText;

                    if (!text.TopLeft.State.Flags.HasFlag(exclude))
                    {
                        yield return text.TopLeft;
                    }

                    if (!text.BottomRight.State.Flags.HasFlag(exclude))
                    {
                        yield return text.BottomRight;
                    }
                }
                else if (shape is XImage)
                {
                    var image = shape as XImage;

                    if (!image.TopLeft.State.Flags.HasFlag(exclude))
                    {
                        yield return image.TopLeft;
                    }

                    if (!image.BottomRight.State.Flags.HasFlag(exclude))
                    {
                        yield return image.BottomRight;
                    }
                }
                else if (shape is XPath)
                {
                    var path = shape as XPath;

                    foreach (var point in GetAllPathPoints(path))
                    {
                        if (!point.State.Flags.HasFlag(exclude))
                        {
                            yield return point;
                        }
                    }
                }
                else if (shape is XGroup)
                {
                    var group = shape as XGroup;

                    foreach (var point in GetAllPoints(group.Shapes, exclude))
                    {
                        if (!point.State.Flags.HasFlag(exclude))
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
                yield break;

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
                else if (shape is XPath)
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
                if (!point.State.Flags.HasFlag(ShapeStateFlags.Locked))
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
                if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked))
                {
                    shape.Move(dx, dy);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void AddDocument(string name = "New")
        {
            if (_project == null)
                return;

            var document = Document.Create(name);

            if (_enableHistory)
            {
                var previous = _project.Documents;
                var next = _project.Documents.Add(document);
                _history.Snapshot(previous, next, (p) => _project.Documents = p);
                _project.Documents = next;
            }
            else
            {
                _project.Documents = _project.Documents.Add(document);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        public void AddDocument(Document document)
        {
            if (_project == null)
                return;

            if (_enableHistory)
            {
                var previous = _project.Documents;
                var next = _project.Documents.Add(document);
                _history.Snapshot(previous, next, (p) => _project.Documents = p);
                _project.Documents = next;
            }
            else
            {
                _project.Documents = _project.Documents.Add(document);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="index"></param>
        public void AddDocumentAt(Document document, int index)
        {
            if (_project == null)
                return;

            if (_enableHistory)
            {
                var previous = _project.Documents;
                var next = _project.Documents.Insert(index, document);
                _history.Snapshot(previous, next, (p) => _project.Documents = p);
                _project.Documents = next;
            }
            else
            {
                _project.Documents = _project.Documents.Insert(index, document);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void AddContainer(string name = "New")
        {
            if (_project == null || _project.CurrentDocument == null)
                return;

            var document = _project.CurrentDocument;
            var container = Container.Create(name);

            if (_enableHistory)
            {
                var previous = document.Containers;
                var next = document.Containers.Add(container);
                _history.Snapshot(previous, next, (p) => document.Containers = p);
                document.Containers = next;
            }
            else
            {
                document.Containers = document.Containers.Add(container);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public void AddContainer(Container container)
        {
            if (_project == null || _project.CurrentDocument == null)
                return;

            var document = _project.CurrentDocument;

            if (_enableHistory)
            {
                var previous = document.Containers;
                var next = document.Containers.Add(container);
                _history.Snapshot(previous, next, (p) => document.Containers = p);
                document.Containers = next;
            }
            else
            {
                document.Containers = document.Containers.Add(container);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="index"></param>
        public void AddContainerAt(Container container, int index)
        {
            if (_project == null || _project.CurrentDocument == null)
                return;

            var document = _project.CurrentDocument;

            if (_enableHistory)
            {
                var previous = document.Containers;
                var next = document.Containers.Insert(index, container);
                _history.Snapshot(previous, next, (p) => document.Containers = p);
                document.Containers = next;
            }
            else
            {
                document.Containers = document.Containers.Insert(index, container);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void AddTemplate(string name = "New")
        {
            if (_project == null)
                return;

            var template = Container.Create(name, true);

            if (_enableHistory)
            {
                var previous = _project.Templates;
                var next = _project.Templates.Add(template);
                _history.Snapshot(previous, next, (p) => _project.Templates = p);
                _project.Templates = next;
            }
            else
            {
                _project.Templates = _project.Templates.Add(template);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        public void AddTemplate(Container template)
        {
            if (_project == null)
                return;

            if (_enableHistory)
            {
                var previous = _project.Templates;
                var next = _project.Templates.Add(template);
                _history.Snapshot(previous, next, (p) => _project.Templates = p);
                _project.Templates = next;
            }
            else
            {
                _project.Templates = _project.Templates.Add(template);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void AddLayer(string name = "New")
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            var container = _project.CurrentContainer;
            var layer = Layer.Create(name, container);

            if (_enableHistory)
            {
                var previous = container.Layers;
                var next = container.Layers.Add(layer);
                _history.Snapshot(previous, next, (p) => container.Layers = p);
                container.Layers = next;
            }
            else
            {
                container.Layers = container.Layers.Add(layer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        public void AddLayer(Layer layer)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            var container = _project.CurrentContainer;

            if (_enableHistory)
            {
                var previous = container.Layers;
                var next = container.Layers.Add(layer);
                _history.Snapshot(previous, next, (p) => container.Layers = p);
                container.Layers = next;
            }
            else
            {
                container.Layers = container.Layers.Add(layer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public void AddShape(BaseShape shape)
        {
            var layer = _project.CurrentContainer.CurrentLayer;

            if (_enableHistory)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.Add(shape);
                _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
            else
            {
                layer.Shapes = layer.Shapes.Add(shape);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        public void AddShapes(IEnumerable<BaseShape> shapes)
        {
            var layer = _project.CurrentContainer.CurrentLayer;

            if (_enableHistory)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.AddRange(shapes);
                _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
            else
            {
                layer.Shapes = layer.Shapes.AddRange(shapes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="property"></param>
        public void AddProperty(Data data, Property property)
        {
            if (_enableHistory)
            {
                var previous = data.Properties;
                var next = data.Properties.Add(property);
                _history.Snapshot(previous, next, (p) => data.Properties = p);
                data.Properties = next;
            }
            else
            {
                data.Properties = data.Properties.Add(property);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="binding"></param>
        public void AddBinding(Data data, Binding binding)
        {
            if (_enableHistory)
            {
                var previous = data.Bindings;
                var next = data.Bindings.Add(binding);
                _history.Snapshot(previous, next, (p) => data.Bindings = p);
                data.Bindings = next;
            }
            else
            {
                data.Bindings = data.Bindings.Add(binding);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="property"></param>
        public void AddProperty(Container container, Property property)
        {
            var previous = container.Data.Properties;

            if (_enableHistory)
            {
                var next = container.Data.Properties.Add(property);
                _history.Snapshot(previous, next, (p) => container.Data.Properties = p);
                container.Data.Properties = next;
            }
            else
            {
                container.Data.Properties = container.Data.Properties.Add(property);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columns"></param>
        public void AddDatabase(string name = "Db", int columns = 2)
        {
            if (_project == null)
                return;

            var db = Database.Create(name);
            var builder = ImmutableArray.CreateBuilder<Column>();

            for (int i = 0; i < columns; i++)
            {
                builder.Add(Column.Create("Column" + i, db));
            }

            db.Columns = builder.ToImmutable();

            if (_enableHistory)
            {
                var previous = _project.Databases;
                var next = _project.Databases.Add(db);
                _history.Snapshot(previous, next, (p) => _project.Databases = p);
                _project.Databases = next;
            }
            else
            {
                _project.Databases = _project.Databases.Add(db);
            }

            _project.CurrentDatabase = db;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public void AddDatabase(Database db)
        {
            if (_project == null)
                return;

            if (_enableHistory)
            {
                var previous = _project.Databases;
                var next = _project.Databases.Add(db);
                _history.Snapshot(previous, next, (p) => _project.Databases = p);
                _project.Databases = next;
            }
            else
            {
                _project.Databases = _project.Databases.Add(db);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        public void AddColumn(object owner, string name = "Column")
        {
            if (owner != null && owner is Database)
            {
                var db = owner as Database;
                if (db.Columns == null)
                {
                    db.Columns = ImmutableArray.Create<Column>();
                }

                if (_enableHistory)
                {
                    var previous = db.Columns;
                    var next = db.Columns.Add(Column.Create(name + db.Columns.Length, db));
                    _history.Snapshot(previous, next, (p) => db.Columns = p);
                    db.Columns = next;
                }
                else
                {
                    db.Columns = db.Columns.Add(Column.Create(name + db.Columns.Length, db));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void AddRecord(string value = "<empty>")
        {
            if (_project == null || _project.CurrentDatabase == null)
                return;

            var db = _project.CurrentDatabase;

            var values = Enumerable.Repeat(value, db.Columns.Length).Select(c => Value.Create(c));
            var record = Record.Create(
                db.Columns,
                ImmutableArray.CreateRange<Value>(values),
                db);

            if (_enableHistory)
            {
                var previous = db.Records;
                var next = db.Records.Add(record);
                _history.Snapshot(previous, next, (p) => db.Records = p);
                db.Records = next;
            }
            else
            {
                db.Records = db.Records.Add(record);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="property"></param>
        /// <param name="path"></param>
        public void AddBinding(object owner, string property = "", string path = "")
        {
            if (owner != null && owner is Data)
            {
                var data = owner as Data;
                if (data.Bindings == null)
                {
                    data.Bindings = ImmutableArray.Create<Binding>();
                }

                AddBinding(data, Binding.Create(property, path, data));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddProperty(object owner, string name = "New", string value = "")
        {
            if (owner != null)
            {
                if (owner is Data)
                {
                    var data = owner as Data;
                    if (data.Properties == null)
                    {
                        data.Properties = ImmutableArray.Create<Property>();
                    }

                    AddProperty(data, Property.Create(name, value, data));
                }
                else if (owner is Container)
                {
                    var container = owner as Container;
                    if (container.Data.Properties == null)
                    {
                        container.Data.Properties = ImmutableArray.Create<Property>();
                    }

                    AddProperty(container, Property.Create(name, value, container.Data));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void AddGroupLibrary(string name = "New")
        {
            if (_project == null || _project.GroupLibraries == null)
                return;

            var gl = GroupLibrary.Create(name);

            if (_enableHistory)
            {
                var previous = _project.GroupLibraries;
                var next = _project.GroupLibraries.Add(gl);
                _history.Snapshot(previous, next, (p) => _project.GroupLibraries = p);
                _project.GroupLibraries = next;
            }
            else
            {
                _project.GroupLibraries = _project.GroupLibraries.Add(gl);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void AddStyleLibrary(string name = "New")
        {
            if (_project == null || _project.StyleLibraries == null)
                return;

            var sl = StyleLibrary.Create(name);

            if (_enableHistory)
            {
                var previous = _project.StyleLibraries;
                var next = _project.StyleLibraries.Add(sl);
                _history.Snapshot(previous, next, (p) => _project.StyleLibraries = p);
                _project.StyleLibraries = next;
            }
            else
            {
                _project.StyleLibraries = _project.StyleLibraries.Add(sl);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void AddStyle(string name = "New")
        {
            if (_project == null || _project.CurrentStyleLibrary == null)
                return;

            var sl = _project.CurrentStyleLibrary;

            if (_enableHistory)
            {
                var previous = sl.Styles;
                var next = sl.Styles.Add(ShapeStyle.Create(name));
                _history.Snapshot(previous, next, (p) => sl.Styles = p);
                sl.Styles = next;
            }
            else
            {
                sl.Styles = sl.Styles.Add(ShapeStyle.Create(name));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        public void AddGroup(XGroup group)
        {
            if (_project == null || _project.CurrentGroupLibrary == null)
                return;

            var gl = _project.CurrentGroupLibrary;

            if (_enableHistory)
            {
                var previous = gl.Groups;
                var next = gl.Groups.Add(group);
                _history.Snapshot(previous, next, (p) => gl.Groups = p);
                gl.Groups = next;
            }
            else
            {
                gl.Groups = gl.Groups.Add(group);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void AddGroup(string name = "New")
        {
            if (_project == null || _project.CurrentGroupLibrary == null)
                return;

            var gl = _project.CurrentGroupLibrary;
            var group = XGroup.Create(name);

            if (_enableHistory)
            {
                var previous = gl.Groups;
                var next = gl.Groups.Add(group);
                _history.Snapshot(previous, next, (p) => gl.Groups = p);
                gl.Groups = next;
            }
            else
            {
                gl.Groups = gl.Groups.Add(group);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public async Task<string> AddImageKey(string path)
        {
            if (_project == null)
                return null;

            if (path == null || string.IsNullOrEmpty(path))
            {
                var key = await GetImageKey();
                if (key == null || string.IsNullOrEmpty(key))
                    return null;

                return key;
            }
            else
            {
                var bytes = System.IO.File.ReadAllBytes(path);
                var key = _project.AddImageFromFile(path, bytes);
                return key;
            }
        }

        /// <summary>
        /// Removes the <see cref="Project.CurrentTemplate"/> object from the <see cref="Project.Templates"/> collection.
        /// </summary>
        public void RemoveCurrentTemplate()
        {
            if (_project == null)
                return;

            var template = _project.CurrentTemplate;
            if (template == null)
                return;

            if (_enableHistory)
            {
                var previous = _project.Templates;
                var next = _project.Templates.Remove(_project.CurrentTemplate);
                _history.Snapshot(previous, next, (p) => _project.Templates = p);
                _project.Templates = next;
            }
            else
            {
                _project.Templates = _project.Templates.Remove(_project.CurrentTemplate);
            }

            _project.CurrentTemplate = _project.Templates.FirstOrDefault();
        }

        /// <summary>
        /// Removes the <see cref="Project.CurrentGroupLibrary"/> object from the <see cref="Project.GroupLibraries"/> collection.
        /// </summary>
        public void RemoveCurrentGroupLibrary()
        {
            if (_project == null)
                return;

            var gl = _project.CurrentGroupLibrary;
            if (gl == null)
                return;

            if (_enableHistory)
            {
                var previous = _project.GroupLibraries;
                var next = _project.GroupLibraries.Remove(gl);
                _history.Snapshot(previous, next, (p) => _project.GroupLibraries = p);
                _project.GroupLibraries = next;
            }
            else
            {
                _project.GroupLibraries = _project.GroupLibraries.Remove(gl);
            }

            _project.CurrentGroupLibrary = _project.GroupLibraries.FirstOrDefault();
        }

        /// <summary>
        /// Removes the <see cref="Project.CurrentGroupLibrary"/> <see cref="GroupLibrary.CurrentGroup"/> object from the <see cref="Project.CurrentGroupLibrary"/> <see cref="GroupLibrary.Groups"/> collection.
        /// </summary>
        public void RemoveCurrentGroup()
        {
            if (_project == null || _project.CurrentGroupLibrary == null)
                return;

            var group = _project.CurrentGroupLibrary.CurrentGroup;
            if (group == null)
                return;

            var gl = _project.CurrentGroupLibrary;

            if (_enableHistory)
            {
                var previous = gl.Groups;
                var next = gl.Groups.Remove(group);
                _history.Snapshot(previous, next, (p) => gl.Groups = p);
                gl.Groups = next;
            }
            else
            {
                gl.Groups = gl.Groups.Remove(group);
            }

            _project.CurrentGroupLibrary.CurrentGroup = _project.CurrentGroupLibrary.Groups.FirstOrDefault();
        }

        /// <summary>
        /// Removes the <see cref="Project.CurrentContainer"/> <see cref="Container.CurrentLayer"/> object from the <see cref="Project.CurrentContainer"/> <see cref="Container.Layers"/> collection.
        /// </summary>
        public void RemoveCurrentLayer()
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            var layer = _project.CurrentContainer.CurrentLayer;
            if (layer == null)
                return;

            var container = _project.CurrentContainer;

            if (_enableHistory)
            {
                var previous = container.Layers;
                var next = container.Layers.Remove(layer);
                _history.Snapshot(previous, next, (p) => container.Layers = p);
                container.Layers = next;
            }
            else
            {
                container.Layers = container.Layers.Remove(layer);
            }

            _project.CurrentContainer.CurrentLayer = _project.CurrentContainer.Layers.FirstOrDefault();
        }

        /// <summary>
        /// Removes the <see cref="Project.CurrentContainer"/> <see cref="Container.CurrentShape"/> object from the <see cref="Project.CurrentContainer"/> <see cref="Container.CurrentLayer"/> <see cref="Layer.Shapes"/> collection.
        /// </summary>
        public void RemoveCurrentShape()
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            var shape = _project.CurrentContainer.CurrentShape;
            if (shape == null)
                return;

            var layer = _project.CurrentContainer.CurrentLayer;

            if (_enableHistory)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.Remove(shape);
                _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
            else
            {
                layer.Shapes = layer.Shapes.Remove(shape);
            }

            _project.CurrentContainer.CurrentShape = _project.CurrentContainer.CurrentLayer.Shapes.FirstOrDefault();
        }

        /// <summary>
        /// Removes the <see cref="Project.CurrentStyleLibrary"/> object from the <see cref="Project.StyleLibraries"/> collection.
        /// </summary>
        public void RemoveCurrentStyleLibrary()
        {
            if (_project == null)
                return;

            var sg = _project.CurrentStyleLibrary;
            if (sg == null)
                return;

            if (_enableHistory)
            {
                var previous = _project.StyleLibraries;
                var next = _project.StyleLibraries.Remove(sg);
                _history.Snapshot(previous, next, (p) => _project.StyleLibraries = p);
                _project.StyleLibraries = next;
            }
            else
            {
                _project.StyleLibraries = _project.StyleLibraries.Remove(sg);
            }

            _project.CurrentStyleLibrary = _project.StyleLibraries.FirstOrDefault();
        }

        /// <summary>
        /// Removes the <see cref="Project.CurrentStyleLibrary"/> <see cref="StyleLibrary.CurrentStyle"/> object from the <see cref="Project.CurrentStyleLibrary"/> <see cref="StyleLibrary.Styles"/> collection.
        /// </summary>
        public void RemoveCurrentStyle()
        {
            if (_project == null || _project.CurrentStyleLibrary == null)
                return;

            var style = _project.CurrentStyleLibrary.CurrentStyle;
            if (style == null)
                return;

            var sg = _project.CurrentStyleLibrary;

            if (_enableHistory)
            {
                var previous = sg.Styles;
                var next = sg.Styles.Remove(style);
                _history.Snapshot(previous, next, (p) => sg.Styles = p);
                sg.Styles = next;
            }
            else
            {
                sg.Styles = sg.Styles.Remove(style);
            }

            _project.CurrentStyleLibrary.CurrentStyle = _project.CurrentStyleLibrary.Styles.FirstOrDefault();
        }

        /// <summary>
        /// Removes the <see cref="Database"/> object from the <see cref="Project.Databases"/> collection.
        /// </summary>
        /// <param name="db">The <see cref="Database"/> to remove.</param>
        public void RemoveDatabase(object db)
        {
            if (_project == null)
                return;

            if (db != null && db is Database)
            {
                if (_enableHistory)
                {
                    var previous = _project.Databases;
                    var next = _project.Databases.Remove(db as Database);
                    _history.Snapshot(previous, next, (p) => _project.Databases = p);
                    _project.Databases = next;
                }
                else
                {
                    _project.Databases = _project.Databases.Remove(db as Database);
                }

                _project.CurrentDatabase = _project.Databases.FirstOrDefault();
            }
        }

        /// <summary>
        /// Removes the <see cref="Column"/> object from <see cref="Column.Owner"/> <see cref="Database.Columns"/> collection.
        /// </summary>
        /// <param name="parameter">The <see cref="Column"/> to remove.</param>
        public void RemoveColumn(object parameter)
        {
            if (parameter != null && parameter is Column)
            {
                var column = parameter as Column;
                var owner = column.Owner;

                if (owner is Database)
                {
                    var db = owner as Database;
                    if (db.Columns != null)
                    {
                        if (_enableHistory)
                        {
                            var previous = db.Columns;
                            var next = db.Columns.Remove(column);
                            _history.Snapshot(previous, next, (p) => db.Columns = p);
                            db.Columns = next;
                        }
                        else
                        {
                            db.Columns = db.Columns.Remove(column);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the <see cref="Project.CurrentDatabase"/> <see cref="Database.CurrentRecord"/> object from the <see cref="Project.CurrentDatabase"/> <see cref="Database.Records"/> collection.
        /// </summary>
        public void RemoveRecord()
        {
            if (_project == null || _project.CurrentDatabase == null)
                return;

            var db = _project.CurrentDatabase;
            if (db.CurrentRecord != null)
            {
                var record = db.CurrentRecord;

                if (_enableHistory)
                {
                    var previous = db.Records;
                    var next = db.Records.Remove(record);
                    _history.Snapshot(previous, next, (p) => db.Records = p);
                    db.Records = next;
                }
                else
                {
                    db.Records = db.Records.Remove(record);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public void ResetRecord(object owner)
        {
            if (owner != null && owner is Data)
            {
                var data = owner as Data;
                var record = data.Record;

                if (record != null)
                {
                    if (_enableHistory)
                    {
                        var previous = record;
                        var next = default(Record);
                        _history.Snapshot(previous, next, (p) => data.Record = p);
                        data.Record = next;
                    }
                    else
                    {
                        data.Record = default(Record);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void RemoveBinding(object parameter)
        {
            if (parameter != null && parameter is Binding)
            {
                var binding = parameter as Binding;
                var owner = binding.Owner;

                if (owner != null && owner is Data)
                {
                    var data = owner;
                    if (data.Bindings != null)
                    {
                        if (_enableHistory)
                        {
                            var previous = data.Bindings;
                            var next = data.Bindings.Remove(binding);
                            _history.Snapshot(previous, next, (p) => data.Bindings = p);
                            data.Bindings = next;
                        }
                        else
                        {
                            data.Bindings = data.Bindings.Remove(binding);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void RemoveProperty(object parameter)
        {
            if (parameter != null && parameter is Property)
            {
                var property = parameter as Property;
                var owner = property.Owner;

                if (owner is Data)
                {
                    var data = owner;
                    if (data.Properties != null)
                    {
                        if (_enableHistory)
                        {
                            var previous = data.Properties;
                            var next = data.Properties.Remove(property);
                            _history.Snapshot(previous, next, (p) => data.Properties = p);
                            data.Properties = next;
                        }
                        else
                        {
                            data.Properties = data.Properties.Remove(property);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void RemoveImageKey(object parameter)
        {
            if (_project == null)
                return;

            if (parameter == null)
                return;

            var key = parameter as string;
            if (key != null)
            {
                _project.RemoveImage(key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        public void ApplyStyle(ShapeStyle style)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            var container = _project.CurrentContainer;

            if (_enableHistory)
            {
                if (_renderers[0].State.SelectedShape != null)
                {
                    var shape = _renderers[0].State.SelectedShape;
                    var previous = shape.Style;
                    var next = style;
                    _history.Snapshot(previous, next, (p) => shape.Style = p);
                    shape.Style = next;
                }
                else if (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0)
                {
                    foreach (var shape in _renderers[0].State.SelectedShapes)
                    {
                        var previous = shape.Style;
                        var next = style;
                        _history.Snapshot(previous, next, (p) => shape.Style = p);
                        shape.Style = next;
                    }
                }
            }
            else
            {
                if (_renderers[0].State.SelectedShape != null)
                {
                    _renderers[0].State.SelectedShape.Style = style;
                }
                else if (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0)
                {
                    foreach (var shape in _renderers[0].State.SelectedShapes)
                    {
                        shape.Style = style;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="style"></param>
        public void ApplyStyle(BaseShape shape, ShapeStyle style)
        {
            if (_enableHistory)
            {
                var previous = shape.Style;
                var next = style;
                _history.Snapshot(previous, next, (p) => shape.Style = p);
                shape.Style = next;
            }
            else
            {
                shape.Style = style;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ApplyStyle(ShapeStyle style, double x, double y)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            var container = _project.CurrentContainer;
            var result = ShapeBounds.HitTest(container, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null)
            {
                if (_enableHistory)
                {
                    var previous = result.Style;
                    var next = style;
                    _history.Snapshot(previous, next, (p) => result.Style = p);
                    result.Style = next;
                }
                else
                {
                    result.Style = style;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        public void ApplyTemplate(Container template)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            var container = _project.CurrentContainer;

            if (_enableHistory)
            {
                var previous = container.Template;
                var next = template;
                _history.Snapshot(previous, next, (p) => container.Template = p);
                container.Template = next;
            }
            else
            {
                container.Template = template;
            }
        }

        /// <summary>
        /// Updates the destination <see cref="Database"/> using data from source <see cref="Database"/>.
        /// </summary>
        /// <param name="destination">The destination database.</param>
        /// <param name="source">The source database.</param>
        public void ApplyDatabase(Database destination, Database source)
        {
            if (_project == null)
                return;

            if (source.Columns.Length <= 1)
                return;

            // check for the Id column
            if (source.Columns[0].Name != Database.IdColumnName)
                return;

            // skip Id column for update
            if (source.Columns.Length - 1 != destination.Columns.Length)
                return;

            // check column names
            for (int i = 1; i < source.Columns.Length; i++)
            {
                if (source.Columns[i].Name != destination.Columns[i - 1].Name)
                    return;
            }

            bool isDirty = false;
            var recordsBuilder = destination.Records.ToBuilder();

            for (int i = 0; i < destination.Records.Length; i++)
            {
                var record = destination.Records[i];

                var result = source.Records.FirstOrDefault(r => r.Id == record.Id);
                if (result != null)
                {
                    // update existing record
                    for (int j = 1; j < result.Values.Length; j++)
                    {
                        var valuesBuilder = record.Values.ToBuilder();
                        valuesBuilder[j - 1] = result.Values[j];
                        record.Values = valuesBuilder.ToImmutable();
                    }
                    isDirty = true;
                }
                else
                {
                    var r = source.Records[i];

                    // use existing columns
                    r.Columns = destination.Columns;

                    // skip Id column
                    r.Values = r.Values.Skip(1).ToImmutableArray();

                    recordsBuilder.Add(r);
                    isDirty = true;
                }
            }

            if (isDirty)
            {
                var builder = _project.Databases.ToBuilder();
                var index = builder.IndexOf(destination);
                destination.Records = recordsBuilder.ToImmutable();
                builder[index] = destination;

                if (_enableHistory)
                {
                    var previous = _project.Databases;
                    var next = builder.ToImmutable();
                    _history.Snapshot(previous, next, (p) => _project.Databases = p);
                    _project.Databases = next;
                }
                else
                {
                    _project.Databases = builder.ToImmutable();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        public void ApplyRecord(Record record)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            var container = _project.CurrentContainer;

            if (_enableHistory)
            {
                if (_renderers[0].State.SelectedShape != null)
                {
                    var shape = _renderers[0].State.SelectedShape;
                    var previous = shape.Data.Record;
                    var next = record;
                    _history.Snapshot(previous, next, (p) => shape.Data.Record = p);
                    shape.Data.Record = next;
                }
                else if (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0)
                {
                    foreach (var shape in _renderers[0].State.SelectedShapes)
                    {
                        var previous = shape.Data.Record;
                        var next = record;
                        _history.Snapshot(previous, next, (p) => shape.Data.Record = p);
                        shape.Data.Record = next;
                    }
                }
            }
            else
            {
                if (_renderers[0].State.SelectedShape != null)
                {
                    _renderers[0].State.SelectedShape.Data.Record = record;
                }
                else if (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0)
                {
                    foreach (var shape in _renderers[0].State.SelectedShapes)
                    {
                        shape.Data.Record = record;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="record"></param>
        public void ApplyRecord(BaseShape shape, Record record)
        {
            if (_enableHistory)
            {
                var previous = shape.Data.Record;
                var next = record;
                _history.Snapshot(previous, next, (p) => shape.Data.Record = p);
                shape.Data.Record = next;
            }
            else
            {
                shape.Data.Record = record;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ApplyRecord(Record record, double x, double y)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            var container = _project.CurrentContainer;
            var result = ShapeBounds.HitTest(container, new Vector2(x, y), _project.Options.HitTreshold);
            if (result != null)
            {
                if (_enableHistory)
                {
                    var previous = result.Data.Record;
                    var next = record;
                    _history.Snapshot(previous, next, (p) => result.Data.Record = p);
                    result.Data.Record = next;
                }
                else
                {
                    result.Data.Record = record;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public XGroup GroupSelected(string name = "g")
        {
            var shapes = _renderers[0].State.SelectedShapes;
            var layer = _project.CurrentContainer.CurrentLayer;
            if (shapes == null)
                return null;

            // TODO: Group method changes SelectedShapes State properties.
            var g = XGroup.Group(name, shapes);

            var builder = layer.Shapes.ToBuilder();
            foreach (var shape in shapes)
            {
                builder.Remove(shape);
            }
            builder.Add(g);

            if (_enableHistory)
            {
                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
            else
            {
                layer.Shapes = builder.ToImmutable();
            }

            Select(_project.CurrentContainer, g);

            return g;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="original"></param>
        /// <param name="groupShapes"></param>
        private void Ungroup(IEnumerable<BaseShape> shapes, IList<BaseShape> original, bool groupShapes)
        {
            if (shapes == null)
                return;

            foreach (var shape in shapes)
            {
                if (shape is XGroup)
                {
                    var g = shape as XGroup;
                    Ungroup(g.Shapes, original, groupShapes: true);
                    Ungroup(g.Connectors, original, groupShapes: true);
                    original.Remove(g);
                }
                else if (groupShapes)
                {
                    if (shape is XPoint)
                    {
                        shape.State.Flags &=
                            ~(ShapeStateFlags.Connector
                            | ShapeStateFlags.None
                            | ShapeStateFlags.Input
                            | ShapeStateFlags.Output);
                        shape.State.Flags |= ShapeStateFlags.Standalone;
                        original.Add(shape);
                    }
                    else
                    {
                        shape.State.Flags |= ShapeStateFlags.Standalone;
                        original.Add(shape);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UngroupSelected()
        {
            var shapes = _renderers[0].State.SelectedShapes;
            var shape = _renderers[0].State.SelectedShape;
            var layer = _project.CurrentContainer.CurrentLayer;

            if (shape != null && shape is XGroup && layer != null)
            {
                var builder = layer.Shapes.ToBuilder();

                var g = shape as XGroup;
                Ungroup(g.Shapes, builder, groupShapes: true);
                Ungroup(g.Connectors, builder, groupShapes: true);
                builder.Remove(g);

                if (_enableHistory)
                {
                    var previous = layer.Shapes;
                    var next = builder.ToImmutable();
                    _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;
                }
                else
                {
                    layer.Shapes = builder.ToImmutable();
                }

                _renderers[0].State.SelectedShape = null;
                layer.Invalidate();
            }

            if (shapes != null && layer != null)
            {
                var builder = layer.Shapes.ToBuilder();

                Ungroup(shapes, builder, groupShapes: false);

                if (_enableHistory)
                {
                    var previous = layer.Shapes;
                    var next = builder.ToImmutable();
                    _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;
                }
                else
                {
                    layer.Shapes = builder.ToImmutable();
                }

                layer.Invalidate();
                _renderers[0].State.SelectedShapes = null;
            }
        }

        /// <summary>
        /// Move shape from source index to target index position in an array. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="targetIndex"></param>
        private void Move(BaseShape source, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                var layer = _project.CurrentContainer.CurrentLayer;
                var items = layer.Shapes;
                if (items != null)
                {
                    var builder = items.ToBuilder();
                    builder.Insert(targetIndex + 1, source);
                    builder.RemoveAt(sourceIndex);

                    if (_enableHistory)
                    {
                        var previous = layer.Shapes;
                        var next = builder.ToImmutable();
                        _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                        layer.Shapes = next;
                    }
                    else
                    {
                        layer.Shapes = builder.ToImmutable();
                    }
                }
            }
            else
            {
                var layer = _project.CurrentContainer.CurrentLayer;
                var items = layer.Shapes;
                if (items != null)
                {
                    int removeIndex = sourceIndex + 1;
                    if (items.Length + 1 > removeIndex)
                    {
                        var builder = items.ToBuilder();
                        builder.Insert(targetIndex, source);
                        builder.RemoveAt(removeIndex);

                        if (_enableHistory)
                        {
                            var previous = layer.Shapes;
                            var next = builder.ToImmutable();
                            _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                            layer.Shapes = next;
                        }
                        else
                        {
                            layer.Shapes = builder.ToImmutable();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Bring a shape to the top of the stack.
        /// </summary>
        /// <param name="source"></param>
        public void BringToFront(BaseShape source)
        {
            if (_project == null
                || _project.CurrentContainer == null
                || _project.CurrentContainer.CurrentLayer == null)
                return;

            var layer = _project.CurrentContainer.CurrentLayer;
            var items = layer.Shapes;

            int sourceIndex = items.IndexOf(source);
            int targetIndex = items.Length - 1;
            if (targetIndex >= 0 && sourceIndex != targetIndex)
            {
                Move(source, sourceIndex, targetIndex);
            }
        }

        /// <summary>
        /// Move a shape one step closer to the front of the stack.
        /// </summary>
        /// <param name="source"></param>
        public void BringForward(BaseShape source)
        {
            if (_project == null
                || _project.CurrentContainer == null
                || _project.CurrentContainer.CurrentLayer == null)
                return;

            var layer = _project.CurrentContainer.CurrentLayer;
            var items = layer.Shapes;

            int sourceIndex = items.IndexOf(source);
            int targetIndex = sourceIndex + 1;
            if (targetIndex < items.Length)
            {
                Move(source, sourceIndex, targetIndex);
            }
        }

        /// <summary>
        /// Move a shape one step down within the stack.
        /// </summary>
        /// <param name="source"></param>
        public void SendBackward(BaseShape source)
        {
            if (_project == null
                || _project.CurrentContainer == null
                || _project.CurrentContainer.CurrentLayer == null)
                return;

            var layer = _project.CurrentContainer.CurrentLayer;
            var items = layer.Shapes;

            int sourceIndex = items.IndexOf(source);
            int targetIndex = sourceIndex - 1;
            if (targetIndex >= 0)
            {
                Move(source, sourceIndex, targetIndex);
            }
        }

        /// <summary>
        /// Move a shape to the bottom of the stack.
        /// </summary>
        /// <param name="source"></param>
        public void SendToBack(BaseShape source)
        {
            if (_project == null
                || _project.CurrentContainer == null
                || _project.CurrentContainer.CurrentLayer == null)
                return;

            var layer = _project.CurrentContainer.CurrentLayer;
            var items = layer.Shapes;

            int sourceIndex = items.IndexOf(source);
            int targetIndex = 0;
            if (sourceIndex != targetIndex)
            {
                Move(source, sourceIndex, targetIndex);
            }
        }

        /// <summary>
        /// Bring selected shapes to the top of the stack.
        /// </summary>
        public void BringToFrontSelected()
        {
            var source = _renderers[0].State.SelectedShape;
            if (source != null)
            {
                BringToFront(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    BringToFront(s);
                }
            }
        }

        /// <summary>
        /// Move selected shapes one step closer to the front of the stack.
        /// </summary>
        public void BringForwardSelected()
        {
            var source = _renderers[0].State.SelectedShape;
            if (source != null)
            {
                BringForward(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    BringForward(s);
                }
            }
        }

        /// <summary>
        /// Move selected shapes one step down within the stack.
        /// </summary>
        public void SendBackwardSelected()
        {
            var source = _renderers[0].State.SelectedShape;
            if (source != null)
            {
                SendBackward(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    SendBackward(s);
                }
            }
        }

        /// <summary>
        /// Move selected shapes to the bottom of the stack.
        /// </summary>
        public void SendToBackSelected()
        {
            var source = _renderers[0].State.SelectedShape;
            if (source != null)
            {
                SendToBack(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    SendToBack(s);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void MoveSelectedByWithHistory(double dx, double dy)
        {
            if (_renderers[0].State.SelectedShape != null)
            {
                var state = _renderers[0].State.SelectedShape.State;

                switch (_project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            if (!state.Flags.HasFlag(ShapeStateFlags.Locked))
                            {
                                var shape = _renderers[0].State.SelectedShape;
                                var shapes = Enumerable.Repeat(shape, 1);
                                var points = GetAllPoints(shapes, ShapeStateFlags.Connector).Distinct().ToList();

                                MovePointsBy(points, dx, dy);

                                if (_enableHistory)
                                {
                                    var previous = new { DeltaX = -dx, DeltaY = -dy, Points = points };
                                    var next = new { DeltaX = dx, DeltaY = dy, Points = points };
                                    _history.Snapshot(previous, next, (s) => MovePointsBy(s.Points, s.DeltaX, s.DeltaY));
                                }
                            }
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            if (!state.Flags.HasFlag(ShapeStateFlags.Locked) && !state.Flags.HasFlag(ShapeStateFlags.Connector))
                            {
                                var shape = _renderers[0].State.SelectedShape;
                                var shapes = Enumerable.Repeat(shape, 1).ToList();

                                MoveShapesBy(shapes, dx, dy);

                                if (_enableHistory)
                                {
                                    var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = shapes };
                                    var next = new { DeltaX = dx, DeltaY = dy, Shapes = shapes };
                                    _history.Snapshot(previous, next, (s) => MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY));
                                }
                            }
                        }
                        break;
                }
            }

            if (_renderers[0].State.SelectedShapes != null)
            {
                var shapes = _renderers[0].State.SelectedShapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));

                switch (_project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            var points = GetAllPoints(shapes, ShapeStateFlags.Connector).Distinct().ToList();

                            MovePointsBy(points, dx, dy);

                            if (_enableHistory)
                            {
                                var previous = new { DeltaX = -dx, DeltaY = -dy, Points = points };
                                var next = new { DeltaX = dx, DeltaY = dy, Points = points };
                                _history.Snapshot(previous, next, (s) => MovePointsBy(s.Points, s.DeltaX, s.DeltaY));
                            }
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            MoveShapesBy(shapes, dx, dy);

                            if (_enableHistory)
                            {
                                var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = shapes.ToList() };
                                var next = new { DeltaX = dx, DeltaY = dy, Shapes = shapes.ToList() };
                                _history.Snapshot(previous, next, (s) => MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY));
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void MoveUpSelected()
        {
            double dy = _project.Options.SnapToGrid ? -_project.Options.SnapY : -1.0;
            MoveSelectedByWithHistory(0.0, dy);
        }

        /// <summary>
        /// 
        /// </summary>
        public void MoveDownSelected()
        {
            double dy = _project.Options.SnapToGrid ? _project.Options.SnapY : 1.0;
            MoveSelectedByWithHistory(0.0, dy);
        }

        /// <summary>
        /// 
        /// </summary>
        public void MoveLeftSelected()
        {
            double dx = _project.Options.SnapToGrid ? -_project.Options.SnapX : -1.0;
            MoveSelectedByWithHistory(dx, 0.0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void MoveRightSelected()
        {
            double dx = _project.Options.SnapToGrid ? _project.Options.SnapX : 1.0;
            MoveSelectedByWithHistory(dx, 0.0);
        }

        /// <summary>
        /// Removes container object from owner document <see cref="Document.Containers"/> collection.
        /// </summary>
        /// <param name="container">The container object to remove from document <see cref="Document.Containers"/> collection.</param>
        public void Delete(Container container)
        {
            if (_project == null || _project.Documents == null)
                return;

            var document = _project.Documents.FirstOrDefault(d => d.Containers.Contains(container));
            if (document != null)
            {
                if (_enableHistory)
                {
                    var previous = document.Containers;
                    var next = document.Containers.Remove(container);
                    _history.Snapshot(previous, next, (p) => document.Containers = p);
                    document.Containers = next;
                }
                else
                {
                    document.Containers = document.Containers.Remove(container);
                }

                _project.CurrentDocument = document;
                _project.CurrentContainer = document.Containers.FirstOrDefault();
                _project.Selected = _project.CurrentContainer;
            }
        }

        /// <summary>
        /// Removes document object from project <see cref="Project.Documents"/> collection.
        /// </summary>
        /// <param name="document">The document object to remove from project <see cref="Project.Documents"/> collection.</param>
        public void Delete(Document document)
        {
            if (_project == null || _project.Documents == null)
                return;

            if (_enableHistory)
            {
                var previous = _project.Documents;
                var next = _project.Documents.Remove(document);
                _history.Snapshot(previous, next, (p) => _project.Documents = p);
                _project.Documents = next;
            }
            else
            {
                _project.Documents = _project.Documents.Remove(document);
            }

            _project.CurrentDocument = _project.Documents.FirstOrDefault();
            if (_project.CurrentDocument != null)
            {
                _project.CurrentContainer = _project.CurrentDocument.Containers.FirstOrDefault();
            }
            else
            {
                _project.CurrentContainer = default(Container);
            }
            _project.Selected = _project.CurrentContainer;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteSelected()
        {
            if (_project == null
                || _project.CurrentContainer == null
                || _project.CurrentContainer.CurrentLayer == null)
                return;

            if (_renderers[0].State.SelectedShape != null)
            {
                var layer = _project.CurrentContainer.CurrentLayer;

                if (_enableHistory)
                {
                    var previous = layer.Shapes;
                    var next = layer.Shapes.Remove(_renderers[0].State.SelectedShape);
                    _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;
                }
                else
                {
                    layer.Shapes = layer.Shapes.Remove(_renderers[0].State.SelectedShape);
                }

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

                if (_enableHistory)
                {
                    var previous = layer.Shapes;
                    var next = builder.ToImmutable();
                    _history.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;
                }
                else
                {
                    layer.Shapes = builder.ToImmutable();
                }

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
            if (container == null)
                return;

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
            if (container == null)
                return;

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
            if (_renderers == null)
                return;

            _renderers[0].State.SelectedShape = default(BaseShape);
            _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public void Deselect(Container container)
        {
            if (container == null || _renderers == null)
                return;

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
            if (container == null)
                return false;

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
            if (container == null)
                return false;

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
            if (_project == null || _project.CurrentContainer == null)
                return false;

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
            if (_project == null || _project.CurrentContainer == null || _project.Options == null)
                return false;

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
                    _project.Options.PointShape,
                    line.IsStroked);

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

                AddShape(split);

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
            if (_project == null || _project.Options == null)
                return false;

            // Points must be aligned horizontally or vertically.
            if (p0.X != p1.X && p0.Y != p1.Y)
                return false;

            // Line must be horizontal or vertical.
            if (line.Start.X != line.End.X && line.Start.Y != line.End.Y)
                return false;

            XLine split;
            if (line.Start.X > line.End.X || line.Start.Y > line.End.Y)
            {
                split = XLine.Create(
                    p0,
                    line.End,
                    line.Style,
                    _project.Options.PointShape,
                    line.IsStroked);
                line.End = p1;
            }
            else
            {
                split = XLine.Create(
                    p1,
                    line.End,
                    line.Style,
                    _project.Options.PointShape,
                    line.IsStroked);
                line.End = p0;
            }

            AddShape(split);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool TryToConnect(XGroup group)
        {
            if (_project == null
                || _project.CurrentContainer == null
                || _project.CurrentContainer.CurrentLayer == null
                || _project.Options == null)
                return false;

            var layer = _project.CurrentContainer.CurrentLayer;
            if (group.Connectors.Length > 0)
            {
                var wires = Editor.GetAllShapes<XLine>(layer.Shapes);
                var dict = new Dictionary<XLine, IList<XPoint>>();

                // Find possible group to line connections.
                foreach (var connector in group.Connectors)
                {
                    var p = new Vector2(connector.X, connector.Y);
                    var t = _project.Options.HitTreshold;
                    XLine result = null;
                    foreach (var line in wires)
                    {
                        if (ShapeBounds.HitTestLine(line, p, t, 0, 0))
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

                // Try to split lines using group connectors.
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
            return _project != null
                && _project.CurrentContainer != null
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
            return _project != null
                && _project.CurrentContainer != null
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
            return _project != null
                && _project.CurrentContainer != null
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
            return _project != null
                && _project.CurrentContainer != null
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
            return _project != null
                && _project.CurrentContainer != null
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
            Helpers[CurrentTool].LeftDown(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void LeftUp(double x, double y)
        {
            Helpers[CurrentTool].LeftUp(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void RightDown(double x, double y)
        {
            Helpers[CurrentTool].RightDown(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void RightUp(double x, double y)
        {
            Helpers[CurrentTool].RightUp(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(double x, double y)
        {
            Helpers[CurrentTool].Move(x, y);
        }

        /// <summary>
        /// Creates a new <see cref="Editor"/> instance.
        /// </summary>
        /// <param name="project">The project to edit.</param>
        /// <param name="renderers">The shape renderer's.</param>
        /// <param name="enableObserver">Enable project observer.</param>
        /// <param name="enableHistory">Enable project history.</param>
        /// <param name="currentTool">The current tool.</param>
        /// <param name="currentPathTool">The current path tool.</param>
        /// <returns></returns>
        public static Editor Create(
            Project project,
            IRenderer[] renderers = null,
            bool enableObserver = true,
            bool enableHistory = true,
            Tool currentTool = Tool.Selection,
            PathTool currentPathTool = PathTool.Line)
        {
            var editor = new Editor()
            {
                CurrentTool = currentTool,
                CurrentPathTool = currentPathTool,
                EnableObserver = enableObserver,
                EnableHistory = enableHistory
            };

            var helpers = ImmutableDictionary.CreateBuilder<Tool, Helper>();
            helpers.Add(Tool.None, new NoneHelper(editor));
            helpers.Add(Tool.Selection, new SelectionHelper(editor));
            helpers.Add(Tool.Point, new PointHelper(editor));
            helpers.Add(Tool.Line, new LineHelper(editor));
            helpers.Add(Tool.Arc, new ArcHelper(editor));
            helpers.Add(Tool.Bezier, new BezierHelper(editor));
            helpers.Add(Tool.QBezier, new QBezierHelper(editor));
            helpers.Add(Tool.Path, new PathHelper(editor));
            helpers.Add(Tool.Rectangle, new RectangleHelper(editor));
            helpers.Add(Tool.Ellipse, new EllipseHelper(editor));
            helpers.Add(Tool.Text, new TextHelper(editor));
            helpers.Add(Tool.Image, new ImageHelper(editor));
            editor.Helpers = helpers.ToImmutable();

            editor.Project = project;
            editor.ProjectPath = string.Empty;
            editor.IsProjectDirty = false;

            editor.Renderers = renderers;

            if (editor.Renderers != null)
            {
                foreach (var renderer in editor.Renderers)
                {
                    if (renderer.State != null)
                    {
                        renderer.State.ImageCache = project;
                    }
                }
            }

            editor.Invalidate = () => { };

            if (editor.EnableObserver)
            {
                editor.Observer = new Observer(editor);
            }

            if (editor.EnableHistory)
            {
                editor.History = new History();
            }

            return editor;
        }
    }
}
