// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D
{
    /// <summary>
    /// Observe edited project changes.
    /// </summary>
    public class Observer : IDisposable
    {
        private readonly Editor _editor;
        private readonly Action _invalidateContainer;
        private readonly Action _invalidateStyles;
        private readonly Action _invalidateLayers;
        private readonly Action _invalidateShapes;

        /// <summary>
        /// Gets or sets flag indicating whether observer is paused.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Observer"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="Editor"/> object.</param>
        public Observer(Editor editor)
        {
            _editor = editor;

            _invalidateContainer = () =>
            {
                if (IsPaused)
                {
                    return;
                }
            };

            _invalidateStyles = () =>
            {
                if (IsPaused)
                {
                    return;
                }

                if (_editor.Project != null && _editor.Project.CurrentContainer != null)
                {
                    foreach (var renderer in _editor.Renderers)
                    {
                        renderer.ClearCache(isZooming: false);
                    }
                    _editor.Project.CurrentContainer.Invalidate();
                }
            };

            _invalidateLayers = () =>
            {
                if (IsPaused)
                {
                    return;
                }

                if (_editor.Project != null && _editor.Project.CurrentContainer != null)
                {
                    _editor.Project.CurrentContainer.Invalidate();
                }
            };

            _invalidateShapes = () =>
            {
                if (IsPaused)
                {
                    return;
                }

                if (_editor.Project != null && _editor.Project.CurrentContainer != null)
                {
                    _editor.Project.CurrentContainer.Invalidate();
                }
            };

            if (_editor.Project != null)
            {
                Add(_editor.Project);
            }
        }

        private void MarkAsDirty()
        {
            if (_editor != null)
            {
                _editor.IsProjectDirty = true;
            }
        }

        private void DatabaseObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        private void ColumnObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void RecordObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Record.Columns))
            {
                var record = sender as Record;
                Remove(record.Columns);
                Add(record.Columns);
            }

            if (e.PropertyName == nameof(Record.Values))
            {
                var record = sender as Record;
                Remove(record.Values);
                Add(record.Values);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ValueObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void ProjectObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Project.Databases))
            {
                var project = sender as Project;
                Remove(project.Databases);
                Add(project.Databases);
            }

            if (e.PropertyName == nameof(Project.StyleLibraries))
            {
                var project = sender as Project;
                Remove(project.StyleLibraries);
                Add(project.StyleLibraries);
            }

            if (e.PropertyName == nameof(Project.GroupLibraries))
            {
                var project = sender as Project;
                Remove(project.GroupLibraries);
                Add(project.GroupLibraries);
            }

            if (e.PropertyName == nameof(Project.Templates))
            {
                var project = sender as Project;
                Remove(project.Templates);
                Add(project.Templates);
            }

            if (e.PropertyName == nameof(Project.Documents))
            {
                var project = sender as Project;
                Remove(project.Documents);
                Add(project.Documents);
            }

            _invalidateShapes();

            // NOTE: Do not mark project as dirty when 'Current*' property changes.
            if (!e.PropertyName.StartsWith("Current"))
            {
                MarkAsDirty();
            }
        }

        private void DocumentObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Document.Containers))
            {
                var document = sender as Document;
                Remove(document.Containers);
                Add(document.Containers);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ContainerObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Data.Properties))
            {
                var container = sender as Container;
                Remove(container.Data.Properties);
                Add(container.Data.Properties);
            }

            if (e.PropertyName == nameof(Container.Layers))
            {
                var container = sender as Container;
                Remove(container.Layers);
                Add(container.Layers);
            }

            _invalidateContainer();

            // NOTE: Do not mark project as dirty when current shape changes.
            // NOTE: Do not mark project as dirty when current layer changes.
            if (e.PropertyName != nameof(Container.CurrentShape)
                && e.PropertyName != nameof(Container.CurrentLayer))
            {
                MarkAsDirty();
            }
        }

        private void ContainerBackgroudObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _editor.Project.CurrentContainer.Notify(nameof(Container.Background));
            if (_editor.Project.CurrentContainer.Template != null)
            {
                _editor.Project.CurrentContainer.Template.Notify(nameof(Container.Background));
            }
            MarkAsDirty();
        }

        private void InvalidateLayerObserver(object sender, InvalidateLayerEventArgs e)
        {
            if (_editor.Invalidate != null)
            {
                _editor.Invalidate();
            }
        }

        private void LayerObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Layer.Shapes))
            {
                var layer = sender as Layer;
                Remove(layer.Shapes);
                Add(layer.Shapes);
            }

            _invalidateLayers();
            MarkAsDirty();
        }

        private void ShapeObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void StyleLibraryObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        private void GroupLibraryObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Library<XGroup>.Items))
            {
                var sg = sender as Library<XGroup>;
                Remove(sg.Items);
                Add(sg.Items);
            }

            // NOTE: Do not mark project as dirty when current group changes.
            if (e.PropertyName != nameof(Library<XGroup>.Selected))
            {
                MarkAsDirty();
            }
        }

        private void StyleObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateStyles();
            MarkAsDirty();
        }

        private void PropertyObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void DataObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Data.Properties))
            {
                var data = sender as Data;
                Remove(data.Properties);
                Add(data.Properties);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void StateObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidateShapes();
            MarkAsDirty();
        }

        private void Add(Database database)
        {
            if (database == null)
                return;

            database.PropertyChanged += DatabaseObserver;

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

            database.PropertyChanged -= DatabaseObserver;

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

            column.PropertyChanged += ColumnObserver;
        }

        private void Remove(Column column)
        {
            if (column == null)
                return;

            column.PropertyChanged -= ColumnObserver;
        }

        private void Add(Record record)
        {
            if (record == null)
                return;

            record.PropertyChanged += RecordObserver;

            if (record.Values != null)
            {
                Add(record.Values);
            }
        }

        private void Remove(Record record)
        {
            if (record == null)
                return;

            record.PropertyChanged -= RecordObserver;

            if (record.Values != null)
            {
                Remove(record.Values);
            }
        }

        private void Add(Value value)
        {
            if (value == null)
                return;

            value.PropertyChanged += ValueObserver;
        }

        private void Remove(Value value)
        {
            if (value == null)
                return;

            value.PropertyChanged -= ValueObserver;
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

        private void Add(Project project)
        {
            if (project == null)
                return;

            project.PropertyChanged += ProjectObserver;

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

        private void Remove(Project project)
        {
            if (project == null)
                return;

            project.PropertyChanged -= ProjectObserver;

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

        private void Add(Document document)
        {
            if (document == null)
                return;

            document.PropertyChanged += DocumentObserver;

            if (document.Containers != null)
            {
                foreach (var container in document.Containers)
                {
                    Add(container);
                }
            }
        }

        private void Remove(Document document)
        {
            if (document == null)
                return;

            document.PropertyChanged -= DocumentObserver;

            if (document.Containers != null)
            {
                foreach (var container in document.Containers)
                {
                    Remove(container);
                }
            }
        }

        private void Add(Container container)
        {
            if (container == null)
                return;

            container.PropertyChanged += ContainerObserver;

            if (container.Background != null)
            {
                container.Background.PropertyChanged += ContainerBackgroudObserver;
            }

            if (container.Layers != null)
            {
                Add(container.Layers);
            }

            if (container.Data.Properties != null)
            {
                Add(container.Data.Properties);
            }

            container.WorkingLayer.InvalidateLayer += InvalidateLayerObserver;
            container.HelperLayer.InvalidateLayer += InvalidateLayerObserver;

            //Add(container.WorkingLayer);
            //Add(container.HelperLayer);
        }

        private void Remove(Container container)
        {
            if (container == null)
                return;

            container.PropertyChanged -= ContainerObserver;

            if (container.Background != null)
            {
                container.Background.PropertyChanged -= ContainerBackgroudObserver;
            }

            if (container.Layers != null)
            {
                Add(container.Layers);
            }

            if (container.Data.Properties != null)
            {
                Remove(container.Data.Properties);
            }

            container.WorkingLayer.InvalidateLayer -= InvalidateLayerObserver;
            container.HelperLayer.InvalidateLayer -= InvalidateLayerObserver;

            //Remove(container.WorkingLayer);
            //Remove(container.HelperLayer);
        }

        private void Add(Layer layer)
        {
            if (layer == null)
                return;

            layer.PropertyChanged += LayerObserver;

            if (layer.Shapes != null)
            {
                Add(layer.Shapes);
            }

            layer.InvalidateLayer += InvalidateLayerObserver;
        }

        private void Remove(Layer layer)
        {
            if (layer == null)
                return;

            layer.PropertyChanged -= LayerObserver;

            if (layer.Shapes != null)
            {
                Remove(layer.Shapes);
            }

            layer.InvalidateLayer -= InvalidateLayerObserver;
        }

        private void Add(BaseShape shape)
        {
            if (shape == null)
                return;

            shape.PropertyChanged += ShapeObserver;

            if (shape.Data != null)
            {
                if (shape.Data.Properties != null)
                {
                    Add(shape.Data.Properties);
                }

                shape.Data.PropertyChanged += DataObserver;
            }

            if (shape.State != null)
            {
                shape.State.PropertyChanged += StateObserver;
            }

            if (shape is XPoint)
            {
                var point = shape as XPoint;
                if (point.Shape != null)
                {
                    point.Shape.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XLine)
            {
                var line = shape as XLine;

                if (line.Start != null)
                {
                    line.Start.PropertyChanged += ShapeObserver;
                }

                if (line.End != null)
                {
                    line.End.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XRectangle)
            {
                var rectangle = shape as XRectangle;

                if (rectangle.TopLeft != null)
                {
                    rectangle.TopLeft.PropertyChanged += ShapeObserver;
                }

                if (rectangle.BottomRight != null)
                {
                    rectangle.BottomRight.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XEllipse)
            {
                var ellipse = shape as XEllipse;

                if (ellipse.TopLeft != null)
                {
                    ellipse.TopLeft.PropertyChanged += ShapeObserver;
                }

                if (ellipse.BottomRight != null)
                {
                    ellipse.BottomRight.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XArc)
            {
                var arc = shape as XArc;

                if (arc.Point1 != null)
                {
                    arc.Point1.PropertyChanged += ShapeObserver;
                }

                if (arc.Point2 != null)
                {
                    arc.Point2.PropertyChanged += ShapeObserver;
                }

                if (arc.Point3 != null)
                {
                    arc.Point3.PropertyChanged += ShapeObserver;
                }

                if (arc.Point4 != null)
                {
                    arc.Point4.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XBezier)
            {
                var bezier = shape as XBezier;

                if (bezier.Point1 != null)
                {
                    bezier.Point1.PropertyChanged += ShapeObserver;
                }

                if (bezier.Point2 != null)
                {
                    bezier.Point2.PropertyChanged += ShapeObserver;
                }

                if (bezier.Point3 != null)
                {
                    bezier.Point3.PropertyChanged += ShapeObserver;
                }

                if (bezier.Point4 != null)
                {
                    bezier.Point4.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XQBezier)
            {
                var qbezier = shape as XQBezier;

                if (qbezier.Point1 != null)
                {
                    qbezier.Point1.PropertyChanged += ShapeObserver;
                }

                if (qbezier.Point2 != null)
                {
                    qbezier.Point2.PropertyChanged += ShapeObserver;
                }

                if (qbezier.Point3 != null)
                {
                    qbezier.Point3.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XText)
            {
                var text = shape as XText;

                if (text.TopLeft != null)
                {
                    text.TopLeft.PropertyChanged += ShapeObserver;
                }

                if (text.BottomRight != null)
                {
                    text.BottomRight.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XImage)
            {
                var image = shape as XImage;

                if (image.TopLeft != null)
                {
                    image.TopLeft.PropertyChanged += ShapeObserver;
                }

                if (image.BottomRight != null)
                {
                    image.BottomRight.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XPath)
            {
                // TODO: Observer path sub properties.
            }
            else if (shape is XGroup)
            {
                var group = shape as XGroup;

                if (group != null)
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

            shape.PropertyChanged -= ShapeObserver;

            if (shape.Data != null)
            {
                if (shape.Data.Properties != null)
                {
                    Remove(shape.Data.Properties);
                }

                shape.Data.PropertyChanged -= DataObserver;
            }

            if (shape.State != null)
            {
                shape.State.PropertyChanged -= StateObserver;
            }

            if (shape is XPoint)
            {
                var point = shape as XPoint;
                if (point.Shape != null)
                {
                    point.Shape.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XLine)
            {
                var line = shape as XLine;

                if (line.Start != null)
                {
                    line.Start.PropertyChanged -= ShapeObserver;
                }

                if (line.End != null)
                {
                    line.End.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XRectangle)
            {
                var rectangle = shape as XRectangle;

                if (rectangle.TopLeft != null)
                {
                    rectangle.TopLeft.PropertyChanged -= ShapeObserver;
                }

                if (rectangle.BottomRight != null)
                {
                    rectangle.BottomRight.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XEllipse)
            {
                var ellipse = shape as XEllipse;

                if (ellipse.TopLeft != null)
                {
                    ellipse.TopLeft.PropertyChanged -= ShapeObserver;
                }

                if (ellipse.BottomRight != null)
                {
                    ellipse.BottomRight.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XArc)
            {
                var arc = shape as XArc;

                if (arc.Point1 != null)
                {
                    arc.Point1.PropertyChanged -= ShapeObserver;
                }

                if (arc.Point2 != null)
                {
                    arc.Point2.PropertyChanged -= ShapeObserver;
                }

                if (arc.Point3 != null)
                {
                    arc.Point3.PropertyChanged -= ShapeObserver;
                }

                if (arc.Point4 != null)
                {
                    arc.Point4.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XBezier)
            {
                var bezier = shape as XBezier;

                if (bezier.Point1 != null)
                {
                    bezier.Point1.PropertyChanged -= ShapeObserver;
                }

                if (bezier.Point2 != null)
                {
                    bezier.Point2.PropertyChanged -= ShapeObserver;
                }

                if (bezier.Point3 != null)
                {
                    bezier.Point3.PropertyChanged -= ShapeObserver;
                }

                if (bezier.Point4 != null)
                {
                    bezier.Point4.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XQBezier)
            {
                var qbezier = shape as XQBezier;

                if (qbezier.Point1 != null)
                {
                    qbezier.Point1.PropertyChanged -= ShapeObserver;
                }

                if (qbezier.Point2 != null)
                {
                    qbezier.Point2.PropertyChanged -= ShapeObserver;
                }

                if (qbezier.Point3 != null)
                {
                    qbezier.Point3.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XText)
            {
                var text = shape as XText;

                if (text.TopLeft != null)
                {
                    text.TopLeft.PropertyChanged -= ShapeObserver;
                }

                if (text.BottomRight != null)
                {
                    text.BottomRight.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XImage)
            {
                var image = shape as XImage;

                if (image.TopLeft != null)
                {
                    image.TopLeft.PropertyChanged -= ShapeObserver;
                }

                if (image.BottomRight != null)
                {
                    image.BottomRight.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XPath)
            {
                // TODO: Stop observing path sub properties.
            }
            else if (shape is XGroup)
            {
                var group = shape as XGroup;

                if (group != null)
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

        private void Add(Library<ShapeStyle> sg)
        {
            if (sg == null)
                return;

            if (sg.Items != null)
            {
                Add(sg.Items);
            }

            sg.PropertyChanged += StyleLibraryObserver;
        }

        private void Remove(Library<ShapeStyle> sg)
        {
            if (sg == null)
                return;

            if (sg.Items != null)
            {
                Remove(sg.Items);
            }

            sg.PropertyChanged -= StyleLibraryObserver;
        }

        private void Add(Library<XGroup> gl)
        {
            if (gl == null)
                return;

            if (gl.Items != null)
            {
                Add(gl.Items);
            }

            gl.PropertyChanged += GroupLibraryObserver;
        }

        private void Remove(Library<XGroup> gl)
        {
            if (gl == null)
                return;

            if (gl.Items != null)
            {
                Remove(gl.Items);
            }

            gl.PropertyChanged -= GroupLibraryObserver;
        }

        private void Add(ShapeStyle style)
        {
            if (style == null)
                return;

            style.PropertyChanged += StyleObserver;

            if (style.Stroke != null)
            {
                style.Stroke.PropertyChanged += StyleObserver;
            }

            if (style.Fill != null)
            {
                style.Fill.PropertyChanged += StyleObserver;
            }

            if (style.LineStyle != null)
            {
                style.LineStyle.PropertyChanged += StyleObserver;

                if (style.LineStyle.FixedLength != null)
                {
                    style.LineStyle.FixedLength.PropertyChanged += StyleObserver;
                }
            }

            if (style.StartArrowStyle != null)
            {
                style.StartArrowStyle.PropertyChanged += StyleObserver;

                if (style.StartArrowStyle.Stroke != null)
                {
                    style.StartArrowStyle.Stroke.PropertyChanged += StyleObserver;
                }

                if (style.StartArrowStyle.Fill != null)
                {
                    style.StartArrowStyle.Fill.PropertyChanged += StyleObserver;
                }
            }

            if (style.EndArrowStyle != null)
            {
                style.EndArrowStyle.PropertyChanged += StyleObserver;

                if (style.EndArrowStyle.Stroke != null)
                {
                    style.EndArrowStyle.Stroke.PropertyChanged += StyleObserver;
                }

                if (style.EndArrowStyle.Fill != null)
                {
                    style.EndArrowStyle.Fill.PropertyChanged += StyleObserver;
                }
            }

            if (style.TextStyle != null)
            {
                style.TextStyle.PropertyChanged += StyleObserver;

                if (style.TextStyle.FontStyle != null)
                {
                    style.TextStyle.FontStyle.PropertyChanged += StyleObserver;
                }
            }
        }

        private void Remove(ShapeStyle style)
        {
            if (style == null)
                return;

            style.PropertyChanged -= StyleObserver;

            if (style.Stroke != null)
            {
                style.Stroke.PropertyChanged -= StyleObserver;
            }

            if (style.Fill != null)
            {
                style.Fill.PropertyChanged -= StyleObserver;
            }

            if (style.LineStyle != null)
            {
                style.LineStyle.PropertyChanged -= StyleObserver;

                if (style.LineStyle.FixedLength != null)
                {
                    style.LineStyle.FixedLength.PropertyChanged -= StyleObserver;
                }
            }

            if (style.StartArrowStyle != null)
            {
                style.StartArrowStyle.PropertyChanged -= StyleObserver;

                if (style.StartArrowStyle.Stroke != null)
                {
                    style.StartArrowStyle.Stroke.PropertyChanged -= StyleObserver;
                }

                if (style.StartArrowStyle.Fill != null)
                {
                    style.StartArrowStyle.Fill.PropertyChanged -= StyleObserver;
                }
            }

            if (style.EndArrowStyle != null)
            {
                style.EndArrowStyle.PropertyChanged -= StyleObserver;

                if (style.EndArrowStyle.Stroke != null)
                {
                    style.EndArrowStyle.Stroke.PropertyChanged -= StyleObserver;
                }

                if (style.EndArrowStyle.Fill != null)
                {
                    style.EndArrowStyle.Fill.PropertyChanged -= StyleObserver;
                }
            }

            if (style.TextStyle != null)
            {
                style.TextStyle.PropertyChanged -= StyleObserver;

                if (style.TextStyle.FontStyle != null)
                {
                    style.TextStyle.FontStyle.PropertyChanged -= StyleObserver;
                }
            }
        }

        private void Add(Property property)
        {
            if (property == null)
                return;

            property.PropertyChanged += PropertyObserver;
        }

        private void Remove(Property property)
        {
            if (property == null)
                return;

            property.PropertyChanged += PropertyObserver;
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

        private void Add(IEnumerable<Document> documents)
        {
            if (documents == null)
                return;

            foreach (var document in documents)
            {
                Add(document);
            }
        }

        private void Remove(IEnumerable<Document> documents)
        {
            if (documents == null)
                return;

            foreach (var document in documents)
            {
                Remove(document);
            }
        }

        private void Add(IEnumerable<Container> containers)
        {
            if (containers == null)
                return;

            foreach (var container in containers)
            {
                Add(container);
            }
        }

        private void Remove(IEnumerable<Container> containers)
        {
            if (containers == null)
                return;

            foreach (var container in containers)
            {
                Remove(container);
            }
        }

        private void Add(IEnumerable<Layer> layers)
        {
            if (layers == null)
                return;

            foreach (var layer in layers)
            {
                Add(layer);
            }
        }

        private void Remove(IEnumerable<Layer> layers)
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

        private void Add(IEnumerable<Library<XGroup>> gl)
        {
            if (gl == null)
                return;

            foreach (var g in gl)
            {
                Add(g);
            }
        }

        private void Remove(IEnumerable<Library<XGroup>> gl)
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
        /// Performs freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        ~Observer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">The flag indicating whether disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_editor != null && _editor.Project != null)
                {
                    Remove(_editor.Project);
                }
            }
        }
    }
}
