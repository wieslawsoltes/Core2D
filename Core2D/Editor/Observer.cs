// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Project;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using System;
using System.Collections.Generic;

namespace Core2D.Editor
{
    /// <summary>
    /// Project property changes observer.
    /// </summary>
    public sealed class Observer : IDisposable
    {
        private readonly ShapeEditor _editor;
        private readonly Action _invalidateContainer;
        private readonly Action _invalidateStyles;
        private readonly Action _invalidateLayers;
        private readonly Action _invalidateShapes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Observer"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ShapeEditor"/> object.</param>
        public Observer(ShapeEditor editor)
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

        private void DatabaseObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(XDatabase.Columns))
            {
                var database = sender as XDatabase;
                Remove(database.Columns);
                Add(database.Columns);
            }

            if (e.PropertyName == nameof(XDatabase.Records))
            {
                var database = sender as XDatabase;
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
            if (e.PropertyName == nameof(XRecord.Columns))
            {
                var record = sender as XRecord;
                Remove(record.Columns);
                Add(record.Columns);
            }

            if (e.PropertyName == nameof(XRecord.Values))
            {
                var record = sender as XRecord;
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
            if (e.PropertyName == nameof(XProject.Databases))
            {
                var project = sender as XProject;
                Remove(project.Databases);
                Add(project.Databases);
            }

            if (e.PropertyName == nameof(XProject.StyleLibraries))
            {
                var project = sender as XProject;
                Remove(project.StyleLibraries);
                Add(project.StyleLibraries);
            }

            if (e.PropertyName == nameof(XProject.GroupLibraries))
            {
                var project = sender as XProject;
                Remove(project.GroupLibraries);
                Add(project.GroupLibraries);
            }

            if (e.PropertyName == nameof(XProject.Templates))
            {
                var project = sender as XProject;
                Remove(project.Templates);
                Add(project.Templates);
            }

            if (e.PropertyName == nameof(XProject.Documents))
            {
                var project = sender as XProject;
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
            if (e.PropertyName == nameof(XDocument.Pages))
            {
                var document = sender as XDocument;
                Remove(document.Pages);
                Add(document.Pages);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void PageObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(XContext.Properties))
            {
                var container = sender as XPage;
                Remove(container.Data.Properties);
                Add(container.Data.Properties);
            }

            if (e.PropertyName == nameof(XContainer.Layers))
            {
                var container = sender as XContainer;
                Remove(container.Layers);
                Add(container.Layers);
            }

            _invalidateContainer();

            // NOTE: Do not mark project as dirty when current shape changes.
            // NOTE: Do not mark project as dirty when current layer changes.
            if (e.PropertyName != nameof(XContainer.CurrentShape)
                && e.PropertyName != nameof(XContainer.CurrentLayer))
            {
                MarkAsDirty();
            }
        }

        private void TemplateBackgroudObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _editor.Project.CurrentContainer.Notify(nameof(XTemplate.Background));
            var page = _editor.Project.CurrentContainer as XPage;
            if (page != null)
            {
                page.Template.Notify(nameof(XTemplate.Background));
            }
            _invalidateLayers();
            MarkAsDirty();
        }

        private void InvalidateLayerObserver(object sender, InvalidateLayerEventArgs e)
        {
            _editor?.Invalidate?.Invoke();
        }

        private void LayerObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(XLayer.Shapes))
            {
                var layer = sender as XLayer;
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
            if (e.PropertyName == nameof(XLibrary<ShapeStyle>.Items))
            {
                var sg = sender as XLibrary<ShapeStyle>;
                Remove(sg.Items);
                Add(sg.Items);
            }

            _invalidateStyles();

            // NOTE: Do not mark project as dirty when current style changes.
            if (e.PropertyName != nameof(XLibrary<ShapeStyle>.Selected))
            {
                MarkAsDirty();
            }
        }

