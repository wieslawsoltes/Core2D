// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Project;
using Core2D.Shape;
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

        private void ObserveDatabase(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        private void ObserveColumn(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveRecord(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        private void ObserveValue(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveProject(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProjectContainer.Databases))
            {
                var project = sender as ProjectContainer;
                Remove(project.Databases);
                Add(project.Databases);
            }

            if (e.PropertyName == nameof(ProjectContainer.StyleLibraries))
            {
                var project = sender as ProjectContainer;
                Remove(project.StyleLibraries);
                Add(project.StyleLibraries);
            }

            if (e.PropertyName == nameof(ProjectContainer.GroupLibraries))
            {
                var project = sender as ProjectContainer;
                Remove(project.GroupLibraries);
                Add(project.GroupLibraries);
            }

            if (e.PropertyName == nameof(ProjectContainer.Templates))
            {
                var project = sender as ProjectContainer;
                Remove(project.Templates);
                Add(project.Templates);
            }

            if (e.PropertyName == nameof(ProjectContainer.Documents))
            {
                var project = sender as ProjectContainer;
                Remove(project.Documents);
                Add(project.Documents);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveDocument(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DocumentContainer.Pages))
            {
                var document = sender as DocumentContainer;
                Remove(document.Pages);
                Add(document.Pages);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObservePage(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Data.Context.Properties))
            {
                var container = sender as PageContainer;
                Remove((IEnumerable<Property>)container.Data.Properties);
                Add((IEnumerable<Property>)container.Data.Properties);
            }

            if (e.PropertyName == nameof(PageContainer.Layers))
            {
                var container = sender as PageContainer;
                Remove(container.Layers);
                Add(container.Layers);
            }

            _invalidateContainer();
            MarkAsDirty();
        }

        private void ObserveTemplateBackgroud(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _editor.Project.CurrentContainer.Notify(nameof(PageContainer.Background));
            var page = _editor.Project.CurrentContainer;
            if (page != null)
            {
                page.Template.Notify(nameof(PageContainer.Background));
            }
            _invalidateLayers();
            MarkAsDirty();
        }

        private void ObserveInvalidateLayer(object sender, InvalidateLayerEventArgs e)
        {
            _editor?.Invalidate?.Invoke();
        }

        private void ObserveLayer(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LayerContainer.Shapes))
            {
                var layer = sender as LayerContainer;
                Remove(layer.Shapes);
                Add(layer.Shapes);
            }

            _invalidateLayers();
            MarkAsDirty();
        }

        private void ObserveShape(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveStyleLibrary(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Library<ShapeStyle>.Items))
            {
                var sg = sender as Library<ShapeStyle>;
                Remove(sg.Items);
                Add(sg.Items);
            }

            _invalidateStyles();

            // NOTE: Do not mark project as dirty when current style changes.
            if (e.PropertyName != nameof(Library<ShapeStyle>.Selected))
            {
                MarkAsDirty();
            }
        }

        private void ObserveGroupLibrary(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Library<GroupShape>.Items))
            {
                var sg = sender as Library<GroupShape>;
                Remove(sg.Items);
                Add(sg.Items);
            }
            MarkAsDirty();
        }

        private void ObserveStyle(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateStyles();
            MarkAsDirty();
        }

        private void ObserveProperty(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveData(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Data.Context.Properties))
            {
                var data = sender as Data.Context;
                Remove(data.Properties);
                Add(data.Properties);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveState(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveTransform(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        private void Add(Options options)
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

        private void Remove(Options options)
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

        private void Add(ProjectContainer project)
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

        private void Remove(ProjectContainer project)
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

        private void Add(DocumentContainer document)
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

        private void Remove(DocumentContainer document)
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

        private void Add(PageContainer container)
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
                Add((Data.Context)container.Data);
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

        private void Remove(PageContainer container)
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
                Remove((Data.Context)container.Data);
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

        private void Add(LayerContainer layer)
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

        private void Remove(LayerContainer layer)
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

        private void Add(BaseShape shape)
        {
            if (shape == null)
                return;

            shape.PropertyChanged += ObserveShape;

            if (shape.Data != null)
            {
                Add((Data.Context)shape.Data);
            }

            if (shape.State != null)
            {
                shape.State.PropertyChanged += ObserveState;
            }

            if (shape.Transform != null)
            {
                shape.Transform.PropertyChanged += ObserveTransform;
            }

            if (shape is PointShape)
            {
                var point = shape as PointShape;
                if (point.Shape != null)
                {
                    point.Shape.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is LineShape)
            {
                var line = shape as LineShape;

                if (line.Start != null)
                {
                    line.Start.PropertyChanged += ObserveShape;
                }

                if (line.End != null)
                {
                    line.End.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is RectangleShape)
            {
                var rectangle = shape as RectangleShape;

                if (rectangle.TopLeft != null)
                {
                    rectangle.TopLeft.PropertyChanged += ObserveShape;
                }

                if (rectangle.BottomRight != null)
                {
                    rectangle.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is EllipseShape)
            {
                var ellipse = shape as EllipseShape;

                if (ellipse.TopLeft != null)
                {
                    ellipse.TopLeft.PropertyChanged += ObserveShape;
                }

                if (ellipse.BottomRight != null)
                {
                    ellipse.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is ArcShape)
            {
                var arc = shape as ArcShape;

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
            else if (shape is CubicBezierShape)
            {
                var cubicBezier = shape as CubicBezierShape;

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
            else if (shape is QuadraticBezierShape)
            {
                var quadraticBezier = shape as QuadraticBezierShape;

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
            else if (shape is TextShape)
            {
                var text = shape as TextShape;

                if (text.TopLeft != null)
                {
                    text.TopLeft.PropertyChanged += ObserveShape;
                }

                if (text.BottomRight != null)
                {
                    text.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is ImageShape)
            {
                var image = shape as ImageShape;

                if (image.TopLeft != null)
                {
                    image.TopLeft.PropertyChanged += ObserveShape;
                }

                if (image.BottomRight != null)
                {
                    image.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is PathShape)
            {
                var path = shape as PathShape;

                if (path.Geometry != null)
                {
                    Add(path.Geometry);
                }
            }
            else if (shape is GroupShape)
            {

                if (shape is GroupShape group)
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
        }

        private void Remove(BaseShape shape)
        {
            if (shape == null)
                return;

            shape.PropertyChanged -= ObserveShape;

            if (shape.Data != null)
            {
                Remove((Data.Context)shape.Data);
            }

            if (shape.State != null)
            {
                shape.State.PropertyChanged -= ObserveState;
            }

            if (shape.Transform != null)
            {
                shape.Transform.PropertyChanged -= ObserveTransform;
            }

            if (shape is PointShape)
            {
                var point = shape as PointShape;
                if (point.Shape != null)
                {
                    point.Shape.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is LineShape)
            {
                var line = shape as LineShape;

                if (line.Start != null)
                {
                    line.Start.PropertyChanged -= ObserveShape;
                }

                if (line.End != null)
                {
                    line.End.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is RectangleShape)
            {
                var rectangle = shape as RectangleShape;

                if (rectangle.TopLeft != null)
                {
                    rectangle.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (rectangle.BottomRight != null)
                {
                    rectangle.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is EllipseShape)
            {
                var ellipse = shape as EllipseShape;

                if (ellipse.TopLeft != null)
                {
                    ellipse.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (ellipse.BottomRight != null)
                {
                    ellipse.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is ArcShape)
            {
                var arc = shape as ArcShape;

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
            else if (shape is CubicBezierShape)
            {
                var cubicBezier = shape as CubicBezierShape;

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
            else if (shape is QuadraticBezierShape)
            {
                var quadraticBezier = shape as QuadraticBezierShape;

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
            else if (shape is TextShape)
            {
                var text = shape as TextShape;

                if (text.TopLeft != null)
                {
                    text.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (text.BottomRight != null)
                {
                    text.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is ImageShape)
            {
                var image = shape as ImageShape;

                if (image.TopLeft != null)
                {
                    image.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (image.BottomRight != null)
                {
                    image.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is PathShape)
            {
                var path = shape as PathShape;

                if (path.Geometry != null)
                {
                    Remove(path.Geometry);
                }
            }
            else if (shape is GroupShape)
            {

                if (shape is GroupShape group)
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
        }

        private void Add(PathGeometry geometry)
        {
            if (geometry == null)
                return;

            geometry.PropertyChanged += ObserveShape;

            if (geometry.Figures != null)
            {
                Add(geometry.Figures);
            }
        }

        private void Remove(PathGeometry geometry)
        {
            if (geometry == null)
                return;

            geometry.PropertyChanged -= ObserveShape;

            if (geometry.Figures != null)
            {
                Remove(geometry.Figures);
            }
        }

        private void Add(PathFigure figure)
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

        private void Remove(PathFigure figure)
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

        private void Add(PathSegment segment)
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

        private void Remove(PathSegment segment)
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

        private void Add(Data.Context data)
        {
            if (data == null)
                return;

            if (data.Properties != null)
            {
                Add(data.Properties);
            }

            data.PropertyChanged += ObserveData;
        }

        private void Remove(Data.Context data)
        {
            if (data == null)
                return;

            if (data.Properties != null)
            {
                Remove(data.Properties);
            }

            data.PropertyChanged -= ObserveData;
        }

        private void Add(Library<ShapeStyle> sg)
        {
            if (sg == null)
                return;

            if (sg.Items != null)
            {
                Add(sg.Items);
            }

            sg.PropertyChanged += ObserveStyleLibrary;
        }

        private void Remove(Library<ShapeStyle> sg)
        {
            if (sg == null)
                return;

            if (sg.Items != null)
            {
                Remove(sg.Items);
            }

            sg.PropertyChanged -= ObserveStyleLibrary;
        }

        private void Add(Library<GroupShape> gl)
        {
            if (gl == null)
                return;

            if (gl.Items != null)
            {
                Add(gl.Items);
            }

            gl.PropertyChanged += ObserveGroupLibrary;
        }

        private void Remove(Library<GroupShape> gl)
        {
            if (gl == null)
                return;

            if (gl.Items != null)
            {
                Remove(gl.Items);
            }

            gl.PropertyChanged -= ObserveGroupLibrary;
        }

        private void Add(ShapeStyle style)
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

        private void Remove(ShapeStyle style)
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

        private void Add(IEnumerable<DocumentContainer> documents)
        {
            if (documents == null)
                return;

            foreach (var document in documents)
            {
                Add(document);
            }
        }

        private void Remove(IEnumerable<DocumentContainer> documents)
        {
            if (documents == null)
                return;

            foreach (var document in documents)
            {
                Remove(document);
            }
        }

        private void Add(IEnumerable<PageContainer> containers)
        {
            if (containers == null)
                return;

            foreach (var page in containers)
            {
                Add(page);
            }
        }

        private void Remove(IEnumerable<PageContainer> containers)
        {
            if (containers == null)
                return;

            foreach (var page in containers)
            {
                Remove(page);
            }
        }

        private void Add(IEnumerable<LayerContainer> layers)
        {
            if (layers == null)
                return;

            foreach (var layer in layers)
            {
                Add(layer);
            }
        }

        private void Remove(IEnumerable<LayerContainer> layers)
        {
            if (layers == null)
                return;

            foreach (var layer in layers)
            {
                Remove(layer);
            }
        }

        private void Add(IEnumerable<BaseShape> shapes)
        {
            if (shapes == null)
                return;

            foreach (var shape in shapes)
            {
                Add(shape);
            }
        }

        private void Remove(IEnumerable<BaseShape> shapes)
        {
            if (shapes == null)
                return;

            foreach (var shape in shapes)
            {
                Remove(shape);
            }
        }

        private void Add(IEnumerable<PathFigure> figures)
        {
            if (figures == null)
                return;

            foreach (var figure in figures)
            {
                Add(figure);
            }
        }

        private void Remove(IEnumerable<PathFigure> figures)
        {
            if (figures == null)
                return;

            foreach (var figure in figures)
            {
                Remove(figure);
            }
        }

        private void Add(IEnumerable<PathSegment> segments)
        {
            if (segments == null)
                return;

            foreach (var segment in segments)
            {
                Add(segment);
            }
        }

        private void Remove(IEnumerable<PathSegment> segments)
        {
            if (segments == null)
                return;

            foreach (var segment in segments)
            {
                Remove(segment);
            }
        }

        private void Add(IEnumerable<ShapeStyle> styles)
        {
            if (styles == null)
                return;

            foreach (var style in styles)
            {
                Add(style);
            }
        }

        private void Remove(IEnumerable<ShapeStyle> styles)
        {
            if (styles == null)
                return;

            foreach (var style in styles)
            {
                Remove(style);
            }
        }

        private void Add(IEnumerable<Library<ShapeStyle>> sgs)
        {
            if (sgs == null)
                return;

            foreach (var sg in sgs)
            {
                Add(sg);
            }
        }

        private void Remove(IEnumerable<Library<ShapeStyle>> sgs)
        {
            if (sgs == null)
                return;

            foreach (var sg in sgs)
            {
                Remove(sg);
            }
        }

        private void Add(IEnumerable<Library<GroupShape>> gl)
        {
            if (gl == null)
                return;

            foreach (var g in gl)
            {
                Add(g);
            }
        }

        private void Remove(IEnumerable<Library<GroupShape>> gl)
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
        /// Dispose unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        ~ProjectObserver()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        /// <param name="disposing">The flag indicating whether disposing.</param>
        private void Dispose(bool disposing)
        {
            if (disposing && _editor?.Project != null)
            {
                Remove(_editor.Project);
            }
        }
    }
}
