#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor.Tools.Decorators;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Spatial;
using static System.Math;

namespace Core2D.ViewModels.Editor
{
    public partial class ProjectEditorViewModel
    {
        public void OnShowDecorator()
        {
            if (PageState is null)
            {
                return;
            }

            if (PageState.DrawDecorators == false)
            {
                return;
            }

            var shapes = Project?.SelectedShapes?.ToList();
            if (shapes is null || shapes.Count <= 0)
            {
                return;
            }

            if (PageState.Decorator is null)
            {
                PageState.Decorator = new BoxDecoratorViewModel(ServiceProvider);
            }

            PageState.Decorator.Layer = Project?.CurrentContainer?.WorkingLayer;
            PageState.Decorator.Shapes = shapes;
            PageState.Decorator.Update(true);
            PageState.Decorator.Show();
        }

        public void OnUpdateDecorator()
        {
            if (PageState is null)
            {
                return;
            }

            if (PageState.DrawDecorators == false)
            {
                return;
            }

            PageState.Decorator?.Update(false);
        }

        public void OnHideDecorator()
        {
            if (PageState is null)
            {
                return;
            }

            if (PageState.DrawDecorators == false)
            {
                return;
            }

            PageState.Decorator?.Hide();
        }

        public void OnShowOrHideDecorator()
        {
            if (PageState is null)
            {
                return;
            }

            if (PageState.DrawDecorators == false)
            {
                return;
            }

            if (Project?.SelectedShapes?.Count == 1 && Project.SelectedShapes?.FirstOrDefault() is PointShapeViewModel)
            {
                OnHideDecorator();
                return;
            }

            if (Project?.SelectedShapes?.Count == 1 && Project.SelectedShapes?.FirstOrDefault() is LineShapeViewModel)
            {
                OnHideDecorator();
                return;
            }

            if (Project?.SelectedShapes is { })
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
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);
                Select(
                    Project?.CurrentContainer?.CurrentLayer,
                    new HashSet<BaseShapeViewModel>(Project?.CurrentContainer?.CurrentLayer?.Shapes));
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnDeselectAll()
        {
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);
                OnUpdateDecorator();
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnClearAll()
        {
            try
            {
                var container = Project?.CurrentContainer;
                if (container is null)
                {
                    return;
                }
                
                foreach (var layer in container.Layers)
                {
                    Project?.ClearLayer(layer);
                }

                container.WorkingLayer.Shapes = ImmutableArray.Create<BaseShapeViewModel>();
                container.HelperLayer.Shapes = ImmutableArray.Create<BaseShapeViewModel>();

                Project.CurrentContainer.InvalidateLayer();
                OnHideDecorator();
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnSelect(IEnumerable<BaseShapeViewModel> shapes)
        {
            if (shapes?.Count() == 1)
            {
                Select(Project?.CurrentContainer?.CurrentLayer, shapes.FirstOrDefault());
            }
            else
            {
                Select(Project?.CurrentContainer?.CurrentLayer, new HashSet<BaseShapeViewModel>(shapes));
            }
        }

        public void OnDeleteSelected()
        {
            if (Project?.CurrentContainer?.CurrentLayer is null || PageState is null)
            {
                return;
            }

            if (!(Project.SelectedShapes?.Count > 0))
            {
                return;
            }
            
            var layer = Project.CurrentContainer.CurrentLayer;

            var builder = layer.Shapes.ToBuilder();
            foreach (var shape in Project.SelectedShapes)
            {
                builder.Remove(shape);
            }

            var previous = layer.Shapes;
            var next = builder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            Project.SelectedShapes = default;
            layer.RaiseInvalidateLayer();

            OnHideDecorator();
        }

        public void Deselect()
        {
            if (Project?.SelectedShapes is { })
            {
                Project.SelectedShapes = default;
            }

            OnHideDecorator();
        }

        public void Select(LayerContainerViewModel? layer, BaseShapeViewModel shape)
        {
            if (PageState is { })
            {
                if (Project is { })
                {
                    Project.SelectedShapes = new HashSet<BaseShapeViewModel> { shape };
                }

                if (PageState.DrawPoints == true)
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
                CanvasPlatform?.InvalidateControl?.Invoke();
            }
        }

        public void Select(LayerContainerViewModel? layer, ISet<BaseShapeViewModel> shapes)
        {
            if (PageState is { })
            {
                if (Project is { })
                {
                    Project.SelectedShapes = shapes;
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
                CanvasPlatform?.InvalidateControl?.Invoke();
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
                CanvasPlatform?.InvalidateControl?.Invoke();
            }
        }

        public bool TryToSelectShape(LayerContainerViewModel? layer, double x, double y, bool deselect = true)
        {
            if (layer is null)
            {
                return false;
            }

            var shapes = layer.Shapes.Reverse();
            var radius = Project.Options.HitThreshold / PageState.ZoomX;

            var point = HitTest?.TryToGetPoint(shapes, new Point2(x, y), radius, PageState.ZoomX);
            if (point is { })
            {
                Select(layer, point);
                return true;
            }

            var shape = HitTest?.TryToGetShape(shapes, new Point2(x, y), radius, PageState.ZoomX);
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
            var radius = Project.Options.HitThreshold / PageState.ZoomX;
            var result = HitTest.TryToGetShapes(shapes, rect, radius, PageState.ZoomX);
            if (result is { })
            {
                if (result.Count > 0)
                {
                    if (includeSelected)
                    {
                        if (Project?.SelectedShapes is { })
                        {
                            foreach (var shape in Project.SelectedShapes)
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
                                Select(layer, result.FirstOrDefault());
                            }
                            else
                            {
                                Select(layer, result);
                            }
                            return true;
                        }
                    }
                    else
                    {
                        if (result.Count == 1)
                        {
                            Select(layer, result.FirstOrDefault());
                        }
                        else
                        {
                            Select(layer, result);
                        }
                        return true;
                    }
                }
            }

            if (deselect)
            {
                Deselect(layer);
            }

            return false;
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
            if (Project?.CurrentContainer?.CurrentLayer is null)
            {
                return false;
            }

            if (Project.SelectedShapes?.Count > 1)
            {
                return false;
            }

            if (Project.SelectedShapes?.Count == 1 && HoveredShapeViewModel != Project.SelectedShapes?.FirstOrDefault())
            {
                return false;
            }
            
            var shapes = Project.CurrentContainer?.CurrentLayer?.Shapes.Reverse();

            var radius = Project.Options.HitThreshold / PageState.ZoomX;
            var point = HitTest.TryToGetPoint(shapes, new Point2(x, y), radius, PageState.ZoomX);
            if (point is { })
            {
                Hover(Project.CurrentContainer?.CurrentLayer, point);
                return true;
            }
            else
            {
                var shape = HitTest.TryToGetShape(shapes, new Point2(x, y), radius, PageState.ZoomX);
                if (shape is { })
                {
                    Hover(Project.CurrentContainer?.CurrentLayer, shape);
                    return true;
                }
                else
                {
                    if (Project.SelectedShapes?.Count == 1 && HoveredShapeViewModel == Project.SelectedShapes?.FirstOrDefault())
                    {
                        Dehover(Project.CurrentContainer?.CurrentLayer);
                    }
                }
            }

            return false;
        }

        public PointShapeViewModel? TryToGetConnectionPoint(double x, double y)
        {
            if (Project?.Options is null)
            {
                return null;
            }
            if (!Project.Options.TryToConnect)
            {
                return null;
            }
            var shapes = Project.CurrentContainer?.CurrentLayer?.Shapes.Reverse();
            var radius = Project.Options.HitThreshold / PageState.ZoomX;
            return HitTest?.TryToGetPoint(shapes, new Point2(x, y), radius, PageState.ZoomX);
        }

        private void SwapLineStart(LineShapeViewModel? line, PointShapeViewModel? point)
        {
            if (line?.Start is null || point is null)
            {
                return;
            }
            var previous = line.Start;
            var next = point;
            Project?.History?.Snapshot(previous, next, (p) => line.Start = p);
            line.Start = next;
        }

        private void SwapLineEnd(LineShapeViewModel? line, PointShapeViewModel? point)
        {
            if (line?.End is null || point is null)
            {
                return;
            }
            var previous = line.End;
            var next = point;
            Project?.History?.Snapshot(previous, next, (p) => line.End = p);
            line.End = next;
        }

        public bool TryToSplitLine(double x, double y, PointShapeViewModel point, bool select = false)
        {
            if (Project?.CurrentContainer is null || Project?.Options is null)
            {
                return false;
            }

            var shapes = Project.CurrentContainer.CurrentLayer.Shapes.Reverse();
            var radius = Project.Options.HitThreshold / PageState.ZoomX;
            var result = HitTest.TryToGetShape(shapes, new Point2(x, y), radius, PageState.ZoomX);

            if (result is not LineShapeViewModel line)
            {
                return false;
            }
            
            if (!Project.Options.SnapToGrid)
            {
                var a = new Point2(line.Start.X, line.Start.Y);
                var b = new Point2(line.End.X, line.End.Y);
                var target = new Point2(x, y);
                var nearest = target.NearestOnLine(a, b);
                point.X = nearest.X;
                point.Y = nearest.Y;
            }

            var split = ViewModelFactory.CreateLineShape(
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

            Project.AddShape(Project.CurrentContainer.CurrentLayer, split);

            if (@select)
            {
                Select(Project.CurrentContainer.CurrentLayer, point);
            }

            return true;

        }

        public bool TryToSplitLine(LineShapeViewModel line, PointShapeViewModel p0, PointShapeViewModel p1)
        {
            if (Project?.Options is null)
            {
                return false;
            }

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
                split = ViewModelFactory.CreateLineShape(
                    p0,
                    line.End,
                    (ShapeStyleViewModel)line.Style.Copy(null),
                    line.IsStroked);

                SwapLineEnd(line, p1);
            }
            else
            {
                split = ViewModelFactory.CreateLineShape(
                    p1,
                    line.End,
                    (ShapeStyleViewModel)line.Style.Copy(null),
                    line.IsStroked);

                SwapLineEnd(line, p0);
            }

            Project.AddShape(Project.CurrentContainer.CurrentLayer, split);

            return true;
        }

        public bool TryToConnectLines(IList<LineShapeViewModel> lines, ImmutableArray<PointShapeViewModel> connectors)
        {
            if (connectors.Length <= 0)
            {
                return false;
            }

            var lineToPointsMap = new Dictionary<LineShapeViewModel, IList<PointShapeViewModel>>();
            var threshold = Project?.Options is null || PageState is null ? 1.0 : Project.Options.HitThreshold / PageState.ZoomX;
            var scale = PageState?.ZoomX ?? 1.0;

            // Find possible connector to line connections.
            FinConnectors(lines, connectors, lineToPointsMap, threshold, scale);

            // Try to split lines using connectors.
            return TryToSplitUsingConnectors(lineToPointsMap, threshold);
        }

        private void FinConnectors(IList<LineShapeViewModel> lines, ImmutableArray<PointShapeViewModel> connectors, IDictionary<LineShapeViewModel, IList<PointShapeViewModel>> lineToPointsMap, double threshold, double scale)
        {
            if (HitTest is null)
            {
                return;
            }

            foreach (var connector in connectors)
            {
                LineShapeViewModel? result = null;
                
                foreach (var line in lines)
                {
                    if (HitTest.Contains(line, new Point2(connector.X, connector.Y), threshold, scale))
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
