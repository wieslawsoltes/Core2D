#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor.Tools.Decorators;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Spatial;
using Core2D.ViewModels.Layout;
using static System.Math;

namespace Core2D.ViewModels.Editor
{
    public interface ISelectionService
    {
        (decimal sx, decimal sy) TryToSnap(InputArgs args);
        void OnShowDecorator();
        void OnUpdateDecorator();
        void OnHideDecorator();
        void OnShowOrHideDecorator();
        void OnSelectAll();
        void OnDeselectAll();
        void OnClearAll();
        void OnSelect(IEnumerable<BaseShapeViewModel> shapes);
        void OnDeleteSelected();
        void Deselect();
        void Deselect(LayerContainerViewModel? layer);
        void Select(LayerContainerViewModel? layer, BaseShapeViewModel shape);
        void Select(LayerContainerViewModel? layer, ISet<BaseShapeViewModel> shapes);
        bool TryToSelectShape(LayerContainerViewModel? layer, double x, double y, bool deselect = true);
        bool TryToSelectShapes(LayerContainerViewModel? layer, RectangleShapeViewModel? rectangle, bool deselect = true, bool includeSelected = false);
        void Hover(LayerContainerViewModel? layer, BaseShapeViewModel shape);
        void Dehover(LayerContainerViewModel? layer);
        bool TryToHoverShape(double x, double y);
        PointShapeViewModel? TryToGetConnectionPoint(double x, double y);
        bool TryToSplitLine(double x, double y, PointShapeViewModel point, bool select = false);
        bool TryToSplitLine(LineShapeViewModel line, PointShapeViewModel p0, PointShapeViewModel p1);
        bool TryToConnectLines(IList<LineShapeViewModel> lines, ImmutableArray<PointShapeViewModel> connectors);
    }

    public class SelectionServiceViewModel : ViewModelBase, ISelectionService
    {
        public SelectionServiceViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        private BaseShapeViewModel? HoveredShapeViewModel { get; set; }

        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }

        public (decimal sx, decimal sy) TryToSnap(InputArgs args)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is { } && project.Options?.SnapToGrid == true)
            {
                return (
                    PointUtil.Snap((decimal)args.X, (decimal)project.Options.SnapX),
                    PointUtil.Snap((decimal)args.Y, (decimal)project.Options.SnapY));
            }
            else
            {
                return ((decimal)args.X, (decimal)args.Y);
            }
        }

        public void OnShowDecorator()
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return;
            }

            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;
            if (pageState is null)
            {
                return;
            }

            if (pageState.DrawDecorators == false)
            {
                return;
            }

            var shapes = project?.SelectedShapes?.ToList();
            if (shapes is null || shapes.Count <= 0)
            {
                return;
            }

            if (pageState.Decorator is null)
            {
                pageState.Decorator = new BoxDecoratorViewModel(ServiceProvider);
            }

            pageState.Decorator.Layer = project?.CurrentContainer?.WorkingLayer;
            pageState.Decorator.Shapes = shapes;
            pageState.Decorator.Update(true);
            pageState.Decorator.Show();
        }

        public void OnUpdateDecorator()
        {
            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;
            if (pageState is null)
            {
                return;
            }

            if (pageState.DrawDecorators == false)
            {
                return;
            }

            pageState.Decorator?.Update(false);
        }

        public void OnHideDecorator()
        {
            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;
            if (pageState is null)
            {
                return;
            }

            if (pageState.DrawDecorators == false)
            {
                return;
            }

            pageState.Decorator?.Hide();
        }

        public void OnShowOrHideDecorator()
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return;
            }

            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;
            if (pageState is null)
            {
                return;
            }

            if (pageState.DrawDecorators == false)
            {
                return;
            }

            if (project?.SelectedShapes?.Count == 1 && project.SelectedShapes?.FirstOrDefault() is PointShapeViewModel)
            {
                OnHideDecorator();
                return;
            }

            if (project?.SelectedShapes?.Count == 1 && project.SelectedShapes?.FirstOrDefault() is LineShapeViewModel)
            {
                OnHideDecorator();
                return;
            }

            if (project?.SelectedShapes is { })
            {
                OnShowDecorator();
            }
            else
            {
                OnHideDecorator();
            }
        }

        public void OnSelectAll()
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return;
            }

            try
            {
                Deselect(project?.CurrentContainer?.CurrentLayer);
                Select(
                    project?.CurrentContainer?.CurrentLayer,
                    new HashSet<BaseShapeViewModel>(project?.CurrentContainer?.CurrentLayer?.Shapes));
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public void OnDeselectAll()
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return;
            }

            try
            {
                Deselect(project?.CurrentContainer?.CurrentLayer);
                OnUpdateDecorator();
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public void OnClearAll()
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return;
            }

            try
            {
                var container = project?.CurrentContainer;
                if (container is null)
                {
                    return;
                }
                
                foreach (var layer in container.Layers)
                {
                    project?.ClearLayer(layer);
                }

                container.WorkingLayer.Shapes = ImmutableArray.Create<BaseShapeViewModel>();
                container.HelperLayer.Shapes = ImmutableArray.Create<BaseShapeViewModel>();

                project.CurrentContainer.InvalidateLayer();
                OnHideDecorator();
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public void OnSelect(IEnumerable<BaseShapeViewModel> shapes)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return;
            }

            if (shapes?.Count() == 1)
            {
                Select(project?.CurrentContainer?.CurrentLayer, shapes.FirstOrDefault());
            }
            else
            {
                Select(project?.CurrentContainer?.CurrentLayer, new HashSet<BaseShapeViewModel>(shapes));
            }
        }

        public void OnDeleteSelected()
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return;
            }

            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;
            if (pageState is null)
            {
                return;
            }

            if (project?.CurrentContainer?.CurrentLayer is null || pageState is null)
            {
                return;
            }

            if (!(project.SelectedShapes?.Count > 0))
            {
                return;
            }
            
            var layer = project.CurrentContainer.CurrentLayer;

            var builder = layer.Shapes.ToBuilder();
            foreach (var shape in project.SelectedShapes)
            {
                builder.Remove(shape);
            }

            var previous = layer.Shapes;
            var next = builder.ToImmutable();
            project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            project.SelectedShapes = default;
            layer.RaiseInvalidateLayer();

            OnHideDecorator();
        }

        public void Deselect()
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return;
            }

            if (project?.SelectedShapes is { })
            {
                project.SelectedShapes = default;
            }

            OnHideDecorator();
        }

        public void Select(LayerContainerViewModel? layer, BaseShapeViewModel shape)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;

            if (pageState is { })
            {
                if (project is { })
                {
                    project.SelectedShapes = new HashSet<BaseShapeViewModel> { shape };
                }

                if (pageState.DrawPoints == true)
                {
                    OnHideDecorator();
                }
                else
                {
                    if (shape is PointShapeViewModel || shape is LineShapeViewModel)
                    {
                        OnHideDecorator();
                    }
                    else
                    {
                        OnShowDecorator();
                    }
                }
            }

            if (layer?.Owner is FrameContainerViewModel owner)
            {
                owner.CurrentShape = shape;
            }

            if (layer is { })
            {
                layer.RaiseInvalidateLayer();
            }
            else
            {
                ServiceProvider.GetService<IEditorCanvasPlatform>()?.InvalidateControl?.Invoke();
            }
        }

        public void Select(LayerContainerViewModel? layer, ISet<BaseShapeViewModel> shapes)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;

            if (pageState is { })
            {
                if (project is { })
                {
                    project.SelectedShapes = shapes;
                }

                OnShowDecorator();
            }

            if (layer?.Owner is FrameContainerViewModel owner && owner.CurrentShape is { })
            {
                owner.CurrentShape = default;
            }

            if (layer is { })
            {
                layer.RaiseInvalidateLayer();
            }
            else
            {
                ServiceProvider.GetService<IEditorCanvasPlatform>()?.InvalidateControl?.Invoke();
            }
        }

        public void Deselect(LayerContainerViewModel? layer)
        {
            Deselect();

            if (layer?.Owner is FrameContainerViewModel owner && owner.CurrentShape is { })
            {
                owner.CurrentShape = default;
            }

            if (layer is { })
            {
                layer.RaiseInvalidateLayer();
            }
            else
            {
                ServiceProvider.GetService<IEditorCanvasPlatform>()?.InvalidateControl?.Invoke();
            }
        }

        public bool TryToSelectShape(LayerContainerViewModel? layer, double x, double y, bool deselect = true)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return false;
            }

            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;
            if (pageState is null)
            {
                return false;
            }

            var hitTest = ServiceProvider.GetService<IHitTest>();

            if (layer is null)
            {
                return false;
            }

            var shapes = layer.Shapes.Reverse();
            var radius = project.Options.HitThreshold / pageState.ZoomX;

            var point = hitTest?.TryToGetPoint(shapes, new Point2(x, y), radius, pageState.ZoomX);
            if (point is { })
            {
                Select(layer, point);
                return true;
            }

            var shape = hitTest?.TryToGetShape(shapes, new Point2(x, y), radius, pageState.ZoomX);
            if (shape is { })
            {
                Select(layer, shape);
                return true;
            }

            if (deselect)
            {
                Deselect(layer);
            }

            return false;
        }

        public bool TryToSelectShapes(LayerContainerViewModel? layer, RectangleShapeViewModel? rectangle, bool deselect = true, bool includeSelected = false)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return false;
            }

            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;
            if (pageState is null)
            {
                return false;
            }

            var hitTest = ServiceProvider.GetService<IHitTest>();

            if (layer is null || rectangle is null)
            {
                return false;
            }
            
            var rect = Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y);
            var shapes = layer.Shapes;
            var radius = project.Options.HitThreshold / pageState.ZoomX;
            var result = hitTest.TryToGetShapes(shapes, rect, radius, pageState.ZoomX);
            if (result is { Count: > 0 })
            {
                if (includeSelected)
                {
                    if (TryToSelectShapesIncludeSelected(layer, result))
                    {
                        return true;
                    }
                }
                else
                {
                    return TryToSelectShapesResetSelected(layer, result);
                }
            }

            if (deselect)
            {
                Deselect(layer);
            }

            return false;
        }

        private bool TryToSelectShapesIncludeSelected(LayerContainerViewModel? layer, ISet<BaseShapeViewModel> result)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return false;
            }

            if (project.SelectedShapes is { })
            {
                foreach (var shape in project.SelectedShapes)
                {
                    if (result.Contains(shape))
                    {
                        result.Remove(shape);
                    }
                    else
                    {
                        result.Add(shape);
                    }
                }
            }

            if (result.Count > 0)
            {
                if (result.Count == 1)
                {
                    Select(layer, result.First());
                }
                else
                {
                    Select(layer, result);
                }

                return true;
            }

            return false;
        }

        private bool TryToSelectShapesResetSelected(LayerContainerViewModel? layer, ISet<BaseShapeViewModel> result)
        {
            if (result.Count == 1)
            {
                Select(layer, result.First());
            }
            else
            {
                Select(layer, result);
            }

            return true;
        }

        public void Hover(LayerContainerViewModel? layer, BaseShapeViewModel shape)
        {
            if (layer is null)
            {
                return;
            }
            
            Select(layer, shape);
            HoveredShapeViewModel = shape;
        }

        public void Dehover(LayerContainerViewModel? layer)
        {
            if (layer is null || HoveredShapeViewModel is null)
            {
                return;
            }
            
            HoveredShapeViewModel = default;
            Deselect(layer);
        }

        public bool TryToHoverShape(double x, double y)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return false;
            }

            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;
            if (pageState is null)
            {
                return false;
            }

            if (project?.CurrentContainer?.CurrentLayer is null)
            {
                return false;
            }

            if (project.SelectedShapes?.Count > 1)
            {
                return false;
            }

            if (project.SelectedShapes?.Count == 1 && HoveredShapeViewModel != project.SelectedShapes?.FirstOrDefault())
            {
                return false;
            }
            
            var hitTest = ServiceProvider.GetService<IHitTest>();

            var shapes = project.CurrentContainer?.CurrentLayer?.Shapes.Reverse();

            var radius = project.Options.HitThreshold / pageState.ZoomX;
            var point = hitTest.TryToGetPoint(shapes, new Point2(x, y), radius, pageState.ZoomX);
            if (point is { })
            {
                Hover(project.CurrentContainer?.CurrentLayer, point);
                return true;
            }

            var shape = hitTest.TryToGetShape(shapes, new Point2(x, y), radius, pageState.ZoomX);
            if (shape is { })
            {
                Hover(project.CurrentContainer?.CurrentLayer, shape);
                return true;
            }

            if (project.SelectedShapes?.Count == 1 && HoveredShapeViewModel == project.SelectedShapes?.FirstOrDefault())
            {
                Dehover(project.CurrentContainer?.CurrentLayer);
            }

            return false;
        }

        public PointShapeViewModel? TryToGetConnectionPoint(double x, double y)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return null;
            }

            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;
            if (pageState is null)
            {
                return null;
            }

            if (project?.Options is null)
            {
                return null;
            }
            
            if (!project.Options.TryToConnect)
            {
                return null;
            }
            
            var hitTest = ServiceProvider.GetService<IHitTest>();

            var shapes = project.CurrentContainer?.CurrentLayer?.Shapes.Reverse();
            var radius = project.Options.HitThreshold / pageState.ZoomX;
            return hitTest?.TryToGetPoint(shapes, new Point2(x, y), radius, pageState.ZoomX);
        }

        private void SwapLineStart(LineShapeViewModel? line, PointShapeViewModel? point)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return;
            }

            if (line?.Start is null || point is null)
            {
                return;
            }
            var previous = line.Start;
            var next = point;
            project?.History?.Snapshot(previous, next, (p) => line.Start = p);
            line.Start = next;
        }

        private void SwapLineEnd(LineShapeViewModel? line, PointShapeViewModel? point)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return;
            }

            if (line?.End is null || point is null)
            {
                return;
            }
            var previous = line.End;
            var next = point;
            project?.History?.Snapshot(previous, next, (p) => line.End = p);
            line.End = next;
        }

        public bool TryToSplitLine(double x, double y, PointShapeViewModel point, bool select = false)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return false;
            }

            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;
            if (pageState is null)
            {
                return false;
            }

            if (project?.CurrentContainer is null || project?.Options is null)
            {
                return false;
            }

            var hitTest = ServiceProvider.GetService<IHitTest>();
            var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();

            var shapes = project.CurrentContainer.CurrentLayer.Shapes.Reverse();
            var radius = project.Options.HitThreshold / pageState.ZoomX;
            var result = hitTest.TryToGetShape(shapes, new Point2(x, y), radius, pageState.ZoomX);

            if (result is not LineShapeViewModel line)
            {
                return false;
            }
            
            if (!project.Options.SnapToGrid)
            {
                var a = new Point2(line.Start.X, line.Start.Y);
                var b = new Point2(line.End.X, line.End.Y);
                var target = new Point2(x, y);
                var nearest = target.NearestOnLine(a, b);
                point.X = nearest.X;
                point.Y = nearest.Y;
            }

            var split = viewModelFactory?.CreateLineShape(
                x, y,
                (ShapeStyleViewModel)line.Style.Copy(null),
                line.IsStroked);

            var ds = point.DistanceTo(line.Start);
            var de = point.DistanceTo(line.End);

            if (ds < de)
            {
                split.Start = line.Start;
                split.End = point;
                SwapLineStart(line, point);
            }
            else
            {
                split.Start = point;
                split.End = line.End;
                SwapLineEnd(line, point);
            }

            project.AddShape(project.CurrentContainer.CurrentLayer, split);

            if (@select)
            {
                Select(project.CurrentContainer.CurrentLayer, point);
            }

            return true;

        }

        public bool TryToSplitLine(LineShapeViewModel line, PointShapeViewModel p0, PointShapeViewModel p1)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return false;
            }

            if (project?.Options is null)
            {
                return false;
            }

            var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();

            // Points must be aligned horizontally or vertically.
            if (p0.X != p1.X && p0.Y != p1.Y)
            {
                return false;
            }

            // Line must be horizontal or vertical.
            if (line.Start.X != line.End.X && line.Start.Y != line.End.Y)
            {
                return false;
            }

            LineShapeViewModel split;
            if (line.Start.X > line.End.X || line.Start.Y > line.End.Y)
            {
                split = viewModelFactory?.CreateLineShape(
                    p0,
                    line.End,
                    (ShapeStyleViewModel)line.Style.Copy(null),
                    line.IsStroked);

                SwapLineEnd(line, p1);
            }
            else
            {
                split = viewModelFactory?.CreateLineShape(
                    p1,
                    line.End,
                    (ShapeStyleViewModel)line.Style.Copy(null),
                    line.IsStroked);

                SwapLineEnd(line, p0);
            }

            project.AddShape(project.CurrentContainer?.CurrentLayer, split);

            return true;
        }

        public bool TryToConnectLines(IList<LineShapeViewModel> lines, ImmutableArray<PointShapeViewModel> connectors)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return false;
            }

            var pageState = ServiceProvider.GetService<ProjectEditorViewModel>()?.Renderer?.State;
            if (pageState is null)
            {
                return false;
            }

            if (connectors.Length <= 0)
            {
                return false;
            }

            var lineToPointsMap = new Dictionary<LineShapeViewModel, IList<PointShapeViewModel>>();
            var threshold = project?.Options is null || pageState is null ? 1.0 : project.Options.HitThreshold / pageState.ZoomX;
            var scale = pageState?.ZoomX ?? 1.0;

            // Find possible connector to line connections.
            FinConnectors(lines, connectors, lineToPointsMap, threshold, scale);

            // Try to split lines using connectors.
            return TryToSplitUsingConnectors(lineToPointsMap, threshold);
        }

        private void FinConnectors(IList<LineShapeViewModel> lines, ImmutableArray<PointShapeViewModel> connectors, IDictionary<LineShapeViewModel, IList<PointShapeViewModel>> lineToPointsMap, double threshold, double scale)
        {
            var hitTest = ServiceProvider.GetService<IHitTest>();
            if (hitTest is null)
            {
                return;
            }

            foreach (var connector in connectors)
            {
                LineShapeViewModel? result = null;
                
                foreach (var line in lines)
                {
                    if (hitTest.Contains(line, new Point2(connector.X, connector.Y), threshold, scale))
                    {
                        result = line;
                        break;
                    }
                }

                if (result is null)
                {
                    continue;
                }

                if (lineToPointsMap.ContainsKey(result))
                {
                    lineToPointsMap[result].Add(connector);
                }
                else
                {
                    lineToPointsMap.Add(result, new List<PointShapeViewModel>());
                    lineToPointsMap[result].Add(connector);
                }
            }
        }
   
        private bool TryToSplitUsingConnectors(IDictionary<LineShapeViewModel, IList<PointShapeViewModel>> lineToPointsMap, double threshold)
        {
            var success = false;
            
            foreach (var kv in lineToPointsMap)
            {
                var line = kv.Key;
                var points = kv.Value;
                if (points.Count != 2)
                {
                    continue;
                }
                var p0 = points[0];
                var p1 = points[1];
                var horizontal = Abs(p0.Y - p1.Y) < threshold;
                var vertical = Abs(p0.X - p1.X) < threshold;

                // Points are aligned horizontally.
                if (horizontal && !vertical)
                {
                    success = p0.X <= p1.X ? TryToSplitLine(line, p0, p1) : TryToSplitLine(line, p1, p0);
                }

                // Points are aligned vertically.
                if (!horizontal && vertical)
                {
                    success = p0.Y >= p1.Y ? TryToSplitLine(line, p1, p0) : TryToSplitLine(line, p0, p1);
                }
            }

            return success;
        }
    }
}
