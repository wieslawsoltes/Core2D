// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    /// <summary>
    /// Project property changes observer.
    /// </summary>
    public class ProjectObserver : IDisposable
    {
        private readonly ProjectEditor _editor;
        private readonly Action _invalidateContainer;
        private readonly Action _invalidateStyles;
        private readonly Action _invalidateLayers;
        private readonly Action _invalidateShapes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectObserver"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        public ProjectObserver(ProjectEditor editor)
        {
            if (editor?.Project != null)
            {
                _editor = editor;

                _invalidateContainer = () => { };
                _invalidateStyles = () => InvalidateAndClearCache();
                _invalidateLayers = () => Invalidate();
                _invalidateShapes = () => Invalidate();

                Add(_editor.Project);
            }
        }

        private void Invalidate()
        {
            if (_editor?.Project?.CurrentContainer != null)
            {
                _editor.Project.CurrentContainer.Invalidate();
            }
        }

        private void InvalidateAndClearCache()
        {
            if (_editor?.Project?.CurrentContainer != null)
            {
                foreach (var renderer in _editor.Renderers)
                {
                    renderer.ClearCache(isZooming: false);
                }
                _editor.Project.CurrentContainer.Invalidate();
            }
        }

        private void MarkAsDirty()
        {
            if (_editor != null)
            {
                _editor.IsProjectDirty = true;
            }
        }

        private void ObserveDatabase(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Database.Columns))
            {
                var database = sender as Database;
                Remove(database.Columns);
                Add(database.Columns);
            }

            if (e.PropertyName == nameof(Database.Records))
            {
                var database = sender as Database;
                Remove(database.Records);
                Add(database.Records);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveColumn(object sender, PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveRecord(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Record.Values))
            {
                var record = sender as Record;
                Remove(record.Values);
                Add(record.Values);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveValue(object sender, PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveProject(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IProjectContainer.Databases))
            {
                var project = sender as IProjectContainer;
                Remove(project.Databases);
                Add(project.Databases);
            }

            if (e.PropertyName == nameof(IProjectContainer.StyleLibraries))
            {
                var project = sender as IProjectContainer;
                Remove(project.StyleLibraries);
                Add(project.StyleLibraries);
            }

            if (e.PropertyName == nameof(IProjectContainer.GroupLibraries))
            {
                var project = sender as IProjectContainer;
                Remove(project.GroupLibraries);
                Add(project.GroupLibraries);
            }

            if (e.PropertyName == nameof(IProjectContainer.Templates))
            {
                var project = sender as IProjectContainer;
                Remove(project.Templates);
                Add(project.Templates);
            }

            if (e.PropertyName == nameof(IProjectContainer.Documents))
            {
                var project = sender as IProjectContainer;
                Remove(project.Documents);
                Add(project.Documents);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveDocument(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IDocumentContainer.Pages))
            {
                var document = sender as IDocumentContainer;
                Remove(document.Pages);
                Add(document.Pages);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObservePage(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Context.Properties))
            {
                var container = sender as IPageContainer;
                Remove((IEnumerable<Property>)container.Data.Properties);
                Add((IEnumerable<Property>)container.Data.Properties);
            }

            if (e.PropertyName == nameof(IPageContainer.Layers))
            {
                var container = sender as IPageContainer;
                Remove(container.Layers);
                Add(container.Layers);
            }

            _invalidateContainer();
            MarkAsDirty();
        }

        private void ObserveTemplateBackgroud(object sender, PropertyChangedEventArgs e)
        {
            _editor.Project.CurrentContainer.Notify(nameof(IPageContainer.Background));
            var page = _editor.Project.CurrentContainer;
            if (page != null)
            {
                page.Template.Notify(nameof(IPageContainer.Background));
            }
            _invalidateLayers();
            MarkAsDirty();
        }

        private void ObserveInvalidateLayer(object sender, InvalidateLayerEventArgs e)
        {
            _editor?.CanvasPlatform?.Invalidate?.Invoke();
        }

        private void ObserveLayer(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ILayerContainer.Shapes))
            {
                var layer = sender as ILayerContainer;
                Remove(layer.Shapes);
                Add(layer.Shapes);
            }

            _invalidateLayers();
            MarkAsDirty();
        }

        private void ObserveShape(object sender, PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveStyleLibrary(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ILibrary<IShapeStyle>.Items))
            {
                var sg = sender as ILibrary<IShapeStyle>;
                Remove(sg.Items);
                Add(sg.Items);
            }

            _invalidateStyles();

            // NOTE: Do not mark project as dirty when current style changes.
            if (e.PropertyName != nameof(ILibrary<IShapeStyle>.Selected))
            {
                MarkAsDirty();
            }
        }

        private void ObserveGroupLibrary(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ILibrary<IGroupShape>.Items))
            {
                var sg = sender as ILibrary<IGroupShape>;
                Remove(sg.Items);
                Add(sg.Items);
            }
            MarkAsDirty();
        }

        private void ObserveStyle(object sender, PropertyChangedEventArgs e)
        {
            _invalidateStyles();
            MarkAsDirty();
        }

        private void ObserveProperty(object sender, PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveData(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Context.Properties))
            {
                var data = sender as Context;
                Remove(data.Properties);
                Add(data.Properties);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveState(object sender, PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveTransform(object sender, PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void Add(Database database)
        {
            if (database == null)
                return;

            database.PropertyChanged += ObserveDatabase;

            if (database.Columns != null)
            {
                Add(database.Columns);
            }

            if (database.Records != null)
            {
                Add(database.Records);
            }
        }

        private void Remove(Database database)
        {
            if (database == null)
                return;

            database.PropertyChanged -= ObserveDatabase;

            if (database.Columns != null)
            {
                Remove(database.Columns);
            }

            if (database.Records != null)
            {
                Remove(database.Records);
            }
        }

        private void Add(Column column)
        {
            if (column == null)
                return;

            column.PropertyChanged += ObserveColumn;
        }

        private void Remove(Column column)
        {
            if (column == null)
                return;

            column.PropertyChanged -= ObserveColumn;
        }

        private void Add(Record record)
        {
            if (record == null)
                return;

            record.PropertyChanged += ObserveRecord;

            if (record.Values != null)
            {
                Add(record.Values);
            }
        }

        private void Remove(Record record)
        {
            if (record == null)
                return;

            record.PropertyChanged -= ObserveRecord;

            if (record.Values != null)
            {
                Remove(record.Values);
            }
        }

        private void Add(Value value)
        {
            if (value == null)
                return;

            value.PropertyChanged += ObserveValue;
        }

        private void Remove(Value value)
        {
            if (value == null)
                return;

            value.PropertyChanged -= ObserveValue;
        }

        private void Add(IOptions options)
        {
            if (options == null)
                return;

            if (options.PointShape != null)
            {
                Add(options.PointShape);
            }

            if (options.PointStyle != null)
            {
                Add(options.PointStyle);
            }

            if (options.SelectionStyle != null)
            {
                Add(options.SelectionStyle);
            }

            if (options.HelperStyle != null)
            {
                Add(options.HelperStyle);
            }
        }

        private void Remove(IOptions options)
        {
            if (options == null)
                return;

            if (options.PointShape != null)
            {
                Remove(options.PointShape);
            }

            if (options.PointStyle != null)
            {
                Remove(options.PointStyle);
            }

            if (options.SelectionStyle != null)
            {
                Remove(options.SelectionStyle);
            }

            if (options.HelperStyle != null)
            {
                Remove(options.HelperStyle);
            }
        }

        private void Add(IProjectContainer project)
        {
            if (project == null)
                return;

            project.PropertyChanged += ObserveProject;

            Add(project.Options);

            if (project.Databases != null)
            {
                foreach (var database in project.Databases)
                {
                    Add(database);
                }
            }

            if (project.Documents != null)
            {
                foreach (var document in project.Documents)
                {
                    Add(document);
                }
            }

            if (project.Templates != null)
            {
                foreach (var template in project.Templates)
                {
                    Add(template);
                }
            }

            if (project.StyleLibraries != null)
            {
                foreach (var sg in project.StyleLibraries)
                {
                    Add(sg);
                }
            }
        }

        private void Remove(IProjectContainer project)
        {
            if (project == null)
                return;

            project.PropertyChanged -= ObserveProject;

            Remove(project.Options);

            if (project.Databases != null)
            {
                foreach (var database in project.Databases)
                {
                    Remove(database);
                }
            }

            if (project.Documents != null)
            {
                foreach (var document in project.Documents)
                {
                    Remove(document);
                }
            }

            if (project.Templates != null)
            {
                foreach (var template in project.Templates)
                {
                    Remove(template);
                }
            }

            if (project.StyleLibraries != null)
            {
                foreach (var sg in project.StyleLibraries)
                {
                    Remove(sg);
                }
            }
        }

        private void Add(IDocumentContainer document)
        {
            if (document == null)
                return;

            document.PropertyChanged += ObserveDocument;

            if (document.Pages != null)
            {
                foreach (var container in document.Pages)
                {
                    Add(container);
                }
            }
        }

        private void Remove(IDocumentContainer document)
        {
            if (document == null)
                return;

            document.PropertyChanged -= ObserveDocument;

            if (document.Pages != null)
            {
                foreach (var container in document.Pages)
                {
                    Remove(container);
                }
            }
        }

        private void Add(IPageContainer container)
        {
            if (container == null)
                return;

            container.PropertyChanged += ObservePage;

            if (container.Background != null)
            {
                container.Background.PropertyChanged += ObserveTemplateBackgroud;
            }

            if (container.Layers != null)
            {
                Add(container.Layers);
            }

            if (container.Data != null)
            {
                Add((Context)container.Data);
            }

            if (container.WorkingLayer != null)
            {
                container.WorkingLayer.InvalidateLayer += ObserveInvalidateLayer;
            }

            if (container.HelperLayer != null)
            {
                container.HelperLayer.InvalidateLayer += ObserveInvalidateLayer;
            }
        }

        private void Remove(IPageContainer container)
        {
            if (container == null)
                return;

            container.PropertyChanged -= ObservePage;

            if (container.Background != null)
            {
                container.Background.PropertyChanged -= ObserveTemplateBackgroud;
            }

            if (container.Layers != null)
            {
                Remove(container.Layers);
            }

            if (container.Data != null)
            {
                Remove((Context)container.Data);
            }

            if (container.WorkingLayer != null)
            {
                container.WorkingLayer.InvalidateLayer -= ObserveInvalidateLayer;
            }

            if (container.HelperLayer != null)
            {
                container.HelperLayer.InvalidateLayer -= ObserveInvalidateLayer;
            }
        }

        private void Add(ILayerContainer layer)
        {
            if (layer == null)
                return;

            layer.PropertyChanged += ObserveLayer;

            if (layer.Shapes != null)
            {
                Add(layer.Shapes);
            }

            layer.InvalidateLayer += ObserveInvalidateLayer;
        }

        private void Remove(ILayerContainer layer)
        {
            if (layer == null)
                return;

            layer.PropertyChanged -= ObserveLayer;

            if (layer.Shapes != null)
            {
                Remove(layer.Shapes);
            }

            layer.InvalidateLayer -= ObserveInvalidateLayer;
        }

        private void Add(IBaseShape shape)
        {
            if (shape == null)
                return;

            shape.PropertyChanged += ObserveShape;

            if (shape.Data != null)
            {
                Add((Context)shape.Data);
            }

            if (shape.State != null)
            {
                shape.State.PropertyChanged += ObserveState;
            }

            if (shape.Transform != null)
            {
                shape.Transform.PropertyChanged += ObserveTransform;
            }

            if (shape is IPointShape point)
            {
                if (point.Shape != null)
                {
                    point.Shape.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is ILineShape line)
            {
                if (line.Start != null)
                {
                    line.Start.PropertyChanged += ObserveShape;
                }

                if (line.End != null)
                {
                    line.End.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is IRectangleShape rectangle)
            {
                if (rectangle.TopLeft != null)
                {
                    rectangle.TopLeft.PropertyChanged += ObserveShape;
                }

                if (rectangle.BottomRight != null)
                {
                    rectangle.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is IEllipseShape ellipse)
            {
                if (ellipse.TopLeft != null)
                {
                    ellipse.TopLeft.PropertyChanged += ObserveShape;
                }

                if (ellipse.BottomRight != null)
                {
                    ellipse.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is IArcShape arc)
            {
                if (arc.Point1 != null)
                {
                    arc.Point1.PropertyChanged += ObserveShape;
                }

                if (arc.Point2 != null)
                {
                    arc.Point2.PropertyChanged += ObserveShape;
                }

                if (arc.Point3 != null)
                {
                    arc.Point3.PropertyChanged += ObserveShape;
                }

                if (arc.Point4 != null)
                {
                    arc.Point4.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is ICubicBezierShape cubicBezier)
            {
                if (cubicBezier.Point1 != null)
                {
                    cubicBezier.Point1.PropertyChanged += ObserveShape;
                }

                if (cubicBezier.Point2 != null)
                {
                    cubicBezier.Point2.PropertyChanged += ObserveShape;
                }

                if (cubicBezier.Point3 != null)
                {
                    cubicBezier.Point3.PropertyChanged += ObserveShape;
                }

                if (cubicBezier.Point4 != null)
                {
                    cubicBezier.Point4.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is IQuadraticBezierShape quadraticBezier)
            {
                if (quadraticBezier.Point1 != null)
                {
                    quadraticBezier.Point1.PropertyChanged += ObserveShape;
                }

                if (quadraticBezier.Point2 != null)
                {
                    quadraticBezier.Point2.PropertyChanged += ObserveShape;
                }

                if (quadraticBezier.Point3 != null)
                {
                    quadraticBezier.Point3.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is ITextShape text)
            {
                if (text.TopLeft != null)
                {
                    text.TopLeft.PropertyChanged += ObserveShape;
                }

                if (text.BottomRight != null)
                {
                    text.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is IImageShape image)
            {
                if (image.TopLeft != null)
                {
                    image.TopLeft.PropertyChanged += ObserveShape;
                }

                if (image.BottomRight != null)
                {
                    image.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is IPathShape path)
            {
                if (path.Geometry != null)
                {
                    Add(path.Geometry);
                }
            }
            else if (shape is IGroupShape group)
            {
                if (group.Shapes != null)
                {
                    Add(group.Shapes);
                }

                if (group.Connectors != null)
                {
                    Add(group.Connectors);
                }
            }
        }

        private void Remove(IBaseShape shape)
        {
            if (shape == null)
                return;

            shape.PropertyChanged -= ObserveShape;

            if (shape.Data != null)
            {
                Remove((Context)shape.Data);
            }

            if (shape.State != null)
            {
                shape.State.PropertyChanged -= ObserveState;
            }

            if (shape.Transform != null)
            {
                shape.Transform.PropertyChanged -= ObserveTransform;
            }

            if (shape is IPointShape point)
            {
                if (point.Shape != null)
                {
                    point.Shape.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is ILineShape line)
            {
                if (line.Start != null)
                {
                    line.Start.PropertyChanged -= ObserveShape;
                }

                if (line.End != null)
                {
                    line.End.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is IRectangleShape rectangle)
            {
                if (rectangle.TopLeft != null)
                {
                    rectangle.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (rectangle.BottomRight != null)
                {
                    rectangle.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is IEllipseShape ellipse)
            {
                if (ellipse.TopLeft != null)
                {
                    ellipse.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (ellipse.BottomRight != null)
                {
                    ellipse.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is IArcShape arc)
            {
                if (arc.Point1 != null)
                {
                    arc.Point1.PropertyChanged -= ObserveShape;
                }

                if (arc.Point2 != null)
                {
                    arc.Point2.PropertyChanged -= ObserveShape;
                }

                if (arc.Point3 != null)
                {
                    arc.Point3.PropertyChanged -= ObserveShape;
                }

                if (arc.Point4 != null)
                {
                    arc.Point4.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is ICubicBezierShape cubicBezier)
            {
                if (cubicBezier.Point1 != null)
                {
                    cubicBezier.Point1.PropertyChanged -= ObserveShape;
                }

                if (cubicBezier.Point2 != null)
                {
                    cubicBezier.Point2.PropertyChanged -= ObserveShape;
                }

                if (cubicBezier.Point3 != null)
                {
                    cubicBezier.Point3.PropertyChanged -= ObserveShape;
                }

                if (cubicBezier.Point4 != null)
                {
                    cubicBezier.Point4.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is IQuadraticBezierShape quadraticBezier)
            {
                if (quadraticBezier.Point1 != null)
                {
                    quadraticBezier.Point1.PropertyChanged -= ObserveShape;
                }

                if (quadraticBezier.Point2 != null)
                {
                    quadraticBezier.Point2.PropertyChanged -= ObserveShape;
                }

                if (quadraticBezier.Point3 != null)
                {
                    quadraticBezier.Point3.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is ITextShape text)
            {
                if (text.TopLeft != null)
                {
                    text.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (text.BottomRight != null)
                {
                    text.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is IImageShape image)
            {
                if (image.TopLeft != null)
                {
                    image.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (image.BottomRight != null)
                {
                    image.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is IPathShape path)
            {
                if (path.Geometry != null)
                {
                    Remove(path.Geometry);
                }
            }
            else if (shape is IGroupShape group)
            {
                if (group.Shapes != null)
                {
                    Remove(group.Shapes);
                }

                if (group.Connectors != null)
                {
                    Remove(group.Connectors);
                }
            }
        }

        private void Add(IPathGeometry geometry)
        {
            if (geometry == null)
                return;

            geometry.PropertyChanged += ObserveShape;

            if (geometry.Figures != null)
            {
                Add(geometry.Figures);
            }
        }

        private void Remove(IPathGeometry geometry)
        {
            if (geometry == null)
                return;

            geometry.PropertyChanged -= ObserveShape;

            if (geometry.Figures != null)
            {
                Remove(geometry.Figures);
            }
        }

        private void Add(IPathFigure figure)
        {
            if (figure == null)
                return;

            figure.PropertyChanged += ObserveShape;

            if (figure.StartPoint != null)
            {
                figure.StartPoint.PropertyChanged += ObserveShape;
            }

            if (figure.Segments != null)
            {
                Add(figure.Segments);
            }
        }

        private void Remove(IPathFigure figure)
        {
            if (figure == null)
                return;

            figure.PropertyChanged -= ObserveShape;

            if (figure.StartPoint != null)
            {
                figure.StartPoint.PropertyChanged -= ObserveShape;
            }

            if (figure.Segments != null)
            {
                Remove(figure.Segments);
            }
        }

        private void Add(IPathSegment segment)
        {
            if (segment == null)
                return;

            segment.PropertyChanged += ObserveShape;

            if (segment is LineSegment)
            {
                var lineSegment = segment as LineSegment;

                lineSegment.Point.PropertyChanged += ObserveShape;
            }
            else if (segment is ArcSegment)
            {
                var arcSegment = segment as ArcSegment;

                arcSegment.Point.PropertyChanged += ObserveShape;
                arcSegment.Size.PropertyChanged += ObserveShape;
            }
            else if (segment is CubicBezierSegment)
            {
                var cubicBezierSegment = segment as CubicBezierSegment;

                cubicBezierSegment.Point1.PropertyChanged += ObserveShape;
                cubicBezierSegment.Point2.PropertyChanged += ObserveShape;
                cubicBezierSegment.Point3.PropertyChanged += ObserveShape;
            }
            else if (segment is QuadraticBezierSegment)
            {
                var quadraticBezierSegment = segment as QuadraticBezierSegment;

                quadraticBezierSegment.Point1.PropertyChanged += ObserveShape;
                quadraticBezierSegment.Point2.PropertyChanged += ObserveShape;
            }
            else if (segment is PolyLineSegment)
            {
                var polyLineSegment = segment as PolyLineSegment;

                Add(polyLineSegment.Points);
            }
            else if (segment is PolyCubicBezierSegment)
            {
                var polyCubicBezierSegment = segment as PolyCubicBezierSegment;

                Add(polyCubicBezierSegment.Points);
            }
            else if (segment is PolyQuadraticBezierSegment)
            {
                var polyQuadraticBezierSegment = segment as PolyQuadraticBezierSegment;

                Add(polyQuadraticBezierSegment.Points);
            }
        }

        private void Remove(IPathSegment segment)
        {
            if (segment == null)
                return;

            segment.PropertyChanged -= ObserveShape;

            if (segment is LineSegment)
            {
                var lineSegment = segment as LineSegment;

                lineSegment.Point.PropertyChanged -= ObserveShape;
            }
            else if (segment is ArcSegment)
            {
                var arcSegment = segment as ArcSegment;

                arcSegment.Point.PropertyChanged -= ObserveShape;
                arcSegment.Size.PropertyChanged -= ObserveShape;
            }
            else if (segment is CubicBezierSegment)
            {
                var cubicBezierSegment = segment as CubicBezierSegment;

                cubicBezierSegment.Point1.PropertyChanged -= ObserveShape;
                cubicBezierSegment.Point2.PropertyChanged -= ObserveShape;
                cubicBezierSegment.Point3.PropertyChanged -= ObserveShape;
            }
            else if (segment is QuadraticBezierSegment)
            {
                var quadraticBezierSegment = segment as QuadraticBezierSegment;

                quadraticBezierSegment.Point1.PropertyChanged -= ObserveShape;
                quadraticBezierSegment.Point2.PropertyChanged -= ObserveShape;
            }
            else if (segment is PolyLineSegment)
            {
                var polyLineSegment = segment as PolyLineSegment;

                Remove(polyLineSegment.Points);
            }
            else if (segment is PolyCubicBezierSegment)
            {
                var polyCubicBezierSegment = segment as PolyCubicBezierSegment;

                Remove(polyCubicBezierSegment.Points);
            }
            else if (segment is PolyQuadraticBezierSegment)
            {
                var polyQuadraticBezierSegment = segment as PolyQuadraticBezierSegment;

                Remove(polyQuadraticBezierSegment.Points);
            }
        }

        private void Add(Context data)
        {
            if (data == null)
                return;

            if (data.Properties != null)
            {
                Add(data.Properties);
            }

            data.PropertyChanged += ObserveData;
        }

        private void Remove(Context data)
        {
            if (data == null)
                return;

            if (data.Properties != null)
            {
                Remove(data.Properties);
            }

            data.PropertyChanged -= ObserveData;
        }

        private void Add(ILibrary<IShapeStyle> sg)
        {
            if (sg == null)
                return;

            if (sg.Items != null)
            {
                Add(sg.Items);
            }

            sg.PropertyChanged += ObserveStyleLibrary;
        }

        private void Remove(ILibrary<IShapeStyle> sg)
        {
            if (sg == null)
                return;

            if (sg.Items != null)
            {
                Remove(sg.Items);
            }

            sg.PropertyChanged -= ObserveStyleLibrary;
        }

        private void Add(ILibrary<IGroupShape> gl)
        {
            if (gl == null)
                return;

            if (gl.Items != null)
            {
                Add(gl.Items);
            }

            gl.PropertyChanged += ObserveGroupLibrary;
        }

        private void Remove(ILibrary<IGroupShape> gl)
        {
            if (gl == null)
                return;

            if (gl.Items != null)
            {
                Remove(gl.Items);
            }

            gl.PropertyChanged -= ObserveGroupLibrary;
        }

        private void Add(IShapeStyle style)
        {
            if (style == null)
                return;

            style.PropertyChanged += ObserveStyle;

            if (style.Stroke != null)
            {
                style.Stroke.PropertyChanged += ObserveStyle;
            }

            if (style.Fill != null)
            {
                style.Fill.PropertyChanged += ObserveStyle;
            }

            if (style.LineStyle != null)
            {
                style.LineStyle.PropertyChanged += ObserveStyle;

                if (style.LineStyle.FixedLength != null)
                {
                    style.LineStyle.FixedLength.PropertyChanged += ObserveStyle;
                }
            }

            if (style.StartArrowStyle != null)
            {
                style.StartArrowStyle.PropertyChanged += ObserveStyle;

                if (style.StartArrowStyle.Stroke != null)
                {
                    style.StartArrowStyle.Stroke.PropertyChanged += ObserveStyle;
                }

                if (style.StartArrowStyle.Fill != null)
                {
                    style.StartArrowStyle.Fill.PropertyChanged += ObserveStyle;
                }
            }

            if (style.EndArrowStyle != null)
            {
                style.EndArrowStyle.PropertyChanged += ObserveStyle;

                if (style.EndArrowStyle.Stroke != null)
                {
                    style.EndArrowStyle.Stroke.PropertyChanged += ObserveStyle;
                }

                if (style.EndArrowStyle.Fill != null)
                {
                    style.EndArrowStyle.Fill.PropertyChanged += ObserveStyle;
                }
            }

            if (style.TextStyle != null)
            {
                style.TextStyle.PropertyChanged += ObserveStyle;

                if (style.TextStyle.FontStyle != null)
                {
                    style.TextStyle.FontStyle.PropertyChanged += ObserveStyle;
                }
            }
        }

        private void Remove(IShapeStyle style)
        {
            if (style == null)
                return;

            style.PropertyChanged -= ObserveStyle;

            if (style.Stroke != null)
            {
                style.Stroke.PropertyChanged -= ObserveStyle;
            }

            if (style.Fill != null)
            {
                style.Fill.PropertyChanged -= ObserveStyle;
            }

            if (style.LineStyle != null)
            {
                style.LineStyle.PropertyChanged -= ObserveStyle;

                if (style.LineStyle.FixedLength != null)
                {
                    style.LineStyle.FixedLength.PropertyChanged -= ObserveStyle;
                }
            }

            if (style.StartArrowStyle != null)
            {
                style.StartArrowStyle.PropertyChanged -= ObserveStyle;

                if (style.StartArrowStyle.Stroke != null)
                {
                    style.StartArrowStyle.Stroke.PropertyChanged -= ObserveStyle;
                }

                if (style.StartArrowStyle.Fill != null)
                {
                    style.StartArrowStyle.Fill.PropertyChanged -= ObserveStyle;
                }
            }

            if (style.EndArrowStyle != null)
            {
                style.EndArrowStyle.PropertyChanged -= ObserveStyle;

                if (style.EndArrowStyle.Stroke != null)
                {
                    style.EndArrowStyle.Stroke.PropertyChanged -= ObserveStyle;
                }

                if (style.EndArrowStyle.Fill != null)
                {
                    style.EndArrowStyle.Fill.PropertyChanged -= ObserveStyle;
                }
            }

            if (style.TextStyle != null)
            {
                style.TextStyle.PropertyChanged -= ObserveStyle;

                if (style.TextStyle.FontStyle != null)
                {
                    style.TextStyle.FontStyle.PropertyChanged -= ObserveStyle;
                }
            }
        }

        private void Add(Property property)
        {
            if (property == null)
                return;

            property.PropertyChanged += ObserveProperty;
        }

        private void Remove(Property property)
        {
            if (property == null)
                return;

            property.PropertyChanged -= ObserveProperty;
        }

        private void Add(IEnumerable<Database> databases)
        {
            if (databases == null)
                return;

            foreach (var database in databases)
            {
                Add(database);
            }
        }

        private void Remove(IEnumerable<Database> databases)
        {
            if (databases == null)
                return;

            foreach (var database in databases)
            {
                Remove(database);
            }
        }

        private void Add(IEnumerable<Column> columns)
        {
            if (columns == null)
                return;

            foreach (var column in columns)
            {
                Add(column);
            }
        }

        private void Remove(IEnumerable<Column> columns)
        {
            if (columns == null)
                return;

            foreach (var column in columns)
            {
                Remove(column);
            }
        }

        private void Add(IEnumerable<Record> records)
        {
            if (records == null)
                return;

            foreach (var record in records)
            {
                Add(record);
            }
        }

        private void Remove(IEnumerable<Record> records)
        {
            if (records == null)
                return;

            foreach (var record in records)
            {
                Remove(record);
            }
        }

        private void Add(IEnumerable<Value> values)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                Add(value);
            }
        }

        private void Remove(IEnumerable<Value> values)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                Remove(value);
            }
        }

        private void Add(IEnumerable<IDocumentContainer> documents)
        {
            if (documents == null)
                return;

            foreach (var document in documents)
            {
                Add(document);
            }
        }

        private void Remove(IEnumerable<IDocumentContainer> documents)
        {
            if (documents == null)
                return;

            foreach (var document in documents)
            {
                Remove(document);
            }
        }

        private void Add(IEnumerable<IPageContainer> containers)
        {
            if (containers == null)
                return;

            foreach (var page in containers)
            {
                Add(page);
            }
        }

        private void Remove(IEnumerable<IPageContainer> containers)
        {
            if (containers == null)
                return;

            foreach (var page in containers)
            {
                Remove(page);
            }
        }

        private void Add(IEnumerable<ILayerContainer> layers)
        {
            if (layers == null)
                return;

            foreach (var layer in layers)
            {
                Add(layer);
            }
        }

        private void Remove(IEnumerable<ILayerContainer> layers)
        {
            if (layers == null)
                return;

            foreach (var layer in layers)
            {
                Remove(layer);
            }
        }

        private void Add(IEnumerable<IBaseShape> shapes)
        {
            if (shapes == null)
                return;

            foreach (var shape in shapes)
            {
                Add(shape);
            }
        }

        private void Remove(IEnumerable<IBaseShape> shapes)
        {
            if (shapes == null)
                return;

            foreach (var shape in shapes)
            {
                Remove(shape);
            }
        }

        private void Add(IEnumerable<IPathFigure> figures)
        {
            if (figures == null)
                return;

            foreach (var figure in figures)
            {
                Add(figure);
            }
        }

        private void Remove(IEnumerable<IPathFigure> figures)
        {
            if (figures == null)
                return;

            foreach (var figure in figures)
            {
                Remove(figure);
            }
        }

        private void Add(IEnumerable<IPathSegment> segments)
        {
            if (segments == null)
                return;

            foreach (var segment in segments)
            {
                Add(segment);
            }
        }

        private void Remove(IEnumerable<IPathSegment> segments)
        {
            if (segments == null)
                return;

            foreach (var segment in segments)
            {
                Remove(segment);
            }
        }

        private void Add(IEnumerable<IShapeStyle> styles)
        {
            if (styles == null)
                return;

            foreach (var style in styles)
            {
                Add(style);
            }
        }

        private void Remove(IEnumerable<IShapeStyle> styles)
        {
            if (styles == null)
                return;

            foreach (var style in styles)
            {
                Remove(style);
            }
        }

        private void Add(IEnumerable<ILibrary<IShapeStyle>> sgs)
        {
            if (sgs == null)
                return;

            foreach (var sg in sgs)
            {
                Add(sg);
            }
        }

        private void Remove(IEnumerable<ILibrary<IShapeStyle>> sgs)
        {
            if (sgs == null)
                return;

            foreach (var sg in sgs)
            {
                Remove(sg);
            }
        }

        private void Add(IEnumerable<ILibrary<IGroupShape>> gl)
        {
            if (gl == null)
                return;

            foreach (var g in gl)
            {
                Add(g);
            }
        }

        private void Remove(IEnumerable<ILibrary<IGroupShape>> gl)
        {
            if (gl == null)
                return;

            foreach (var g in gl)
            {
                Remove(g);
            }
        }

        private void Add(IEnumerable<Property> properties)
        {
            if (properties == null)
                return;

            foreach (var property in properties)
            {
                Add(property);
            }
        }

        private void Remove(IEnumerable<Property> properties)
        {
            if (properties == null)
                return;

            foreach (var property in properties)
            {
                Remove(property);
            }
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public void Dispose()
        {
            if (_editor?.Project != null)
            {
                Remove(_editor.Project);
            }
        }
    }
}