        private void GroupLibraryObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(XLibrary<XGroup>.Items))
            {
                var sg = sender as XLibrary<XGroup>;
                Remove(sg.Items);
                Add(sg.Items);
            }

            // NOTE: Do not mark project as dirty when current group changes.
            if (e.PropertyName != nameof(XLibrary<XGroup>.Selected))
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
            if (e.PropertyName == nameof(XContext.Properties))
            {
                var data = sender as XContext;
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

        private void Add(XDatabase database)
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

        private void Remove(XDatabase database)
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

        private void Add(XColumn column)
        {
            if (column == null)
                return;

            column.PropertyChanged += ColumnObserver;
        }

        private void Remove(XColumn column)
        {
            if (column == null)
                return;

            column.PropertyChanged -= ColumnObserver;
        }

        private void Add(XRecord record)
        {
            if (record == null)
                return;

            record.PropertyChanged += RecordObserver;

            if (record.Values != null)
            {
                Add(record.Values);
            }
        }

        private void Remove(XRecord record)
        {
            if (record == null)
                return;

            record.PropertyChanged -= RecordObserver;

            if (record.Values != null)
            {
                Remove(record.Values);
            }
        }

        private void Add(XValue value)
        {
            if (value == null)
                return;

            value.PropertyChanged += ValueObserver;
        }

        private void Remove(XValue value)
        {
            if (value == null)
                return;

            value.PropertyChanged -= ValueObserver;
        }

        private void Add(XOptions options)
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

        private void Remove(XOptions options)
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

        private void Add(XProject project)
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

        private void Remove(XProject project)
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

        private void Add(XDocument document)
        {
            if (document == null)
                return;

            document.PropertyChanged += DocumentObserver;

            if (document.Pages != null)
            {
                foreach (var container in document.Pages)
                {
                    Add(container);
                }
            }
        }

        private void Remove(XDocument document)
        {
            if (document == null)
                return;

            document.PropertyChanged -= DocumentObserver;

            if (document.Pages != null)
            {
                foreach (var container in document.Pages)
                {
                    Remove(container);
                }
            }
        }

        private void Add(XPage page)
        {
            if (page == null)
                return;

            page.PropertyChanged += PageObserver;

            if (page.Layers != null)
            {
                Add(page.Layers);
            }

            if (page.Data != null)
            {
                Add(page.Data);
            }

            if (page.WorkingLayer != null)
            {
                page.WorkingLayer.InvalidateLayer += InvalidateLayerObserver;
            }

            if (page.HelperLayer != null)
            {
                page.HelperLayer.InvalidateLayer += InvalidateLayerObserver;
            }
        }

        private void Remove(XPage page)
        {
            if (page == null)
                return;

            page.PropertyChanged -= PageObserver;

            if (page.Layers != null)
            {
                Add(page.Layers);
            }

            if (page.Data != null)
            {
                Remove(page.Data);
            }

            if (page.WorkingLayer != null)
            {
                page.WorkingLayer.InvalidateLayer -= InvalidateLayerObserver;
            }

            if (page.HelperLayer != null)
            {
                page.HelperLayer.InvalidateLayer -= InvalidateLayerObserver;
            }
        }

        private void Add(XTemplate template)
        {
            if (template == null)
                return;

            template.PropertyChanged += PageObserver;

            if (template.Background != null)
            {
                template.Background.PropertyChanged += TemplateBackgroudObserver;
            }

            if (template.Layers != null)
            {
                Add(template.Layers);
            }

            if (template.WorkingLayer != null)
            {
                template.WorkingLayer.InvalidateLayer += InvalidateLayerObserver;
            }

            if (template.HelperLayer != null)
            {
                template.HelperLayer.InvalidateLayer += InvalidateLayerObserver;
            }
        }

        private void Remove(XTemplate template)
        {
            if (template == null)
                return;

            template.PropertyChanged -= PageObserver;

            if (template.Background != null)
            {
                template.Background.PropertyChanged -= TemplateBackgroudObserver;
            }

            if (template.Layers != null)
            {
                Add(template.Layers);
            }

            if (template.WorkingLayer != null)
            {
                template.WorkingLayer.InvalidateLayer -= InvalidateLayerObserver;
            }

            if (template.HelperLayer != null)
            {
                template.HelperLayer.InvalidateLayer -= InvalidateLayerObserver;
            }
        }

        private void Add(XLayer layer)
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

        private void Remove(XLayer layer)
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
                Add(shape.Data);
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
            else if (shape is XCubicBezier)
            {
                var cubicBezier = shape as XCubicBezier;

                if (cubicBezier.Point1 != null)
                {
                    cubicBezier.Point1.PropertyChanged += ShapeObserver;
                }

                if (cubicBezier.Point2 != null)
                {
                    cubicBezier.Point2.PropertyChanged += ShapeObserver;
                }

                if (cubicBezier.Point3 != null)
                {
                    cubicBezier.Point3.PropertyChanged += ShapeObserver;
                }

                if (cubicBezier.Point4 != null)
                {
                    cubicBezier.Point4.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XQuadraticBezier)
            {
                var quadraticBezier = shape as XQuadraticBezier;

                if (quadraticBezier.Point1 != null)
                {
                    quadraticBezier.Point1.PropertyChanged += ShapeObserver;
                }

                if (quadraticBezier.Point2 != null)
                {
                    quadraticBezier.Point2.PropertyChanged += ShapeObserver;
                }

                if (quadraticBezier.Point3 != null)
                {
                    quadraticBezier.Point3.PropertyChanged += ShapeObserver;
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
                Remove(shape.Data);
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
            else if (shape is XCubicBezier)
            {
                var cubicBezier = shape as XCubicBezier;

                if (cubicBezier.Point1 != null)
                {
                    cubicBezier.Point1.PropertyChanged -= ShapeObserver;
                }

                if (cubicBezier.Point2 != null)
                {
                    cubicBezier.Point2.PropertyChanged -= ShapeObserver;
                }

                if (cubicBezier.Point3 != null)
                {
                    cubicBezier.Point3.PropertyChanged -= ShapeObserver;
                }

                if (cubicBezier.Point4 != null)
                {
                    cubicBezier.Point4.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XQuadraticBezier)
            {
                var quadraticBezier = shape as XQuadraticBezier;

                if (quadraticBezier.Point1 != null)
                {
                    quadraticBezier.Point1.PropertyChanged -= ShapeObserver;
                }

                if (quadraticBezier.Point2 != null)
                {
                    quadraticBezier.Point2.PropertyChanged -= ShapeObserver;
                }

                if (quadraticBezier.Point3 != null)
                {
                    quadraticBezier.Point3.PropertyChanged -= ShapeObserver;
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

        private void Add(XContext data)
        {
            if (data == null)
                return;

            if (data.Properties != null)
            {
                Add(data.Properties);
            }

            data.PropertyChanged += DataObserver;
        }

        private void Remove(XContext data)
        {
            if (data == null)
                return;

            if (data.Properties != null)
            {
                Remove(data.Properties);
            }

            data.PropertyChanged -= DataObserver;
        }

        private void Add(XLibrary<ShapeStyle> sg)
        {
            if (sg == null)
                return;

            if (sg.Items != null)
            {
                Add(sg.Items);
            }

            sg.PropertyChanged += StyleLibraryObserver;
        }

        private void Remove(XLibrary<ShapeStyle> sg)
        {
            if (sg == null)
                return;

            if (sg.Items != null)
            {
                Remove(sg.Items);
            }

            sg.PropertyChanged -= StyleLibraryObserver;
        }

        private void Add(XLibrary<XGroup> gl)
        {
            if (gl == null)
                return;

            if (gl.Items != null)
            {
                Add(gl.Items);
            }

            gl.PropertyChanged += GroupLibraryObserver;
        }

        private void Remove(XLibrary<XGroup> gl)
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

        private void Add(XProperty property)
        {
            if (property == null)
                return;

            property.PropertyChanged += PropertyObserver;
        }

        private void Remove(XProperty property)
        {
            if (property == null)
                return;

            property.PropertyChanged += PropertyObserver;
        }

        private void Add(IEnumerable<XDatabase> databases)
        {
            if (databases == null)
                return;

            foreach (var database in databases)
            {
                Add(database);
            }
        }

        private void Remove(IEnumerable<XDatabase> databases)
        {
            if (databases == null)
                return;

            foreach (var database in databases)
            {
                Remove(database);
            }
        }

        private void Add(IEnumerable<XColumn> columns)
        {
            if (columns == null)
                return;

            foreach (var column in columns)
            {
                Add(column);
            }
        }

        private void Remove(IEnumerable<XColumn> columns)
        {
            if (columns == null)
                return;

            foreach (var column in columns)
            {
                Remove(column);
            }
        }

        private void Add(IEnumerable<XRecord> records)
        {
            if (records == null)
                return;

            foreach (var record in records)
            {
                Add(record);
            }
        }

        private void Remove(IEnumerable<XRecord> records)
        {
            if (records == null)
                return;

            foreach (var record in records)
            {
                Remove(record);
            }
        }

        private void Add(IEnumerable<XValue> values)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                Add(value);
            }
        }

        private void Remove(IEnumerable<XValue> values)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                Remove(value);
            }
        }

        private void Add(IEnumerable<XDocument> documents)
        {
            if (documents == null)
                return;

            foreach (var document in documents)
            {
                Add(document);
            }
        }

        private void Remove(IEnumerable<XDocument> documents)
        {
            if (documents == null)
                return;

            foreach (var document in documents)
            {
                Remove(document);
            }
        }

        private void Add(IEnumerable<XPage> pages)
        {
            if (pages == null)
                return;

            foreach (var page in pages)
            {
                Add(page);
            }
        }

        private void Remove(IEnumerable<XPage> pages)
        {
            if (pages == null)
                return;

            foreach (var page in pages)
            {
                Remove(page);
            }
        }

        private void Add(IEnumerable<XTemplate> templates)
        {
            if (templates == null)
                return;

            foreach (var template in templates)
            {
                Add(template);
            }
        }

        private void Remove(IEnumerable<XTemplate> templates)
        {
            if (templates == null)
                return;

            foreach (var template in templates)
            {
                Remove(template);
            }
        }

        private void Add(IEnumerable<XLayer> layers)
        {
            if (layers == null)
                return;

            foreach (var layer in layers)
            {
                Add(layer);
            }
        }

        private void Remove(IEnumerable<XLayer> layers)
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

        private void Add(IEnumerable<XLibrary<ShapeStyle>> sgs)
        {
            if (sgs == null)
                return;

            foreach (var sg in sgs)
            {
                Add(sg);
            }
        }

        private void Remove(IEnumerable<XLibrary<ShapeStyle>> sgs)
        {
            if (sgs == null)
                return;

            foreach (var sg in sgs)
            {
                Remove(sg);
            }
        }

        private void Add(IEnumerable<XLibrary<XGroup>> gl)
        {
            if (gl == null)
                return;

            foreach (var g in gl)
            {
                Add(g);
            }
        }

        private void Remove(IEnumerable<XLibrary<XGroup>> gl)
        {
            if (gl == null)
                return;

            foreach (var g in gl)
            {
                Remove(g);
            }
        }

        private void Add(IEnumerable<XProperty> properties)
        {
            if (properties == null)
                return;

            foreach (var property in properties)
            {
                Add(property);
            }
        }

        private void Remove(IEnumerable<XProperty> properties)
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
        ~Observer()
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
