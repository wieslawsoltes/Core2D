using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Renderer;
using Core2D.Scripting;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    public class ProjectObserver : IDisposable
    {
        private readonly ProjectEditorViewModel _editorViewModel;
        private readonly Action _invalidateContainer;
        private readonly Action _invalidateStyles;
        private readonly Action _invalidateLayers;
        private readonly Action _invalidateShapes;

        public ProjectObserver(ProjectEditorViewModel editorViewModel)
        {
            if (editorViewModel?.Project != null)
            {
                _editorViewModel = editorViewModel;

                _invalidateContainer = () => { };
                _invalidateStyles = () => Invalidate();
                _invalidateLayers = () => Invalidate();
                _invalidateShapes = () => Invalidate();

                Add(_editorViewModel.Project);
            }
        }

        private void Invalidate()
        {
            _editorViewModel?.Project?.CurrentContainerViewModel?.InvalidateLayer();
        }

        private void MarkAsDirty()
        {
            if (_editorViewModel != null)
            {
                _editorViewModel.IsProjectDirty = true;
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
                var container = sender as PageContainerViewModel;
                Remove(container.Properties);
                Add(container.Properties);
            }

            if (e.PropertyName == nameof(PageContainerViewModel.Layers))
            {
                var container = sender as PageContainerViewModel;
                Remove(container.Layers);
                Add(container.Layers);
            }

            _invalidateContainer();
            MarkAsDirty();
        }

        private void ObserveTemplateBackgroud(object sender, PropertyChangedEventArgs e)
        {
            _editorViewModel.Project.CurrentContainerViewModel.RaisePropertyChanged(nameof(PageContainerViewModel.Background));
            var page = _editorViewModel.Project.CurrentContainerViewModel;
            page?.Template.RaisePropertyChanged(nameof(PageContainerViewModel.Background));
            _invalidateLayers();
            MarkAsDirty();
        }

        private void ObserveGridStrokeColor(object sender, PropertyChangedEventArgs e)
        {
            _editorViewModel.Project.CurrentContainerViewModel.RaisePropertyChanged(nameof(IGrid.GridStrokeColorViewModel));
            var page = _editorViewModel.Project.CurrentContainerViewModel;
            page?.Template.RaisePropertyChanged(nameof(IGrid.GridStrokeColorViewModel));
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
            _editorViewModel?.CanvasPlatform?.InvalidateControl?.Invoke();
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

        private void Add(DatabaseViewModel databaseViewModel)
        {
            if (databaseViewModel == null)
            {
                return;
            }

            databaseViewModel.PropertyChanged += ObserveDatabase;

            if (databaseViewModel.Columns != null)
            {
                Add(databaseViewModel.Columns);
            }

            if (databaseViewModel.Records != null)
            {
                Add(databaseViewModel.Records);
            }
        }

        private void Remove(DatabaseViewModel databaseViewModel)
        {
            if (databaseViewModel == null)
            {
                return;
            }

            databaseViewModel.PropertyChanged -= ObserveDatabase;

            if (databaseViewModel.Columns != null)
            {
                Remove(databaseViewModel.Columns);
            }

            if (databaseViewModel.Records != null)
            {
                Remove(databaseViewModel.Records);
            }
        }

        private void Add(ColumnViewModel columnViewModel)
        {
            if (columnViewModel == null)
            {
                return;
            }

            columnViewModel.PropertyChanged += ObserveColumn;
        }

        private void Remove(ColumnViewModel columnViewModel)
        {
            if (columnViewModel == null)
            {
                return;
            }

            columnViewModel.PropertyChanged -= ObserveColumn;
        }

        private void Add(RecordViewModel recordViewModel)
        {
            if (recordViewModel == null)
            {
                return;
            }

            recordViewModel.PropertyChanged += ObserveRecord;

            if (recordViewModel.Values != null)
            {
                Add(recordViewModel.Values);
            }
        }

        private void Remove(RecordViewModel recordViewModel)
        {
            if (recordViewModel == null)
            {
                return;
            }

            recordViewModel.PropertyChanged -= ObserveRecord;

            if (recordViewModel.Values != null)
            {
                Remove(recordViewModel.Values);
            }
        }

        private void Add(ValueViewModel valueViewModel)
        {
            if (valueViewModel == null)
            {
                return;
            }

            valueViewModel.PropertyChanged += ObserveValue;
        }

        private void Remove(ValueViewModel valueViewModel)
        {
            if (valueViewModel == null)
            {
                return;
            }

            valueViewModel.PropertyChanged -= ObserveValue;
        }

        private void Add(OptionsViewModel optionsViewModel)
        {
            if (optionsViewModel == null)
            {
                return;
            }
        }

        private void Remove(OptionsViewModel optionsViewModel)
        {
            if (optionsViewModel == null)
            {
                return;
            }
        }

        private void Add(ProjectContainerViewModel project)
        {
            if (project == null)
            {
                return;
            }

            project.PropertyChanged += ObserveProject;

            Add(project.OptionsViewModel);

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

            if (project.Scripts != null)
            {
                foreach (var script in project.Scripts)
                {
                    Add(script);
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

        private void Remove(ProjectContainerViewModel project)
        {
            if (project == null)
            {
                return;
            }

            project.PropertyChanged -= ObserveProject;

            Remove(project.OptionsViewModel);

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

            if (project.Scripts != null)
            {
                foreach (var script in project.Scripts)
                {
                    Remove(script);
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

        private void Add(DocumentContainerViewModel document)
        {
            if (document == null)
            {
                return;
            }

            document.PropertyChanged += ObserveDocument;

            if (document.Pages != null)
            {
                foreach (var container in document.Pages)
                {
                    Add(container);
                }
            }
        }

        private void Remove(DocumentContainerViewModel document)
        {
            if (document == null)
            {
                return;
            }

            document.PropertyChanged -= ObserveDocument;

            if (document.Pages != null)
            {
                foreach (var container in document.Pages)
                {
                    Remove(container);
                }
            }
        }

        private void Add(PageContainerViewModel containerViewModel)
        {
            if (containerViewModel == null)
            {
                return;
            }

            containerViewModel.PropertyChanged += ObservePage;

            if (containerViewModel.Background != null)
            {
                containerViewModel.Background.PropertyChanged += ObserveTemplateBackgroud;
            }

            if (containerViewModel.GridStrokeColorViewModel != null)
            {
                containerViewModel.GridStrokeColorViewModel.PropertyChanged += ObserveGridStrokeColor;
            }

            containerViewModel.PropertyChanged += ObserveGrid;

            if (containerViewModel.Layers != null)
            {
                Add(containerViewModel.Layers);
            }

            if (containerViewModel is IDataObject data)
            {
                Add(data);
            }

            if (containerViewModel.WorkingLayer != null)
            {
                containerViewModel.WorkingLayer.InvalidateLayerHandler += ObserveInvalidateLayer;
            }

            if (containerViewModel.HelperLayer != null)
            {
                containerViewModel.HelperLayer.InvalidateLayerHandler += ObserveInvalidateLayer;
            }
        }

        private void Remove(PageContainerViewModel containerViewModel)
        {
            if (containerViewModel == null)
            {
                return;
            }

            containerViewModel.PropertyChanged -= ObservePage;

            if (containerViewModel.Background != null)
            {
                containerViewModel.Background.PropertyChanged -= ObserveTemplateBackgroud;
            }

            if (containerViewModel.GridStrokeColorViewModel != null)
            {
                containerViewModel.GridStrokeColorViewModel.PropertyChanged -= ObserveGridStrokeColor;
            }

            containerViewModel.PropertyChanged -= ObserveGrid;

            if (containerViewModel.Layers != null)
            {
                Remove(containerViewModel.Layers);
            }

            if (containerViewModel is IDataObject data)
            {
                Remove(data);
            }

            if (containerViewModel.WorkingLayer != null)
            {
                containerViewModel.WorkingLayer.InvalidateLayerHandler -= ObserveInvalidateLayer;
            }

            if (containerViewModel.HelperLayer != null)
            {
                containerViewModel.HelperLayer.InvalidateLayerHandler -= ObserveInvalidateLayer;
            }
        }

        private void Add(LayerContainerViewModel layer)
        {
            if (layer == null)
            {
                return;
            }

            layer.PropertyChanged += ObserveLayer;

            if (layer.Shapes != null)
            {
                Add(layer.Shapes);
            }

            layer.InvalidateLayerHandler += ObserveInvalidateLayer;
        }

        private void Remove(LayerContainerViewModel layer)
        {
            if (layer == null)
            {
                return;
            }

            layer.PropertyChanged -= ObserveLayer;

            if (layer.Shapes != null)
            {
                Remove(layer.Shapes);
            }

            layer.InvalidateLayerHandler -= ObserveInvalidateLayer;
        }

        private void Add(BaseShapeViewModel shapeViewModel)
        {
            if (shapeViewModel == null)
            {
                return;
            }

            shapeViewModel.PropertyChanged += ObserveShape;

            if (shapeViewModel.StyleViewModel != null)
            {
                Add(shapeViewModel.StyleViewModel);
            }

            if (shapeViewModel is IDataObject data)
            {
                Add(data);
            }

            if (shapeViewModel is LineShapeViewModel line)
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
            else if (shapeViewModel is RectangleShapeViewModel rectangle)
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
            else if (shapeViewModel is EllipseShapeViewModel ellipse)
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
            else if (shapeViewModel is ArcShapeViewModelViewModel arc)
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
            else if (shapeViewModel is CubicBezierShapeViewModel cubicBezier)
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
            else if (shapeViewModel is QuadraticBezierShapeViewModel quadraticBezier)
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
            else if (shapeViewModel is TextShapeViewModel text)
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
            else if (shapeViewModel is ImageShapeViewModel image)
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
            else if (shapeViewModel is PathShapeViewModel path)
            {
                if (path.GeometryViewModel != null)
                {
                    Add(path.GeometryViewModel);
                }
            }
            else if (shapeViewModel is GroupShapeViewModel group)
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

        private void Remove(BaseShapeViewModel shapeViewModel)
        {
            if (shapeViewModel == null)
            {
                return;
            }

            shapeViewModel.PropertyChanged -= ObserveShape;

            if (shapeViewModel.StyleViewModel != null)
            {
                Remove(shapeViewModel.StyleViewModel);
            }

            if (shapeViewModel is IDataObject data)
            {
                Remove(data);
            }

            if (shapeViewModel is LineShapeViewModel line)
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
            else if (shapeViewModel is RectangleShapeViewModel rectangle)
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
            else if (shapeViewModel is EllipseShapeViewModel ellipse)
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
            else if (shapeViewModel is ArcShapeViewModelViewModel arc)
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
            else if (shapeViewModel is CubicBezierShapeViewModel cubicBezier)
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
            else if (shapeViewModel is QuadraticBezierShapeViewModel quadraticBezier)
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
            else if (shapeViewModel is TextShapeViewModel text)
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
            else if (shapeViewModel is ImageShapeViewModel image)
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
            else if (shapeViewModel is PathShapeViewModel path)
            {
                if (path.GeometryViewModel != null)
                {
                    Remove(path.GeometryViewModel);
                }
            }
            else if (shapeViewModel is GroupShapeViewModel group)
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

        private void Add(PathGeometryViewModel geometryViewModel)
        {
            if (geometryViewModel == null)
            {
                return;
            }

            geometryViewModel.PropertyChanged += ObserveShape;

            if (geometryViewModel.Figures != null)
            {
                Add(geometryViewModel.Figures);
            }
        }

        private void Remove(PathGeometryViewModel geometryViewModel)
        {
            if (geometryViewModel == null)
            {
                return;
            }

            geometryViewModel.PropertyChanged -= ObserveShape;

            if (geometryViewModel.Figures != null)
            {
                Remove(geometryViewModel.Figures);
            }
        }

        private void Add(PathFigureViewModel figureViewModel)
        {
            if (figureViewModel == null)
            {
                return;
            }

            figureViewModel.PropertyChanged += ObserveShape;

            if (figureViewModel.StartPoint != null)
            {
                figureViewModel.StartPoint.PropertyChanged += ObserveShape;
            }

            if (figureViewModel.Segments != null)
            {
                Add(figureViewModel.Segments);
            }
        }

        private void Remove(PathFigureViewModel figureViewModel)
        {
            if (figureViewModel == null)
            {
                return;
            }

            figureViewModel.PropertyChanged -= ObserveShape;

            if (figureViewModel.StartPoint != null)
            {
                figureViewModel.StartPoint.PropertyChanged -= ObserveShape;
            }

            if (figureViewModel.Segments != null)
            {
                Remove(figureViewModel.Segments);
            }
        }

        private void Add(PathSegmentViewModel segmentViewModel)
        {
            if (segmentViewModel == null)
            {
                return;
            }

            segmentViewModel.PropertyChanged += ObserveShape;

            if (segmentViewModel is LineSegmentViewModel)
            {
                var lineSegment = segmentViewModel as LineSegmentViewModel;

                lineSegment.Point.PropertyChanged += ObserveShape;
            }
            else if (segmentViewModel is ArcSegmentViewModel)
            {
                var arcSegment = segmentViewModel as ArcSegmentViewModel;

                arcSegment.Point.PropertyChanged += ObserveShape;
                arcSegment.Size.PropertyChanged += ObserveShape;
            }
            else if (segmentViewModel is CubicBezierSegmentViewModel)
            {
                var cubicBezierSegment = segmentViewModel as CubicBezierSegmentViewModel;

                cubicBezierSegment.Point1.PropertyChanged += ObserveShape;
                cubicBezierSegment.Point2.PropertyChanged += ObserveShape;
                cubicBezierSegment.Point3.PropertyChanged += ObserveShape;
            }
            else if (segmentViewModel is QuadraticBezierSegmentViewModel)
            {
                var quadraticBezierSegment = segmentViewModel as QuadraticBezierSegmentViewModel;

                quadraticBezierSegment.Point1.PropertyChanged += ObserveShape;
                quadraticBezierSegment.Point2.PropertyChanged += ObserveShape;
            }
        }

        private void Remove(PathSegmentViewModel segmentViewModel)
        {
            if (segmentViewModel == null)
            {
                return;
            }

            segmentViewModel.PropertyChanged -= ObserveShape;

            if (segmentViewModel is LineSegmentViewModel)
            {
                var lineSegment = segmentViewModel as LineSegmentViewModel;

                lineSegment.Point.PropertyChanged -= ObserveShape;
            }
            else if (segmentViewModel is ArcSegmentViewModel)
            {
                var arcSegment = segmentViewModel as ArcSegmentViewModel;

                arcSegment.Point.PropertyChanged -= ObserveShape;
                arcSegment.Size.PropertyChanged -= ObserveShape;
            }
            else if (segmentViewModel is CubicBezierSegmentViewModel)
            {
                var cubicBezierSegment = segmentViewModel as CubicBezierSegmentViewModel;

                cubicBezierSegment.Point1.PropertyChanged -= ObserveShape;
                cubicBezierSegment.Point2.PropertyChanged -= ObserveShape;
                cubicBezierSegment.Point3.PropertyChanged -= ObserveShape;
            }
            else if (segmentViewModel is QuadraticBezierSegmentViewModel)
            {
                var quadraticBezierSegment = segmentViewModel as QuadraticBezierSegmentViewModel;

                quadraticBezierSegment.Point1.PropertyChanged -= ObserveShape;
                quadraticBezierSegment.Point2.PropertyChanged -= ObserveShape;
            }
        }

        private void Add(IDataObject data)
        {
            if (data == null)
            {
                return;
            }

            if (data.Properties != null)
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
            if (data == null)
            {
                return;
            }

            if (data.Properties != null)
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
            if (sg == null)
            {
                return;
            }

            if (sg.Items != null)
            {
                Add(sg.Items);
            }

            sg.PropertyChanged += ObserveStyleLibrary;
        }

        private void Remove(LibraryViewModel<ShapeStyleViewModel> sg)
        {
            if (sg == null)
            {
                return;
            }

            if (sg.Items != null)
            {
                Remove(sg.Items);
            }

            sg.PropertyChanged -= ObserveStyleLibrary;
        }

        private void Add(LibraryViewModel<GroupShapeViewModel> gl)
        {
            if (gl == null)
            {
                return;
            }

            if (gl.Items != null)
            {
                Add(gl.Items);
            }

            gl.PropertyChanged += ObserveGroupLibrary;
        }

        private void Remove(LibraryViewModel<GroupShapeViewModel> gl)
        {
            if (gl == null)
            {
                return;
            }

            if (gl.Items != null)
            {
                Remove(gl.Items);
            }

            gl.PropertyChanged -= ObserveGroupLibrary;
        }

        private void Add(ShapeStyleViewModel styleViewModel)
        {
            if (styleViewModel == null)
            {
                return;
            }

            styleViewModel.PropertyChanged += ObserveStyle;

            if (styleViewModel.Stroke != null)
            {
                styleViewModel.Stroke.PropertyChanged += ObserveStyle;
                
                if (styleViewModel.Stroke.ColorViewModel != null)
                {
                    styleViewModel.Stroke.ColorViewModel.PropertyChanged += ObserveStyle;
                }

                if (styleViewModel.Stroke.StartArrowStyleViewModel != null)
                {
                    styleViewModel.Stroke.StartArrowStyleViewModel.PropertyChanged += ObserveStyle;
                }

                if (styleViewModel.Stroke.EndArrowStyleViewModel != null)
                {
                    styleViewModel.Stroke.EndArrowStyleViewModel.PropertyChanged += ObserveStyle;
                }
            }

            if (styleViewModel.Fill != null)
            {
                styleViewModel.Fill.PropertyChanged += ObserveStyle;

                if (styleViewModel.Fill.ColorViewModel != null)
                {
                    styleViewModel.Fill.ColorViewModel.PropertyChanged += ObserveStyle;
                }
            }

            if (styleViewModel.TextStyleViewModel != null)
            {
                styleViewModel.TextStyleViewModel.PropertyChanged += ObserveStyle;
            }
        }

        private void Remove(ShapeStyleViewModel styleViewModel)
        {
            if (styleViewModel == null)
            {
                return;
            }

            styleViewModel.PropertyChanged -= ObserveStyle;

            if (styleViewModel.Stroke != null)
            {
                styleViewModel.Stroke.PropertyChanged -= ObserveStyle;
                
                if (styleViewModel.Stroke.ColorViewModel != null)
                {
                    styleViewModel.Stroke.ColorViewModel.PropertyChanged -= ObserveStyle;
                }

                if (styleViewModel.Stroke.StartArrowStyleViewModel != null)
                {
                    styleViewModel.Stroke.StartArrowStyleViewModel.PropertyChanged -= ObserveStyle;
                }

                if (styleViewModel.Stroke.EndArrowStyleViewModel != null)
                {
                    styleViewModel.Stroke.EndArrowStyleViewModel.PropertyChanged -= ObserveStyle;
                }
            }

            if (styleViewModel.Fill != null)
            {
                styleViewModel.Fill.PropertyChanged -= ObserveStyle;
                
                if (styleViewModel.Fill.ColorViewModel != null)
                {
                    styleViewModel.Fill.ColorViewModel.PropertyChanged -= ObserveStyle;
                }
            }

            if (styleViewModel.TextStyleViewModel != null)
            {
                styleViewModel.TextStyleViewModel.PropertyChanged -= ObserveStyle;
            }
        }

        private void Add(PropertyViewModel propertyViewModel)
        {
            if (propertyViewModel == null)
            {
                return;
            }

            propertyViewModel.PropertyChanged += ObserveProperty;
        }

        private void Remove(PropertyViewModel propertyViewModel)
        {
            if (propertyViewModel == null)
            {
                return;
            }

            propertyViewModel.PropertyChanged -= ObserveProperty;
        }

        private void Add(IEnumerable<DatabaseViewModel> databases)
        {
            if (databases == null)
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
            if (databases == null)
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
            if (columns == null)
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
            if (columns == null)
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
            if (records == null)
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
            if (records == null)
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
            if (values == null)
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
            if (values == null)
            {
                return;
            }

            foreach (var value in values)
            {
                Remove(value);
            }
        }

        private void Add(ScriptViewModel scriptViewModel)
        {
            if (scriptViewModel == null)
            {
                return;
            }

            scriptViewModel.PropertyChanged += ObserveScript;
        }

        private void Remove(ScriptViewModel scriptViewModel)
        {
            if (scriptViewModel == null)
            {
                return;
            }

            scriptViewModel.PropertyChanged -= ObserveScript;
        }

        private void Add(IEnumerable<ScriptViewModel> scripts)
        {
            if (scripts == null)
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
            if (scripts == null)
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
            if (documents == null)
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
            if (documents == null)
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
            if (containers == null)
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
            if (containers == null)
            {
                return;
            }

            foreach (var page in containers)
            {
                Remove(page);
            }
        }

        private void Add(IEnumerable<LayerContainerViewModel> layers)
        {
            if (layers == null)
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
            if (layers == null)
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
            if (shapes == null)
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
            if (shapes == null)
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
            if (figures == null)
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
            if (figures == null)
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
            if (segments == null)
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
            if (segments == null)
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
            if (styles == null)
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
            if (styles == null)
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
            if (sgs == null)
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
            if (sgs == null)
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
            if (gl == null)
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
            if (gl == null)
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
            if (properties == null)
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
            if (properties == null)
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
            if (_editorViewModel?.Project != null)
            {
                Remove(_editorViewModel.Project);
            }
        }
    }
}
