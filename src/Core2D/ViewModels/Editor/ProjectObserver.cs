#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Scripting;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor
{
    public partial class ProjectObserver : IDisposable
    {
        private readonly ProjectEditorViewModel _editor;
        private readonly Action _invalidateContainer;
        private readonly Action _invalidateStyles;
        private readonly Action _invalidateLayers;
        private readonly Action _invalidateShapes;

        public ProjectObserver(ProjectEditorViewModel editor)
        {
            if (editor?.Project is { })
            {
                _editor = editor;

                _invalidateContainer = () => { };
                _invalidateStyles = () => Invalidate();
                _invalidateLayers = () => Invalidate();
                _invalidateShapes = () => Invalidate();

                Add(_editor.Project);
            }
        }

        private void Invalidate()
        {
            _editor?.Project?.CurrentContainer?.InvalidateLayer();
        }

        private void MarkAsDirty()
        {
            if (_editor is { })
            {
                _editor.IsProjectDirty = true;
            }
        }

        private void ObserveDatabase(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DatabaseViewModel.Columns))
            {
                var database = sender as DatabaseViewModel;
                Remove(database.Columns);
                Add(database.Columns);
            }

            if (e.PropertyName == nameof(DatabaseViewModel.Records))
            {
                var database = sender as DatabaseViewModel;
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
            if (e.PropertyName == nameof(RecordViewModel.Values))
            {
                var record = sender as RecordViewModel;
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
            if (e.PropertyName == nameof(ProjectContainerViewModel.Databases))
            {
                var project = sender as ProjectContainerViewModel;
                Remove(project.Databases);
                Add(project.Databases);
            }

            if (e.PropertyName == nameof(ProjectContainerViewModel.StyleLibraries))
            {
                var project = sender as ProjectContainerViewModel;
                Remove(project.StyleLibraries);
                Add(project.StyleLibraries);
            }

            if (e.PropertyName == nameof(ProjectContainerViewModel.GroupLibraries))
            {
                var project = sender as ProjectContainerViewModel;
                Remove(project.GroupLibraries);
                Add(project.GroupLibraries);
            }

            if (e.PropertyName == nameof(ProjectContainerViewModel.Templates))
            {
                var project = sender as ProjectContainerViewModel;
                Remove(project.Templates);
                Add(project.Templates);
            }

            if (e.PropertyName == nameof(ProjectContainerViewModel.Scripts))
            {
                var project = sender as ProjectContainerViewModel;
                Remove(project.Scripts);
                Add(project.Scripts);
            }

            if (e.PropertyName == nameof(ProjectContainerViewModel.Documents))
            {
                var project = sender as ProjectContainerViewModel;
                Remove(project.Documents);
                Add(project.Documents);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObserveDocument(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DocumentContainerViewModel.Pages))
            {
                var document = sender as DocumentContainerViewModel;
                Remove(document.Pages);
                Add(document.Pages);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        private void ObservePage(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IDataObject.Properties))
            {
                var container = sender as FrameContainerViewModel;
                Remove(container.Properties);
                Add(container.Properties);
            }

            if (e.PropertyName == nameof(FrameContainerViewModel.Layers))
            {
                var container = sender as FrameContainerViewModel;
                Remove(container.Layers);
                Add(container.Layers);
            }

            _invalidateContainer();
            MarkAsDirty();
        }

        private void ObserveTemplateBackground(object sender, PropertyChangedEventArgs e)
        {
            _editor.Project.CurrentContainer.RaisePropertyChanged(nameof(TemplateContainerViewModel.Background));
            var container = _editor.Project.CurrentContainer;
            if (container is PageContainerViewModel page)
            {
                page.Template.RaisePropertyChanged(nameof(TemplateContainerViewModel.Background));
            }
            if (container is TemplateContainerViewModel template)
            {
                template.RaisePropertyChanged(nameof(TemplateContainerViewModel.Background));
            }
            _invalidateLayers();
            MarkAsDirty();
        }

        private void ObserveGridStrokeColor(object sender, PropertyChangedEventArgs e)
        {
            _editor.Project.CurrentContainer.RaisePropertyChanged(nameof(IGrid.GridStrokeColor));
            var container = _editor.Project.CurrentContainer;
            if (container is PageContainerViewModel page)
            {
                page.Template.RaisePropertyChanged(nameof(IGrid.GridStrokeColor));
            }
            if (container is TemplateContainerViewModel template)
            {
                template.RaisePropertyChanged(nameof(IGrid.GridStrokeColor));
            }
            _invalidateLayers();
            MarkAsDirty();
        }

        private void ObserveGrid(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IGrid.IsGridEnabled)
                || e.PropertyName == nameof(IGrid.IsBorderEnabled)
                || e.PropertyName == nameof(IGrid.GridOffsetLeft)
                || e.PropertyName == nameof(IGrid.GridOffsetTop)
                || e.PropertyName == nameof(IGrid.GridOffsetRight)
                || e.PropertyName == nameof(IGrid.GridOffsetBottom)
                || e.PropertyName == nameof(IGrid.GridCellWidth)
                || e.PropertyName == nameof(IGrid.GridCellHeight)
                || e.PropertyName == nameof(IGrid.GridStrokeThickness))
            {
                _invalidateLayers();
                MarkAsDirty();
            }
        }

        private void ObserveScript(object sender, PropertyChangedEventArgs e)
        {
            MarkAsDirty();
        }

        private void ObserveInvalidateLayer(object sender, InvalidateLayerEventArgs e)
        {
            _editor?.CanvasPlatform?.InvalidateControl?.Invoke();
        }

        private void ObserveLayer(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LayerContainerViewModel.Shapes))
            {
                var layer = sender as LayerContainerViewModel;
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
            if (e.PropertyName == nameof(LibraryViewModel<ShapeStyleViewModel>.Items))
            {
                var sg = sender as LibraryViewModel<ShapeStyleViewModel>;
                Remove(sg.Items);
                Add(sg.Items);
            }

            _invalidateStyles();

            // NOTE: Do not mark project as dirty when current style changes.
            if (e.PropertyName != nameof(LibraryViewModel<ShapeStyleViewModel>.Selected))
            {
                MarkAsDirty();
            }
        }

        private void ObserveGroupLibrary(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LibraryViewModel<GroupShapeViewModel>.Items))
            {
                var sg = sender as LibraryViewModel<GroupShapeViewModel>;
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
            if (e.PropertyName == nameof(IDataObject.Properties))
            {
                var data = sender as IDataObject;
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

        private void Add(DatabaseViewModel database)
        {
            if (database is null)
            {
                return;
            }

            database.PropertyChanged += ObserveDatabase;

            if (database.Columns is { })
            {
                Add(database.Columns);
            }

            if (database.Records is { })
            {
                Add(database.Records);
            }
        }

        private void Remove(DatabaseViewModel database)
        {
            if (database is null)
            {
                return;
            }

            database.PropertyChanged -= ObserveDatabase;

            if (database.Columns is { })
            {
                Remove(database.Columns);
            }

            if (database.Records is { })
            {
                Remove(database.Records);
            }
        }

        private void Add(ColumnViewModel column)
        {
            if (column is null)
            {
                return;
            }

            column.PropertyChanged += ObserveColumn;
        }

        private void Remove(ColumnViewModel column)
        {
            if (column is null)
            {
                return;
            }

            column.PropertyChanged -= ObserveColumn;
        }

        private void Add(RecordViewModel record)
        {
            if (record is null)
            {
                return;
            }

            record.PropertyChanged += ObserveRecord;

            if (record.Values is { })
            {
                Add(record.Values);
            }
        }

        private void Remove(RecordViewModel record)
        {
            if (record is null)
            {
                return;
            }

            record.PropertyChanged -= ObserveRecord;

            if (record.Values is { })
            {
                Remove(record.Values);
            }
        }

        private void Add(ValueViewModel value)
        {
            if (value is null)
            {
                return;
            }

            value.PropertyChanged += ObserveValue;
        }

        private void Remove(ValueViewModel value)
        {
            if (value is null)
            {
                return;
            }

            value.PropertyChanged -= ObserveValue;
        }

        private void Add(OptionsViewModel options)
        {
            if (options is null)
            {
                return;
            }
        }

        private void Remove(OptionsViewModel options)
        {
            if (options is null)
            {
                return;
            }
        }

        private void Add(ProjectContainerViewModel project)
        {
            if (project is null)
            {
                return;
            }

            project.PropertyChanged += ObserveProject;

            Add(project.Options);

            if (project.Databases is { })
            {
                foreach (var database in project.Databases)
                {
                    Add(database);
                }
            }

            if (project.Documents is { })
            {
                foreach (var document in project.Documents)
                {
                    Add(document);
                }
            }

            if (project.Templates is { })
            {
                foreach (var template in project.Templates)
                {
                    Add(template);
                }
            }

            if (project.Scripts is { })
            {
                foreach (var script in project.Scripts)
                {
                    Add(script);
                }
            }

            if (project.StyleLibraries is { })
            {
                foreach (var sg in project.StyleLibraries)
                {
                    Add(sg);
                }
            }
        }

        private void Remove(ProjectContainerViewModel project)
        {
            if (project is null)
            {
                return;
            }

            project.PropertyChanged -= ObserveProject;

            Remove(project.Options);

            if (project.Databases is { })
            {
                foreach (var database in project.Databases)
                {
                    Remove(database);
                }
            }

            if (project.Documents is { })
            {
                foreach (var document in project.Documents)
                {
                    Remove(document);
                }
            }

            if (project.Templates is { })
            {
                foreach (var template in project.Templates)
                {
                    Remove(template);
                }
            }

            if (project.Scripts is { })
            {
                foreach (var script in project.Scripts)
                {
                    Remove(script);
                }
            }

            if (project.StyleLibraries is { })
            {
                foreach (var sg in project.StyleLibraries)
                {
                    Remove(sg);
                }
            }
        }

        private void Add(DocumentContainerViewModel document)
        {
            if (document is null)
            {
                return;
            }

            document.PropertyChanged += ObserveDocument;

            if (document.Pages is { })
            {
                foreach (var container in document.Pages)
                {
                    Add(container);
                }
            }
        }

        private void Remove(DocumentContainerViewModel document)
        {
            if (document is null)
            {
                return;
            }

            document.PropertyChanged -= ObserveDocument;

            if (document.Pages is { })
            {
                foreach (var container in document.Pages)
                {
                    Remove(container);
                }
            }
        }

        private void Add(PageContainerViewModel page)
        {
            if (page is null)
            {
                return;
            }

            page.PropertyChanged += ObservePage;

            if (page.Layers is { })
            {
                Add(page.Layers);
            }

            if (page is IDataObject data)
            {
                Add(data);
            }

            if (page.WorkingLayer is { })
            {
                page.WorkingLayer.InvalidateLayerHandler += ObserveInvalidateLayer;
            }

            if (page.HelperLayer is { })
            {
                page.HelperLayer.InvalidateLayerHandler += ObserveInvalidateLayer;
            }
        }

        private void Remove(PageContainerViewModel page)
        {
            if (page is null)
            {
                return;
            }

            page.PropertyChanged -= ObservePage;

            if (page.Layers is { })
            {
                Remove(page.Layers);
            }

            if (page is IDataObject data)
            {
                Remove(data);
            }

            if (page.WorkingLayer is { })
            {
                page.WorkingLayer.InvalidateLayerHandler -= ObserveInvalidateLayer;
            }

            if (page.HelperLayer is { })
            {
                page.HelperLayer.InvalidateLayerHandler -= ObserveInvalidateLayer;
            }
        }

        private void Add(TemplateContainerViewModel template)
        {
            if (template is null)
            {
                return;
            }

            template.PropertyChanged += ObservePage;

            if (template.Background is { })
            {
                template.Background.PropertyChanged += ObserveTemplateBackground;
            }

            if (template.GridStrokeColor is { })
            {
                template.GridStrokeColor.PropertyChanged += ObserveGridStrokeColor;
            }

            template.PropertyChanged += ObserveGrid;

            if (template.Layers is { })
            {
                Add(template.Layers);
            }

            if (template is IDataObject data)
            {
                Add(data);
            }

            if (template.WorkingLayer is { })
            {
                template.WorkingLayer.InvalidateLayerHandler += ObserveInvalidateLayer;
            }

            if (template.HelperLayer is { })
            {
                template.HelperLayer.InvalidateLayerHandler += ObserveInvalidateLayer;
            }
        }

        private void Remove(TemplateContainerViewModel template)
        {
            if (template is null)
            {
                return;
            }

            template.PropertyChanged -= ObservePage;

            if (template.Background is { })
            {
                template.Background.PropertyChanged -= ObserveTemplateBackground;
            }

            if (template.GridStrokeColor is { })
            {
                template.GridStrokeColor.PropertyChanged -= ObserveGridStrokeColor;
            }

            template.PropertyChanged -= ObserveGrid;

            if (template.Layers is { })
            {
                Remove(template.Layers);
            }

            if (template is IDataObject data)
            {
                Remove(data);
            }

            if (template.WorkingLayer is { })
            {
                template.WorkingLayer.InvalidateLayerHandler -= ObserveInvalidateLayer;
            }

            if (template.HelperLayer is { })
            {
                template.HelperLayer.InvalidateLayerHandler -= ObserveInvalidateLayer;
            }
        }

        private void Add(LayerContainerViewModel layer)
        {
            if (layer is null)
            {
                return;
            }

            layer.PropertyChanged += ObserveLayer;

            if (layer.Shapes is { })
            {
                Add(layer.Shapes);
            }

            layer.InvalidateLayerHandler += ObserveInvalidateLayer;
        }

        private void Remove(LayerContainerViewModel layer)
        {
            if (layer is null)
            {
                return;
            }

            layer.PropertyChanged -= ObserveLayer;

            if (layer.Shapes is { })
            {
                Remove(layer.Shapes);
            }

            layer.InvalidateLayerHandler -= ObserveInvalidateLayer;
        }

        private void Add(BaseShapeViewModel shape)
        {
            if (shape is null)
            {
                return;
            }

            shape.PropertyChanged += ObserveShape;

            if (shape.Style is { })
            {
                Add(shape.Style);
            }

            if (shape is IDataObject data)
            {
                Add(data);
            }

            if (shape is LineShapeViewModel line)
            {
                if (line.Start is { })
                {
                    line.Start.PropertyChanged += ObserveShape;
                }

                if (line.End is { })
                {
                    line.End.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is RectangleShapeViewModel rectangle)
            {
                if (rectangle.TopLeft is { })
                {
                    rectangle.TopLeft.PropertyChanged += ObserveShape;
                }

                if (rectangle.BottomRight is { })
                {
                    rectangle.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is EllipseShapeViewModel ellipse)
            {
                if (ellipse.TopLeft is { })
                {
                    ellipse.TopLeft.PropertyChanged += ObserveShape;
                }

                if (ellipse.BottomRight is { })
                {
                    ellipse.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is ArcShapeViewModel arc)
            {
                if (arc.Point1 is { })
                {
                    arc.Point1.PropertyChanged += ObserveShape;
                }

                if (arc.Point2 is { })
                {
                    arc.Point2.PropertyChanged += ObserveShape;
                }

                if (arc.Point3 is { })
                {
                    arc.Point3.PropertyChanged += ObserveShape;
                }

                if (arc.Point4 is { })
                {
                    arc.Point4.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is CubicBezierShapeViewModel cubicBezier)
            {
                if (cubicBezier.Point1 is { })
                {
                    cubicBezier.Point1.PropertyChanged += ObserveShape;
                }

                if (cubicBezier.Point2 is { })
                {
                    cubicBezier.Point2.PropertyChanged += ObserveShape;
                }

                if (cubicBezier.Point3 is { })
                {
                    cubicBezier.Point3.PropertyChanged += ObserveShape;
                }

                if (cubicBezier.Point4 is { })
                {
                    cubicBezier.Point4.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is QuadraticBezierShapeViewModel quadraticBezier)
            {
                if (quadraticBezier.Point1 is { })
                {
                    quadraticBezier.Point1.PropertyChanged += ObserveShape;
                }

                if (quadraticBezier.Point2 is { })
                {
                    quadraticBezier.Point2.PropertyChanged += ObserveShape;
                }

                if (quadraticBezier.Point3 is { })
                {
                    quadraticBezier.Point3.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is TextShapeViewModel text)
            {
                if (text.TopLeft is { })
                {
                    text.TopLeft.PropertyChanged += ObserveShape;
                }

                if (text.BottomRight is { })
                {
                    text.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is ImageShapeViewModel image)
            {
                if (image.TopLeft is { })
                {
                    image.TopLeft.PropertyChanged += ObserveShape;
                }

                if (image.BottomRight is { })
                {
                    image.BottomRight.PropertyChanged += ObserveShape;
                }
            }
            else if (shape is PathShapeViewModel path)
            {
                if (path.Geometry is { })
                {
                    Add(path.Geometry);
                }
            }
            else if (shape is GroupShapeViewModel group)
            {
                if (group.Shapes is { })
                {
                    Add(group.Shapes);
                }

                if (group.Connectors is { })
                {
                    Add(group.Connectors);
                }
            }
        }

        private void Remove(BaseShapeViewModel shape)
        {
            if (shape is null)
            {
                return;
            }

            shape.PropertyChanged -= ObserveShape;

            if (shape.Style is { })
            {
                Remove(shape.Style);
            }

            if (shape is IDataObject data)
            {
                Remove(data);
            }

            if (shape is LineShapeViewModel line)
            {
                if (line.Start is { })
                {
                    line.Start.PropertyChanged -= ObserveShape;
                }

                if (line.End is { })
                {
                    line.End.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is RectangleShapeViewModel rectangle)
            {
                if (rectangle.TopLeft is { })
                {
                    rectangle.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (rectangle.BottomRight is { })
                {
                    rectangle.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is EllipseShapeViewModel ellipse)
            {
                if (ellipse.TopLeft is { })
                {
                    ellipse.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (ellipse.BottomRight is { })
                {
                    ellipse.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is ArcShapeViewModel arc)
            {
                if (arc.Point1 is { })
                {
                    arc.Point1.PropertyChanged -= ObserveShape;
                }

                if (arc.Point2 is { })
                {
                    arc.Point2.PropertyChanged -= ObserveShape;
                }

                if (arc.Point3 is { })
                {
                    arc.Point3.PropertyChanged -= ObserveShape;
                }

                if (arc.Point4 is { })
                {
                    arc.Point4.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is CubicBezierShapeViewModel cubicBezier)
            {
                if (cubicBezier.Point1 is { })
                {
                    cubicBezier.Point1.PropertyChanged -= ObserveShape;
                }

                if (cubicBezier.Point2 is { })
                {
                    cubicBezier.Point2.PropertyChanged -= ObserveShape;
                }

                if (cubicBezier.Point3 is { })
                {
                    cubicBezier.Point3.PropertyChanged -= ObserveShape;
                }

                if (cubicBezier.Point4 is { })
                {
                    cubicBezier.Point4.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is QuadraticBezierShapeViewModel quadraticBezier)
            {
                if (quadraticBezier.Point1 is { })
                {
                    quadraticBezier.Point1.PropertyChanged -= ObserveShape;
                }

                if (quadraticBezier.Point2 is { })
                {
                    quadraticBezier.Point2.PropertyChanged -= ObserveShape;
                }

                if (quadraticBezier.Point3 is { })
                {
                    quadraticBezier.Point3.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is TextShapeViewModel text)
            {
                if (text.TopLeft is { })
                {
                    text.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (text.BottomRight is { })
                {
                    text.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is ImageShapeViewModel image)
            {
                if (image.TopLeft is { })
                {
                    image.TopLeft.PropertyChanged -= ObserveShape;
                }

                if (image.BottomRight is { })
                {
                    image.BottomRight.PropertyChanged -= ObserveShape;
                }
            }
            else if (shape is PathShapeViewModel path)
            {
                if (path.Geometry is { })
                {
                    Remove(path.Geometry);
                }
            }
            else if (shape is GroupShapeViewModel group)
            {
                if (group.Shapes is { })
                {
                    Remove(group.Shapes);
                }

                if (group.Connectors is { })
                {
                    Remove(group.Connectors);
                }
            }
        }

        private void Add(PathGeometryViewModel geometry)
        {
            if (geometry is null)
            {
                return;
            }

            geometry.PropertyChanged += ObserveShape;

            if (geometry.Figures is { })
            {
                Add(geometry.Figures);
            }
        }

        private void Remove(PathGeometryViewModel geometry)
        {
            if (geometry is null)
            {
                return;
            }

            geometry.PropertyChanged -= ObserveShape;

            if (geometry.Figures is { })
            {
                Remove(geometry.Figures);
            }
        }

        private void Add(PathFigureViewModel figure)
        {
            if (figure is null)
            {
                return;
            }

            figure.PropertyChanged += ObserveShape;

            if (figure.StartPoint is { })
            {
                figure.StartPoint.PropertyChanged += ObserveShape;
            }

            if (figure.Segments is { })
            {
                Add(figure.Segments);
            }
        }

        private void Remove(PathFigureViewModel figure)
        {
            if (figure is null)
            {
                return;
            }

            figure.PropertyChanged -= ObserveShape;

            if (figure.StartPoint is { })
            {
                figure.StartPoint.PropertyChanged -= ObserveShape;
            }

            if (figure.Segments is { })
            {
                Remove(figure.Segments);
            }
        }

        private void Add(PathSegmentViewModel segment)
        {
            if (segment is null)
            {
                return;
            }

            segment.PropertyChanged += ObserveShape;

            if (segment is LineSegmentViewModel)
            {
                var lineSegment = segment as LineSegmentViewModel;

                lineSegment.Point.PropertyChanged += ObserveShape;
            }
            else if (segment is ArcSegmentViewModel)
            {
                var arcSegment = segment as ArcSegmentViewModel;

                arcSegment.Point.PropertyChanged += ObserveShape;
                arcSegment.Size.PropertyChanged += ObserveShape;
            }
            else if (segment is CubicBezierSegmentViewModel)
            {
                var cubicBezierSegment = segment as CubicBezierSegmentViewModel;

                cubicBezierSegment.Point1.PropertyChanged += ObserveShape;
                cubicBezierSegment.Point2.PropertyChanged += ObserveShape;
                cubicBezierSegment.Point3.PropertyChanged += ObserveShape;
            }
            else if (segment is QuadraticBezierSegmentViewModel)
            {
                var quadraticBezierSegment = segment as QuadraticBezierSegmentViewModel;

                quadraticBezierSegment.Point1.PropertyChanged += ObserveShape;
                quadraticBezierSegment.Point2.PropertyChanged += ObserveShape;
            }
        }

        private void Remove(PathSegmentViewModel segment)
        {
            if (segment is null)
            {
                return;
            }

            segment.PropertyChanged -= ObserveShape;

            if (segment is LineSegmentViewModel)
            {
                var lineSegment = segment as LineSegmentViewModel;

                lineSegment.Point.PropertyChanged -= ObserveShape;
            }
            else if (segment is ArcSegmentViewModel)
            {
                var arcSegment = segment as ArcSegmentViewModel;

                arcSegment.Point.PropertyChanged -= ObserveShape;
                arcSegment.Size.PropertyChanged -= ObserveShape;
            }
            else if (segment is CubicBezierSegmentViewModel)
            {
                var cubicBezierSegment = segment as CubicBezierSegmentViewModel;

                cubicBezierSegment.Point1.PropertyChanged -= ObserveShape;
                cubicBezierSegment.Point2.PropertyChanged -= ObserveShape;
                cubicBezierSegment.Point3.PropertyChanged -= ObserveShape;
            }
            else if (segment is QuadraticBezierSegmentViewModel)
            {
                var quadraticBezierSegment = segment as QuadraticBezierSegmentViewModel;

                quadraticBezierSegment.Point1.PropertyChanged -= ObserveShape;
                quadraticBezierSegment.Point2.PropertyChanged -= ObserveShape;
            }
        }

        private void Add(IDataObject data)
        {
            if (data is null)
            {
                return;
            }

            if (data.Properties is { })
            {
                Add(data.Properties);
            }

            if (data is ViewModelBase observable)
            {
                observable.PropertyChanged += ObserveData;
            }
        }

        private void Remove(IDataObject data)
        {
            if (data is null)
            {
                return;
            }

            if (data.Properties is { })
            {
                Remove(data.Properties);
            }

            if (data is ViewModelBase observable)
            {
                observable.PropertyChanged -= ObserveData;
            }
        }

        private void Add(LibraryViewModel<ShapeStyleViewModel> sg)
        {
            if (sg is null)
            {
                return;
            }

            if (sg.Items is { })
            {
                Add(sg.Items);
            }

            sg.PropertyChanged += ObserveStyleLibrary;
        }

        private void Remove(LibraryViewModel<ShapeStyleViewModel> sg)
        {
            if (sg is null)
            {
                return;
            }

            if (sg.Items is { })
            {
                Remove(sg.Items);
            }

            sg.PropertyChanged -= ObserveStyleLibrary;
        }

        private void Add(LibraryViewModel<GroupShapeViewModel> gl)
        {
            if (gl is null)
            {
                return;
            }

            if (gl.Items is { })
            {
                Add(gl.Items);
            }

            gl.PropertyChanged += ObserveGroupLibrary;
        }

        private void Remove(LibraryViewModel<GroupShapeViewModel> gl)
        {
            if (gl is null)
            {
                return;
            }

            if (gl.Items is { })
            {
                Remove(gl.Items);
            }

            gl.PropertyChanged -= ObserveGroupLibrary;
        }

        private void Add(ShapeStyleViewModel style)
        {
            if (style is null)
            {
                return;
            }

            style.PropertyChanged += ObserveStyle;

            if (style.Stroke is { })
            {
                style.Stroke.PropertyChanged += ObserveStyle;
                
                if (style.Stroke.Color is { })
                {
                    style.Stroke.Color.PropertyChanged += ObserveStyle;
                }

                if (style.Stroke.StartArrow is { })
                {
                    style.Stroke.StartArrow.PropertyChanged += ObserveStyle;
                }

                if (style.Stroke.EndArrow is { })
                {
                    style.Stroke.EndArrow.PropertyChanged += ObserveStyle;
                }
            }

            if (style.Fill is { })
            {
                style.Fill.PropertyChanged += ObserveStyle;

                if (style.Fill.Color is { })
                {
                    style.Fill.Color.PropertyChanged += ObserveStyle;
                }
            }

            if (style.TextStyle is { })
            {
                style.TextStyle.PropertyChanged += ObserveStyle;
            }
        }

        private void Remove(ShapeStyleViewModel style)
        {
            if (style is null)
            {
                return;
            }

            style.PropertyChanged -= ObserveStyle;

            if (style.Stroke is { })
            {
                style.Stroke.PropertyChanged -= ObserveStyle;
                
                if (style.Stroke.Color is { })
                {
                    style.Stroke.Color.PropertyChanged -= ObserveStyle;
                }

                if (style.Stroke.StartArrow is { })
                {
                    style.Stroke.StartArrow.PropertyChanged -= ObserveStyle;
                }

                if (style.Stroke.EndArrow is { })
                {
                    style.Stroke.EndArrow.PropertyChanged -= ObserveStyle;
                }
            }

            if (style.Fill is { })
            {
                style.Fill.PropertyChanged -= ObserveStyle;
                
                if (style.Fill.Color is { })
                {
                    style.Fill.Color.PropertyChanged -= ObserveStyle;
                }
            }

            if (style.TextStyle is { })
            {
                style.TextStyle.PropertyChanged -= ObserveStyle;
            }
        }

        private void Add(PropertyViewModel property)
        {
            if (property is null)
            {
                return;
            }

            property.PropertyChanged += ObserveProperty;
        }

        private void Remove(PropertyViewModel property)
        {
            if (property is null)
            {
                return;
            }

            property.PropertyChanged -= ObserveProperty;
        }

        private void Add(IEnumerable<DatabaseViewModel> databases)
        {
            if (databases is null)
            {
                return;
            }

            foreach (var database in databases)
            {
                Add(database);
            }
        }

        private void Remove(IEnumerable<DatabaseViewModel> databases)
        {
            if (databases is null)
            {
                return;
            }

            foreach (var database in databases)
            {
                Remove(database);
            }
        }

        private void Add(IEnumerable<ColumnViewModel> columns)
        {
            if (columns is null)
            {
                return;
            }

            foreach (var column in columns)
            {
                Add(column);
            }
        }

        private void Remove(IEnumerable<ColumnViewModel> columns)
        {
            if (columns is null)
            {
                return;
            }

            foreach (var column in columns)
            {
                Remove(column);
            }
        }

        private void Add(IEnumerable<RecordViewModel> records)
        {
            if (records is null)
            {
                return;
            }

            foreach (var record in records)
            {
                Add(record);
            }
        }

        private void Remove(IEnumerable<RecordViewModel> records)
        {
            if (records is null)
            {
                return;
            }

            foreach (var record in records)
            {
                Remove(record);
            }
        }

        private void Add(IEnumerable<ValueViewModel> values)
        {
            if (values is null)
            {
                return;
            }

            foreach (var value in values)
            {
                Add(value);
            }
        }

        private void Remove(IEnumerable<ValueViewModel> values)
        {
            if (values is null)
            {
                return;
            }

            foreach (var value in values)
            {
                Remove(value);
            }
        }

        private void Add(ScriptViewModel script)
        {
            if (script is null)
            {
                return;
            }

            script.PropertyChanged += ObserveScript;
        }

        private void Remove(ScriptViewModel script)
        {
            if (script is null)
            {
                return;
            }

            script.PropertyChanged -= ObserveScript;
        }

        private void Add(IEnumerable<ScriptViewModel> scripts)
        {
            if (scripts is null)
            {
                return;
            }

            foreach (var script in scripts)
            {
                Add(script);
            }
        }

        private void Remove(IEnumerable<ScriptViewModel> scripts)
        {
            if (scripts is null)
            {
                return;
            }

            foreach (var script in scripts)
            {
                Remove(script);
            }
        }

        private void Add(IEnumerable<DocumentContainerViewModel> documents)
        {
            if (documents is null)
            {
                return;
            }

            foreach (var document in documents)
            {
                Add(document);
            }
        }

        private void Remove(IEnumerable<DocumentContainerViewModel> documents)
        {
            if (documents is null)
            {
                return;
            }

            foreach (var document in documents)
            {
                Remove(document);
            }
        }

        private void Add(IEnumerable<PageContainerViewModel> containers)
        {
            if (containers is null)
            {
                return;
            }

            foreach (var page in containers)
            {
                Add(page);
            }
        }

        private void Remove(IEnumerable<PageContainerViewModel> containers)
        {
            if (containers is null)
            {
                return;
            }

            foreach (var page in containers)
            {
                Remove(page);
            }
        }

        private void Add(IEnumerable<TemplateContainerViewModel> templates)
        {
            if (templates is null)
            {
                return;
            }

            foreach (var template in templates)
            {
                Add(template);
            }
        }

        private void Remove(IEnumerable<TemplateContainerViewModel> templates)
        {
            if (templates is null)
            {
                return;
            }

            foreach (var template in templates)
            {
                Remove(template);
            }
        }

        private void Add(IEnumerable<LayerContainerViewModel> layers)
        {
            if (layers is null)
            {
                return;
            }

            foreach (var layer in layers)
            {
                Add(layer);
            }
        }

        private void Remove(IEnumerable<LayerContainerViewModel> layers)
        {
            if (layers is null)
            {
                return;
            }

            foreach (var layer in layers)
            {
                Remove(layer);
            }
        }

        private void Add(IEnumerable<BaseShapeViewModel> shapes)
        {
            if (shapes is null)
            {
                return;
            }

            foreach (var shape in shapes)
            {
                Add(shape);
            }
        }

        private void Remove(IEnumerable<BaseShapeViewModel> shapes)
        {
            if (shapes is null)
            {
                return;
            }

            foreach (var shape in shapes)
            {
                Remove(shape);
            }
        }

        private void Add(IEnumerable<PathFigureViewModel> figures)
        {
            if (figures is null)
            {
                return;
            }

            foreach (var figure in figures)
            {
                Add(figure);
            }
        }

        private void Remove(IEnumerable<PathFigureViewModel> figures)
        {
            if (figures is null)
            {
                return;
            }

            foreach (var figure in figures)
            {
                Remove(figure);
            }
        }

        private void Add(IEnumerable<PathSegmentViewModel> segments)
        {
            if (segments is null)
            {
                return;
            }

            foreach (var segment in segments)
            {
                Add(segment);
            }
        }

        private void Remove(IEnumerable<PathSegmentViewModel> segments)
        {
            if (segments is null)
            {
                return;
            }

            foreach (var segment in segments)
            {
                Remove(segment);
            }
        }

        private void Add(IEnumerable<ShapeStyleViewModel> styles)
        {
            if (styles is null)
            {
                return;
            }

            foreach (var style in styles)
            {
                Add(style);
            }
        }

        private void Remove(IEnumerable<ShapeStyleViewModel> styles)
        {
            if (styles is null)
            {
                return;
            }

            foreach (var style in styles)
            {
                Remove(style);
            }
        }

        private void Add(IEnumerable<LibraryViewModel<ShapeStyleViewModel>> sgs)
        {
            if (sgs is null)
            {
                return;
            }

            foreach (var sg in sgs)
            {
                Add(sg);
            }
        }

        private void Remove(IEnumerable<LibraryViewModel<ShapeStyleViewModel>> sgs)
        {
            if (sgs is null)
            {
                return;
            }

            foreach (var sg in sgs)
            {
                Remove(sg);
            }
        }

        private void Add(IEnumerable<LibraryViewModel<GroupShapeViewModel>> gl)
        {
            if (gl is null)
            {
                return;
            }

            foreach (var g in gl)
            {
                Add(g);
            }
        }

        private void Remove(IEnumerable<LibraryViewModel<GroupShapeViewModel>> gl)
        {
            if (gl is null)
            {
                return;
            }

            foreach (var g in gl)
            {
                Remove(g);
            }
        }

        private void Add(IEnumerable<PropertyViewModel> properties)
        {
            if (properties is null)
            {
                return;
            }

            foreach (var property in properties)
            {
                Add(property);
            }
        }

        private void Remove(IEnumerable<PropertyViewModel> properties)
        {
            if (properties is null)
            {
                return;
            }

            foreach (var property in properties)
            {
                Remove(property);
            }
        }

        public void Dispose()
        {
            if (_editor?.Project is { })
            {
                Remove(_editor.Project);
            }
        }
    }
}
